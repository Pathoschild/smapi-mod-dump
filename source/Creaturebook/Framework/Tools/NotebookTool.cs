/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KediDili/Creaturebook
**
*************************************************/

using System;
using StardewValley;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using System.IO;
using System.Xml.Serialization;
using System.Linq;
using Creaturebook.Framework.Models;

namespace Creaturebook.Framework.Tools
{
    [XmlType("Mods_KediDili_NotebookTool")]
    public class NotebookTool : Tool
    {
        public bool IsOnAttractor;

        private Lazy<Texture2D> texture = new(() =>
        {
            return Game1.content.Load<Texture2D>(Path.Combine("KediDili.Creaturebook", "NoteItem"));
        });

        public NotebookTool() : base("NotebookTool", 0, 245, 0, false, numAttachmentSlots: 1)
        {                                         //245 is empty space on tools spritesheet, useful if you don't want your tool to be sprited

        }
        public override Item getOne()
        {
            return new NotebookTool();
        }

        protected override string loadDisplayName()
        {
            return ModEntry.Helper.Translation.Get("CB.Notebook.Item.Name");
        }

        protected override string loadDescription()
        {
            return ModEntry.Helper.Translation.Get("CB.Notebook.Item.Desc");
        }

        //who.FarmerSprite.StopAnimation(); method yeets farmer animation with tool, useful if you don't want your tool to have special farmer animation
        public override void endUsing(GameLocation location, Farmer who)
        {
            who.FarmerSprite.StopAnimation();
            DoFunction(location, Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y, 0, who);
        }
        public override int attachmentSlots()
        {
            return 1;
        }

        public override bool onRelease(GameLocation location, int x, int y, Farmer who)
        {
            return true;
        }
        public override void drawAttachments(SpriteBatch b, int x, int y)
        {
            if (attachments[0] == null)
            {
                b.Draw(Game1.menuTexture, new Vector2(x + 5, y), Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 43), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.86f);
            }
            else
            {
                b.Draw(Game1.menuTexture, new Vector2(x + 5, y), Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.86f);
                attachments[0].drawInMenu(b, new Vector2(x + 5, y), 1f);
            }
        }

        public override StardewValley.Object attach(StardewValley.Object o)
        {
            if (attachments[0] == null)
            {
                attachments[0] = o;
                return null;
            }
            else
            {
                StardewValley.Object attachedObject = attachments[0];
                attachments[0] = null;
                return attachedObject;
            }
        }

        public override bool canThisBeAttached(StardewValley.Object o)
        {
            return true;
        }

