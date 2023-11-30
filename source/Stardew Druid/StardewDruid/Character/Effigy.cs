/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using xTile.Dimensions;
using System.Xml.Serialization;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewModdingAPI;
using System.IO;
using StardewDruid.Cast;
using StardewValley.TerrainFeatures;

namespace StardewDruid.Character
{
    public class Effigy : StardewDruid.Character.Character
    {

        public List<Vector2> ritesDone;

        public Effigy()
            : base()
        {

        }

        public Effigy(Vector2 position, string map)
            : base(position, map, "Effigy")
        {

            ritesDone = new();
        
        }

        public override bool checkAction(Farmer who, GameLocation l)
        {
           
            base.checkAction(who, l);

            if (!Mod.instance.dialogue.ContainsKey("Effigy"))
            {

                Mod.instance.dialogue["Effigy"] = new Dialogue.Effigy() { npc = this };

            }

            Mod.instance.dialogue["Effigy"].DialogueApproach();

            return true;

        }

        public override List<Vector2> RoamAnalysis()
        {

            List<Vector2> scarecrows = new();

            foreach(SerializableDictionary<Vector2,StardewValley.Object> objDictionary in currentLocation.Objects)
            {
                
                foreach (KeyValuePair<Vector2,StardewValley.Object> objPair in objDictionary)
                {

                    if (objPair.Value.IsScarecrow())
                    {

                        scarecrows.Add(objPair.Key * 64);

                    }

                }

                if(scarecrows.Count >= 20)
                {

                    break;

                }

            }

            List<Vector2> roampoints = base.RoamAnalysis();

            scarecrows.AddRange(roampoints);

            return scarecrows;

        }

        public void AnimateCast()
        {

            switch (moveDirection)
            {

                case 0:

                    string blessing = Mod.instance.ActiveBlessing();

                    switch (blessing)
                    {

                        case "water":
                            Sprite.currentFrame = 26;
                            break;

                        case "stars":
                            Sprite.currentFrame = 34;
                            break;

                        case "fates":
                            Sprite.currentFrame = 42;
                            break;

                        default: // earth
                            Sprite.currentFrame = 18;
                            break;

                    }

                    break;

                case 1:

                    Sprite.currentFrame = 17;
                    break;

                case 2:

                    Sprite.currentFrame = 16;
                    break;

                case 3:

                    Sprite.currentFrame = 19;
                    break;

            }

            Sprite.UpdateSourceRect();

        }

        public override void AnimateMovement(GameTime time)
        {

            if (timers.ContainsKey("cast"))
            {

                AnimateCast();

                return;

            }

            switch (moveDirection)
            {
                case 0:

                    Sprite.AnimateUp(time);

                    if (Sprite.currentFrame >= 8 && Sprite.currentFrame < 12) {

                        string blessing = Mod.instance.ActiveBlessing();

                        switch (blessing)
                        {

                            case "earth": // return

                                return;

                            case "water":

                                Sprite.currentFrame += 12;

                                break;

                            case "stars":

                                Sprite.currentFrame += 20;

                                break;

                            case "fates":

                                Sprite.currentFrame += 28;

                                break;

                        }

                    }

                    Sprite.UpdateSourceRect();

                    break;

                case 1:
                    Sprite.AnimateRight(time);
                    break;

                case 2:
                    Sprite.AnimateDown(time);
                    break;

                default:
                    Sprite.AnimateLeft(time);
                    break;

            }

            return;

        }

        public override void ReachedRoamPosition()
        {

            Vector2 scareVector = roamVectors[roamIndex] / 64;

            if (ritesDone.Contains(scareVector))
            {

                return;

            }

            if (!currentLocation.Objects.ContainsKey(scareVector))
            {
                return;

            }

            if (!currentLocation.Objects[scareVector].IsScarecrow())
            {
                return;

            }

            Halt();

            AnimateCast();

            timers["cast"] = 30;

            Rite effigyRite = Mod.instance.NewRite(false);

            for (int i = 1; i < 5; i++)
            {

                List<Vector2> tileVectors = ModUtility.GetTilesWithinRadius(currentLocation, scareVector, i);

                foreach (Vector2 tileVector in tileVectors)
                {

                    if (currentLocation.terrainFeatures.ContainsKey(tileVector))
                    {

                        TerrainFeature terrainFeature = currentLocation.terrainFeatures[tileVector];

                        switch (terrainFeature.GetType().Name.ToString())
                        {

                            case "HoeDirt":

                                effigyRite.effectCasts[tileVector] = new Cast.Earth.Crop(tileVector, effigyRite, true, true);

                                break;

                        }

                        continue;

                    }

                }

            }

            if (currentLocation.Name == Game1.player.currentLocation.Name && Utility.isOnScreen(Position, 128))
            {

                ModUtility.AnimateRadiusDecoration(currentLocation, scareVector, "Earth", 1f, 1f, 1500);

                Game1.player.currentLocation.playSoundPitched("discoverMineral", 1000);

            }

            effigyRite.CastEffect(false);

            ritesDone.Add(scareVector);

        }

    }

}
