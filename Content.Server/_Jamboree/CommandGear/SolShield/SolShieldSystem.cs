using Content.Goobstation.Shared.Boomerang;
using Content.Shared._Jamboree.CommandGear.SolShield;
using Content.Shared.Clothing;
using Content.Shared.Inventory;
using Content.Shared.Throwing;

namespace Content.Server._Jamboree.CommandGear.SolShield;

public sealed class SolShieldSystem : EntitySystem
{
    [Dependency] private readonly BoomerangSystem _boomerang = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;
    public override void Initialize()
    {
        SubscribeLocalEvent<SolShieldComponent, ThrownEvent>(OnThrown);
        SubscribeLocalEvent<SolShieldComponent, LandEvent>(OnLanded);
    }

    private void OnThrown(EntityUid uid, SolShieldComponent component, ThrownEvent args)
    {
        if (args.User == null)
            return;

        var user = args.User.Value;

        var boomerang = EnsureComp<BoomerangComponent>(uid);

        boomerang.Thrower = null;

        if (_inventory.TryGetSlotEntity(user, "gloves", out var gloves) &&
            HasComp<SolGlovesComponent>(gloves.Value))
        {
            _boomerang.SetThrower((uid, boomerang), user);
        }
    }

    private void OnLanded(EntityUid uid, SolShieldComponent component, LandEvent args)
    {
        if (!TryComp<BoomerangComponent>(uid, out var boomerang))
            return;

        if (boomerang.Thrower == null)
            return;

        var thrower = boomerang.Thrower.Value;

        if (!_inventory.TryGetSlotEntity(thrower, "gloves", out var gloves) ||
            !HasComp<SolGlovesComponent>(gloves.Value))
        {
            boomerang.Thrower = null;
            component.Thrown = false;
        }
    }
}
