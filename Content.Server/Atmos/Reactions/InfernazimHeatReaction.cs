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
        GasMixture mixture, // Mixture is basically how much gas is in the tile.
        IGasMixtureHolder? holder, // Holder is the entity that holds the gas mixture, such as a tile or a container.
        AtmosphereSystem atmosphereSystem, // AtmosphereSystem is the system that handles all the gas reactions and other gas related stuff.
        float reactionDelta) // reactionDelta is the time since the last reaction, in seconds.
    {
        if (reactionDelta <= 0f)
            return ReactionResult.NoReaction;

        var reacted = false;

        var infernazim = mixture.GetMoles(Gas.Infernazim); // Get the amount of Infernazim in the mixture.
        if (infernazim <= 0f)
            return ReactionResult.NoReaction; // If there is no Infernazim in the mixture, then we don't want to do anything.

        var oxygen = mixture.GetMoles(Gas.Oxygen); // Get the amount of Oxygen in the mixture.
        var kaltrenoxide = mixture.GetMoles(Gas.Kaltrenoxide); // Get the amount of Kaltrenoxide in the mixture.



        var catalyst = oxygen + kaltrenoxide; // The catalyst is the sum of Oxygen and Kaltrenoxide, which are the gases that Infernazim reacts with.

        if (catalyst > 0f)
        {
            var catalystFactor = MathF.Min(1f, catalyst / 5f);

            var converted = MathF.Min(
                infernazim,
                ConversionPerSecond * reactionDelta * catalystFactor); // The amount of Infernazim that will be converted to Water Vapor, based on the amount of catalyst present and the reaction delta.

            if (converted > 0f)
            {
                mixture.AdjustMoles(Gas.Infernazim, -converted); // Remove the converted amount of Infernazim from the mixture.
                mixture.AdjustMoles(Gas.WaterVapor, converted); // Ditto but oposite. Add water vapor instead lol.
                reacted = true;

                infernazim -= converted; // DELETE THE INFERNAZIUM RAHHHH
            }
        }


        var decay = MathF.Min(infernazim, PassiveDecayPerSecond * reactionDelta); // The amount of Infernazim that will decay away, based on the passive decay rate and the reaction delta.
        if (decay > 0f)
        {
            mixture.AdjustMoles(Gas.Infernazim, -decay);
            reacted = true; // Infernazim decays away over time, even when nothing is touching it...Too bad not even the dying star can stop its own decay huh?
        }

        if (infernazim > 0f && mixture.Temperature < TargetTemperature)
        {
            var heatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true); // Get the heat capacity of the mixture, which is how much energy it takes to change the temperature of the mixture by 1 degree.

            if (heatCapacity > Atmospherics.MinimumHeatCapacity)
            {
                mixture.Temperature += EnergyPerSecond * reactionDelta / heatCapacity; // GRRRR HEAT IT UP RAHH!
                reacted = true;
            }
        }

        return reacted ? ReactionResult.Reacting : ReactionResult.NoReaction; // Return whether or not the reaction actually did anything.
    }
}
