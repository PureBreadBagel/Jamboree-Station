// SPDX-FileCopyrightText: 2025 Gorse1 <elliotreece80@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server.Speech.Components;

namespace Content.Server.Speech.EntitySystems;

public sealed class PGAccentSystem : EntitySystem
{
    [Dependency] private readonly ReplacementAccentSystem _replacement = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PGAccentComponent, AccentGetEvent>(OnAccent);
    }

    private void OnAccent(EntityUid uid, PGAccentComponent component, AccentGetEvent args)
    {
        var message = args.Message;

        message = _replacement.ApplyReplacements(message, "pg_accent");

        args.Message = message;
    }
}
