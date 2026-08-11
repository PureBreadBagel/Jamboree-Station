// SPDX-FileCopyrightText: 2026 PureBreadBagel
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Goobstation.Shared.Chemistry;
using Robust.Client.GameObjects;

namespace Content.Goobstation.Client.Chemistry.Visualizers;

/// <summary>
/// Toggles a beaker overlay layer and the powered state of the dispenser sprite.
/// </summary>
public sealed class EnergyReagentDispenserVisualizerSystem : VisualizerSystem<EnergyReagentDispenserVisualsComponent>
{
    protected override void OnAppearanceChange(EntityUid uid, EnergyReagentDispenserVisualsComponent component, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
            return;

        var powered = !AppearanceSystem.TryGetData<bool>(uid, EnergyReagentDispenserVisuals.Powered, out var poweredData, args.Component) || poweredData;
        var hasBeaker = AppearanceSystem.TryGetData<bool>(uid, EnergyReagentDispenserVisuals.Beaker, out var beaker, args.Component) && beaker;

        SpriteSystem.LayerSetRsiState((uid, args.Sprite), 0, powered ? component.BaseState : component.NoPowerState);

        if (SpriteSystem.LayerMapTryGet((uid, args.Sprite), EnergyReagentDispenserVisualLayers.Beaker, out var beakerLayer, false))
            SpriteSystem.LayerSetVisible((uid, args.Sprite), beakerLayer, powered && hasBeaker);
    }
}
