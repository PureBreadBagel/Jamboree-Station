// SPDX-FileCopyrightText: 2025 Gorse1 <elliotreece80@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Text.RegularExpressions;
using Content.Server.Speech.Components;
using Content.Shared.Speech;

namespace Content.Server.Speech.EntitySystems;

public sealed class StiltedSpeechSystem : EntitySystem
{
    // @formatter:off
    private static readonly Regex RegexStartOfWord = new(@"(?:^|\s)([a-z])");
    // @formatter:on

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<StiltedSpeechComponent, AccentGetEvent>(OnAccent);
    }

    private void OnAccent(EntityUid uid, StiltedSpeechComponent component, AccentGetEvent args)
    {
        var message = args.Message;

        // Capitalize the first letter of every word
        message = RegexStartOfWord.Replace(message, match => match.Value.ToUpper());

        args.Message = message;
    }
}
