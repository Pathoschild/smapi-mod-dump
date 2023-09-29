/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley;
using StardewModdingAPI.Events;
using System.Collections.Generic;
using NovoMundo.Farm1;
using NovoMundo.Farm2;
using NovoMundo.Managers;

namespace NovoMundo.NPCs
{
    public class Call_Builders
    {
        internal NPC npc;
        internal string locationName;
        internal bool isBuilding = false;
        internal Building buildingUnderConstruction;
       
        public void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (Game1.player.daysUntilHouseUpgrade.Value > 0)
            {
                callBuilders(-1);
                return;
            }
            if (Game1.getFarm().isThereABuildingUnderConstruction())
            {
                Game1.getFarm().removeTemporarySpritesWithIDLocal(16846f);
                Building building = Game1.getFarm().getBuildingUnderConstruction();
                Game1.getFarm().temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(399, 262, (building.daysOfConstructionLeft.Value == 1) ? 29 : 9, 43), new Vector2(building.tileX.Value + building.tilesWide.Value / 2, building.tileY.Value + building.tilesHigh.Value / 2) * 64f + new Vector2(-16f, -144f), false, 0f, Color.White)
                {
                    id = 99999f,
                    scale = 4f,
                    interval = 999999f,
                    animationLength = 1,
                    totalNumberOfLoops = 99999,
                    layerDepth = ((building.tileY.Value + building.tilesHigh.Value / 2) * 64 + 32) / 10000f
                });
                callBuilders(0);
            }
            if (Property_Manager.QuarryLand().isThereABuildingUnderConstruction())
            {
                Property_Manager.QuarryLand().removeTemporarySpritesWithIDLocal(16846f);
                Building building = Property_Manager.QuarryLand().getBuildingUnderConstruction();
                Property_Manager.QuarryLand().temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(399, 262, (building.daysOfConstructionLeft.Value == 1) ? 29 : 9, 43), new Vector2(building.tileX.Value + building.tilesWide.Value / 2, building.tileY.Value + building.tilesHigh.Value / 2) * 64f + new Vector2(-16f, -144f), false, 0f, Color.White)
                {
                    id = 99998f,
                    scale = 4f,
                    interval = 999999f,
                    animationLength = 1,
                    totalNumberOfLoops = 99999,
                    layerDepth = ((building.tileY.Value + building.tilesHigh.Value / 2) * 64 + 32) / 10000f
                });
                callBuilders(1);
            }
            if (Property_Manager.PlantationLand().isThereABuildingUnderConstruction())
            {
                Property_Manager.PlantationLand().removeTemporarySpritesWithIDLocal(16846f);
                Building building = Property_Manager.PlantationLand().getBuildingUnderConstruction();
                Property_Manager.PlantationLand().temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(399, 262, (building.daysOfConstructionLeft.Value == 1) ? 29 : 9, 43), new Vector2(building.tileX.Value + building.tilesWide.Value / 2, building.tileY.Value + building.tilesHigh.Value / 2) * 64f + new Vector2(-16f, -144f), false, 0f, Color.White)
                {
                    id = 9997f,
                    scale = 4f,
                    interval = 999999f,
                    animationLength = 1,
                    totalNumberOfLoops = 99999,
                    layerDepth = ((building.tileY.Value + building.tilesHigh.Value / 2) * 64 + 32) / 10000f
                });
                callBuilders(2);
            }
        }
        public void callBuilders(int locationType)
        {
            npc = Game1.getCharacterFromName("nmBuilder1");
            if (locationType == -1)
            {
                Game1.warpCharacter(npc, "Farm", new Vector2(Game1.getFarm().GetMainFarmHouseEntry().X + 4, Game1.getFarm().GetMainFarmHouseEntry().Y - 1));
                animateBuilder();
            }
            if (locationType == 0)
            {
                isBuilding = Game1.getFarm().isThereABuildingUnderConstruction();
                buildingUnderConstruction = Game1.getFarm().getBuildingUnderConstruction();            
                locationName = "Farm";
            }
            if (locationType == 1)
            {
                isBuilding = Property_Manager.QuarryLand().isThereABuildingUnderConstruction();
                buildingUnderConstruction = Property_Manager.QuarryLand().getBuildingUnderConstruction();
                locationName = "NMFarm1";
            }
            if (locationType == 2)
            {
                isBuilding = Property_Manager.PlantationLand().isThereABuildingUnderConstruction();
                buildingUnderConstruction = Property_Manager.PlantationLand().getBuildingUnderConstruction();
                locationName = "NMFarm2";
            }
            if (isBuilding)
            {
                setWhichBuilder();

            }
        }
        public void setWhichBuilder()
        {
            if (buildingUnderConstruction.daysUntilUpgrade.Value > 0 && buildingUnderConstruction.indoors.Value != null)
            {
                if (npc.currentLocation != null)
                {
                    npc.currentLocation.characters.Remove(npc);
                }
                npc.currentLocation = buildingUnderConstruction.indoors.Value;
                if (npc.currentLocation != null && !npc.currentLocation.characters.Contains(npc))
                {
                    npc.currentLocation.addCharacter(npc);
                }
                if (buildingUnderConstruction.nameOfIndoorsWithoutUnique.Contains("Shed"))
                {
                    npc.setTilePosition(2, 2);
                    npc.position.X -= 28f;
                }
                else
                {
                    npc.setTilePosition(1, 5);
                }
            }
            else
            {
                Game1.warpCharacter(npc, locationName, new Vector2(buildingUnderConstruction.tileX.Value + buildingUnderConstruction.tilesWide.Value / 2, buildingUnderConstruction.tileY.Value + buildingUnderConstruction.tilesHigh.Value / 2));
                npc.position.X += 16f;
                npc.position.Y -= 32f;
            }
            animateBuilder();
            return;
        }
        public void animateBuilder()
        {
            npc.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>
            {
                new FarmerSprite.AnimationFrame(24, 75),
                new FarmerSprite.AnimationFrame(25, 75),
                new FarmerSprite.AnimationFrame(26, 300, secondaryArm: false, flip: false, setBuilderWorkingSound),
                new FarmerSprite.AnimationFrame(27, 1000, secondaryArm: false, flip: false, setBuilderWorkingSoundPauses)
            });
        }
        public void setBuilderWorkingSound(Farmer who)
        {
            if (Game1.currentLocation.Equals(npc.currentLocation) && Utility.isOnScreen(npc.Position, 256))
            {
                Game1.playSound((Game1.random.NextDouble() < 0.1) ? "clank" : "axchop");
                npc.shake(250);
            }
        }
        public void setBuilderWorkingSoundPauses(Farmer who)
        {
            if (Game1.random.NextDouble() < 0.4)
            {
                npc.Sprite.CurrentAnimation[npc.Sprite.currentAnimationIndex] = new FarmerSprite.AnimationFrame(27, 300, secondaryArm: false, flip: false, setBuilderWorkingSoundPauses);
            }
            else if (Game1.random.NextDouble() < 0.25)
            {
                npc.Sprite.CurrentAnimation[npc.Sprite.currentAnimationIndex] = new FarmerSprite.AnimationFrame(23, Game1.random.Next(500, 4000), secondaryArm: false, flip: false, setBuilderWorkingSoundPauses);
            }
            else
            {
                npc.Sprite.CurrentAnimation[npc.Sprite.currentAnimationIndex] = new FarmerSprite.AnimationFrame(27, Game1.random.Next(1000, 4000), secondaryArm: false, flip: false, setBuilderWorkingSoundPauses);
            }
        }
    }

    
}

       
