// SPDX-FileCopyrightText: 2025 Baine Junk <wym0n@proton.me>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Client.UserInterface.Fragments;
using Content.Shared._EinsteinEngines.CartridgeLoader;
using Content.Shared._EinsteinEngines.CartridgeLoader.Cartridges;
using Robust.Client.UserInterface;

/// <summary>
/// ADAPTED FROM SECWATCH - DELTAV
/// </summary>

namespace Content.Client._EinsteinEngines.CartridgeLoader.Cartridges;

public sealed partial class PsiWatchUi : UIFragment
{
    private PsiWatchUiFragment? _fragment;

    public override Control GetUIFragmentRoot()
    {
        return _fragment!;
    }

    public override void Setup(BoundUserInterface ui, EntityUid? owner)
    {
        _fragment = new PsiWatchUiFragment();
    }

    public override void UpdateState(BoundUserInterfaceState state)
    {
        if (state is PsiWatchUiState cast)
            _fragment?.UpdateState(cast);
    }
}
