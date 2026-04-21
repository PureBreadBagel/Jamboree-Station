using Content.Shared._Jamboree.WeaponAction;
using Content.Shared._Shitmed.Targeting;
using Content.Shared.Damage;
using Content.Shared.Ian;
using Content.Shared.Mobs.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Physics.Events;

namespace Content.Server._Jamboree.WeaponAction;

public sealed class WeaponActionAttackSystem : EntitySystem

{
    [Dependency] DamageableSystem _damage = default!;
    [Dependency] SharedAudioSystem _audio = default!;
    public override void Initialize()
    {
        SubscribeLocalEvent<WeaponActionEntityComponent, StartCollideEvent>(OnAttackCollide);
    }

    private void OnAttackCollide(Entity<WeaponActionEntityComponent> ent, ref StartCollideEvent args)
    {
        if (!HasComp<MobStateComponent>(args.OtherEntity) || HasComp<WeaponActionComponent>(args.OtherEntity))
            return;

        _damage.TryChangeDamage(args.OtherEntity, ent.Comp.Damage, false, true, targetPart: TargetBodyPart.Chest);

        _audio.PlayPvs(ent.Comp.HitSound, ent, new AudioParams(-2f, 1f, SharedAudioSystem.DefaultSoundRange, 1f, false, 0f));
    }

}
