/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/poohnhi/PoohCore
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Characters;
using System;
using Microsoft.Xna.Framework;
using Force.DeepCloner;
using System.Collections.Generic;
using System.Xml.Linq;
using xTile.Dimensions;

namespace PoohCore
{
    internal class Patches
    {
        private static IMonitor Monitor;
        internal static void behaviorOnFarmerLocationEntry_Postfix(StardewValley.NPC __instance, GameLocation location, Farmer who)
        {
            try
            {
                CharacterData data = __instance.GetData();
                if (data != null && data.CustomFields.TryGetValue("poohnhi.PoohCore/HighCharacter", out string HighCharacterValue))
                {
                    if (HighCharacterValue != null && HighCharacterValue != "false")
                    {
                        if (__instance.Sprite != null && __instance.Sprite.CurrentAnimation == null && __instance.Sprite.SourceRect.Height > 32)
                        {
                            __instance.Sprite.SpriteWidth = __instance.Sprite.SourceRect.Width;
                            __instance.Sprite.SpriteHeight = __instance.Sprite.SourceRect.Height;
                            __instance.Sprite.currentFrame = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Monitor.Log($"Failed in {nameof(behaviorOnFarmerLocationEntry_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }
        internal static void addActor_Postfix(StardewValley.Event __instance, string name, int x, int y, int facingDirection, GameLocation location)
        {
            try
            {
                bool isOptionalNpc;
                if (ModEntry.ListOfModifiedNPC.Contains(name))
                {
                    NPC actorByName = __instance.getActorByName(name, out isOptionalNpc);
                    if (actorByName != null)
                    {
                        __instance.actors.Remove(actorByName);
                        NPC myNPC = Game1.getCharacterFromName(name);
                        string textureNameForCharacter = NPC.getTextureNameForCharacter(name);
                        NPC nPC;
                        nPC = new NPC(new AnimatedSprite("Characters\\" + textureNameForCharacter, 0, myNPC.Sprite.SourceRect.Width, myNPC.Sprite.SourceRect.Height), actorByName.Position, actorByName.currentLocation.Name, actorByName.FacingDirection, actorByName.Name, actorByName.Portrait, eventActor: true);
                        if (__instance.isFestival)
                        {
                            try
                            {
                                if (__instance.TryGetFestivalDialogueForYear(nPC, nPC.Name, out var dialogue))
                                {
                                    nPC.setNewDialogue(dialogue);
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                        __instance.actors.Add(nPC);
                    }
                }
            }
            catch (Exception ex)
            {
                //Monitor.Log($"Failed in {nameof(behaviorOnFarmerLocationEntry_Postfix)}:\n{ex}", LogLevel.Trace);
            }
        }
        internal static void resetForNewDay_Prefix(StardewValley.NPC __instance, int dayOfMonth)
        {
            try
            {
                if (__instance != null)
                {
                    CharacterData data = __instance.GetData();
                    if (data != null)
                    {
                        try
                        {
                            var test = data.CustomFields.TryGetValue("poohnhi.PoohCore/WideCharacter", out string WideCharacterValue);
                            if (WideCharacterValue != null && WideCharacterValue != "false")
                            {
                                __instance.forceOneTileWide.Set(true);
                            }
                        }
                        catch
                        {

                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                //Monitor.Log($"Failed in {nameof(resetForNewDay_Prefix)}:\n{ex}", LogLevel.Error);
            }
        }
    }
}
