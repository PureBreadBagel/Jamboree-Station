using Content.Server._EinsteinEngines.Psionics.Glimmer;
using Content.Server._EinsteinEngines.StationEvents.Components;
using Content.Server.StationEvents.Events;
using Content.Shared._EinsteinEngines.Psionics.Glimmer;
using Content.Shared.GameTicking.Components;

namespace Content.Server._EinsteinEngines.StationEvents.Events;

public sealed class GlimmerEventSystem : StationEventSystem<GlimmerEventComponent>
{
    [Dependency] private readonly GlimmerSystem _glimmerSystem = default!;

    protected override void Ended(EntityUid uid, GlimmerEventComponent component, GameRuleComponent gameRule, GameRuleEndedEvent args)
    {
        base.Ended(uid, component, gameRule, args);

        var glimmerBurned = RobustRandom.Next(component.GlimmerBurnLower, component.GlimmerBurnUpper);
        _glimmerSystem.DeltaGlimmerInput(-glimmerBurned);

        var reportEv = new GlimmerEventEndedEvent(component.SophicReport, glimmerBurned);
        RaiseLocalEvent(reportEv);
    }
}

public sealed class GlimmerEventEndedEvent : EntityEventArgs
{
    public string Message = "";
    public int GlimmerBurned = 0;

    public GlimmerEventEndedEvent(string message, int glimmerBurned)
    {
        Message = message;
        GlimmerBurned = glimmerBurned;
    }
}
