using System;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.Tools;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.Reflection;
using StardewValley.Locations;
using Netcode;
using EquivalentExchange.Models;

namespace EquivalentExchange
{
    public class Alchemy
    {
        //default experience progression values, only multiplied by 10... that I'm gonna try to balance around, somehow.
        public static readonly int[] alchemyExperienceNeededPerLevel = new int[] { 1000, 3800, 7700, 13000, 21500, 33000, 48000, 69000, 100000, 150000 };

        //needed for rebound rolls
        public static Random alchemyRandom = new Random();

        //overloaded method for how much experience is needed to reach a specific level.
        public static int GetAlchemyExperienceNeededForLevel(int level)
        {
            if (level > 0 && level < 11)
                return alchemyExperienceNeededPerLevel[level - 1];
            return int.MaxValue;
        }

        //how much experience is needed to reach next level
        public static int GetAlchemyExperienceNeededForNextLevel(int alchemyLevel)
        {
            return GetAlchemyExperienceNeededForLevel(alchemyLevel + 1);
        }

        //handles draining energy/stamina on successful transmute
        public static void HandleAlchemyEnergyDeduction(double energyCost, bool isForcedStaminaDrain)
        {
            double remainingStaminaCost = energyCost;
            // if the stamina drain is "forced" it means you can't pay for this transaction with energy
            // the *entire* cost goes to stamina.
            if (!isForcedStaminaDrain)
            {
                //if you have any alkahestry energy, it will try to use as much as it can
                double alkahestryCost = (double)Math.Min(EquivalentExchange.CurrentEnergy, energyCost);

                //and deduct that from whatever stamina cost might be left over (which may be all of it)
                remainingStaminaCost -= alkahestryCost;

                Alchemy.ReduceAlkahestryEnergy(alkahestryCost);
            }
            Game1.player.Stamina -= (float)remainingStaminaCost;
            EquivalentExchange.AddAlchemyExperience((int)Math.Floor(Math.Max(energyCost, 1D)));
        }

        public static bool HandleTransmuteEvent(Item heldItem, int actualValue)
        {

            // if the recipes list doesn't contain the item you're holding, you can't transmute that.
            var recipes = EquivalentExchange.GetTransmutationFormulas();

            // sorted recipe list
            var validRecipes = recipes.GetRecipesForOutput(heldItem.parentSheetIndex);

            if (validRecipes.Count == 0)
            {
                return true;
            }

            // use more sorting magic to find a potential recipe from the player's inventory
            // prioritize the recipes by their input costs (cheapest inputs are prioritized by the function GetRecipesForOutput)
            var optimalRecipe = validRecipes.FindBestRecipe(Game1.player);

            // something has stopped us from finding a valid recipe. The player either doesn't have the necessary items
            // or doesn't have the energy to do the transmutation.
            if (optimalRecipe == null)
            {
                return true;
            }

            var breakRepeaterLoop = false;

            Alchemy.HandleAlchemyEnergyDeduction(optimalRecipe.GetEnergyCost(), false);

            if (optimalRecipe.GetEnergyCost() > EquivalentExchange.CurrentEnergy)
            {
                breakRepeaterLoop = true;
            }
            
            Alchemy.IncreaseTotalTransmuteValue((int)Math.Floor(Math.Max(1D, optimalRecipe.GetEnergyCost())));
                        
            Util.TakeItemFromPlayer(optimalRecipe.InputId, optimalRecipe.GetInputCost(), Game1.player);

            Item spawnedItem = heldItem.getOne();
            spawnedItem.Stack = optimalRecipe.GetOutputQuantity();

            Util.GiveItemToPlayer((StardewValley.Object)spawnedItem, Game1.player);            

            SoundUtil.PlayMagickySound();

            return breakRepeaterLoop;
        }        
        
        public static void IncreaseTotalTransmuteValue(int transmuteValue)
        {
            EquivalentExchange.AddTotalValueTransmuted(transmuteValue);
        }

        public static void ReduceAlkahestryEnergy(double energyCost)
        {
            EquivalentExchange.CurrentEnergy -= (float)energyCost;
        }

        internal static void RestoreAlkahestryEnergyForNewDay()
        {
            EquivalentExchange.CurrentEnergy = EquivalentExchange.MaxEnergy;
        }

