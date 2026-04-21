using Content.Goobstation.Shared.Boomerang;
using Content.Shared._Jamboree.CommandGear.SolShield;
using Content.Shared.Clothing;
using Content.Shared.Throwing;

namespace Content.Server._Jamboree.CommandGear.SolShield;

public sealed class SolShieldSystem : EntitySystem
{
    [Dependency] private readonly BoomerangSystem _boomerang = default!;
    public override void Initialize()
    {
        SubscribeLocalEvent<SolShieldComponent, ThrownEvent>(OnThrown);
        SubscribeLocalEvent<SolShieldComponent, LandEvent>(OnLanded);
    }

    private void OnThrown(EntityUid uid, SolShieldComponent component, ThrownEvent args)
    {
        if (!TryComp<BoomerangComponent>(uid, out var boomerang))
            boomerang = AddComp<BoomerangComponent>(uid);

        if (args.User == null)
            return;

        if (boomerang.CurrentHops > 0)
            return;

        EntityUid? gloveuser = null;

        var query = EntityQueryEnumerator<SolGlovesWearerComponent>();
        while (query.MoveNext(out var glovesuser, out _))
        {
            gloveuser = glovesuser;
            break;
        }

        gloveuser ??= args.User;

        _boomerang.SetThrower((uid, boomerang), gloveuser);
    }

    private void OnLanded(EntityUid uid, SolShieldComponent component, LandEvent args)
    {
        if (!TryComp<BoomerangComponent>(uid, out var boomerang))
            return;

        if (boomerang.Thrower == null)
            component.Thrown = false;
    }
}
