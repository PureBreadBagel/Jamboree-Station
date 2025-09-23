// SPDX-FileCopyrightText: 2025 Gorse1 <elliotreece80@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Text.RegularExpressions;
using Content.Server.Speech.Components;
using Content.Shared.Speech;

namespace Content.Server.Speech.EntitySystems;

public sealed class TransatlanticAccentSystem : EntitySystem
{
    [Dependency] private readonly ReplacementAccentSystem _replacement = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<TransatlanticAccentComponent, AccentGetEvent>(OnAccent);
    }

    private void OnAccent(EntityUid uid, TransatlanticAccentComponent component, AccentGetEvent args)
    {
        var message = args.Message;

        message = _replacement.ApplyReplacements(message, "transatlantic_accent");

        args.Message = message;
    }
}
