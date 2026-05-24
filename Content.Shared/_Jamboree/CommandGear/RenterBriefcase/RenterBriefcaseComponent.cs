// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared._Jamboree.RenterBriefcase;

[RegisterComponent]
public sealed partial class RenterBriefcaseComponent : Component
{
    [DataField]
    public ResPath PocketDimensionPath = new ResPath("/Maps/_Jamboree/Nonstations/rentersbriefcase.yml");

    [DataField]
    public EntProtoId ExitPortalPrototype = "PortalBlue";

    [ViewVariables]
    public bool PortalEnabled = false;

    [ViewVariables]
    public EntityUid? ExitPortal;

    [DataField]
    public SoundSpecifier OpenPortalSound = new SoundPathSpecifier("/Audio/Machines/high_tech_confirm.ogg")
    {
        Params = AudioParams.Default.WithVolume(-2f)
    };

    [DataField]
    public SoundSpecifier ClosePortalSound = new SoundPathSpecifier("/Audio/Machines/button.ogg");

    [ViewVariables]
    public EntityUid? PocketDimensionMap;

    [Serializable, NetSerializable]
    public enum BriefcaseVisualState : byte
    {
        Icon,
        Opening,
        Open,
        Closing
    }

    [Serializable, NetSerializable]
    public enum BriefcaseVisuals : byte
    {
        State
    }
}
