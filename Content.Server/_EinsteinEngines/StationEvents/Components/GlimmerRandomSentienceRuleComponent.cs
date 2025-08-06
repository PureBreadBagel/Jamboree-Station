using Content.Server._EinsteinEngines.StationEvents.Events;

namespace Content.Server._EinsteinEngines.StationEvents.Components;

[RegisterComponent, Access(typeof(GlimmerRandomSentienceRule))]
public sealed partial class GlimmerRandomSentienceRuleComponent : Component
{
    [DataField("maxMakeSentient")]
    public int MaxMakeSentient = 4;
}