        public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
        {
            base.DoFunction(location, x, y, power, who);

            Attractor attractor = new();
            Vector2 cursorTile = ModEntry.Helper.Input.GetCursorPosition().GrabTile;
            for (int i = 0; i < ModEntry.Attractors.Count; i++)
                if (Game1.currentLocation.Objects.ContainsKey(cursorTile) && Game1.currentLocation.Objects[cursorTile].Name == ModEntry.Attractors[i].Name)
                {
                    IsOnAttractor = ModEntry.Attractors[i].IsEligible(cursorTile, "Ready", i); 
                    attractor = ModEntry.Attractors[i];
                }
            
            if (!IsOnAttractor)
            {
                var mousePos = ModEntry.Helper.Input.GetCursorPosition().Tile;
                CheckCreatures(mousePos);
            }
            else
            {
                Discover(attractor.attracted, "");
            }
        }
        private void CheckCreatures(Vector2 mousepos)
        {  
            foreach (var Characters in Game1.currentLocation.characters)
            {
                foreach (var chapter in ModEntry.Chapters)
                {
                    for (int i = 0; i < chapter.Creatures.Length; i++)
                    {
                        string ID = Convert.ToString(chapter.Creatures[i].ID);
                        if (chapter.Creatures[i].OverrideDefaultNaming != null && chapter.Creatures[i].OverrideDefaultNaming.Length > 0)
                        {
                            foreach (var item in chapter.Creatures[i].OverrideDefaultNaming)
                            {
                                if ((Characters.Name.Equals(chapter.CreatureNamePrefix + "_" + ID) || item.Equals(Characters.Name)) && Characters.getTileLocation() == mousepos && Game1.player.modData[ModEntry.MyModID + "_IsNotebookObtained"] == "true")
                                {
                                    Game1.playSound("click");
                                    bool discover = Game1.player.modData[ModEntry.MyModID + "_" + chapter.PackID + "." + chapter.CreatureNamePrefix + "_" + ID] == "null";
                                    Discover(discover, false, "", chapter, chapter.Creatures[i]);
                                    return;
                                }
                            }
                        }
                        else if (chapter.Creatures[i].OverrideDefaultNaming == null)
                        {
                            if (Characters.Name.Equals(chapter.CreatureNamePrefix + "_" + ID) && Characters.getTileLocation() == mousepos && Game1.player.modData[ModEntry.MyModID + "_IsNotebookObtained"] == "true")
                            {
                                Game1.playSound("click");
                                bool discover = Game1.player.modData[ModEntry.MyModID + "_" + chapter.PackID + "." + chapter.CreatureNamePrefix + "_" + ID] == "null";
                                Discover(discover, false, "", chapter, chapter.Creatures[i]);
                                return;
                            }
                        }
                        else if (attachments[0] != null)
                        {
                            int result = -1;
                            if (string.IsNullOrEmpty(chapter.Creatures[i].UseThisItem))
                                continue;
                            else
                            {
                                int itemID = 0;
                                if (!int.TryParse(chapter.Creatures[i].UseThisItem, out result) && ModEntry.Helper.ModRegistry.IsLoaded("spacechase0.JsonAssets") && ModEntry.jsonAssetsAPI != null)
                                {
                                    itemID = ModEntry.jsonAssetsAPI.GetObjectId(chapter.Creatures[i].UseThisItem);
                                }
                                if (attachments[0].ParentSheetIndex.ToString() == chapter.Creatures[i].UseThisItem || attachments[0].ParentSheetIndex == itemID)
                                {
                                    Game1.playSound("click");
                                    bool discover = Game1.player.modData[ModEntry.MyModID + "_" + chapter.PackID + "." + chapter.CreatureNamePrefix + "_" + ID] == "null";
                                    Discover(discover, false, "", chapter, chapter.Creatures[i]);
                                    attachments[0] = null;
                                    return;
                                }
                                else if (chapter.EnableSets)
                                {
                                    for (int l = 0; l < chapter.Sets.Length; l++)
                                    {
                                        if (attachments[0].ParentSheetIndex == chapter.Sets[l].DiscoverWithThisItem && 0 != chapter.Sets[l].DiscoverWithThisItem)
                                        {
                                            Game1.playSound("click");
                                            int random2 = Game1.random.Next(chapter.Sets[l].CreaturesBelongingToThisSet.Length);
                                            bool discover = Game1.player.modData[ModEntry.MyModID + "_" + chapter.PackID + "." + chapter.CreatureNamePrefix + "_" + ID] == "null";
                                            Discover(discover, false, "", chapter, chapter.Creatures[random2]);
                                            attachments[0] = null;
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                        else 
                        {
                            for (int x = 0; x < Game1.currentLocation.Map.Layers.Count; x++)
                            {
                                for (int l = 0; l < Game1.currentLocation.Map.Layers[x].Tiles.Array.Length; l++)
                                {
                                    for (int e = 0; e < Game1.currentLocation.Map.Layers[x].Tiles.Array.Length; e++)
                                    {
                                        if (Game1.currentLocation.Map.Layers[x].Tiles.Array[l, e] is null)
                                        return;
                                        foreach (var property in Game1.currentLocation.Map.Layers[x].Tiles.Array[l, e].Properties)
                                        {
                                            if (Game1.currentLocation.Map.Layers[x].Id == "Back" && property.Key == "Creaturebook" && property.Value.ToString().StartsWith("Discover"))
                                            {
                                                Game1.playSound("click");
                                                bool discover = Game1.player.modData[ModEntry.MyModID + "_" + property.Value.ToString()[8..]] == "null";
                                                Discover(discover, true, property.Value.ToString()[8..], chapter, chapter.Creatures[i]);
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public void Discover(Creature creature, string ID)
        {
            Chapter chapter = Chapter.FindChapter(ID, creature.prefix, creature);

            IsCreatureDiscovered isCreatureDiscovered = new();
            
            Discover(!Convert.ToBoolean(isCreatureDiscovered.GetValues(chapter.CreatureNamePrefix).ToArray().ToString()), false, null, chapter, creature);
        }
        public void Discover(bool ShouldBeDiscovered, bool IsFromTileData, string property, Chapter chapter, Creature creature)
        {
            SDate CurrentDate = SDate.Now();
            string convertedCurrentDate = CurrentDate.DaysSinceStart.ToString();
            string hudMessage = ModEntry.Helper.Translation.Get("CB.discoveredHUDMessage");
            string hudMessage_AlreadyDiscovered = ModEntry.Helper.Translation.Get("CB.discoveredHUDMessage.Already");
            string creatureName = string.IsNullOrEmpty(creature.NameKey) ? chapter.FromContentPack.Translation.Get(chapter.CreatureNamePrefix + "_" + creature.ID.ToString() + "_name") : chapter.FromContentPack.Translation.Get(creature.NameKey);
            string hudMessage_ChapterReward = ModEntry.Helper.Translation.Get("CB.discoveredHUDMessage.ChapterReward");
            string hudMessage_SetReward = ModEntry.Helper.Translation.Get("CB.discoveredHUDMessage.SetReward");

            if (ShouldBeDiscovered && !IsFromTileData)
            {
                Game1.player.modData[ModEntry.MyModID + "_" + chapter.PackID + "." + chapter.CreatureNamePrefix + "_" + creature.ID] = convertedCurrentDate;
                Game1.addHUDMessage(new HUDMessage(hudMessage + creatureName, 1));
                DiscoveredCreaturesFromAChapter a = new();
                IsSetFullyDiscovered f = new();
                int count = a.GetValues(chapter.FromContentPack.Manifest.UniqueID + chapter.CreatureNamePrefix).Count();
                if (count == chapter.Creatures.Length && !string.IsNullOrEmpty(chapter.RewardItem))
                {
                    Game1.addHUDMessage(new HUDMessage(hudMessage_ChapterReward, 1));
                    int x = -1;
                    int rewardID = 0;
                    if (ModEntry.jsonAssetsAPI != null && !int.TryParse(chapter.RewardItem, out x))
                        rewardID = ModEntry.jsonAssetsAPI.GetObjectId(chapter.RewardItem);
                    else
                        rewardID = Convert.ToInt32(chapter.RewardItem);
                    Game1.player.addItemByMenuIfNecessary(new StardewValley.Object(rewardID, 1));
                }
                if (chapter.EnableSets)
                {
                    for (int i = 0; i < chapter.Sets.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(chapter.Sets[i].RewardItem) && "true" == f.GetValues(chapter.Sets[i].InternalName).ToArray()[0])
                        {
                            Game1.addHUDMessage(new HUDMessage(hudMessage_SetReward, 1));
                            int x = -1;
                            int rewardID = 0;
                            if (ModEntry.jsonAssetsAPI != null && !int.TryParse(chapter.Sets[i].RewardItem, out x))
                                rewardID = ModEntry.jsonAssetsAPI.GetObjectId(chapter.Sets[i].RewardItem);
                            else
                                rewardID = Convert.ToInt32(chapter.Sets[i].RewardItem);
                            Game1.player.addItemByMenuIfNecessary(new StardewValley.Object(rewardID, 1));
                        }
                    }
                }
            }
            else if (ShouldBeDiscovered && IsFromTileData)
            {
                Game1.player.modData[ModEntry.MyModID + "_" + property] = convertedCurrentDate;
                Game1.addHUDMessage(new HUDMessage(hudMessage + creatureName, 1));
            }
            else
                Game1.addHUDMessage(new HUDMessage(hudMessage_AlreadyDiscovered, 1));
        }
        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            spriteBatch.Draw(texture.Value, location + new Vector2(32f, 32f), null, color * transparency, 0f, new Vector2(8f, 8f), 4f * scaleSize, SpriteEffects.None, layerDepth);
        }
    }
}
