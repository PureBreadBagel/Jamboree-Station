using Content.Shared._Jamboree.CommandGear.SolShield;
using Content.Shared.Inventory.Events;

namespace Content.Server._Jamboree.CommandGear.SolShield;

public sealed class SolShieldGloveSystem : EntitySystem
{
    public override void Initialize()
    {
        SubscribeLocalEvent<SolGlovesComponent, GotEquippedEvent>(OnEquipped);
        SubscribeLocalEvent<SolGlovesComponent, GotUnequippedEvent>(OnUnequipped);
    }

    private void OnEquipped(Entity<SolGlovesComponent> ent, ref GotEquippedEvent args)
    {
        var user = args.Equipee;

        EnsureComp<SolGlovesWearerComponent>(user);
    }

    private void OnUnequipped(Entity<SolGlovesComponent> ent, ref GotUnequippedEvent args)
    {
        var user = args.Equipee;

        if (TryComp<SolGlovesWearerComponent>(user, out _))
            RemComp<SolGlovesWearerComponent>(user);
    }
}
