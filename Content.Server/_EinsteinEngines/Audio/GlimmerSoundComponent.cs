using Content.Server._EinsteinEngines.Psionics.Glimmer;
using Content.Shared.Audio;
using Content.Shared._EinsteinEngines.Psionics.Glimmer;
using Robust.Shared.Audio;
using Robust.Shared.ComponentTrees;
using Robust.Shared.GameStates;
using Robust.Shared.Physics;
using Robust.Shared.Serialization;

namespace Content.Server._EinsteinEngines.Audio
{
    [RegisterComponent]
    [Access(typeof(SharedAmbientSoundSystem), typeof(GlimmerReactiveSystem))]
    public sealed partial class GlimmerSoundComponent : Component
    {
        [DataField("glimmerTier", required: true), ViewVariables(VVAccess.ReadWrite)] // only for map editing
        public Dictionary<string, SoundSpecifier> Sound { get; set; } = new();

        public bool GetSound(GlimmerTier glimmerTier, out SoundSpecifier? spec)
        {
            return Sound.TryGetValue(glimmerTier.ToString(), out spec);
        }
    }
}
