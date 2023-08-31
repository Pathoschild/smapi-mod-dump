/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/i-saac-b/PostBoxMod
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.Buildings;
using StardewValley;

namespace PostBoxMod
{
    [XmlType("Mods_ib_Postbox")]
    public class Postbox : Building
    {
        // for usage with menu: unused presently
        public static bool inUse = false;
        public static List<StardewValley.Object> lastDelivery = new List<StardewValley.Object>();
        
        // necessary for regular building function
        private Rectangle baseSourceRect = new Rectangle(0, 0, 48, 48);
        private static readonly BluePrint Blueprint = new BluePrint("Postbox");

        // logging, translation
        private static IMonitor Monitor;
        private static IModHelper Helper;
        
        // configurable relationship factor, verbosity
        public static float factor = 1f;
        public static bool verbose = false;

        // current mailing target
        public static string target = "";
        public static List<Tuple<StardewValley.Object, String, Farmer>> outgoing = new List<Tuple<StardewValley.Object, String, Farmer>>();


        public static void Initialize(IMonitor monitor, ModConfig config, IModHelper helper)
        {
            Monitor = monitor;
            factor = config.PostedGiftRelationshipModifier;
            verbose = config.VerboseGifting;
            Helper = helper;
        }

        public Postbox(BluePrint b, Vector2 tileLocation) : base(Postbox.Blueprint, tileLocation)
        { 
        }

        public Postbox() : base(Postbox.Blueprint, Vector2.Zero) 
        {
            inUse = false;
            target = "";
        }

        public override Rectangle getSourceRectForMenu()
        {
            return new Rectangle(0, 0, 64, base.texture.Value.Bounds.Height);
        }

        public override void load()
        {
            base.load();
        }

        public override bool doAction(Vector2 tileLocation, Farmer who)
        {
            bool worked = false;
            if ((int)base.daysOfConstructionLeft <= 0)
            {
                List<String> friends = new List<string>();
                foreach (string name in who.friendshipData.Keys)
                {
                    if (who.friendshipData[name].GiftsToday == 0 &&
                        who.friendshipData[name].GiftsThisWeek < 2 &&
                        Game1.getCharacterFromName(name) != null)
                    {
                        friends.Add(name);
                    }
                }
                if (inUse){ // useless until menu & inUse are re-enabled
                    Game1.showRedMessage($"{Helper.Translation.Get("postbox-occupied")}");
                    worked = true;
                }
                // inUse = true; // for use with a menu
                if (tileLocation.X == (float)((int)base.tileX + 1) && tileLocation.Y == (float)((int)base.tileY + 1))
                {
                    // if player not holding something giftable 
                    if (target != "" && who.ActiveObject != null && who.ActiveObject.canBeGivenAsGift() && who.ActiveObject.canBeTrashed())
                    {
                        // menu disabled for ease of use. may eventually update to use this again.
                        //ItemGrabMenu itemGrabMenu = new ItemGrabMenu(null, reverseGrab: true, showReceivingMenu: false, null, loadGift, null, null, true, true, false, true, false, 0, null, -1, this);
                        //itemGrabMenu.initializeUpperRightCloseButton();
                        //itemGrabMenu.setBackgroundTransparency(b: false);
                        //Game1.activeClickableMenu = itemGrabMenu;
                        this.loadGift(who.ActiveObject, who);
                        worked = true;
                    }
                    else
                    {
                        // hypothetically this could be the cause of some odd behavior in multiplayer
                        // but since local instances of this mod don't communicate,
                        // we should never see one PostageMenu overwrite another. I hope.
                        if(friends.Count > 0)
                        {
                            Game1.activeClickableMenu = new PostageMenu(friends, PostageMenu.prepGift);
                        }
                        else
                        {
                            Game1.showRedMessage($"{Helper.Translation.Get("postbox-noMoreRecipients")}");
                        }
                        worked = true;
                    }
                }
            }
            // inUse = false; // for use with a menu
            return worked;
        }

        public void loadGift(Item item, Farmer who)
        {
            // player actually selected an item
            if (item != null)
            {
                // target is giftable today
                NPC receiver = Game1.getCharacterFromName(target);
                if (((StardewValley.Object)item).canBeGivenAsGift())
                {
                    if (who.friendshipData[target].GiftsToday == 0 &&
                        who.friendshipData[target].GiftsThisWeek < 2)
                    {
                        Game1.showGlobalMessage($"{item.Name} {Helper.Translation.Get("postbox-sentGift")} {target}!");
                        if (who.IsMainPlayer)
                        {
                            outgoing.Add(new Tuple<StardewValley.Object, string, Farmer>((StardewValley.Object)item, target, who));
                        }
                        else
                        {
                            PostageMessage message = new PostageMessage(item.ParentSheetIndex, target, who.uniqueMultiplayerID);
                            Helper.Multiplayer.SendMessage(message, "PostageMessage", modIDs: new[] { "i-saac-b.PostBoxMod" }, playerIDs: new[] { Game1.serverHost.Value.UniqueMultiplayerID });
                        }
                        // lastDelivery.Add(item); // for recovering last sent item. not implemented
                        who.friendshipData[target].GiftsToday++;
                        who.friendshipData[target].GiftsThisWeek++;
                        who.friendshipData[target].LastGiftDate = new WorldDate(Game1.Date);
                        who.reduceActiveItemByOne();
                        target = "";
                    }
                    else
                    {
                        Game1.showGlobalMessage($"{target} {Helper.Translation.Get("postbox-tooManyGifts")}");
                        target = "";
                    }
                }                    
            }
        }

        public override void dayUpdate(int dayOfMonth)
        {
            foreach (Tuple<StardewValley.Object, string, Farmer> postage in outgoing)
            {
                NPC receiver = Game1.getCharacterFromName(postage.Item2);
                if (verbose) {
                    Game1.chatBox.addInfoMessage($"{postage.Item1.Name} {Helper.Translation.Get("postbox-sentGift")} {postage.Item2} {Helper.Translation.Get("postbox-from")} {postage.Item3.displayName}!");
                }
                Monitor.Log("Sending " + postage.Item1.Name + " to " + postage.Item2 + " from " + postage.Item3.displayName, LogLevel.Debug);
                receiver.receiveGift(postage.Item1, postage.Item3, false, 1 * factor, false);
            }
            outgoing.Clear();
            target = "";
            base.dayUpdate(dayOfMonth);
        }

        public override void drawInMenu(SpriteBatch b, int x, int y)
        {
            b.Draw(base.texture.Value, new Vector2(x, y), this.getSourceRectForMenu(), base.color, 0f, new Vector2(0f, 0f), 4f, SpriteEffects.None, 0.89f);
        }

        public override void draw(SpriteBatch b)
        {
            if (base.isMoving)
            {
                return;
            }
            if ((int)base.daysOfConstructionLeft > 0)
            {
                this.drawInConstruction(b);
                return;
            }
            this.drawShadow(b);
            b.Draw(base.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)base.tileX * 64, (int)base.tileY * 64 + (int)base.tilesHigh * 64)), this.baseSourceRect, base.color.Value * base.alpha, 0f, new Vector2(0f, base.texture.Value.Bounds.Height), 4f, SpriteEffects.None, (float)(((int)base.tileY + (int)base.tilesHigh - 1) * 64) / 10000f);
        }
    }
}
