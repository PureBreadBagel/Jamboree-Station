// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server.Medical;
using Content.Shared._Jamboree.CommandGear.DefibrillatorGloves;
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

        if (args.User == args.Target)
            return;

        if (!_inventory.TryGetSlotEntity(args.User, "gloves", out var gloves))
            return;

        if (!TryComp<DefibGlovesComponent>(gloves, out var gloveComp))
            return;

        if (_whitelist.IsWhitelistFail(gloveComp.Whitelist, args.Target))
            return;

        if (!TryComp<DefibrillatorComponent>(gloves.Value, out var defibComp))
            return;

        _defib.TryStartZap(gloves.Value, args.Target, args.User, defibComp);

        args.Handled = true;
    }
}
