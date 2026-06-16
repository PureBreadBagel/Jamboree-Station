using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.Timing;

namespace Content.Client._Jamboree.Overlays;

public sealed class TajaranNightVisionOverlay
{
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly IEntityManager _entity = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    private EntityUid? _lightEntity;
    private readonly TransformSystem _transform;
    private readonly SharedPointLightSystem _light;

    public TajaranNightVisionOverlay()
    {
        IoCManager.InjectDependencies(this);

        _transform = _entity.System<TransformSystem>();
        _light = _entity.System<SharedPointLightSystem>();
    }

    public void InitLight()
    {
        if (_lightEntity != null)
            return;

        var player = _player.LocalEntity;

        if (!_entity.TryGetComponent(player, out TransformComponent? playerXform))
            return;

        _lightEntity ??= _entity.SpawnAttachedTo(null, playerXform.Coordinates);
        _transform.SetParent(_lightEntity.Value, player.Value);
        var light = _entity.EnsureComponent<PointLightComponent>(_lightEntity.Value);
        _light.SetEnabled(_lightEntity.Value, false, light);
        //_light.SetEnergy(_lightEntity.Value, 1f, light);
        //_light.SetColor(_lightEntity.Value, Comp.Color, light);
    }

    public void RemoveLight()
    {
        if (_lightEntity == null)
            return;
        _entity.DeleteEntity(_lightEntity);
        _lightEntity = null;
    }

    public void TurnOnLight(float lightRadius)
    {
        if(_lightEntity == null)
            InitLight();

        if (_lightEntity == null || _timing.ApplyingState)
            return;

        _entity.TryGetComponent<PointLightComponent>(_lightEntity.Value, out var light);
        if (light == null)
            return;

        _light.SetRadius(_lightEntity.Value, lightRadius, light);
        _light.SetEnabled(_lightEntity.Value, true, light);
        //_light.SetEnergy(_lightEntity.Value, 1f, light);
        //_light.SetColor(_lightEntity.Value, Comp.Color, light);
    }

    public void TurnOffLight()
    {
        if(_lightEntity == null)
            InitLight();

        if (_lightEntity == null || _timing.ApplyingState)
            return;

        _entity.TryGetComponent<PointLightComponent>(_lightEntity.Value, out var light);
        if (light == null)
            return;

        _light.SetEnabled(_lightEntity.Value, false, light);
    }
}
