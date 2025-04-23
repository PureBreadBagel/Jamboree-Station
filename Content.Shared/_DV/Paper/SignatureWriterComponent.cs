namespace Content.Shared.DV.Paper;

[RegisterComponent]
public sealed partial class SignatureWriterComponent : Component
{
    /// <summary>
    /// The color used for the signature.
    /// </summary>
    [DataField]
    public Color Color = Color.FromHex("#2F4F4F");

    /// <summary>
    /// The list of colors that can be selected from, for pens with multiple colors.
    /// </summary>
    [DataField]
    public Dictionary<string, Color> ColorList = new();

    /// <summary>
    /// Imp. Replaces the player's name with this string if it isn't null.
    /// </summary>
    [DataField]
    public string? NameOverride = null;
}
