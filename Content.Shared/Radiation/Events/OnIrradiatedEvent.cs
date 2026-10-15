// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Content.Shared.Radiation.Events;

/// <summary>
///     Raised on entity when it was irradiated
///     by some radiation source.
/// </summary>
public readonly record struct OnIrradiatedEvent(float FrameTime, float RadsPerSecond, EntityUid Origin, List<EntityUid>? Sources = null)
{
    public readonly float FrameTime = FrameTime;

    public readonly float RadsPerSecond = RadsPerSecond;

    public readonly EntityUid? Origin = Origin;

    public readonly List<EntityUid>? Sources = Sources;

    public float TotalRads => RadsPerSecond * FrameTime;
}