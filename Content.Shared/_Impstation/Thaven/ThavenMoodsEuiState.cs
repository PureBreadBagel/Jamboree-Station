// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared._Impstation.Thaven;

[Serializable, NetSerializable]
public sealed class ThavenMoodsEuiState : EuiStateBase
{
    public List<ThavenMood> Moods { get; }
    public List<ThavenMood> SharedMoods { get; }
    public NetEntity Target { get; }
    public ThavenMoodsEuiState(List<ThavenMood> moods, List<ThavenMood> sharedMoods, NetEntity target)
    {
        Moods = moods;
        SharedMoods = sharedMoods;
        Target = target;
    }
}

[Serializable, NetSerializable]
public sealed class ThavenMoodsSaveMessage : EuiMessageBase
{
    public List<ThavenMood> Moods { get; }
    public List<ThavenMood> SharedMoods { get; }
    public NetEntity Target { get; }

    public ThavenMoodsSaveMessage(List<ThavenMood> moods, List<ThavenMood> sharedMoods, NetEntity target)
    {
        Moods = moods;
        SharedMoods = sharedMoods;
        Target = target;
    }
}

