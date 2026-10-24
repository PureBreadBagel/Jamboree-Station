// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Collections.Generic;
using System.Linq;
using Content.Goobstation.Maths.FixedPoint;
using Content.Shared._Funkystation.WashingMachine;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Forensics.Components;
using Content.Shared.Glue;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Components;
using Content.Shared.Inventory;
using Content.Shared.Lube;
using Content.Shared.NameModifier.Components;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.Power;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.Timing;

namespace Content.IntegrationTests.Tests._Funkystation;

[TestFixture]
public sealed class WashingMachineDecontaminationTest
{
    [TestPrototypes]
    private const string Prototypes = @"
- type: entity
  id: DecontaminationTestWasher
  components:
  - type: WashingMachine
    washTime: 60
    cooldown: 0
    bluntDamagePerSecond: 0
    selfDamagePerSecond: 0
    waterSprayChance: 0
    thumpSoundChance: 0
  - type: EntityStorage
    open: false
    airtight: false
  - type: Forensics
  - type: Appearance

- type: entity
  id: DecontaminationTestUniform
  name: test uniform
  components:
  - type: Item
    size: Tiny
  - type: Clothing
    slots: [innerclothing]
  - type: Appearance
  - type: Stainable
    maxStainVolume: 1
  - type: SolutionContainerManager
    solutions:
      stain:
        maxVol: 1

- type: entity
  id: DecontaminationTestOccupant
  components:
  - type: Item
    size: Tiny
  - type: Inventory
  - type: ContainerContainer
  - type: CreamPied
  - type: Appearance
";

    [TestCase(false)]
    [TestCase(true)]
    public async Task OnlyCompletedWashRemovesContaminantsAndRestoresNames(bool interrupted)
    {
        await using var pair = await PoolManager.GetServerClient();
        var server = pair.Server;
        var map = await pair.CreateTestMap();

        await server.WaitAssertion(() =>
        {
            var entMan = server.EntMan;
            var storage = server.System<SharedEntityStorageSystem>();
            var creamPie = server.System<SharedCreamPieSystem>();
            var appearance = server.System<SharedAppearanceSystem>();
            var machine = entMan.SpawnEntity("DecontaminationTestWasher", map.GridCoords);
            var item = entMan.SpawnEntity("DecontaminationTestUniform", map.GridCoords);
            var occupant = entMan.SpawnEntity("DecontaminationTestOccupant", map.GridCoords);
            var user = entMan.SpawnEntity(null, map.GridCoords);
            var baseName = entMan.GetComponent<MetaDataComponent>(item).EntityName;
            entMan.AddComponent<LubedComponent>(item);
            entMan.AddComponent<GluedComponent>(item);
            entMan.AddComponent<UnremoveableComponent>(item);
            var pied = entMan.GetComponent<CreamPiedComponent>(occupant);
            creamPie.SetCreamPied(occupant, pied, true);
            var dirtyName = entMan.GetComponent<MetaDataComponent>(item).EntityName;
            Assert.That(dirtyName, Is.Not.EqualTo(baseName));
            Assert.That(storage.Insert(item, machine), Is.True);
            Assert.That(storage.Insert(occupant, machine), Is.True);

            entMan.EventBus.RaiseLocalEvent(machine, new ActivateInWorldEvent(user, machine, complex: true));
            var washer = entMan.GetComponent<WashingMachineComponent>(machine);
            Assert.That(washer.State, Is.EqualTo(WashingMachineState.Washing));
            Assert.That(entMan.HasComponent<LubedComponent>(item), Is.True);
            Assert.That(entMan.HasComponent<GluedComponent>(item), Is.True);
            Assert.That(pied.CreamPied, Is.True);

            if (interrupted)
            {
                var power = new PowerChangedEvent(false, 0);
                entMan.EventBus.RaiseLocalEvent(machine, ref power);
            }
            else
            {
                washer.WashFinishTime = server.ResolveDependency<IGameTiming>().CurTime;
                server.System<SharedWashingMachineSystem>().Update(0);
            }

            Assert.Multiple(() =>
            {
                Assert.That(entMan.HasComponent<LubedComponent>(item), Is.EqualTo(interrupted));
                Assert.That(entMan.HasComponent<GluedComponent>(item), Is.EqualTo(interrupted));
                Assert.That(entMan.HasComponent<UnremoveableComponent>(item), Is.EqualTo(interrupted));
                Assert.That(entMan.HasComponent<NameModifierComponent>(item), Is.EqualTo(interrupted));
                Assert.That(entMan.GetComponent<MetaDataComponent>(item).EntityName,
                    Is.EqualTo(interrupted ? dirtyName : baseName));
                Assert.That(pied.CreamPied, Is.EqualTo(interrupted));
                Assert.That(appearance.TryGetData(occupant, CreamPiedVisuals.Creamed, out bool creamed), Is.True);
                Assert.That(creamed, Is.EqualTo(interrupted));
                Assert.That(washer.State, Is.EqualTo(WashingMachineState.Idle));
                Assert.That(washer.WashFinishTime, Is.Null);
                Assert.That(storage.IsOpen(machine), Is.True);
            });
        });

        await pair.CleanReturnAsync();
    }

