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
    GasMixture mixture,
    IGasMixtureHolder? holder,
    AtmosphereSystem atmosphereSystem,
    float reactionDelta)
    {
        if (reactionDelta <= 0f)
            return ReactionResult.NoReaction;

        var reacted = false;

        var kaltStart = mixture.GetMoles(Gas.Kaltrenoxide);
        if (kaltStart <= 0f)
            return ReactionResult.NoReaction;

        // -------------------------
        // COOLING (temperature effect)
        // -------------------------
        if (mixture.Temperature > TargetTemperature)
        {
            var heatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

            if (heatCapacity > Atmospherics.MinimumHeatCapacity)
            {
                mixture.Temperature -= (CoolingPerSecond * reactionDelta) / heatCapacity;
                reacted = true;
            }
        }

        // -------------------------
        // PASSIVE DECAY
        // -------------------------
        var passiveDecay = MathF.Min(
            kaltStart,
            PassiveDecayPerSecond * reactionDelta);

        if (passiveDecay > 0f)
        {
            mixture.AdjustMoles(Gas.Kaltrenoxide, -passiveDecay);
            reacted = true;
        }

        // -------------------------
        // NITROGEN DESTRUCTION
        // -------------------------
        var nitrogen = mixture.GetMoles(Gas.Nitrogen);

        if (nitrogen > 0f)
        {
            var nitrogenFactor = MathF.Min(1f, nitrogen / 5f);

            var nitrogenDecay = MathF.Min(
                mixture.GetMoles(Gas.Kaltrenoxide),
                NitrogenDecayPerSecond * reactionDelta * nitrogenFactor);

            if (nitrogenDecay > 0f)
            {
                mixture.AdjustMoles(Gas.Kaltrenoxide, -nitrogenDecay);
                reacted = true;
            }
        }

        return reacted ? ReactionResult.Reacting : ReactionResult.NoReaction;
    }
}
