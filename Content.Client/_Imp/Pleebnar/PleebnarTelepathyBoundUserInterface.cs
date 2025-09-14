// SPDX-FileCopyrightText: 2025 Bibble-B-Boop <mysticball8@gmail.com>
// SPDX-FileCopyrightText: 2025 JamboreeBot <JamboreeBot@proton.me>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared._Imp.Pleebnar;
using Robust.Client.UserInterface;

namespace Content.Client._Imp.Pleebnar;

public sealed class PleebnarTelepathyBoundUserInterface : BoundUserInterface
{



    [ViewVariables]
    private PleebnarTelepathyWindow? _window;



    public PleebnarTelepathyBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }
    //handles opening the ui and setting button behaviour
    protected override void Open()
    {
        base.Open();
        _window = this.CreateWindow<PleebnarTelepathyWindow>();
        _window.ReloadVisions();
        _window.AddVisions();

        _window.OnVisionSelect += vision => SendMessage(new PleebnarTelepathyVisionMessage(vision));

    }

    //handles actually updating the ui
    protected override void UpdateState(BoundUserInterfaceState state)
    {
        if (state is not PleebnarTelepathyBuiState cast || _window == null)
        {
            return;
        }

        if (cast.Vision!=null) _window.UpdateState(cast.Vision);
    }
    //handles closing the ui
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        _window?.Close();
    }
}
