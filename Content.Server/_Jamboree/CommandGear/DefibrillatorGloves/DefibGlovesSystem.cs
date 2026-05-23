using Content.Server.Medical;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Medical;
using Content.Shared.Whitelist;

namespace Content.Server._Jamboree.CommandGear.DefibrillatorGloves;

public sealed class DefibGlovesSystem : EntitySystem
{
    [Dependency] private readonly DefibrillatorSystem _defib = default!;
    [Dependency] private readonly EntityWhitelistSystem _whitelist = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;

public override void Initialize()
    {
        SubscribeLocalEvent<InteractHandEvent>(OnGloveDefib);
    }

    private void OnGloveDefib(InteractHandEvent args)
    {
        if (args.Handled)
            return;

        var user = args.User;
        var target = args.Target;

        if (user == target)
            return;

        if (!_inventory.TryGetSlotEntity(user, "gloves", out var gloves))
            return;

        if (!TryComp<Shared._Jamboree.CommandGear.DefibrillatorGloves.DefibGlovesComponent>(gloves, out var gloveComp))
            return;

        if (_whitelist.IsWhitelistFail(gloveComp.Whitelist, target))
            return;

        if (!TryComp<DefibrillatorComponent>(gloves.Value, out var defibComp))
            return;

        _defib.TryStartZap(gloves.Value, target, user, defibComp);

        args.Handled = true;
    }
}
