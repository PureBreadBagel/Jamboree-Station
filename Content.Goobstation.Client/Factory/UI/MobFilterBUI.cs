// SPDX-FileCopyrightText: 2025 JamboreeBot <JamboreeBot@proton.me>
// SPDX-FileCopyrightText: 2025 deltanedas <39013340+deltanedas@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Goobstation.Shared.Factory.Filters;
using Content.Shared.Mobs;
using Robust.Client.UserInterface;

namespace Content.Goobstation.Client.Factory.UI;

public sealed class MobFilterBUI : BoundUserInterface
{
    private MobFilterWindow? _window;

    public MobFilterBUI(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<MobFilterWindow>();
        if (EntMan.TryGetComponent<MobFilterComponent>(Owner, out var comp))
            _window.SelectValues(comp.States);
        _window.OnToggle += state => SendPredictedMessage(new MobFilterToggleMessage(state));
    }
}
