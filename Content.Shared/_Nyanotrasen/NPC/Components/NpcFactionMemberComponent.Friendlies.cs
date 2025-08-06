namespace Content.Shared.NPC.Components;

public sealed partial class NpcFactionMemberComponent : Component
{
    // Nyano - Summary - Begin modified code block: support for specific entities to be friendly.
    /// <summary>
    /// Permanently friendly specific entities. Our summoner, etc.
    /// Would like to separate. Could I do that by extending this method, maybe?
    /// </summary>
    public HashSet<EntityUid> ExceptionalFriendlies = new();
    // Nyano - End modified code block.
}

