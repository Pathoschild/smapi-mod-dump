/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using DynamicGameAssets.Game;
using DynamicGameAssets.PackData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OrnithologistsGuild.Game.Critters;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace OrnithologistsGuild.Game.Items
{
    public class Binoculars : CustomObject
    {
        public int Range;

        private static readonly int AnimateDuration = 750;
        private int? AnimateElapsed;

        public Binoculars(CommonPackData data, int range) : base((ObjectPackData)data)
        {
            this.name = $"{data.ID}_Subclass";

            Range = range;
        }

        public override bool performUseAction(GameLocation location)
        {
            if (location == null || !location.IsOutdoors) return false;

            if (AnimateElapsed.HasValue) return false;
            AnimateElapsed = 0;

            return false;
        }

        private void UseBinoculars(GameLocation location, Farmer f)
        {
            List<string> alreadyIdentified = new List<string>();
            List<string> newlyIdentified = new List<string>();

            var actualRange = (Range + 0.5) * Game1.tileSize;
            var midPoint = f.position + new Vector2(0.5f * Game1.tileSize, -0.25f * Game1.tileSize);

            List<string> spottedBirdieUniqueIds = new List<string>();

            foreach (var critter in location.critters.OrderBy(c => Vector2.Distance(midPoint, c.position)))
            {
                if (critter is BetterBirdie && Vector2.Distance(midPoint, critter.position) <= actualRange)
                {
                    var birdie = (BetterBirdie)critter;
                    var id = birdie.BirdieDef.ID;

                    if (birdie.IsFlying || birdie.IsSpotted) continue;
                    if (spottedBirdieUniqueIds.Contains(birdie.BirdieDef.UniqueID))
                    {
                        birdie.IsSpotted = true;
                        continue;
                    }

                    int? newAttribute;
                    var sighting = SaveDataManager.SaveData.LifeList.GetOrAddEntry(birdie.BirdieDef, out newAttribute);

                    var contentPack = birdie.BirdieDef.ContentPackDef.ContentPack;

                    // Translations
                    var commonNameString = contentPack.Translation.Get($"birdie.{id}.commonName");
                    var scientificNameString = contentPack.Translation.Get($"birdie.{id}.scientificName");
                    var funFactString = contentPack.Translation.Get($"birdie.{id}.funFact");
                    var attributeStrings = Enumerable.Range(1, birdie.BirdieDef.Attributes).ToDictionary(i => i, i => contentPack.Translation.Get($"birdie.{id}.attribute.{i}"));

                    var lines = new List<string>();

                    if (sighting.Identified)
                    {
                        lines.Add(newAttribute.HasValue ? I18n.Items_Binoculars_NewlyIdentified() : I18n.Items_Binoculars_AlreadyIdentified());
                        lines.Add(Utilities.LocaleToUpper(commonNameString.ToString()));

                        if (scientificNameString.HasValue()) lines.Add(scientificNameString.ToString());

                        if (newAttribute.HasValue)
                        {
                            lines.Add(string.Empty);
                            lines.Add(string.Join(Utilities.GetLocaleSeparator(), attributeStrings.Values));

                            if (funFactString.HasValue())
                            {
                                lines.Add(string.Empty);
                                lines.Add(funFactString);
                            }
                        }
                    } else
                    {
                        lines.Add(I18n.Items_Binoculars_NotYetIdentified());
                        lines.Add(I18n.Items_Binoculars_Placeholder());
                        lines.Add(I18n.Items_Binoculars_Placeholder());
                        lines.Add(string.Empty);
                        lines.Add(string.Join(Utilities.GetLocaleSeparator(), attributeStrings.Select(a => sighting.Sightings.Select(s => s.Attribute).Contains(a.Key) ? a.Value : I18n.Items_Binoculars_Placeholder())));
                    }

                    Game1.drawObjectDialogue(string.Join("^", lines));

                    // Ignore the birds on consecutive uses of the binoculars
                    birdie.IsSpotted = true;
                    spottedBirdieUniqueIds.Add(birdie.BirdieDef.UniqueID);
                } else if (critter is Woodpecker && Vector2.Distance(midPoint, critter.position) <= actualRange)
                {
                    
                } else if (critter is Seagull && Vector2.Distance(midPoint, critter.position) <= actualRange)
                {

                } else if (critter is Crow && Vector2.Distance(midPoint, critter.position) <= actualRange)
                {

                } else if (critter is Owl)
                {

                }

                // ... other critter types? Bird? PerchingBird?
            }
        }

        public override bool canStackWith(ISalable other)
        {
            return false;
        }

        public override void actionWhenBeingHeld(Farmer who)
        {
            who.setRunning(false);
            who.canOnlyWalk = true;

            base.actionWhenBeingHeld(who);
        }

        public override void actionWhenStopBeingHeld(Farmer who)
        {
            who.canOnlyWalk = false;
            if (Game1.options.autoRun)
            {
                who.setRunning(true);
            }

            base.actionWhenStopBeingHeld(who);
        }

        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            if (AnimateElapsed.HasValue) {
                AnimateElapsed += Game1.currentGameTime.ElapsedGameTime.Milliseconds;
                if (AnimateElapsed <= AnimateDuration)
                {
                    var factor = Utilities.EaseOutSine(((float)AnimateElapsed.Value / (float)AnimateDuration));

                    var animatedRange = Utility.Lerp(0, Range * Game1.tileSize, factor);
                    var opacity = Utility.Lerp(0.7f, 0.1f, factor);

                    MonoGame.Primitives2D.DrawCircle(
                        spriteBatch,
                        objectPosition + new Vector2(0.5f * Game1.tileSize, 1.5f * Game1.tileSize),
                        animatedRange,
                        (int)animatedRange / 4,
                        Color.AliceBlue * opacity,
                        (Game1.tileSize / 16) * 2);
                }
                else
                {
                    // Animation complete
                    AnimateElapsed = null;

                    // TODO best place to put this? Is rendering done on a separate thread?
                    UseBinoculars(f.currentLocation, f);
                }
            }

            base.drawWhenHeld(spriteBatch, objectPosition, f);
        }
    }
}
