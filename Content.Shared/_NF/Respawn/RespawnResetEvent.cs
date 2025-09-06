// SPDX-FileCopyrightText: 2024 Kill_Me_I_Noobs <118206719+vonsant@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Serialization;

namespace Content.Shared._NF.Respawn;

[Serializable, NetSerializable]
public sealed class RespawnResetEvent(TimeSpan? time) : EntityEventArgs
{
    public readonly TimeSpan? Time = time;
}