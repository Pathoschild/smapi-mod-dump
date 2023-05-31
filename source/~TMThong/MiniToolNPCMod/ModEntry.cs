/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Menus;
using StardewValley;
using System;
using Microsoft.Xna.Framework;
namespace MiniToolNPCMod
{
    public class ModEntry : Mod
    {
        public static ClickableComponent clickableComponent { get; private set; }


        public override void Entry(IModHelper helper)
        {
            this.i18n = helper.Translation;
            helper.Events.Input.ButtonPressed += this.ButtonPressed;
            helper.Events.Display.MenuChanged += delegate (object o, MenuChangedEventArgs e)
            {
                 
                if (e.NewMenu is ProfileMenu profile)
                {
                    ModEntry.clickableComponent = new ClickableComponent(base.Helper.Reflection.GetField<Rectangle>(profile, "characterSpriteBox", true).GetValue(), "");
                }
            };
        }

        private void ButtonPressed(object o, ButtonPressedEventArgs e)
        {
            
            if (Game1.activeClickableMenu is ProfileMenu profile && Context.IsWorldReady)
            {
                int x = (int)e.Cursor.ScreenPixels.X;
                int y = (int)e.Cursor.ScreenPixels.Y;            
                if (ModEntry.clickableComponent.bounds.Contains(x, y))
                {
                    this.ActionClick(profile);
                }
            }
        }
        private void ActionClick(ProfileMenu profile)
        {
             
             
            if (Game1.player.currentLocation != null)
            {
                string title = this.i18n.Get("title");
                Character character = profile.GetCharacter();
                Response[] responses = new Response[]
                {
                    new Response("warpfarmer", this.i18n.Get("warpfarmer", new
                    {
                        f = Game1.player.name,
                        npcname = character.displayName
                    })),
                    new Response("warpnpc", this.i18n.Get("warpnpc", new
                    {
                        f = Game1.player.name,
                        npcname = character.displayName
                    })),
                    new Response("swimming", this.i18n.Get("swimming")),
                    new Response("left", Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13118"))
                };
                Action<Farmer, string> Choose = delegate (Farmer f, string k)
                {
                    if (!(k == "warpfarmer"))
                    {
                        if (!(k == "warpnpc"))
                        {
                            if (k == "swimming")
                            {
                                character.swimming.Value = !character.swimming.Value;
                            }
                        }
                        else
                        {
                            Game1.warpCharacter((NPC)character, f.currentLocation, new Vector2((float)f.getTileX(), (float)f.getTileY()));
                        }
                    }
                    else
                    {
                        Game1.warpFarmer(character.currentLocation.name, character.getTileX(), character.getTileY(), false);
                    }
                };
                Game1.player.currentLocation.createQuestionDialogue(title, responses, new GameLocation.afterQuestionBehavior(Choose.Invoke), null);
            }
        }
        private ITranslationHelper i18n;
    }
}
