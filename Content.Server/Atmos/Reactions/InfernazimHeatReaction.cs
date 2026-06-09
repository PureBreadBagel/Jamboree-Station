using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;
using System;

namespace Content.Server.Atmos.Reactions;

[UsedImplicitly]
[DataDefinition]
public sealed partial class InfernazimHeatReaction : IGasReactionEffect
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

        const float energyPerSecond = 25000f;
        const float maxDeltaTemp = 250f;

        var deltaTemp =
            (energyPerSecond * reactionDelta) / heatCapacity;

        deltaTemp = MathF.Min(deltaTemp, maxDeltaTemp * reactionDelta);

        if (deltaTemp <= 0f)
            return ReactionResult.NoReaction;

        mixture.Temperature += deltaTemp;

        return ReactionResult.Reacting;
    }
}
