using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

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

        var temperature = mixture.Temperature;
        const float targetTemperature = 7800f;

        if (temperature >= targetTemperature)
            return ReactionResult.NoReaction;

        var heatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
        if (heatCapacity <= Atmospherics.MinimumHeatCapacity)
            return ReactionResult.NoReaction;

        var deltaTemp = targetTemperature - temperature;

        var energyChange = deltaTemp * heatCapacity * 0.05f / reactionDelta;

        mixture.Temperature =
            (temperature * heatCapacity + energyChange) / heatCapacity;

        return ReactionResult.Reacting;
    }
}
