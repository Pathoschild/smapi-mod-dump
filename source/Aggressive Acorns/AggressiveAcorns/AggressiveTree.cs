using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using xTile.Dimensions;

namespace AggressiveAcorns
{
    internal class AggressiveTree : Tree
    {
        private GameLocation _location;
        private Vector2 _position;
        private readonly IModConfig _config = AggressiveAcorns.Config;

        /// <summary>
        /// Flag to skip first update, used to prevent spread seeds from updating the night they are created.
        /// As spread seeds are not guaranteed to be hit in the update loop of the night they are planted, clearing this
        /// flag currently relies on the AggressiveTree -> Tree -> AggressiveTree conversion around serialization.
        /// </summary>
        private bool _skipUpdate;


        [UsedImplicitly]
        public AggressiveTree()
        {
        }


        public AggressiveTree([NotNull] Tree tree)
        {
            growthStage.Value = tree.growthStage.Value;
            treeType.Value = tree.treeType.Value;
            health.Value = tree.health.Value;
            flipped.Value = tree.flipped.Value;
            stump.Value = tree.stump.Value;
            tapped.Value = tree.tapped.Value;
            hasSeed.Value = tree.hasSeed.Value;
            fertilized.Value = tree.fertilized.Value;
        }


        private AggressiveTree(int treeType, int growthStage, bool skipFirstUpdate = false)
            : base(treeType, growthStage)
        {
            _skipUpdate = skipFirstUpdate;
        }


        [NotNull]
        public Tree ToTree()
        {
            var tree = new Tree();
            tree.growthStage.Value = growthStage.Value;
            tree.treeType.Value = treeType.Value;
            tree.health.Value = health.Value;
            tree.flipped.Value = flipped.Value;
            tree.stump.Value = stump.Value;
            tree.tapped.Value = tapped.Value;
            tree.hasSeed.Value = hasSeed.Value;
            tree.fertilized.Value = fertilized.Value;

            SyncFieldToTree<NetBool, bool>(tree, "destroy");

            return tree;
        }


        public override bool isPassable([CanBeNull] Character c = null)
        {
            return health.Value <= -99 || growthStage.Value <= _config.MaxPassibleGrowthStage;
        }


        public override void dayUpdate([NotNull] GameLocation environment, Vector2 tileLocation)
        {
            _location = environment;
            _position = tileLocation;

            if (health.Value <= -100)
            {
                SetField<NetBool, bool>("destroy", true);
                _skipUpdate = true;
            }

            ValidateTapped(environment, tileLocation);

            if (!_skipUpdate && TreeCanGrow())
            {
                PopulateSeed();
                TrySpread();
                TryIncreaseStage();
                ManageHibernation();
                TryRegrow();
            }
            else
            {
                _skipUpdate = false;
            }

            // Revert to vanilla type early to prevent serialization issues in mods that serialize during the Saving event.
            // Relies on the fact that Terrain Feature iteration means that dayUpdate only won't be called again for the
            // same tileLocation.
            environment.terrainFeatures[tileLocation] = ToTree();
        }


        public override bool performToolAction(Tool t, int explosion, Vector2 tileLocation, GameLocation location)
        {
            bool prevent = _config.PreventScythe && t is MeleeWeapon;
            return !prevent && base.performToolAction(t, explosion, tileLocation, location);
        }


        // ===========================================================================================================


        private void SetField<TNetField, T>(string name, T value) where TNetField : NetField<T, TNetField>
        {
            AggressiveAcorns.ReflectionHelper.GetField<TNetField>(this, name).GetValue().Value = value;
        }


        private void SyncField<TNetField, T>(object origin, object target, string name)
            where TNetField : NetField<T, TNetField>
        {
            T value = AggressiveAcorns.ReflectionHelper.GetField<TNetField>(origin, name).GetValue().Value;
            AggressiveAcorns.ReflectionHelper.GetField<TNetField>(target, name).GetValue().Value = value;
        }


        private void SyncFieldToTree<TNetField, T>(Tree tree, string name) where TNetField : NetField<T, TNetField>
        {
            SyncField<TNetField, T>(this, tree, name);
        }


        // ===========================================================================================================

        private void ValidateTapped(GameLocation environment, Vector2 tileLocation)
        {
            if (!tapped.Value) return;

            Object objectAtTile = environment.getObjectAtTile((int) tileLocation.X, (int) tileLocation.Y);
            if (objectAtTile == null || !objectAtTile.bigCraftable.Value || objectAtTile.ParentSheetIndex != 105)
            {
                tapped.Value = false;
            }
        }

