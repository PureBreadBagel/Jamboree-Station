using Content.Server._EinsteinEngines.StationEvents.Events;

namespace Content.Server._EinsteinEngines.StationEvents.Components;

[RegisterComponent, Access(typeof(GlimmerRevenantRule))]
public sealed partial class GlimmerRevenantRuleComponent : Component
{
    [DataField("prototype")]
    public string RevenantPrototype = "MobRevenant";
}
