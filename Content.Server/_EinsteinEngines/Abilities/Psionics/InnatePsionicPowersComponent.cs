using Content.Shared._EinsteinEngines.Psionics;
using Robust.Shared.Prototypes;

namespace Content.Server._EinsteinEngines.Abilities.Psionics
{
    [RegisterComponent]
    public sealed partial class InnatePsionicPowersComponent : Component
    {
        /// <summary>
        ///     The list of all powers to be added on Startup
        /// </summary>
        [DataField]
        public List<ProtoId<PsionicPowerPrototype>> PowersToAdd = new();
    }
}
