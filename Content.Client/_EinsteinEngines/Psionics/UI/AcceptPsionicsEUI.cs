// SPDX-FileCopyrightText: 2025 Baine Junk <wym0n@proton.me>
// SPDX-FileCopyrightText: 2025 JamboreeBot <JamboreeBot@proton.me>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Client.Eui;
using Content.Shared._EinsteinEngines.Psionics;
using JetBrains.Annotations;
using Robust.Client.Graphics;

namespace Content.Client._EinsteinEngines.Psionics.UI
{
    [UsedImplicitly]
    public sealed class AcceptPsionicsEui : BaseEui
    {
        private readonly AcceptPsionicsWindow _window;

        public AcceptPsionicsEui()
        {
            _window = new AcceptPsionicsWindow();

            _window.DenyButton.OnPressed += _ =>
            {
                SendMessage(new AcceptPsionicsChoiceMessage(AcceptPsionicsUiButton.Deny));
                _window.Close();
            };

            _window.AcceptButton.OnPressed += _ =>
            {
                SendMessage(new AcceptPsionicsChoiceMessage(AcceptPsionicsUiButton.Accept));
                _window.Close();
            };
        }

        public override void Opened()
        {
            IoCManager.Resolve<IClyde>().RequestWindowAttention();
            _window.OpenCentered();
        }

        public override void Closed()
        {
            _window.Close();
        }

    }
}
