// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Goobstation.Maths.FixedPoint;
using Content.Server.Forensics;
using Content.Server._Starlight.Stains.Components;
using Content.Server._Starlight.Stains.Systems;
using Content.Shared._Funkystation.Stains.Components;
using Content.Shared._Funkystation.Stains.Systems;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Clothing.Components;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.DoAfter;
using Content.Shared.Fluids.Components;
using Content.Shared.Forensics.Components;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.StepTrigger.Components;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.IntegrationTests.Tests._Funkystation;

[TestFixture]
public sealed class StainImprovementsTest
{
    [TestPrototypes]
    private const string Prototypes = @"
- type: entity
  id: StainImprovementsWearer
  components:
  - type: Inventory
  - type: ContainerContainer
  - type: Hands
  - type: Sprite
  - type: DoAfter

- type: entity
  parent: ClothingOuterWinterCoatToggleable
  id: StainImprovementsCoat
  components:
  - type: ToggleableClothing
    clothingPrototypes:
      head: ClothingHeadHatHoodWinterDefault
      gloves: ClothingHandsGlovesColorBlack
";

    [Test]
    public async Task ProductionClothingWaterAndSoapHaveCleaningDependencies()
    {
        await using var pair = await PoolManager.GetServerClient();
        var server = pair.Server;
        var map = await pair.CreateTestMap();

        await server.WaitAssertion(() =>
        {
            var entMan = server.EntMan;
            var solutions = server.System<SharedSolutionContainerSystem>();
            foreach (var prototype in new[]
            {
                "ClothingHandsGlovesColorBlack", "ClothingHeadHatTophat",
                "ClothingHeadHelmetHardsuitEngineering", "ClothingHeadHatHoodWinterDefault",
                "ClothingMaskGas", "ClothingOuterWinterCoat", "ClothingShoesColorBlack",
                "ClothingUniformJumpsuitColorGrey",
            })
            {
                var clothing = entMan.SpawnEntity(prototype, map.GridCoords);
                Assert.That(entMan.HasComponent<StainableComponent>(clothing), Is.True, prototype);
                Assert.That(entMan.GetComponent<ExaminableSolutionComponent>(clothing).Solution,
                    Is.EqualTo("stain"), prototype);
                Assert.That(solutions.TryGetSolution(clothing, "stain", out _, out var stain), Is.True, prototype);
                Assert.That(stain.MaxVolume, Is.EqualTo(FixedPoint2.New(1)), prototype);
            }

            var water = entMan.SpawnEntity("FloorWaterEntity", map.GridCoords);
            var wash = entMan.GetComponent<WaterClothingWashComponent>(water);
            Assert.That(wash.WashAmount, Is.EqualTo(FixedPoint2.New(0.1f)));
            Assert.That(wash.WashInterval, Is.EqualTo(1f));
            Assert.That(entMan.GetComponent<StepTriggerComponent>(water).RequiredTriggeredSpeed, Is.Zero);
            var soap = entMan.SpawnEntity("Soap", map.GridCoords);
            Assert.That(entMan.HasComponent<CleansForensicsComponent>(soap), Is.True);
        });

        await pair.CleanReturnAsync();
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task PartialCleaningVisitsEachWornAndAttachedItemOnce(bool deployAttached)
    {
        await using var pair = await PoolManager.GetServerClient();
        var server = pair.Server;
        var map = await pair.CreateTestMap();

        await server.WaitAssertion(() =>
        {
            var entMan = server.EntMan;
            var inventory = server.System<InventorySystem>();
            var solutions = server.System<SharedSolutionContainerSystem>();
            var stains = server.System<SharedStainSystem>();
            var wearer = entMan.SpawnEntity("StainImprovementsWearer", map.GridCoords);
            var coat = entMan.SpawnEntity("StainImprovementsCoat", map.GridCoords);
            var uniform = entMan.SpawnEntity("ClothingUniformJumpsuitColorGrey", map.GridCoords);
            Assert.That(inventory.TryEquip(wearer, coat, "outerClothing"), Is.True);
            Assert.That(inventory.TryEquip(wearer, uniform, "jumpsuit"), Is.True);
            var toggleable = entMan.GetComponent<ToggleableClothingComponent>(coat);
            Assert.That(toggleable.ClothingUids.Count, Is.EqualTo(2));
            if (deployAttached)
            {
                foreach (var (attached, slot) in toggleable.ClothingUids)
                {
                    server.System<ToggleableClothingSystem>().EquipClothing(wearer, (coat, toggleable), attached, slot);
                    Assert.That(inventory.TryGetSlotEntity(wearer, slot, out var equipped), Is.True);
                    Assert.That(equipped, Is.EqualTo(attached));
                }
            }

            var contents = new List<Solution>();
            foreach (var item in toggleable.ClothingUids.Keys.Append(coat).Append(uniform))
            {
                Assert.That(solutions.TryGetSolution(item, "stain", out var solution, out var stain), Is.True);
                Assert.That(solutions.TryAddSolution(solution.Value, new Solution("Blood", FixedPoint2.New(1))), Is.True);
                contents.Add(stain);
            }

            stains.CleanEquippedClothing(wearer, FixedPoint2.Zero);
            stains.CleanEquippedClothing(wearer, FixedPoint2.New(-1));
            Assert.That(contents.All(stain => stain.Volume == FixedPoint2.New(1)), Is.True);
            stains.CleanEquippedClothing(wearer, FixedPoint2.New(0.1f));
            Assert.That(contents.All(stain => stain.Volume == FixedPoint2.New(0.9f)), Is.True,
                "Deployed attachments are also inventory items, but must only be washed once.");

            Assert.That(solutions.TryGetSolution(coat, "stain", out var coatSolution), Is.True);
            solutions.RemoveAllSolution(coatSolution.Value);
            Assert.That(stains.HasStains(coat), Is.True, "Attached-only stains must be discoverable.");
            Assert.That(stains.CleanStains(coat), Is.True);
            Assert.That(stains.HasStains(coat), Is.False);
            Assert.That(contents.Take(3).All(stain => stain.Volume == FixedPoint2.Zero), Is.True);
            Assert.That(contents.Last().Volume, Is.EqualTo(FixedPoint2.New(0.9f)));
            stains.CleanEquippedClothing(wearer);
            Assert.That(contents.All(stain => stain.Volume == FixedPoint2.Zero), Is.True);
            Assert.That(stains.CleanStains(coat), Is.False);
        });

        await pair.CleanReturnAsync();
    }

    [Test]
    // Seed contacts directly to test washing independently of physics.
#pragma warning disable RA0002
    public async Task WaterWashIsGradualDeduplicatesOverlapsAndResetsAfterLeaving()
    {
        await using var pair = await PoolManager.GetServerClient();
        var server = pair.Server;
        var map = await pair.CreateTestMap();

        await server.WaitAssertion(() =>
        {
            var entMan = server.EntMan;
            var wearer = entMan.SpawnEntity("StainImprovementsWearer", map.GridCoords);
            var uniform = entMan.SpawnEntity("ClothingUniformJumpsuitColorGrey", map.GridCoords);
            Assert.That(server.System<InventorySystem>().TryEquip(wearer, uniform, "jumpsuit"), Is.True);
            var solutions = server.System<SharedSolutionContainerSystem>();
            Assert.That(solutions.TryGetSolution(uniform, "stain", out var solution, out var stain), Is.True);
            Assert.That(solutions.TryAddSolution(solution.Value, new Solution("Blood", FixedPoint2.New(1))), Is.True);

            // Seed contacts to isolate the wash clock from physics and step-trigger timing.
            var triggers = new List<StepTriggerComponent>();
            for (var i = 0; i < 2; i++)
            {
                var water = entMan.SpawnEntity("FloorWaterEntity", map.GridCoords.Offset(new Vector2(i, 0)));
                entMan.AddComponent<StepTriggerActiveComponent>(water);
                var trigger = entMan.GetComponent<StepTriggerComponent>(water);
                trigger.CurrentlySteppedOn.Add(wearer);
                triggers.Add(trigger);
            }

            var wash = server.System<WaterClothingWashSystem>();
            wash.Update(0.5f);
            Assert.That(stain.Volume, Is.EqualTo(FixedPoint2.New(1)));
            wash.Update(0.5f);
            Assert.That(stain.Volume, Is.EqualTo(FixedPoint2.New(0.9f)));
            wash.Update(0.5f);
            foreach (var trigger in triggers)
                trigger.CurrentlySteppedOn.Clear();
            wash.Update(0.5f);
            foreach (var trigger in triggers)
                trigger.CurrentlySteppedOn.Add(wearer);
            wash.Update(0.5f);
            Assert.That(stain.Volume, Is.EqualTo(FixedPoint2.New(0.9f)), "Leaving water resets partial exposure.");
            wash.Update(0.5f);
            Assert.That(stain.Volume, Is.EqualTo(FixedPoint2.New(0.8f)));
            wash.Update(2.5f);
            Assert.That(stain.Volume, Is.EqualTo(FixedPoint2.New(0.6f)), "Long frames retain every wash tick.");
        });

        await pair.CleanReturnAsync();
    }
#pragma warning restore RA0002

    [TestCase(false, false)]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(true, true)]
    public async Task SoapCleansStainOnlyTargetsAndAttachedClothingOnlyOnCompletion(bool hasForensics, bool cancel)
    {
        await using var pair = await PoolManager.GetServerClient();
        var server = pair.Server;
        var map = await pair.CreateTestMap();
        var contents = new List<Solution>();
        EntityUid coat = default;

        await server.WaitAssertion(() =>
        {
            var entMan = server.EntMan;
            var user = entMan.SpawnEntity("StainImprovementsWearer", map.GridCoords);
            var hands = server.System<SharedHandsSystem>();
            hands.AddHand(user, "right", HandLocation.Right);
            var soap = entMan.SpawnEntity("Soap", map.GridCoords);
            Assert.That(hands.TryPickup(user, soap, checkActionBlocker: false, animate: false), Is.True);
            // The completion event belongs to the cleaner, not its optional forensic evidence.
            entMan.RemoveComponent<ForensicsComponent>(soap);
            var cleaner = entMan.GetComponent<CleansForensicsComponent>(soap);
            cleaner.CleanDelay = 0.05f;
            coat = entMan.SpawnEntity("StainImprovementsCoat", map.GridCoords);
            if (hasForensics)
            {
                var evidence = entMan.EnsureComponent<ForensicsComponent>(coat);
                evidence.Fingerprints.Add("old-print");
                evidence.DNAs.Add(("protected-dna", TimeSpan.Zero));
                evidence.CanDnaBeCleaned = false;
            }
            else
                entMan.RemoveComponent<ForensicsComponent>(coat);

            var solutions = server.System<SharedSolutionContainerSystem>();
            var attached = entMan.GetComponent<ToggleableClothingComponent>(coat).ClothingUids.Keys;
            foreach (var item in attached.Append(coat))
            {
                Assert.That(solutions.TryGetSolution(item, "stain", out var solution, out var stain), Is.True);
                Assert.That(solutions.TryAddSolution(solution.Value, new Solution("Blood", FixedPoint2.New(1))), Is.True);
                contents.Add(stain);
            }

            Assert.That(server.System<ForensicsSystem>().TryStartCleaning((soap, cleaner), user, coat), Is.True);
            Assert.That(contents.All(stain => stain.Volume == FixedPoint2.New(1)), Is.True);
            var doAfter = entMan.GetComponent<DoAfterComponent>(user).DoAfters.Values.Single();
            if (cancel)
                server.System<SharedDoAfterSystem>().Cancel(doAfter.Id);
        });

        await server.WaitRunTicks(10);

        await server.WaitAssertion(() =>
        {
            Assert.That(contents.All(stain => stain.Volume == (cancel ? FixedPoint2.New(1) : FixedPoint2.Zero)), Is.True);
            if (hasForensics)
            {
                var evidence = server.EntMan.GetComponent<ForensicsComponent>(coat);
                Assert.That(evidence.Fingerprints.Contains("old-print"), Is.EqualTo(cancel));
                Assert.That(evidence.DNAs, Does.Contain(("protected-dna", TimeSpan.Zero)));
                Assert.That(evidence.Residues.Count, Is.EqualTo(cancel ? 0 : 1));
            }
        });

        await pair.CleanReturnAsync();
    }

