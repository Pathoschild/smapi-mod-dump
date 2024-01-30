/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/


using Microsoft.Xna.Framework;
using StardewDruid.Cast;
using StardewDruid.Character;
using StardewDruid.Dialogue;
using StardewModdingAPI;
using StardewValley;

namespace StardewDruid.Event.World
{
    public class Transform : EventHandle
    {
        public StardewDruid.Character.Dragon avatar;
        public int toolIndex;
        public int attuneableIndex;
        public int extendTime;
        public int moveTimer;
        public Vector2 castPosition;
        public int castTimer;
        public SButton leftButton;
        public bool leftActive;
        public SButton rightButton;
        public bool rightActive;

        public Transform(Vector2 target, Rite rite, int extend)
          : base(target, rite)
        {
            extendTime = extend;
        }

        public override void EventTrigger()
        {
            
            Mod.instance.RegisterEvent(this, "transform");
            
            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + extendTime;
            
            Game1.displayFarmer = false;
            
            avatar = new Dragon(riteData.caster, riteData.caster.Position, riteData.caster.currentLocation.Name, Mod.instance.ColourPreference() + "Dragon");
            
            avatar.currentLocation = Game1.player.currentLocation;
            
            Game1.currentLocation.characters.Add(avatar);
            
            Game1.currentLocation.playSoundPitched("warrior", 700, 0);
            
            castPosition = avatar.Position;
        
        }

        public override bool EventActive()
        {
            if (Game1.player.currentLocation.Name != targetLocation.Name)
            {
                return false;

            }

            if (expireEarly)
            {

                return false;

            }

            if(avatar == null)
            {

                return false;

            }

            if (expireTime <= Game1.currentGameTime.TotalGameTime.TotalSeconds)
            {

                if (!avatar.SafeExit())
                {

                    expireTime += 30;

                    return true;

                }

                return false;
                
            }

            return true;

        }

        public override void AttemptAbort()
        {
            
            if (avatar != null)
            {

                if (avatar.SafeExit())
                {

                    EventRemove();

                    expireEarly = true;

                }

            }

        }

        public override void EventRemove()
        {
            if (Game1.player.CurrentToolIndex == 999)
            {
                
                Game1.player.CurrentToolIndex = toolIndex;

            }

            Game1.displayFarmer = true;
            
            if (avatar == null)
            {
                
                return;

            }

            avatar.ShutDown();
            
            //avatar.currentLocation.characters.Remove(avatar);
            
            avatar = null;

        }

        public override bool EventPerformAction(SButton Button, string Type)
        {
            if (!EventActive())
            {
                
                return false;
            
            }
                
            if (Game1.player.CurrentToolIndex != 999)
            {
                
                int num = Mod.instance.AttuneableWeapon();
                
                if (num == -1)
                {
                
                    return false;
                
                } 
                
                toolIndex = Game1.player.CurrentToolIndex;
                
                attuneableIndex = num;
            
                Game1.player.CurrentToolIndex = 999;
            
            }
            
            if (!Game1.shouldTimePass(false))
            {
                return false;
            }
                
            if (Type == "Special" && rightActive)
            {
                
                avatar.RightClickAction(Button);
                
                rightButton = Button;
                
                return true;
            
            }

            if (!leftActive)
            {
                return false;
            }
                
            avatar.LeftClickAction(Button);
            
            leftButton = Button;
            
            return true;
        
        }

        public override void EventExtend()
        {
            if (avatar == null)
            {
                return;
            }
                
            avatar.PlayerBusy();
        }

        public override void EventDecimal()
        {
            if (avatar != null && !Game1.shouldTimePass(false))
            {
                avatar.PlayerBusy();
            }
                
            if (Game1.player.CurrentToolIndex != 999 || Mod.instance.Helper.Input.IsDown(rightButton) || Mod.instance.Helper.Input.IsDown(leftButton))
            {
                return;
            }
                
            Game1.player.CurrentToolIndex = toolIndex;
        }

        public override void EventInterval()
        {
            foreach (NPC character in avatar.currentLocation.characters)
            {

                if (character is StardewValley.Monsters.Monster)
                {
                    continue;
                }

                if (character is StardewDruid.Character.Character)
                {
                    continue;
                }

                if (character is StardewDruid.Character.Dragon)
                {
                    continue;
                }

                if(Mod.instance.WitnessedRite("ether", character))
                {
                    continue;
                }

                if (!character.isVillager())
                {
                    continue;
                }

                if ((double)Vector2.Distance(character.Position, avatar.Position) < 740.0)
                {
                    if (!Mod.instance.TaskList().ContainsKey("masterTransform"))
                    {
                        
                        Mod.instance.UpdateTask("lessonTransform", 1);
                    
                    }

                    ModUtility.GreetVillager(riteData.caster, character, 15);

                    Reaction.ReactTo(character, "Ether");

                }
            
            }

            base.EventInterval();

        }

    }

}
