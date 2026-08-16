// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
// SPDX-FileCopyrightText: 2026 PureBreadBagel <purebreadbagel@users.noreply.github.com>
// SPDX-License-Identifier: AGPL-3.0-or-later
//

using Content.Server.Atmos.EntitySystems;
using Content.Server.NodeContainer.NodeGroups;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Piping.Unary.Components;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server.Atmos.Reactions;

[UsedImplicitly]
[DataDefinition]
public sealed partial class KaltrenoxideCoolantReaction : IGasReactionEffect
{
    private const float TargetTemperature = 73.15f; // The target temp that Kaltrenoxide wants to be at.
    private const float CoolingPerSecond = 55000f; // How much energy Kalt removes so it can get to the target temp.

    // Slow passive decay, even when nothing is touching it.
    private const float PassiveDecayPerSecond = 0.05f;

    // Extra decay when Nitrogen is present.
    private const float NitrogenDecayPerSecond = 0.25f;

    public ReactionResult React(
    GasMixture mixture, // The gas mixture is where the reaction is happening. It contains the gases and their properties, such as moles and temperature.
    IGasMixtureHolder? holder, // What tile is holding gas.
    AtmosphereSystem atmosphereSystem, // The AtmosphereSystem is a system that handles all the gas reactions and properties in the game.
    float reactionDelta) // Reaction Delta is how much time passed in seconds since the last gas reaction.
    {
        if (reactionDelta <= 0f)
            return ReactionResult.NoReaction; // If no time has passed, there is no reaction!!!!

        var reacted = false;

        var kaltStart = mixture.GetMoles(Gas.Kaltrenoxide);
        if (kaltStart <= 0f)
            return ReactionResult.NoReaction;

        // Kaltrenoxide is stable inside sealed canisters and pipe nets, so nothing changes there.
        if (holder is GasCanisterComponent or IPipeNet)
            return ReactionResult.NoReaction;

        if (mixture.Temperature > TargetTemperature)
        {
            var heatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true); // All this to make sure it doesnt overshoot -200C

            if (heatCapacity > Atmospherics.MinimumHeatCapacity)
            {
                mixture.Temperature = MathF.Max(
                    TargetTemperature,
                    mixture.Temperature - CoolingPerSecond * reactionDelta / heatCapacity);

                reacted = true;
            }
        }

        var passiveDecay = MathF.Min(
            kaltStart,
            PassiveDecayPerSecond * reactionDelta);

        if (passiveDecay > 0f)
        {
            mixture.AdjustMoles(Gas.Kaltrenoxide, -passiveDecay); // Oh to be passively decaying.
            reacted = true;
        }

        var nitrogen = mixture.GetMoles(Gas.Nitrogen);

        if (nitrogen > 0f)
        {
            var nitrogenFactor = MathF.Min(1f, nitrogen / 5f);

            var nitrogenDecay = MathF.Min(
                mixture.GetMoles(Gas.Kaltrenoxide),
                NitrogenDecayPerSecond * reactionDelta * nitrogenFactor);

            if (nitrogenDecay > 0f)
            {
                mixture.AdjustMoles(Gas.Kaltrenoxide, -nitrogenDecay); // Remove Kaltrenoxide from the mixture because it interacts with Nitrogen obviously.
                reacted = true;
            }
        }

        return reacted ? ReactionResult.Reacting : ReactionResult.NoReaction;
    }
}
