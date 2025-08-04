using Content.Shared.Actions;

namespace Content.Shared._EinsteinEngines.Actions.Events;

public sealed partial class DarkSwapActionEvent : InstantActionEvent
{
    [DataField]
    public bool CheckInsulation;
}