    [Test]
    public async Task CompletedWashCleansContainedAndWornStainsAfterHarvestingDna()
    {
        await using var pair = await PoolManager.GetServerClient();
        var server = pair.Server;
        var map = await pair.CreateTestMap();

        await server.WaitAssertion(() =>
        {
            var entMan = server.EntMan;
            var storage = server.System<SharedEntityStorageSystem>();
            var inventory = server.System<InventorySystem>();
            var solutions = server.System<SharedSolutionContainerSystem>();
            var machine = entMan.SpawnEntity("DecontaminationTestWasher", map.GridCoords);
            var occupant = entMan.SpawnEntity("DecontaminationTestOccupant", map.GridCoords);
            var worn = entMan.SpawnEntity("DecontaminationTestUniform", map.GridCoords);
            var contained = entMan.SpawnEntity("DecontaminationTestUniform", map.GridCoords);
            var user = entMan.SpawnEntity(null, map.GridCoords);
            Assert.That(inventory.TryEquip(occupant, worn, "jumpsuit"), Is.True);

            var stains = new List<Solution>();
            var freshness = TimeSpan.FromSeconds(123);
            foreach (var (item, dna) in new[] { (worn, "worn-donor"), (contained, "contained-donor") })
            {
                Assert.That(solutions.TryGetSolution(item, "stain", out var solution, out var stain), Is.True);
                Assert.That(solutions.TryAddSolution(solution.Value,
                    new Solution("Blood", FixedPoint2.New(1), new List<ReagentData> { new DnaData { DNA = dna } })), Is.True);
                stain.Contents.Single().Reagent.Data!.OfType<DnaData>().Single().Freshness = freshness;
                stains.Add(stain);
            }

            Assert.That(storage.Insert(occupant, machine), Is.True);
            Assert.That(storage.Insert(contained, machine), Is.True);
            entMan.EventBus.RaiseLocalEvent(machine, new ActivateInWorldEvent(user, machine, complex: true));
            var washer = entMan.GetComponent<WashingMachineComponent>(machine);
            Assert.That(washer.State, Is.EqualTo(WashingMachineState.Washing));
            Assert.That(stains.All(stain => stain.Volume == FixedPoint2.New(1)), Is.True);
            var evidence = entMan.GetComponent<ForensicsComponent>(machine);
            Assert.That(evidence.DNAs, Is.Empty);

            washer.WashFinishTime = server.ResolveDependency<IGameTiming>().CurTime;
            server.System<SharedWashingMachineSystem>().Update(0);

            Assert.Multiple(() =>
            {
                Assert.That(stains.All(stain => stain.Volume == FixedPoint2.Zero && stain.Contents.Count == 0), Is.True);
                Assert.That(evidence.DNAs, Is.EquivalentTo(new[]
                {
                    ("worn-donor", freshness),
                    ("contained-donor", freshness),
                }));
                Assert.That(inventory.TryGetSlotEntity(occupant, "jumpsuit", out var equipped), Is.True);
                Assert.That(equipped, Is.EqualTo(worn));
                Assert.That(storage.IsOpen(machine), Is.True);
            });
        });

        await pair.CleanReturnAsync();
    }

    [Test]
    public async Task ExhaustedLubePickupRestoresNameWithoutCancellingPickup()
    {
        await using var pair = await PoolManager.GetServerClient();
        var server = pair.Server;
        var map = await pair.CreateTestMap();

        await server.WaitAssertion(() =>
        {
            var entMan = server.EntMan;
            var item = entMan.SpawnEntity("DecontaminationTestUniform", map.GridCoords);
            var user = entMan.SpawnEntity(null, map.GridCoords);
            var baseName = entMan.GetComponent<MetaDataComponent>(item).EntityName;
            entMan.AddComponent<LubedComponent>(item);
            Assert.That(entMan.GetComponent<MetaDataComponent>(item).EntityName, Is.Not.EqualTo(baseName));

            var pickup = new BeforeGettingEquippedHandEvent(user);
            entMan.EventBus.RaiseLocalEvent(item, ref pickup);

            Assert.Multiple(() =>
            {
                Assert.That(pickup.Cancelled, Is.False);
                Assert.That(entMan.HasComponent<LubedComponent>(item), Is.False);
                Assert.That(entMan.HasComponent<NameModifierComponent>(item), Is.False);
                Assert.That(entMan.GetComponent<MetaDataComponent>(item).EntityName, Is.EqualTo(baseName));
            });

            // Removing an already-clean item must remain harmless.
            server.System<SharedLubedSystem>().RemoveLubed(item);
            Assert.That(entMan.GetComponent<MetaDataComponent>(item).EntityName, Is.EqualTo(baseName));
        });

        await pair.CleanReturnAsync();
    }
}
