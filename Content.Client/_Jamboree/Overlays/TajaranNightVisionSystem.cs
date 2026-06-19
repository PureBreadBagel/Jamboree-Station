// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared._Jamboree.Overlays;
using Content.Shared.GameTicking;
using Robust.Client.Player;
using Robust.Shared.Player;

namespace Content.Client._Jamboree.Overlays;

public sealed partial class TajaranNightVisionSystem : EntitySystem
{
    [Dependency] private readonly IPlayerManager _player = default!;

    private TajaranNightVisionOverlay _overlay = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TajaranNightVisionComponent, NightVisionToggledEvent>(OnToggle);

        SubscribeLocalEvent<TajaranNightVisionComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<TajaranNightVisionComponent, ComponentRemove>(OnRemove);

        SubscribeLocalEvent<LocalPlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<LocalPlayerDetachedEvent>(OnPlayerDetached);
        SubscribeLocalEvent<RoundRestartCleanupEvent>(OnRoundRestart);

        _overlay = new TajaranNightVisionOverlay();
    }

    private void OnToggle(Entity<TajaranNightVisionComponent> ent, ref NightVisionToggledEvent args)
    {
        RefreshNightVision(args.Activated, args.LightRadius);
    }

    private void OnStartup(Entity<TajaranNightVisionComponent> ent, ref ComponentStartup args)
    {
        RefreshNightVision(false);
    }

    private void OnRemove(Entity<TajaranNightVisionComponent> ent, ref ComponentRemove args)
    {
        _overlay.RemoveLight();
    }

    private void OnPlayerAttached(LocalPlayerAttachedEvent ev)
    {
        RefreshNightVision(false);
    }

    private void OnPlayerDetached(LocalPlayerDetachedEvent ev)
    {
        _overlay.RemoveLight();
    }

    private void OnRoundRestart(RoundRestartCleanupEvent ev)
    {
        _overlay.RemoveLight();
    }

    private void RefreshNightVision(bool isActive, float lightRadius = 0)
    {
        if (_player.LocalSession?.AttachedEntity is not { } entity)
            return;

        if (isActive)
            _overlay.TurnOnLight(lightRadius);
        else
            _overlay.TurnOffLight();
    }
}