        public static void HandleNormalizeEvent(Item heldItem)
        {
            //get the id of the item the player is holding
            int itemId = heldItem.parentSheetIndex;

            //declare vars to remember how many items of each quality the player has.
            float normalQuality = 0;
            int silverQuality = 0;
            int goldQuality = 0;
            int iridiumQuality = 0;
            
            //search the inventory for items of the same type
            foreach (Item inventoryItem in Game1.player.items)
            {
                if (inventoryItem == null)
                    continue;

                if (inventoryItem.parentSheetIndex != itemId)
                    continue;

                //if the item can't be cast as an object, abort.
                StardewValley.Object itemObject = inventoryItem as StardewValley.Object;
                if (itemObject == null)
                    return;

                switch (itemObject.quality)
                {
                    case 0:
                        normalQuality += itemObject.Stack;
                        break;
                    case 1:
                        silverQuality += itemObject.Stack;
                        break;
                    case 2:
                        goldQuality += itemObject.Stack;
                        break;
                    case 4:
                        iridiumQuality += itemObject.Stack;
                        break;
                    default:
                        break;
                }
            }

            //destroy all the items
            while (Game1.player.hasItemInInventory(itemId, 1))
            {
                Game1.player.removeFirstOfThisItemFromInventory(itemId);
            }

            //calculate the normalized value of all qualities
            normalQuality += (5F / 4F) * silverQuality;
            normalQuality += (3F / 2F) * goldQuality;
            normalQuality += (2F) * iridiumQuality;
            
            normalQuality -= normalQuality % 1F;

            StardewValley.Object newItemObject = new StardewValley.Object(itemId, 1);

            // Any remainder is lost.

            while (normalQuality > 0)
            {
                newItemObject = new StardewValley.Object(itemId, 1);
                Util.GiveItemToPlayer(newItemObject, Game1.player);
                normalQuality--;
            }
        }

        public static int GetToolTransmuteRadius()
        {
            if (EquivalentExchange.IsShiftKeyPressed())
                return 0;
            return Util.GetAlchemyPowerLevel(EquivalentExchange.AlchemyLevel) - 1;
        }

