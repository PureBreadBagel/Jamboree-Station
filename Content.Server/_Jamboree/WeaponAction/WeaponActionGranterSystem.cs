using Content.Shared._Jamboree.WeaponAction;
using Content.Shared.Actions;
using Content.Shared.Hands;

namespace Content.Server._Jamboree.WeaponAction;

public sealed class WeaponActionGranterSystem : EntitySystem
{
    public override void Initialize()
    {
        SubscribeLocalEvent<WeaponActionGranterComponent, GetItemActionsEvent>(OnGetActions);
        SubscribeLocalEvent<WeaponActionGranterComponent, GotEquippedHandEvent>(OnEquipped);
        SubscribeLocalEvent<WeaponActionGranterComponent, UnequippedHandEvent>(OnUnequipped);
    }

    private void OnGetActions(EntityUid uid, WeaponActionGranterComponent comp, GetItemActionsEvent args)
    {
        args.AddAction(ref comp.WeaponActionEntity, comp.WeaponAction);
    }
    private void OnEquipped(Entity<WeaponActionGranterComponent> uid, ref GotEquippedHandEvent args)
    {
        if (!HasComp<WeaponActionComponent>(args.User))
            AddComp<WeaponActionComponent>(args.User);
    }

    private void OnUnequipped(Entity<WeaponActionGranterComponent> uid, ref UnequippedHandEvent args)
    {
        if (HasComp<WeaponActionComponent>(args.User))
            RemComp<WeaponActionComponent>(args.User);
    }
}
