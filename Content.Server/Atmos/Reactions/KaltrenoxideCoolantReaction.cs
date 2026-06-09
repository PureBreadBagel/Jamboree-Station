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

        var heatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
        if (heatCapacity <= Atmospherics.MinimumHeatCapacity)
            return ReactionResult.NoReaction;

        const float coolingPerSecond = 25000f;
        const float minTemperature = 73.15f;
        const float maxDeltaTemp = 250f;

        var deltaTemp =
            (coolingPerSecond * reactionDelta) / heatCapacity;

        deltaTemp = MathF.Min(deltaTemp, maxDeltaTemp * reactionDelta);

        if (deltaTemp <= 0f)
            return ReactionResult.NoReaction;

        var newTemp = mixture.Temperature - deltaTemp;
        mixture.Temperature = MathF.Max(minTemperature, newTemp);

        return ReactionResult.Reacting;
    }
}
