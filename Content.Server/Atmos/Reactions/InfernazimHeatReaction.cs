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
        float heatScale)
    {
        if (heatScale <= 0f)
            return ReactionResult.NoReaction;

        const float targetTemperature = 7800f; // extremely hot gas

        if (mixture.Temperature >= targetTemperature)
            return ReactionResult.NoReaction;

        var heatCap = atmosphereSystem.GetHeatCapacity(mixture, true);
        if (heatCap <= Atmospherics.MinimumHeatCapacity)
            return ReactionResult.NoReaction;

        var deltaEnergy = (targetTemperature - mixture.Temperature) * heatCap * 0.08f / heatScale;

        var newHeatCap = atmosphereSystem.GetHeatCapacity(mixture, true);
        if (newHeatCap <= Atmospherics.MinimumHeatCapacity)
            return ReactionResult.NoReaction;

        mixture.Temperature =
            (mixture.Temperature * heatCap + deltaEnergy) / newHeatCap;

        return ReactionResult.Reacting;
    }
}
