using Content.Shared._EinsteinEngines.StatusIcon;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

/// <summary>
/// EVERYTHING HERE IS A MODIFIED VERSION OF CRIMINAL RECORDS
/// </summary>

namespace Content.Shared._EinsteinEngines.Psionics.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class PsionicsRecordComponent : Component
{
    /// <summary>
    ///     The icon that should be displayed based on the psionics status of the entity.
    /// </summary>
    [DataField, AutoNetworkedField]
    public ProtoId<PsionicsIconPrototype> StatusIcon = "PsionicsIconStatus";
}