    [Test]
    public async Task WringTakesFiveSecondsAndSpillsMainAndAttachedStainsOneTileAhead()
    {
        await using var pair = await PoolManager.GetServerClient();
        var server = pair.Server;
        var map = await pair.CreateTestMap();
        var contents = new List<Solution>();
        EntityUid user = default;

        await server.WaitAssertion(() =>
        {
            var entMan = server.EntMan;
            user = entMan.SpawnEntity("StainImprovementsWearer", map.GridCoords);
            server.System<SharedTransformSystem>().SetLocalRotation(user, Angle.FromDegrees(90));
            // The default test grid has only one tile; the forward spill needs floor beneath it too.
            server.System<SharedMapSystem>().SetTile(map.Grid.Owner, map.Grid.Comp,
                map.GridCoords.Offset(Vector2.UnitX), map.Tile.Tile);
            var hands = server.System<SharedHandsSystem>();
            hands.AddHand(user, "right", HandLocation.Right);
            var coat = entMan.SpawnEntity("StainImprovementsCoat", map.GridCoords);
            Assert.That(hands.TryPickup(user, coat, checkActionBlocker: false, animate: false), Is.True);
            var solutions = server.System<SharedSolutionContainerSystem>();
            var attached = entMan.GetComponent<ToggleableClothingComponent>(coat).ClothingUids.Keys;
            foreach (var item in attached.Append(coat))
            {
                Assert.That(solutions.TryGetSolution(item, "stain", out var solution, out var stain), Is.True);
                Assert.That(solutions.TryAddSolution(solution.Value, new Solution("Blood", FixedPoint2.New(1))), Is.True);
                contents.Add(stain);
            }

            var verb = server.System<SharedVerbSystem>().GetLocalVerbs(coat, user, typeof(Verb))
                .Single(verb => verb.Text == Loc.GetString("stain-verb-wring"));
            verb.Act!.Invoke();
            var doAfter = entMan.GetComponent<DoAfterComponent>(user).DoAfters.Values.Single();
            Assert.That(doAfter.Args.Delay, Is.EqualTo(TimeSpan.FromSeconds(5)));
            Assert.That(contents.All(stain => stain.Volume == FixedPoint2.New(1)), Is.True);
        });

        await server.WaitRunTicks(310);

        await server.WaitAssertion(() =>
        {
            var entMan = server.EntMan;
            Assert.That(contents.All(stain => stain.Volume == FixedPoint2.Zero), Is.True);
            var puddles = entMan.EntityQueryEnumerator<PuddleComponent, TransformComponent>();
            var found = false;
            while (puddles.MoveNext(out var uid, out var puddle, out var xform))
            {
                if (xform.GridUid != map.Grid.Owner)
                    continue;
                Assert.That(xform.LocalPosition, Is.EqualTo(new Vector2(1.5f, 0.5f)));
                Assert.That(server.System<SharedSolutionContainerSystem>()
                    .TryGetSolution(uid, puddle.SolutionName, out _, out var solution), Is.True);
                Assert.That(solution.Volume, Is.EqualTo(FixedPoint2.New(3)));
                found = true;
            }
            Assert.That(found, Is.True);
        });

        await pair.CleanReturnAsync();
    }
}
