// SPDX-FileCopyrightText: 2025 CerberusWolfie <wb.johnb.willis@gmail.com>
// SPDX-FileCopyrightText: 2025 GoobBot <uristmchands@proton.me>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Implants.Components;
using Content.Shared._EinsteinEngines.Language;
using Content.Shared._EinsteinEngines.Language.Components;
using Content.Shared._EinsteinEngines.Language.Events;
using Content.Shared._EinsteinEngines.Language.Systems;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;

namespace Content.Server._EinsteinEngines.Language;

public sealed class TranslatorImplantSystem : EntitySystem
{
    [Dependency] private readonly LanguageSystem _language = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<TranslatorImplantComponent, EntGotInsertedIntoContainerMessage>(OnImplant);
        SubscribeLocalEvent<TranslatorImplantComponent, EntGotRemovedFromContainerMessage>(OnDeImplant);
        SubscribeLocalEvent<ImplantedComponent, DetermineEntityLanguagesEvent>(OnDetermineLanguages);
    }

    private void OnImplant(EntityUid uid, TranslatorImplantComponent component, EntGotInsertedIntoContainerMessage args)
    {
        if (args.Container.ID != ImplanterComponent.ImplantSlotId)
            return;

        var implantee = Transform(uid).ParentUid;
        if (implantee is not { Valid: true } || !TryComp<LanguageKnowledgeComponent>(implantee, out var knowledge))
            return;

        component.Enabled = true;
        // To operate an implant, you need to know its required language intrinsically, because like... it connects to your brain or something,
        // so external translators or other implants can't help you operate it.
        component.SpokenRequirementSatisfied = TranslatorSystem.CheckLanguagesMatch(
            component.RequiredLanguages, knowledge.SpokenLanguages, component.RequiresAllLanguages);

        component.UnderstoodRequirementSatisfied = TranslatorSystem.CheckLanguagesMatch(
            component.RequiredLanguages, knowledge.UnderstoodLanguages, component.RequiresAllLanguages);

        if (component.AddUniversalLanguageSpeaker
            && component.SpokenRequirementSatisfied
            && component.UnderstoodRequirementSatisfied
            && !component.AddedUniversalLanguageSpeaker)
        {
            EnsureComp<UniversalLanguageSpeakerComponent>(implantee);
            component.AddedUniversalLanguageSpeaker = true;
        }

        _language.UpdateEntityLanguages(implantee);
    }

    private void OnDeImplant(EntityUid uid, TranslatorImplantComponent component, EntGotRemovedFromContainerMessage args)
    {
        // Even though the description of this event says it gets raised BEFORE reparenting, that's actually false...
        if (TryComp<SubdermalImplantComponent>(uid, out var subdermal) && subdermal.ImplantedEntity is { Valid: true } implantee)
        {
            if (component.AddedUniversalLanguageSpeaker)
                RemComp<UniversalLanguageSpeakerComponent>(implantee);

            _language.UpdateEntityLanguages(implantee);
        }

        component.Enabled = component.SpokenRequirementSatisfied = component.UnderstoodRequirementSatisfied = component.AddedUniversalLanguageSpeaker = false;
    }

    private void OnDetermineLanguages(EntityUid uid, ImplantedComponent component, ref DetermineEntityLanguagesEvent args)
    {
        // TODO: might wanna find a better solution, i just can't come up with something viable
        foreach (var implant in component.ImplantContainer.ContainedEntities)
        {
            if (!TryComp<TranslatorImplantComponent>(implant, out var translator) || !translator.Enabled)
                continue;

            if (translator.SpokenRequirementSatisfied)
                foreach (var language in translator.SpokenLanguages)
                    args.SpokenLanguages.Add(language);

            if (translator.UnderstoodRequirementSatisfied)
                foreach (var language in translator.UnderstoodLanguages)
                    args.UnderstoodLanguages.Add(language);

            // Centcomm implanter code. Basically gives you all the languages to speak as well as understanding it -- JAMBOREE!
            if (!translator.AddUniversalLanguageSpeaker)
                continue;

            if (translator.SpokenRequirementSatisfied)
                foreach (var language in _proto.EnumeratePrototypes<LanguagePrototype>())
                    if (language.ID != SharedLanguageSystem.UniversalPrototype && language.ID != SharedLanguageSystem.PsychomanticPrototype)
                        args.SpokenLanguages.Add(language.ID);

            if (translator.UnderstoodRequirementSatisfied)
                foreach (var language in _proto.EnumeratePrototypes<LanguagePrototype>())
                    if (language.ID != SharedLanguageSystem.UniversalPrototype && language.ID != SharedLanguageSystem.PsychomanticPrototype)
                        args.UnderstoodLanguages.Add(language.ID);
        }
    }
}
