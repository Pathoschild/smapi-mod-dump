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
using System.Collections;

namespace NovoMundo.NPCs
{
    public class Call_Builders
    {
        public void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            Farm farm = mainfarm(out NPC npc);
            if (farm != null)
            {
                if (Game1.player.daysUntilHouseUpgrade.Value > 0)
                {
                    Game1.warpCharacter(npc, "Farm", new Vector2(farm.GetMainFarmHouseEntry().X + 4, farm.GetMainFarmHouseEntry().Y - 1));
                    animateBuilder("nmBuilder1");
                    return;
                }
                if (Game1.getFarm().isThereABuildingUnderConstruction())
                {
                    Building buildingUnderConstruction = Game1.getFarm().getBuildingUnderConstruction();
                    setWhichBuilder(buildingUnderConstruction, npc, "Farm");
                }
            }

        }
        public static Farm mainfarm(out NPC npc)
        {
            npc = null;
            if (Game1.getFarm().isThereABuildingUnderConstruction())
            {
                npc = Game1.getCharacterFromName("nmBuilder1");
                return Game1.getLocationFromName("Farm") as Farm;
            }
            return null;             
        }

        public void setWhichBuilder(Building buildingUnderConstruction, NPC npc, string locationName)
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
            animateBuilder("nmBuilder1");
            return;
        }

        public void animateBuilder(string builderName)
        {
            NPC npc = Game1.getCharacterFromName(builderName);
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
            string builderName = null;
            if (Game1.getFarm().isThereABuildingUnderConstruction())
            {
                builderName = "nmBuilder1";

            }
            NPC npc = Game1.getCharacterFromName(builderName);
            if (Game1.currentLocation.Equals(npc.currentLocation) && Utility.isOnScreen(npc.Position, 256))
            {
                Game1.playSound((Game1.random.NextDouble() < 0.1) ? "clank" : "axchop");
                npc.shake(250);
            }

        }
        public void setBuilderWorkingSoundPauses(Farmer who)
        {
            string builderName = null;
            if (Game1.getFarm().isThereABuildingUnderConstruction())
            {
                builderName = "nmBuilder1";

            }
            NPC npc = Game1.getCharacterFromName(builderName);
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
