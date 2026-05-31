// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Prototypes;

namespace Content.Shared.EntityEffects.Effects;

public sealed partial class AdjustTemperature : EventEntityEffect<AdjustTemperature>
{
    [DataField]
    public float Amount;

    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
        => Loc.GetString("reagent-effect-guidebook-adjust-temperature",
            ("chance", Probability),
            ("deltasign", MathF.Sign(Amount)),
            ("amount", MathF.Abs(Amount)));
}
