// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Goobstation.Common.Religion;
using Content.Goobstation.Shared.Enchanting.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Robust.Shared.Audio.Systems;

namespace Content.Server._Jamboree.CommandGear.DivineGuide;

public sealed class DivineGuideSystem : EntitySystem
{
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedPopupSystem _popupSystem = default!;
    public override void Initialize()
    {
        SubscribeLocalEvent<Shared._Jamboree.CommandGear.DivineGuide.DivineGuideComponent, UseInHandEvent>(OnUse);
    }

    private void OnUse(EntityUid uid, Shared._Jamboree.CommandGear.DivineGuide.DivineGuideComponent component, UseInHandEvent args)
    {
        if (args.Handled)
            return;

        args.Handled = true;

        if (!TryComp<BibleUserComponent>(args.User, out var bibleUser))
        {
            AddComp<BibleUserComponent>(args.User);
            AddComp<CanEnchantComponent>(args.User);

            var coords = Transform(args.User).Coordinates;
            EntityManager.SpawnEntity(component.SpawnedProto, coords);
            _audio.PlayPvs(component.SoundOnUse, coords);

            if (component.LearnMessage != null)
            {
                _popupSystem.PopupEntity(Loc.GetString(component.LearnMessage), args.User, args.User);
            }

            EntityManager.DeleteEntity(uid);
        }
        else
        {
            if (component.FailedMessage != null)
            {
                _popupSystem.PopupEntity(Loc.GetString(component.FailedMessage), args.User, args.User);
            }
        }
    }
}
