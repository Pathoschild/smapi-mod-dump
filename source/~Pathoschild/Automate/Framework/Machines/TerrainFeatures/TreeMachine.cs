/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Common.Utilities;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.WildTrees;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace Pathoschild.Stardew.Automate.Framework.Machines.TerrainFeatures
{
    /// <summary>A tree machine that provides output.</summary>
    /// <remarks>Derived from <see cref="Tree.shake"/>.</remarks>
    internal class TreeMachine : BaseMachine<Tree>
    {
        /*********
        ** Fields
        *********/
        /// <summary>Get a dropped item if its fields match.</summary>
        private readonly IReflectedMethod TryGetDrop;

        /// <summary>Whether to collect moss on the tree.</summary>
        private readonly bool CollectMoss;

        /// <summary>The moss item to drop.</summary>
        private readonly Cached<Item?> MossDrop;

        /// <summary>The seed items to drop.</summary>
        private readonly Cached<Stack<Item>> SeedDrops;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="tree">The underlying tree.</param>
        /// <param name="location">The machine's in-game location.</param>
        /// <param name="tile">The tree's tile position.</param>
        /// <param name="collectMoss">Whether to collect moss on the tree.</param>
        /// <param name="reflection">Simplifies access to private code.</param>
        public TreeMachine(Tree tree, GameLocation location, Vector2 tile, bool collectMoss, IReflectionHelper reflection)
            : base(tree, location, BaseMachine.GetTileAreaFor(tile))
        {
            this.CollectMoss = collectMoss;
            this.TryGetDrop = reflection.GetMethod(tree, "TryGetDrop");

            // create cached output fields
            // These are needed because the tree can drop multiple items at once, but the chest may only have space for
            // some of them. Since the items are normally just dropped on the ground, trees have no way to track
            // partial collection. These cache fields let us fetch the drops once and then output them incrementally.
            this.MossDrop = new Cached<Item?>(
                getCacheKey: () => $"{Game1.season},{Game1.dayOfMonth},{tree.hasMoss.Value}",
                fetchNew: this.InitMossOutput
            );
            this.SeedDrops = new Cached<Stack<Item>>(
                getCacheKey: () => $"{Game1.season},{Game1.dayOfMonth},{tree.hasSeed.Value}",
                fetchNew: this.InitSeedOutput
            );
        }

        /// <summary>Get the machine's processing state.</summary>
        public override MachineState GetState()
        {
            if (this.Machine.growthStage.Value < Tree.treeStage || this.Machine.stump.Value)
                return MachineState.Disabled;

            return this.CanCollectMoss() || this.CanCollectSeeds()
                ? MachineState.Done
                : MachineState.Processing;
        }

        /// <summary>Get the output item.</summary>
        public override ITrackedStack? GetOutput()
        {
            Tree tree = this.Machine;

            // moss
            if (this.CanCollectMoss())
            {
                Item? mossDrop = this.MossDrop.Value;
                if (mossDrop is not null)
                    return new TrackedItem(mossDrop, onEmpty: _ => tree.hasMoss.Value = false);
            }

            // seeds
            if (this.CanCollectSeeds())
            {
                Stack<Item> seedDrops = this.SeedDrops.Value;
                if (seedDrops.TryPeek(out Item? nextSeed))
                {
                    return new TrackedItem(nextSeed, onEmpty: _ =>
                    {
                        if (seedDrops.Count > 0)
                            seedDrops.Pop();

                        if (seedDrops.Count == 0)
                            tree.hasSeed.Value = false;
                    });
                }
            }

            return null;
        }

        /// <summary>Provide input to the machine.</summary>
        /// <param name="input">The available items.</param>
        /// <returns>Returns whether the machine started processing an item.</returns>
        public override bool SetInput(IStorage input)
        {
            return false; // no input
        }

        /// <summary>Get whether a tree can be automated.</summary>
        /// <param name="tree">The tree to automate.</param>
        public static bool CanAutomate(Tree tree)
        {
            WildTreeData? data = tree.GetData();
            if (data is null)
                return false;

            return
                data.GrowsMoss
                || (
                    data.SeedOnShakeChance > 0
                    && (data.SeedItemId != null || data.SeedDropItems?.Count > 0)
                );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get whether the tree has moss that can be collected.</summary>
        private bool CanCollectMoss()
        {
            return this.CollectMoss && this.Machine.hasMoss.Value;
        }

        /// <summary>Get whether the tree has seeds that can be collected.</summary>
        private bool CanCollectSeeds()
        {
            return
                this.Machine.hasSeed.Value
                && (Game1.IsMultiplayer || Game1.player.ForagingLevel >= 1);
        }

        /// <summary>Get the moss to drop for the current tree.</summary>
        /// <remarks>Derived from <see cref="Tree.shake"/>.</remarks>
        private Item? InitMossOutput()
        {
            return this.CanCollectMoss()
                ? Tree.CreateMossItem()
                : null;
        }

        /// <summary>Get the seeds to drop for the current tree.</summary>
        /// <remarks>Derived from <see cref="Tree.shake"/>.</remarks>
        private Stack<Item> InitSeedOutput(Stack<Item>? stack)
        {
            stack ??= new Stack<Item>();
            stack.Clear();

            if (this.CanCollectSeeds())
            {
                WildTreeData? data = this.Machine.GetData();

                if (data is not null)
                {
                    bool dropDefaultSeed = true;

                    // add SeedDropItems drops
                    if (data.SeedDropItems?.Count > 0)
                    {
                        foreach (WildTreeSeedDropItemData? drop in data.SeedDropItems)
                        {
                            if (drop is null)
                                continue;

                            Item? seed = this.TryGetDrop.Invoke<Item?>(drop, Game1.random, Game1.player, nameof(data.SeedDropItems), null, null);
                            if (seed is null)
                                continue;

                            stack.Push(this.PrepareSeedItem(seed));

                            if (!drop.ContinueOnDrop)
                            {
                                dropDefaultSeed = false;
                                break;
                            }
                        }
                    }

                    // drop default seed
                    if (dropDefaultSeed && data.SeedItemId is not null)
                    {
                        Item seed = this.PrepareSeedItem(
                            ItemRegistry.Create(data.SeedItemId)
                        );
                        stack.Push(seed);
                    }

                    // random Qi bean drop if tree has a seed
                    if (stack.Count > 0 && Game1.random.NextDouble() <= 0.5 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
                        stack.Push(ItemRegistry.Create("(O)890"));
                }
            }

            return stack;
        }

        /// <summary>Apply profession bonuses and tracking data to a seed item before it's output.</summary>
        /// <param name="seed">The item that was produced.</param>
        private Item PrepareSeedItem(Item seed)
        {
            if (Game1.player.professions.Contains(Farmer.botanist) && seed.HasContextTag("forage_item"))
                seed.Quality = SObject.bestQuality;

            return seed;
        }
    }
}
