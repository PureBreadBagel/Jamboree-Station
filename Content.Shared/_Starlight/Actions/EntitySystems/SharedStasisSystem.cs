// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Actions;
using Content.Shared._Starlight.Actions.Components;

namespace Content.Shared._Starlight.Actions.EntitySystems;

/// <summary>
/// Allows mobs to enter nanite induced stasis <see cref="StasisComponent"/>.
/// </summary>
public abstract class SharedStasisSystem : EntitySystem;

/// <summary>
/// Should be relayed upon using the action.
/// </summary>
public sealed partial class PrepareStasisActionEvent : InstantActionEvent;

/// <summary>
/// Should be relayed preparation to stasis being complete.
/// </summary>
public sealed partial class EnterStasisActionEvent : InstantActionEvent;

/// <summary>
/// Should be relayed upon using the action.
/// </summary>
public sealed partial class ExitStasisActionEvent : InstantActionEvent;
