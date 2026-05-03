// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared._Impstation.MobStateClothingVisuals;
using Content.Shared.Item;

namespace Content.Client._Impstation.ClothingMobStateVisuals;

public sealed partial class ClothingMobStateVisualsSystem : SharedMobStateClothingVisualsSystem
{
    [Dependency] private readonly SharedItemSystem _itemSys = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MobStateClothingVisualsComponent, ClothingMobStateChangedEvent>(OnClothingMobStateChanged);
    }

    private void OnClothingMobStateChanged(Entity<MobStateClothingVisualsComponent> ent, ref ClothingMobStateChangedEvent args)
    {
        _itemSys.VisualsChanged(ent);
    }
}
