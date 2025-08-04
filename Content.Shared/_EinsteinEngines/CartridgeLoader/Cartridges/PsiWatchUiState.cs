using Content.Shared._EinsteinEngines.Psionics;
using Robust.Shared.Serialization;

namespace Content.Shared._EinsteinEngines.CartridgeLoader.Cartridges;

/// <summary>
/// Show a list of wanted and suspected people from psionics records.
/// </summary>
[Serializable, NetSerializable]
public sealed class PsiWatchUiState : BoundUserInterfaceState
{
    public readonly List<PsiWatchEntry> Entries;

    public PsiWatchUiState(List<PsiWatchEntry> entries)
    {
        Entries = entries;
    }
}

/// <summary>
/// Entry for a person who is suspected, registered, or abusing.
/// </summary>
[Serializable, NetSerializable]
public record struct PsiWatchEntry(string Name, string Job, PsionicsStatus Status, string? Reason);
