// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Actions;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Shared._Jamboree.Overlays;

public sealed class SharedTajaranNightVisionSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actions = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<TajaranNightVisionComponent, ToggleTajaranNightVisionEvent>(OnToggle);
        SubscribeLocalEvent<TajaranNightVisionComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<TajaranNightVisionComponent, ComponentShutdown>(OnShutdown);
    }

    private void OnMapInit(EntityUid uid, TajaranNightVisionComponent component, MapInitEvent args)
    {
        if (component is { ToggleActionEntity: null, ToggleAction: not null })
            _actions.AddAction(uid, ref component.ToggleActionEntity, component.ToggleAction);
    }

    private void OnShutdown(EntityUid uid, TajaranNightVisionComponent component, ComponentShutdown args)
    {
        _actions.RemoveAction(uid, component.ToggleActionEntity);
    }

    private void OnToggle(EntityUid uid, TajaranNightVisionComponent component, ToggleTajaranNightVisionEvent args)
    {
        if (args.Handled)
            return;

        component.IsActive = !component.IsActive;
        _actions.SetToggled(component.ToggleActionEntity, component.IsActive);
        RaiseNightVisionToggledEvent(uid, args.Performer, component.IsActive, component.LightRadius);

        args.Handled = true;
        Dirty(uid, component);
    }

    private void RaiseNightVisionToggledEvent(EntityUid uid, EntityUid user, bool activated, float lightRadius)
    {
        var ev = new NightVisionToggledEvent(user, activated, lightRadius);
        RaiseLocalEvent(uid, ref ev);
    }
}
[ByRefEvent]
public record struct NightVisionToggledEvent(EntityUid User, bool Activated, float LightRadius);
