/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/MysticalBuildings
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaveOfMemories.Framework.Models
{
    public class LimitedEvent : Event
    {
        public LimitedEvent(string eventString, int eventID = -1, Farmer farmerActor = null) : base(eventString, eventID, farmerActor)
        {

        }

        public new void skipEvent()
        {
            if (this.playerControlSequence)
            {
                this.EndPlayerControlSequence();
            }
            Game1.playSound("drumkit6");

            CaveOfMemories.modHelper.Reflection.GetField<Dictionary<string, Vector3>>(this, "actorPositionsAfterMove").GetValue().Clear();
            foreach (NPC i in this.actors)
            {
                bool ignore_stop_animation = i.Sprite.ignoreStopAnimation;
                i.Sprite.ignoreStopAnimation = true;
                i.Halt();
                i.Sprite.ignoreStopAnimation = ignore_stop_animation;
                this.resetDialogueIfNecessary(i);
            }
            this.farmer.Halt();
            this.farmer.ignoreCollisions = false;
            Game1.exitActiveMenu();
            Game1.dialogueUp = false;
            Game1.dialogueTyping = false;
            Game1.pauseTime = 0f;

            this.endBehaviors(new string[1] { "end" }, Game1.currentLocation);
        }

        public override void command_addCookingRecipe(GameLocation location, GameTime time, string[] split)
        {
            this.CurrentCommand++;
        }

        public override void command_addCraftingRecipe(GameLocation location, GameTime time, string[] split)
        {
            this.CurrentCommand++;
        }

        public override void command_awardFestivalPrize(GameLocation location, GameTime time, string[] split)
        {
            this.CurrentCommand++;
        }

        public override void command_addTool(GameLocation location, GameTime time, string[] split)
        {
            base.CurrentCommand++;
        }

        public override void command_removeItem(GameLocation location, GameTime time, string[] split)
        {
            base.CurrentCommand++;
        }

        public override void command_mail(GameLocation location, GameTime time, string[] split)
        {
            base.CurrentCommand++;
        }
        public override void command_hostMail(GameLocation location, GameTime time, string[] split)
        {
            base.CurrentCommand++;
        }

        public override void command_addMailReceived(GameLocation location, GameTime time, string[] split)
        {
            base.CurrentCommand++;
        }
    }
}
