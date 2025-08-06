using Content.Shared._EinsteinEngines.Psionics;
using Content.Server._EinsteinEngines.CartridgeLoader.Cartridges;

/// <summary>
/// ADAPTED FROM SECWATCH - DELTAV
/// </summary>

namespace Content.Server._EinsteinEngines.CartridgeLoader.Cartridges;

[RegisterComponent, Access(typeof(PsiWatchCartridgeSystem))]
public sealed partial class PsiWatchCartridgeComponent : Component
{
    /// <summary>
    /// Only show people with these statuses.
    /// </summary>
    [DataField]
    public List<PsionicsStatus> Statuses = new()
    {
        PsionicsStatus.Abusing,
        PsionicsStatus.Registered,
        PsionicsStatus.Suspected
    };

    /// <summary>
    /// Station entity thats getting its records checked.
    /// </summary>
    [DataField]
    public EntityUid? Station;
}
