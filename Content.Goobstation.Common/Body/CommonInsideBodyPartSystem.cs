// SPDX-FileCopyrightText: 2025 deltanedas <39013340+deltanedas@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Content.Goobstation.Common.Body;

/// <summary>
/// Minimal API for BodySystem to use while keeping the actual code in goob module
/// </summary>
public abstract class CommonInsideBodyPartSystem : EntitySystem
{
    public abstract void InsertedIntoPart(EntityUid item, EntityUid part);
    public abstract void RemovedFromPart(EntityUid item);
}
