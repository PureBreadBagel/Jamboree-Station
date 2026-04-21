namespace Content.Shared._Jamboree.CommandGear.SolShield;

[RegisterComponent]
public sealed partial class SolGlovesComponent : Component
{
}

[RegisterComponent]
public sealed partial class SolGlovesWearerComponent : Component
{
    public EntityUid? GloveUser = null;
}
