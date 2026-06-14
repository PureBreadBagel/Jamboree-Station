// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server.Atmos.Reactions;

[UsedImplicitly]
[DataDefinition]
public sealed partial class KaltrenoxideCoolantReaction : IGasReactionEffect
{
    private const float TargetTemperature = 73.15f; // The target temp that Kaltrenoxide wants to be at.
    private const float CoolingPerSecond = 80000f; // How much energy Kalt removes so it can get to the target temp.

    // Slow passive decay, even when nothing is touching it.
    private const float PassiveDecayPerSecond = 0.02f;

    // Extra decay when Nitrogen is present.
    private const float NitrogenDecayPerSecond = 0.25f;

    public ReactionResult React(
        GasMixture mixture, // Mixture is basically how much gas is in the tile.
        IGasMixtureHolder? holder, // Holder is the entity that holds the gas mixture, such as a tile or a container.
        AtmosphereSystem atmosphereSystem, // AtmosphereSystem is the system that handles all the gas reactions and other gas related stuff.
        float reactionDelta) // reactionDelta is the time since the last reaction, in seconds.
    {
        if (reactionDelta <= 0f)
            return ReactionResult.NoReaction; // If the reactionDelta is less than or equal to 0, then we don't want to do anything.

        var reacted = false;

        // Normal cooling behavior.
        if (mixture.Temperature > TargetTemperature)
        {
            var heatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true); // Get the heat capacity of the mixture, which is how much energy it takes to change the temperature of the mixture by 1 degree.
            if (heatCapacity > Atmospherics.MinimumHeatCapacity) // If the heat capacity is too low, we don't want to cool it down because it will just make it worse.
            {
                mixture.Temperature -= CoolingPerSecond * reactionDelta / heatCapacity; // Cool the mixture down towards the target temperature.
                reacted = true;
            }
        }

        // Passive decay over time.
        var passiveDecay = PassiveDecayPerSecond * reactionDelta;
        if (passiveDecay > 0f)
        {
            mixture.AdjustMoles(Gas.Kaltrenoxide, -passiveDecay); // Kaltrenoxide decays away...
            reacted = true;
        }

        // Nitrogen kills Kaltrenoixde faster! >:]
        var nitrogenMoles = mixture.GetMoles(Gas.Nitrogen); // Get the amount of Nitrogen in the mixture.
        if (nitrogenMoles > 0f)
        {
            var nitrogenFactor = MathF.Min(1f, nitrogenMoles / 5f);
            var nitrogenDecay = NitrogenDecayPerSecond * reactionDelta * nitrogenFactor;

            mixture.AdjustMoles(Gas.Kaltrenoxide, -nitrogenDecay); // Kaltrenoxide dies due to nitrogens cringyness.
            reacted = true;
        }

        return reacted ? ReactionResult.Reacting : ReactionResult.NoReaction; // Return whether or not the reaction actually did anything.
    }
}
