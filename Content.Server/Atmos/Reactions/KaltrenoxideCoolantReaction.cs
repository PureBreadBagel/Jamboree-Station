// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;
using System;

namespace Content.Server.Atmos.Reactions;

[UsedImplicitly]
[DataDefinition]
public sealed partial class KaltrenoxideCoolantReaction : IGasReactionEffect
{
    public ReactionResult React(
        GasMixture mixture,
        IGasMixtureHolder? holder,
        AtmosphereSystem atmosphereSystem,
        float reactionDelta)
    {
        if (reactionDelta <= 0f)
            return ReactionResult.NoReaction;

        var temperature = mixture.Temperature;

        // Cold but still safely above absolute zero
        const float targetTemperature = 73.15f;

        if (temperature <= targetTemperature)
            return ReactionResult.NoReaction;

        var heatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
        if (heatCapacity <= Atmospherics.MinimumHeatCapacity)
            return ReactionResult.NoReaction;

        var deltaTemp = targetTemperature - temperature;

        // Rate limit cooling so it doesn't instantly collapse atmos pressure/TEG side
        var maxStep = 500f * reactionDelta;

        var step = MathF.Max(deltaTemp, -maxStep);

        mixture.Temperature = temperature + step;

        return ReactionResult.Reacting;
    }
}
