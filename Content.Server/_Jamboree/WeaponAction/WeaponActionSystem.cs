using System.Linq;
using System.Numerics;
using Content.Shared._Jamboree.WeaponAction;
using Content.Shared.Damage;
using Content.Shared.Mobs.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Map;

namespace Content.Server._Jamboree.WeaponAction;

public sealed class WeaponActionSystem : EntitySystem
{
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedGunSystem _gun = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<WeaponActionGranterComponent, EventWardenHalberdAction>(OnWardenHalberdAction);
        SubscribeLocalEvent<WeaponActionGranterComponent, EventZweihanderAction>(OnZweihanderAction);
    }

    private void OnWardenHalberdAction(Entity<WeaponActionGranterComponent> ent, ref EventWardenHalberdAction args)
    {
        if (args.Handled)
            return;

        var uid = ent.Owner;

        args.Handled = true;

        var xform = Transform(uid);

        var (pos, rot) = _transform.GetWorldPositionRotation(xform);

        var dir = rot.ToWorldVec();

        var mapPos = new MapCoordinates(pos + dir * args.Offset, xform.MapID);

        var slash = Spawn(args.Proto, mapPos);

        _gun.ShootProjectile(slash, dir, Vector2.Zero, uid, uid, args.Speed);
        _gun.SetTarget(slash, null, out _);

        _audio.PlayPvs(ent.Comp.UseSound, ent, new AudioParams(-2f, 1f, SharedAudioSystem.DefaultSoundRange, 1f, false, 0f));
    }

    private void OnZweihanderAction(Entity<WeaponActionGranterComponent> ent, ref EventZweihanderAction args)
    {
        if (args.Handled)
            return;

        var uid = ent.Owner;

        args.Handled = true;

        var xform = Transform(uid);

        var (pos, rot) = _transform.GetWorldPositionRotation(xform);

        var dir = rot.ToWorldVec();

        var mapPos = new MapCoordinates(pos + dir * args.Offset, xform.MapID);

        var slash = Spawn(args.Proto, mapPos);

        _gun.ShootProjectile(slash, dir, Vector2.Zero, uid, uid, args.Speed);
        _gun.SetTarget(slash, null, out _);

        _audio.PlayPvs(ent.Comp.UseSound, ent, new AudioParams(-2f, 1f, SharedAudioSystem.DefaultSoundRange, 1f, false, 0f));
    }
}
