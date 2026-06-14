// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server.Atmos.Reactions;

[UsedImplicitly]
[DataDefinition]
public sealed partial class InfernazimHeatReaction : IGasReactionEffect
{
    private const float TargetTemperature = 6000f; // The target temp that Infernazim wants to be at. In Kelvin.
    private const float EnergyPerSecond = 80000f; // How much energy Infernazim generates so it can get to the target temp.

    private const float PassiveDecayPerSecond = 0.02f; // The mole decay rate of Infernazim when nothing is touching it.
    private const float ConversionPerSecond = 0.25f; // The mole conversion rate of Infernazim when it is touching Oxygen or Kaltrenoxide.

    public ReactionResult React(
    GasMixture mixture,
    IGasMixtureHolder? holder,
    AtmosphereSystem atmosphereSystem,
    float reactionDelta)
    {
        if (reactionDelta <= 0f)
            return ReactionResult.NoReaction;

        var reacted = false;

        var infernazimStart = mixture.GetMoles(Gas.Infernazim);
        if (infernazimStart <= 0f)
            return ReactionResult.NoReaction;

        var oxygen = mixture.GetMoles(Gas.Oxygen);
        var kaltrenoxide = mixture.GetMoles(Gas.Kaltrenoxide);

        // -------------------------
        // CONVERSION (O2 / Kalt → Water Vapor)
        // -------------------------
        var catalyst = oxygen + kaltrenoxide;

        if (catalyst > 0f)
        {
            var catalystFactor = MathF.Min(1f, catalyst / 5f);

            var converted = MathF.Min(
                infernazimStart,
                ConversionPerSecond * reactionDelta * catalystFactor);

            if (converted > 0f)
            {
                mixture.AdjustMoles(Gas.Infernazim, -converted);
                mixture.AdjustMoles(Gas.WaterVapor, converted);
                reacted = true;
            }
        }

        // -------------------------
        // PASSIVE DECAY
        // -------------------------
        var decay = MathF.Min(
            mixture.GetMoles(Gas.Infernazim),
            PassiveDecayPerSecond * reactionDelta);

        if (decay > 0f)
        {
            mixture.AdjustMoles(Gas.Infernazim, -decay);
            reacted = true;
        }


        if (infernazimStart > 0f && mixture.Temperature < TargetTemperature)
        {
            var heatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

            if (heatCapacity > Atmospherics.MinimumHeatCapacity)
            {
                mixture.Temperature += EnergyPerSecond * reactionDelta / heatCapacity;
                reacted = true;
            }
        }

        return reacted ? ReactionResult.Reacting : ReactionResult.NoReaction;
    }
}
