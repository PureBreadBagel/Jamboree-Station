using Content.Goobstation.Common.Religion;
using Content.Goobstation.Shared.Enchanting.Components;
using Content.Shared._Jamboree.DivineGuide;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Robust.Shared.Audio.Systems;

namespace Content.Server._Jamboree.DivineGuide;

public sealed class DivineGuideSystem : EntitySystem
{
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedPopupSystem _popupSystem = default!;
    public override void Initialize()
    {
        SubscribeLocalEvent<DivineGuideComponent, UseInHandEvent>(OnUse);
    }

    private void OnUse(EntityUid uid, DivineGuideComponent component, UseInHandEvent args)
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
