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

        const float coolingPerSecond = 80000f;
        mixture.Temperature -= (coolingPerSecond * reactionDelta) / heatCapacity;

        return ReactionResult.Reacting;
    }
}