        private void TryIncreaseStage()
        {
            if (growthStage.Value >= treeStage || (growthStage.Value >= _config.MaxShadedGrowthStage && IsShaded()))
            {
                return;
            }

            // Trees experiencing winter won't grow unless fertilized or set to ignore winter.
            // In addition to this, mushroom trees won't grow if they should be hibernating, even if fertilized.
            if (ExperiencingWinter()
                && ((treeType.Value == mushroomTree && _config.DoMushroomTreesHibernate)
                    || !(_config.DoGrowInWinter || fertilized.Value)))
            {
                return;
            }

            if (_config.DoGrowInstantly)
            {
                growthStage.Value = IsShaded() ? _config.MaxShadedGrowthStage : treeStage;
            }
            else if (Game1.random.NextDouble() < _config.DailyGrowthChance || fertilized.Value)
            {
                growthStage.Value += 1;
            }
        }


        private void ManageHibernation()
        {
            if (treeType.Value != mushroomTree
                || !_config.DoMushroomTreesHibernate
                || !ExperiencesWinter())
            {
                return;
            }

            if (Game1.IsWinter)
            {
                stump.Value = true;
                health.Value = 5;
            }
            else if (Game1.IsSpring && Game1.dayOfMonth <= 1)
            {
                RegrowStumpIfNotShaded();
            }
        }


        private void TryRegrow()
        {
            if (treeType.Value == mushroomTree &&
                _config.DoMushroomTreesRegrow &&
                stump.Value &&
                (!ExperiencingWinter() || (!_config.DoMushroomTreesHibernate && _config.DoGrowInWinter)) &&
                (_config.DoGrowInstantly || Game1.random.NextDouble() < _config.DailyGrowthChance / 2))
            {
                RegrowStumpIfNotShaded();
            }
        }


        private void RegrowStumpIfNotShaded()
        {
            if (IsShaded()) return;

            stump.Value = false;
            health.Value = startingHealth;

            /*  Not currently needed as AggressiveTree is converted to Tree and back around save to allow
             *  serialization (ie. new objects created so rotation is reset).
             *  If this changes (ie. Aggressive Tree cached over save or otherwise reused), must re-enable below code.
             */
            // AggressiveAcorns.ReflectionHelper.GetField<float>(this, "shakeRotation").SetValue(0);
        }


        private void TrySpread()
        {
            if (!(_location is Farm) ||
                growthStage.Value < treeStage ||
                (Game1.IsWinter && !_config.DoSpreadInWinter) ||
                (tapped.Value && !_config.DoTappedSpread) ||
                stump.Value)
            {
                return;
            }

            foreach (Vector2 seedPos in GetSpreadLocations())
            {
                var tileX = (int) seedPos.X;
                var tileY = (int) seedPos.Y;
                if (_config.SeedsReplaceGrass &&
                    _location.terrainFeatures.TryGetValue(seedPos, out var feature) &&
                    feature is Grass)
                {
                    PlaceOffspring(seedPos);
                }
                else if (_location.isTileLocationOpen(new Location(tileX * 64, tileY * 64))
                         && !_location.isTileOccupied(seedPos)
                         && _location.doesTileHaveProperty(tileX, tileY, "Water", "Back") == null
                         && _location.isTileOnMap(seedPos))
                {
                    PlaceOffspring(seedPos);
                }
            }
        }


        private void PlaceOffspring(Vector2 position)
        {
            hasSeed.Value = false;

            var tree = new AggressiveTree(treeType.Value, 0, true);
            _location.terrainFeatures[position] = tree;
        }


        private IEnumerable<Vector2> GetSpreadLocations()
        {
            // pick random tile within +-3 x/y.
            if (Game1.random.NextDouble() < _config.DailySpreadChance)
            {
                int tileX = Game1.random.Next(-3, 4) + (int) _position.X;
                int tileY = Game1.random.Next(-3, 4) + (int) _position.Y;
                var seedPos = new Vector2(tileX, tileY);
                yield return seedPos;
            }
        }


        private void PopulateSeed()
        {
            if (growthStage.Value < treeStage || stump.Value) return;

            if (!_config.DoSeedsPersist)
            {
                hasSeed.Value = false;
            }

            if (Game1.random.NextDouble() < _config.DailySeedChance)
            {
                hasSeed.Value = true;
            }
        }


        private bool TreeCanGrow()
        {
            string prop = _location.doesTileHaveProperty((int) _position.X, (int) _position.Y, "NoSpawn", "Back");
            bool tileCanSpawnTree = prop == null || !(prop.Equals("All") || prop.Equals("Tree") || prop.Equals("True"));
            bool isBlockedSeed = growthStage.Value == 0 && _location.objects.ContainsKey(_position);
            return tileCanSpawnTree && !isBlockedSeed;
        }


        private bool ExperiencingWinter()
        {
            return Game1.IsWinter && ExperiencesWinter();
        }


        private bool ExperiencesWinter()
        {
            return _location.IsOutdoors && !(_location is Desert);
        }


        private bool IsShaded()
        {
            foreach (Vector2 adjacentTile in Utility.getSurroundingTileLocationsArray(_position))
            {
                if (_location.terrainFeatures.TryGetValue(adjacentTile, out TerrainFeature feature)
                    && feature is Tree adjTree
                    && adjTree.growthStage.Value >= treeStage
                    && !adjTree.stump.Value)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
