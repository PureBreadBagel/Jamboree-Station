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
    private const float EnergyPerSecond = 400000f; // How much energy Infernazim generates so it can get to the target temp.

    private const float PassiveDecayPerSecond = 0.20f; // The mole decay rate of Infernazim when nothing is touching it.
    private const float ConversionPerSecond = 0.50f; // The mole conversion rate of Infernazim when it is touching Oxygen or Kaltrenoxide.

    public ReactionResult React(
    GasMixture mixture, // The gas mixture is where the reaction is happening. It contains the gases and their properties, such as moles and temperature.
    IGasMixtureHolder? holder, // What tile is holding gas.
    AtmosphereSystem atmosphereSystem, // The AtmosphereSystem is a system that handles all the gas reactions and properties in the game.
    float reactionDelta) // Reaction Delta is  how much time passed in seconds since the last gas reacton.
    {
        if (reactionDelta <= 0f)
            return ReactionResult.NoReaction;

        var reacted = false;

        var infernazimStart = mixture.GetMoles(Gas.Infernazim);
        if (infernazimStart <= 0f)
            return ReactionResult.NoReaction;

        var oxygen = mixture.GetMoles(Gas.Oxygen);
        var kaltrenoxide = mixture.GetMoles(Gas.Kaltrenoxide);

        var catalyst = oxygen + kaltrenoxide; // The catalyst is the amount of Oxygen and Kaltrenoxide present. The more catalyst, the more Infernazim will be converted to Water Vapor and Kaltrenoxide.

        if (catalyst > 0f)
        {
            var catalystFactor = MathF.Min(1f, catalyst / 5f);

            // Wow, this is a lot of math. This is calculating how much Infernazim will be converted to Water Vapor and Kaltrenoxide based on how much Oxygen and Kaltrenoxide is present. The more Oxygen and Kaltrenoxide, the more Infernazim will be converted.
            var converted = MathF.Min(
                infernazimStart,
                catalyst * ConversionPerSecond * reactionDelta * catalystFactor);

            if (converted > 0f)
            {
                mixture.AdjustMoles(Gas.Infernazim, -converted); // Remove Infernazim from the mixture because it hates OXYGEN, but hates Kaltrenoxide more.
                mixture.AdjustMoles(Gas.WaterVapor, converted);

                // Remove Kaltrenoxide from the mixture because Infernazim hates it more than Oxygen.
                var kaltrenConsumed = MathF.Min(kaltrenoxide, converted);
                mixture.AdjustMoles(Gas.Kaltrenoxide, -kaltrenConsumed);

                reacted = true;
            }
        }

        var decay = MathF.Min( // The mole decay rate of Infernazim when nothing is touching it.
            mixture.GetMoles(Gas.Infernazim),
            PassiveDecayPerSecond * reactionDelta);

        if (decay > 0f)
        {
            mixture.AdjustMoles(Gas.Infernazim, -decay);
            reacted = true;
        }

        var infernazimRemaining = mixture.GetMoles(Gas.Infernazim);

        if (infernazimRemaining > 0f && mixture.Temperature < TargetTemperature)
        {
            var heatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true); // The heat capacity is how much energy it takes to raise the temperature of the gas mixture by 1 degree. The more heat capacity, the less the temperature will change.

            if (heatCapacity > Atmospherics.MinimumHeatCapacity)
            {
                mixture.Temperature = MathF.Min(
                    TargetTemperature,
                    mixture.Temperature + EnergyPerSecond * reactionDelta / heatCapacity);

                reacted = true;
            }
        }

        return reacted ? ReactionResult.Reacting : ReactionResult.NoReaction;
    }
}
