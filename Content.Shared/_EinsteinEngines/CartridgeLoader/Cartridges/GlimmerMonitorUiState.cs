using Robust.Shared.Serialization;
using Content.Shared.CartridgeLoader;

namespace Content.Shared._EinsteinEngines.CartridgeLoader.Cartridges;

[Serializable, NetSerializable]
public sealed class GlimmerMonitorUiState : BoundUserInterfaceState
{
    public List<int> GlimmerValues;

    public GlimmerMonitorUiState(List<int> glimmerValues)
    {
        GlimmerValues = glimmerValues;
    }
}

[Serializable, NetSerializable]
public sealed class GlimmerMonitorSyncMessageEvent : CartridgeMessageEvent
{
    public GlimmerMonitorSyncMessageEvent()
    {}
}