        //this was almost entirely stolen from spacechase0 with very little contribution on my part.
        internal static void HandleToolTransmute(Tool tool)
        {
            int alchemyLevel = (EquivalentExchange.IsShiftKeyPressed() ? 0 : Alchemy.GetToolTransmuteRadius());
            int toolLevel = tool.UpgradeLevel;

            //set last user to dodge a null pointer
            var toolPlayerFieldReflector = tool.GetType().GetField("lastUser", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            toolPlayerFieldReflector.SetValue(tool, Game1.player);

            Point hitLocation = GetMouseHitLocation();

            GameLocation location = Game1.player.currentLocation;

            bool performedAction = false;

            //getting this out of the way, helps with easily determining tool types
            bool isScythe = tool is MeleeWeapon && tool.Name.ToLower().Contains("scythe");
            bool isAxe = tool is StardewValley.Tools.Axe;
            bool isPickaxe = tool is StardewValley.Tools.Pickaxe;
            bool isHoe = tool is StardewValley.Tools.Hoe;
            bool isWateringCan = tool is StardewValley.Tools.WateringCan;

            for (int xOffset = -alchemyLevel; xOffset <= alchemyLevel; xOffset++)
            {
                for (int yOffset = -alchemyLevel; yOffset <= alchemyLevel; yOffset++)
                {
                    if (!isScythe)
                    {
                        if (!IsCapableOfWithstandingToolTransmuteCost(Game1.player, 2F))
                            return;
                    }

                    Vector2 offsetPosition = new Vector2(xOffset + hitLocation.X, yOffset + hitLocation.Y);

                    if (location.objects.ContainsKey(offsetPosition))
                    {
                        if (isAxe || isScythe || isPickaxe || isHoe)
                        {
                            var snapshotPlayerExperience = Game1.player.experiencePoints;
                            performedAction = DoToolFunction(location, Game1.player, tool, (int)offsetPosition.X, (int)offsetPosition.Y);
                            RestorePlayerExperience(snapshotPlayerExperience);
                            if (performedAction && !isScythe)
                            {
                                HandleToolTransmuteConsequence(2F);
                            }

                        }
                    }
                    else if (location.terrainFeatures.ContainsKey(offsetPosition))
                    {
                        //a terrain feature, rather than a tool check, might respond to the tool
                        TerrainFeature terrainFeature = location.terrainFeatures[offsetPosition];

                        //don't break stumps unless the player is in precision mode.
                        if (terrainFeature is Tree && isAxe && (!(terrainFeature as Tree).stump || EquivalentExchange.IsShiftKeyPressed()))
                        {
                            Netcode.NetArray<int, Netcode.NetInt> snapshotPlayerExperience = Game1.player.experiencePoints;
                            //trees get removed automatically
                            performedAction = DoToolFunction(location, Game1.player, tool, (int)offsetPosition.X, (int)offsetPosition.Y);
                            RestorePlayerExperience(snapshotPlayerExperience);

                            if (performedAction)
                            {
                                HandleToolTransmuteConsequence(2F);
                            }
                        }
                        else if (terrainFeature is Grass && location is Farm && isScythe)
                        {
                            int oldHay = (location as Farm).piecesOfHay;
                            var snapshotPlayerExperience = Game1.player.experiencePoints;
                            if (terrainFeature.performToolAction(tool, 0, offsetPosition, location))
                            {
                                location.terrainFeatures.Remove(offsetPosition);
                                //HandleToolTransmuteConsequence(); Scythe transmute is special and doesn't cost anything, but you don't get experience.
                                performedAction = true;
                            }
                            RestorePlayerExperience(snapshotPlayerExperience);

                            //hay get! spawn the sprite animation for acquisition of hay
                            if (oldHay < (location as Farm).piecesOfHay)
                            {
                                SpawnHayAnimationSprite(location, offsetPosition, Game1.player);
                            }
                        }
                        else if (terrainFeature is HoeDirt && isWateringCan && (tool as WateringCan).WaterLeft > 0)
                        {
                            //state of 0 is unwatered.
                            if ((terrainFeature as HoeDirt).state != 1)
                            {
                                var snapshotPlayerExperience = Game1.player.experiencePoints;
                                terrainFeature.performToolAction(tool, 0, offsetPosition, location);
                                RestorePlayerExperience(snapshotPlayerExperience);
                                (tool as WateringCan).WaterLeft = (tool as WateringCan).WaterLeft - 1;
                                SpawnWateringCanAnimationSprite(location, offsetPosition);
                                HandleToolTransmuteConsequence(2F);
                                performedAction = true;
                            }
                        }
                        else if (isPickaxe && terrainFeature is HoeDirt)
                        {
                            var snapshotPlayerExperience = Game1.player.experiencePoints;
                            performedAction = DoToolFunction(location, Game1.player, tool, (int)offsetPosition.X, (int)offsetPosition.Y);
                            RestorePlayerExperience(snapshotPlayerExperience);

                            if (performedAction)
                            {
                                HandleToolTransmuteConsequence(2F);
                            }
                        }
                    }
                    else if ((isPickaxe || isAxe))
                    {
                        ICollection<ResourceClump> largeResourceClusters = null;
                        if (location is Farm)
                        {
                            largeResourceClusters = (NetCollection<ResourceClump>)location.GetType().GetField("resourceClumps", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).GetValue(location);                            
                        }
                        else if (location is MineShaft)
                        {
                            largeResourceClusters = (NetObjectList<ResourceClump>)location.GetType().GetField("resourceClumps", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).GetValue(location);                            
                        }
                        else if (location is Woods)
                        {
                            largeResourceClusters = (location as Woods).stumps;                            
                        }
                        DoLargeResourceClusterAction(largeResourceClusters, tool, offsetPosition, performedAction);
                    }
                    else if (isHoe)
                    {
                        var snapshotPlayerExperience = Game1.player.experiencePoints;
                        performedAction = DoToolFunction(location, Game1.player, tool, (int)offsetPosition.X, (int)offsetPosition.Y);
                        RestorePlayerExperience(snapshotPlayerExperience);

                        if (performedAction)
                        {
                            HandleToolTransmuteConsequence(2F);
                        }
                    }
                }
            }

            if (performedAction)
            {
                SoundUtil.PlayMagickySound();
            }
        }

        private static void DoLargeResourceClusterAction(ICollection<ResourceClump> largeResourceClusters, Tool tool, Vector2 offsetPosition, bool performedAction)
        {
            if (largeResourceClusters != null)
            {
                foreach (var resourceCluster in largeResourceClusters)
                {
                    if (new Rectangle((int)resourceCluster.tile.X, (int)resourceCluster.tile.Y, resourceCluster.width, resourceCluster.height).Contains((int)offsetPosition.X, (int)offsetPosition.Y))
                    {
                        if (TryToDestroyResourceCluster(resourceCluster, tool, 1, offsetPosition))
                        {
                            performedAction = true;
                            if (resourceCluster.health <= 0)
                            {
                                largeResourceClusters.Remove(resourceCluster);
                            }
                        }

                        if (performedAction)
                        {
                            HandleToolTransmuteConsequence(2F);
                        }
                        break;
                    }
                }
            }
        }

        private static Point GetMouseHitLocation()
        {
            //I wrote this part, basically.
            double mouseX = (double)(Game1.getMouseX() + Game1.viewport.X - Game1.player.getStandingX());
            double mouseY = (double)(Game1.getMouseY() + Game1.viewport.Y - Game1.player.getStandingY());

            //figure out where the cursor position should be, relative to the player.
            return new Point((int)Math.Round((mouseX + Game1.player.getStandingX() - (Game1.tileSize / 2)) / Game1.tileSize), (int)Math.Round((mouseY + Game1.player.getStandingY() - (Game1.tileSize / 2)) / Game1.tileSize));
        }

        private static void RestorePlayerExperience(NetArray<int, NetInt> snapshotPlayerExperience)
        {
            for (int i = 0; i < Game1.player.experiencePoints.Length; i++)
            {
                Game1.player.experiencePoints[i] = snapshotPlayerExperience[i];
            }
        }

        //this is a cold copy of the performToolAction from resource cluster with all the dialog warnings removed
        //using reflection to set the shake timer since it's private.
        private static bool TryToDestroyResourceCluster(ResourceClump resourceCluster, Tool tool, int damage, Vector2 offsetPosition)
        {
            if (resourceCluster.tile != offsetPosition)
            {
                offsetPosition = resourceCluster.tile;
            }
            bool performedAction = false;
            if (tool == null)
                return performedAction;
            int debrisType = 12;
            switch (resourceCluster.parentSheetIndex)
            {
                case 622:
                    if (tool is Pickaxe && tool.upgradeLevel < 3)
                    {
                        Game1.playSound("clubhit");
                        Game1.playSound("clank");
                        Game1.player.jitterStrength = 1f;
                        return performedAction;
                    }
                    if (!(tool is Pickaxe))
                        return false;
                    Game1.playSound("hammer");
                    debrisType = 14;
                    break;
                case 672:
                    if (tool is Pickaxe && tool.upgradeLevel < 2)
                    {
                        Game1.playSound("clubhit");
                        Game1.playSound("clank");
                        Game1.player.jitterStrength = 1f;
                        return performedAction;
                    }
                    if (!(tool is Pickaxe))
                        return performedAction;
                    Game1.playSound("hammer");
                    debrisType = 14;
                    break;
                case 752:
                case 754:
                case 756:
                case 758:
                    if (!(tool is Pickaxe))
                        return performedAction;
                    Game1.playSound("hammer");
                    debrisType = 14;

                    //set shake timer with reflection, because it is private.
                    var resourceClusterShakeTimerFieldReflector = resourceCluster.GetType().GetField("shakeTimer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    resourceClusterShakeTimerFieldReflector.SetValue(resourceCluster, 500F);
                    break;
                case 600:
                    if (tool is Axe && tool.upgradeLevel < 1)
                    {
                        Game1.playSound("axe");
                        Game1.player.jitterStrength = 1f;
                        return performedAction;
                    }
                    if (!(tool is Axe))
                        return performedAction;
                    Game1.playSound("axchop");
                    break;
                case 602:
                    if (tool is Axe && tool.upgradeLevel < 2)
                    {
                        Game1.playSound("axe");
                        Game1.player.jitterStrength = 1f;
                        return performedAction;
                    }
                    if (!(tool is Axe))
                        return performedAction;
                    Game1.playSound("axchop");
                    break;
            }
            performedAction = true;
            resourceCluster.health.Set(resourceCluster.health - Math.Max(1f, (float)(tool.upgradeLevel + 1) * 0.75f));
            Game1.createRadialDebris(Game1.currentLocation, debrisType, (int)offsetPosition.X + Game1.random.Next(resourceCluster.width / 2 + 1), (int)offsetPosition.Y + Game1.random.Next(resourceCluster.height / 2 + 1), Game1.random.Next(4, 9), false, -1, false, -1);
            if ((double)resourceCluster.health <= 0.0)
            {
                if (Game1.IsMultiplayer)
                {
                    Random multiplayerRandom1 = Game1.recentMultiplayerRandom;
                }
                else
                {
                    Random random1 = new Random((int)((double)Game1.uniqueIDForThisGame + (double)offsetPosition.X * 7.0 + (double)offsetPosition.Y * 11.0 + (double)Game1.stats.DaysPlayed + (double)resourceCluster.health));
                }
                switch (resourceCluster.parentSheetIndex)
                {
                    case 622:
                        int number1 = 6;
                        if (Game1.IsMultiplayer)
                        {
                            Game1.recentMultiplayerRandom = new Random((int)offsetPosition.X * 1000 + (int)offsetPosition.Y);
                            Random multiplayerRandom2 = Game1.recentMultiplayerRandom;
                        }
                        else
                        {
                            Random random2 = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed + (int)offsetPosition.X * 7 + (int)offsetPosition.Y * 11);
                        }
                        if (Game1.IsMultiplayer)
                        {
                            Game1.createMultipleObjectDebris(386, (int)offsetPosition.X, (int)offsetPosition.Y, number1, tool.getLastFarmerToUse().uniqueMultiplayerID);
                            Game1.createMultipleObjectDebris(390, (int)offsetPosition.X, (int)offsetPosition.Y, number1, tool.getLastFarmerToUse().uniqueMultiplayerID);
                            Game1.createMultipleObjectDebris(535, (int)offsetPosition.X, (int)offsetPosition.Y, 2, tool.getLastFarmerToUse().uniqueMultiplayerID);
                        }
                        else
                        {
                            Game1.createMultipleObjectDebris(386, (int)offsetPosition.X, (int)offsetPosition.Y, number1);
                            Game1.createMultipleObjectDebris(390, (int)offsetPosition.X, (int)offsetPosition.Y, number1);
                            Game1.createMultipleObjectDebris(535, (int)offsetPosition.X, (int)offsetPosition.Y, 2);
                        }
                        Game1.playSound("boulderBreak");
                        Game1.createRadialDebris(Game1.currentLocation, 32, (int)offsetPosition.X, (int)offsetPosition.Y, Game1.random.Next(6, 12), false, -1, false, -1);
                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(5, offsetPosition * (float)Game1.tileSize, Color.White, 8, false, 100f, 0, -1, -1f, -1, 0));
                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(5, (offsetPosition + new Vector2(1f, 0.0f)) * (float)Game1.tileSize, Color.White, 8, false, 110f, 0, -1, -1f, -1, 0));
                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(5, (offsetPosition + new Vector2(1f, 1f)) * (float)Game1.tileSize, Color.White, 8, true, 80f, 0, -1, -1f, -1, 0));
                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(5, (offsetPosition + new Vector2(0.0f, 1f)) * (float)Game1.tileSize, Color.White, 8, false, 90f, 0, -1, -1f, -1, 0));
                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(5, offsetPosition * (float)Game1.tileSize + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)), Color.White, 8, false, 70f, 0, -1, -1f, -1, 0));
                        return performedAction;
                    case 672:
                    case 752:
                    case 754:
                    case 756:
                    case 758:
                        int num = resourceCluster.parentSheetIndex == 672 ? 15 : 10;
                        if (Game1.IsMultiplayer)
                        {
                            Game1.recentMultiplayerRandom = new Random((int)offsetPosition.X * 1000 + (int)offsetPosition.Y);
                            Random multiplayerRandom2 = Game1.recentMultiplayerRandom;
                        }
                        else
                        {
                            Random random3 = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed + (int)offsetPosition.X * 7 + (int)offsetPosition.Y * 11);
                        }
                        if (Game1.IsMultiplayer)
                            Game1.createMultipleObjectDebris(390, (int)offsetPosition.X, (int)offsetPosition.Y, num, tool.getLastFarmerToUse().uniqueMultiplayerID);
                        else
                            Game1.createRadialDebris(Game1.currentLocation, 390, (int)offsetPosition.X, (int)offsetPosition.Y, num, false, -1, true, -1);
                        Game1.playSound("boulderBreak");
                        Game1.createRadialDebris(Game1.currentLocation, 32, (int)offsetPosition.X, (int)offsetPosition.Y, Game1.random.Next(6, 12), false, -1, false, -1);
                        Color color = Color.White;
                        switch (resourceCluster.parentSheetIndex)
                        {
                            case 752:
                                color = new Color(188, 119, 98);
                                break;
                            case 754:
                                color = new Color(168, 120, 95);
                                break;
                            case 756:
                            case 758:
                                color = new Color(67, 189, 238);
                                break;
                        }
                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(48, offsetPosition * (float)Game1.tileSize, color, 5, false, 180f, 0, Game1.tileSize * 2, -1f, Game1.tileSize * 2, 0)
                        {
                            alphaFade = 0.01f
                        });
                        return performedAction;
                    case 600:
                    case 602:
                        int number2 = resourceCluster.parentSheetIndex == 602 ? 8 : 2;
                        if (Game1.IsMultiplayer)
                        {
                            Game1.recentMultiplayerRandom = new Random((int)offsetPosition.X * 1000 + (int)offsetPosition.Y);
                            Random multiplayerRandom2 = Game1.recentMultiplayerRandom;
                        }
                        else
                        {
                            Random random4 = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed + (int)offsetPosition.X * 7 + (int)offsetPosition.Y * 11);
                        }
                        if (Game1.IsMultiplayer)
                            Game1.createMultipleObjectDebris(709, (int)offsetPosition.X, (int)offsetPosition.Y, number2, tool.getLastFarmerToUse().uniqueMultiplayerID);
                        else
                            Game1.createMultipleObjectDebris(709, (int)offsetPosition.X, (int)offsetPosition.Y, number2);
                        Game1.playSound("stumpCrack");
                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(23, offsetPosition * (float)Game1.tileSize, Color.White, 4, false, 140f, 0, Game1.tileSize * 2, -1f, Game1.tileSize * 2, 0));
                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(385, 1522, (int)sbyte.MaxValue, 79), 2000f, 1, 1, offsetPosition * (float)Game1.tileSize + new Vector2(0.0f, 49f), false, false, 1E-05f, 0.016f, Color.White, 1f, 0.0f, 0.0f, 0.0f, false));
                        Game1.createRadialDebris(Game1.currentLocation, 34, (int)offsetPosition.X, (int)offsetPosition.Y, Game1.random.Next(4, 9), false, -1, false, -1);
                        return performedAction;
                }
            }
            else
            {
                var resourceClusterShakeTimerFieldReflector = resourceCluster.GetType().GetField("shakeTimer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                resourceClusterShakeTimerFieldReflector.SetValue(resourceCluster, 100F);
            }
            return performedAction;
        }

        private static void SpawnWateringCanAnimationSprite(GameLocation currentPlayerLocation, Vector2 offsetPosition)
        {
            currentPlayerLocation.temporarySprites.Add(new TemporaryAnimatedSprite(13, new Vector2(offsetPosition.X * (float)Game1.tileSize, offsetPosition.Y * (float)Game1.tileSize), Color.White, 10, Game1.random.NextDouble() < 0.5, 70f, 0, Game1.tileSize, (float)(((double)offsetPosition.Y * (double)Game1.tileSize + (double)(Game1.tileSize / 2)) / 10000.0 - 0.00999999977648258), -1, 0));
        }

        private static void SpawnHayAnimationSprite(GameLocation currentPlayerLocation, Vector2 offsetPosition, StardewValley.Farmer player)
        {
            currentPlayerLocation.temporarySprites.Add(new TemporaryAnimatedSprite(28, offsetPosition * (float)Game1.tileSize + new Vector2((float)Game1.random.Next(-Game1.pixelZoom * 4, Game1.pixelZoom * 4), (float)Game1.random.Next(-Game1.pixelZoom * 4, Game1.pixelZoom * 4)), Color.Green, 8, Game1.random.NextDouble() < 0.5, (float)Game1.random.Next(60, 100), 0, -1, -1f, -1, 0));
            currentPlayerLocation.temporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 178, 16, 16), 750f, 1, 0, player.position - new Vector2(0.0f, (float)(Game1.tileSize * 2)), false, false, player.position.Y / 10000f, 0.005f, Color.White, (float)Game1.pixelZoom, -0.005f, 0.0f, 0.0f, false)
            {
                motion = { Y = -1f },
                layerDepth = (float)(1.0 - (double)Game1.random.Next(100) / 10000.0),
                delayBeforeAnimationStart = Game1.random.Next(350)
            });
        }

        private static double GetToolTransmutationEnergyCost(double level, float cost)
        {
            return cost;
        }

        private static double GetToolTransmutationStaminaCost(double level, float cost)
        {
            double reduction = Math.Pow(Math.Floor(level / 2D), 1.3);
            return cost / reduction;
        }

        private static void HandleToolTransmuteConsequence(float cost)
        {
            Alchemy.HandleAlchemyEnergyDeduction(GetToolTransmutationEnergyCost(EquivalentExchange.AlchemyLevel, cost), false);
            Alchemy.HandleAlchemyEnergyDeduction(GetToolTransmutationStaminaCost(EquivalentExchange.AlchemyLevel, cost), true);            
            Alchemy.IncreaseTotalTransmuteValue((int)Math.Floor(cost));
        }

        public static bool IsCapableOfWithstandingToolTransmuteCost(Farmer player, float cost)
        {
            var energyCost = GetToolTransmutationEnergyCost(EquivalentExchange.AlchemyLevel, cost);
            var staminaCost = GetToolTransmutationStaminaCost(EquivalentExchange.AlchemyLevel, cost);
            double energyCostHandled = Math.Min(EquivalentExchange.CurrentEnergy, energyCost);
            double energyCostOverflow = Math.Max(0D, energyCost - energyCostHandled);
            staminaCost += energyCostOverflow;

            return player.stamina - 1 >= staminaCost;
        }

        private static bool DoToolFunction(GameLocation location, StardewValley.Farmer who, Tool tool, int x, int y)
        {
            bool performedAction = false;
            Vector2 index = new Vector2(x, y);
            Vector2 vector2 = new Vector2((float)(x + 0.5), (float)(y + 0.5));
            if (tool is MeleeWeapon && tool.Name.ToLower().Contains("scythe"))
            {
                var snapshotPlayerExperience = Game1.player.experiencePoints;
                if (location.objects[index] != null)
                {
                    StardewValley.Object hitObject = location.objects[index];
                    if (hitObject.name.Contains("Weed") && hitObject.performToolAction(tool, location))
                    {
                        if (hitObject.type == "Crafting" && hitObject.fragility != 2)
                        {
                            location.debris.Add(new Debris(hitObject.bigCraftable ? -hitObject.parentSheetIndex : hitObject.parentSheetIndex, index, index));
                        }
                        hitObject.performRemoveAction(index, location);
                        location.objects.Remove(index);
                        performedAction = true;
                    }
                }
                else if (location.terrainFeatures.ContainsKey(index) && location.terrainFeatures[index].performToolAction(tool, 0, index, (GameLocation)null))
                {
                    location.terrainFeatures.Remove(index);
                    performedAction = true;
                }
                RestorePlayerExperience(snapshotPlayerExperience);
            }
            else if (tool is Axe)
            {
                var snapshotPlayerExperience = Game1.player.experiencePoints;
                Rectangle rectangle = new Rectangle(x * Game1.tileSize, y * Game1.tileSize, Game1.tileSize, Game1.tileSize);
                location.performToolAction(tool, x, y);
                if (location.terrainFeatures.ContainsKey(index) && location.terrainFeatures[index].performToolAction(tool, 0, index, (GameLocation)null))
                {
                    location.terrainFeatures.Remove(index);
                    performedAction = true;
                }
                Rectangle boundingBox;
                if (location.largeTerrainFeatures != null)
                {
                    for (int index2 = location.largeTerrainFeatures.Count - 1; index2 >= 0; --index2)
                    {
                        boundingBox = location.largeTerrainFeatures[index2].getBoundingBox();
                        if (boundingBox.Intersects(rectangle) && location.largeTerrainFeatures[index2].performToolAction(tool, 0, index, (GameLocation)null))
                        {
                            location.largeTerrainFeatures.RemoveAt(index2);
                        }
                    }
                }
                if (location.terrainFeatures.ContainsKey(index) && location.terrainFeatures[index] is Tree)
                {
                    if (!(location.terrainFeatures[index] as Tree).stump || EquivalentExchange.IsShiftKeyPressed())
                        performedAction = true;
                }
                if (!location.Objects.ContainsKey(index) || location.Objects[index].Type == null || !location.Objects[index].performToolAction(tool, location))
                    return performedAction;
                if (location.Objects[index].type.Equals("Crafting") && location.Objects[index].fragility != 2)
                {
                    var debris1 = location.debris;
                    int objectIndex = location.Objects[index].bigCraftable ? -location.Objects[index].ParentSheetIndex : location.Objects[index].ParentSheetIndex;
                    Debris debris2 = new Debris(objectIndex, index, index);
                    debris1.Add(debris2);
                }
                location.Objects[index].performRemoveAction(index, location);
                location.Objects.Remove(index);
                performedAction = true;
                RestorePlayerExperience(snapshotPlayerExperience);
            }
            else if (tool is Pickaxe)
            {
                var snapshotPlayerExperience = Game1.player.experiencePoints;
                int power = who.toolPower;
                if (location.performToolAction(tool, x, y))
                    return true;
                StardewValley.Object objectHit = (StardewValley.Object)null;
                location.Objects.TryGetValue(index, out objectHit);
                if (objectHit == null)
                {
                    if (location.terrainFeatures.ContainsKey(index) && location.terrainFeatures[index].performToolAction(tool, 0, index, (GameLocation)null))
                    {
                        location.terrainFeatures.Remove(index);
                        performedAction = true;
                    }
                }

                if (objectHit != null)
                {
                    if (objectHit.Name.Equals("Stone"))
                    {
                        Game1.playSound("hammer");
                        if (objectHit.minutesUntilReady > 0)
                        {
                            int num3 = Math.Max(1, tool.upgradeLevel + 1);
                            objectHit.minutesUntilReady.Set(objectHit.minutesUntilReady - num3);
                            objectHit.shakeTimer = 200;
                            if (objectHit.minutesUntilReady > 0)
                            {
                                Game1.createRadialDebris(Game1.currentLocation, 14, x, y, Game1.random.Next(2, 5), false, -1, false, -1);
                                return performedAction;
                            }
                        }
                        if (objectHit.ParentSheetIndex < 200 && !Game1.objectInformation.ContainsKey(objectHit.ParentSheetIndex + 1))
                            location.TemporarySprites.Add(new TemporaryAnimatedSprite(objectHit.ParentSheetIndex + 1, 300f, 1, 2, new Vector2((float)(x) * Game1.tileSize, (float)(y) * Game1.tileSize), true, objectHit.flipped)
                            {
                                alphaFade = 0.01f
                            });
                        else
                            location.TemporarySprites.Add(new TemporaryAnimatedSprite(47, new Vector2((float)(x * Game1.tileSize), (float)(y * Game1.tileSize)), Color.Gray, 10, false, 80f, 0, -1, -1f, -1, 0));
                        Game1.createRadialDebris(location, 14, x, y, Game1.random.Next(2, 5), false, -1, false, -1);
                        location.TemporarySprites.Add(new TemporaryAnimatedSprite(46, new Vector2((float)(x * Game1.tileSize), (float)(y * Game1.tileSize)), Color.White, 10, false, 80f, 0, -1, -1f, -1, 0)
                        {
                            motion = new Vector2(0.0f, -0.6f),
                            acceleration = new Vector2(0.0f, 1f / 500f),
                            alphaFade = 0.015f
                        });
                        if (!location.Name.StartsWith("UndergroundMine"))
                        {
                            if (objectHit.parentSheetIndex == 343 || objectHit.parentSheetIndex == 450)
                            {
                                Random random = new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2 + x * 2000 + y);
                                if (random.NextDouble() < 0.035 && Game1.stats.DaysPlayed > 1U)
                                    Game1.createObjectDebris(535 + (Game1.stats.DaysPlayed <= 60U || random.NextDouble() >= 0.2 ? (Game1.stats.DaysPlayed <= 120U || random.NextDouble() >= 0.2 ? 0 : 2) : 1), x, y, tool.getLastFarmerToUse().uniqueMultiplayerID);
                                if (random.NextDouble() < 0.035 * (who.professions.Contains(21) ? 2.0 : 1.0) && Game1.stats.DaysPlayed > 1U)
                                    Game1.createObjectDebris(382, x, y, tool.getLastFarmerToUse().uniqueMultiplayerID);
                                if (random.NextDouble() < 0.01 && Game1.stats.DaysPlayed > 1U)
                                    Game1.createObjectDebris(390, x, y, tool.getLastFarmerToUse().uniqueMultiplayerID);
                            }
                            location.breakStone(objectHit.parentSheetIndex, x, y, who, new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2 + x * 4000 + y));
                        }
                        else
                            Game1.mine.checkStoneForItems(objectHit.ParentSheetIndex, x, y, who);
                        if (objectHit.minutesUntilReady > 0)
                            return performedAction;
                        location.Objects.Remove(index);
                        Game1.playSound("stoneCrack");
                        performedAction = true;
                    }
                    else
                    {
                        if (!objectHit.performToolAction(tool, location))
                        {
                            return performedAction;
                        }
                        objectHit.performRemoveAction(index, location);
                        if (objectHit.type.Equals("Crafting") && objectHit.fragility != 2)
                        {
                            var debris1 = Game1.currentLocation.debris;
                            int objectIndex = objectHit.bigCraftable ? -objectHit.ParentSheetIndex : objectHit.ParentSheetIndex;
                            Vector2 toolLocation = who.GetToolLocation(false);
                            Rectangle boundingBox = who.GetBoundingBox();
                            double x1 = (double)boundingBox.Center.X;
                            boundingBox = who.GetBoundingBox();
                            double y1 = (double)boundingBox.Center.Y;
                            Vector2 playerPosition = new Vector2((float)x1, (float)y1);
                            Debris debris2 = new Debris(objectIndex, toolLocation, playerPosition);
                            debris1.Add(debris2);
                        }
                        Game1.currentLocation.Objects.Remove(index);
                        performedAction = true;
                    }
                    RestorePlayerExperience(snapshotPlayerExperience);
                }
                else
                {
                    Game1.playSound("woodyHit");
                    if (location.doesTileHaveProperty(x, y, "Diggable", "Back") == null)
                        return false;
                    location.TemporarySprites.Add(new TemporaryAnimatedSprite(12, new Vector2((float)(x * Game1.tileSize), (float)(y * Game1.tileSize)), Color.White, 8, false, 80f, 0, -1, -1f, -1, 0)
                    {
                        alphaFade = 0.015f
                    });
                }
            }
            else if (tool is Hoe)
            {
                var snapshotPlayerExperience = Game1.player.experiencePoints;
                if (location.terrainFeatures.ContainsKey(index))
                {
                    if (location.terrainFeatures[index].performToolAction(tool, 0, index, (GameLocation)null))
                    {
                        location.terrainFeatures.Remove(index);
                        performedAction = true;
                    }
                }
                else
                {
                    if (location.objects.ContainsKey(index) && location.Objects[index].performToolAction(tool, location))
                    {
                        if (location.Objects[index].type.Equals("Crafting") && location.Objects[index].fragility != 2)
                        {
                            var debris1 = location.debris;
                            int objectIndex = location.Objects[index].bigCraftable ? -location.Objects[index].ParentSheetIndex : location.Objects[index].ParentSheetIndex;
                            Vector2 toolLocation = who.GetToolLocation(false);
                            Microsoft.Xna.Framework.Rectangle boundingBox = who.GetBoundingBox();
                            double x1 = (double)boundingBox.Center.X;
                            boundingBox = who.GetBoundingBox();
                            double y1 = (double)boundingBox.Center.Y;
                            Vector2 playerPosition = new Vector2((float)x1, (float)y1);
                            Debris debris2 = new Debris(objectIndex, toolLocation, playerPosition);
                            debris1.Add(debris2);
                        }
                        location.Objects[index].performRemoveAction(index, location);
                        location.Objects.Remove(index);
                        performedAction = true;
                    }
                    if (location.doesTileHaveProperty((int)index.X, (int)index.Y, "Diggable", "Back") != null)
                    {
                        if (location.Name.Equals("UndergroundMine") && !location.isTileOccupied(index, ""))
                        {
                            location.terrainFeatures.Add(index, (TerrainFeature)new HoeDirt());
                            performedAction = true;
                            Game1.removeSquareDebrisFromTile((int)index.X, (int)index.Y);
                            location.checkForBuriedItem((int)index.X, (int)index.Y, false, false);
                            location.temporarySprites.Add(new TemporaryAnimatedSprite(12, new Vector2(vector2.X * (float)Game1.tileSize, vector2.Y * (float)Game1.tileSize), Color.White, 8, Game1.random.NextDouble() < 0.5, 50f, 0, -1, -1f, -1, 0));
                        }
                        else if (!location.isTileOccupied(index, "") && location.isTilePassable(new xTile.Dimensions.Location((int)index.X, (int)index.Y), Game1.viewport))
                        {
                            location.makeHoeDirt(index);
                            performedAction = true;
                            Game1.removeSquareDebrisFromTile((int)index.X, (int)index.Y);
                            location.temporarySprites.Add(new TemporaryAnimatedSprite(12, new Vector2(index.X * (float)Game1.tileSize, index.Y * (float)Game1.tileSize), Color.White, 8, Game1.random.NextDouble() < 0.5, 50f, 0, -1, -1f, -1, 0));
                            location.checkForBuriedItem((int)index.X, (int)index.Y, false, false);
                        }
                    }
                }
                RestorePlayerExperience(snapshotPlayerExperience);
            }
            return performedAction;
        }
    }
}

