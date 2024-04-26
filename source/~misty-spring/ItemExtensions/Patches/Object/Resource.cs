/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using Object = StardewValley.Object;
using static ItemExtensions.Additions.GeneralResource;

namespace ItemExtensions.Patches;

public partial class ObjectPatches
{
    //used so weapon msg isn't repeated 5 times
    private static bool CanShowMessage { get; set; } = true;
    private static void Reset() => CanShowMessage = true;
    
    internal static void Postfix_performToolAction(Object __instance, Tool t)
    {
        try
        {
            if (ModEntry.Ores.TryGetValue(__instance.ItemId, out var resource) == false)
            {
                #if DEBUG
                Log("Not a node.");
                #endif
                return;
            }

            if (__instance.MinutesUntilReady <= 0.0)
            {
                if(resource.Tool.Equals("vanilla"))
                    CheckDrops(resource, __instance.Location, __instance.TileLocation, t);
                return;
            }

            if (ToolMatches(t, resource) == false)
            {
                if (ShouldShowWrongTool(t,resource) && CanShowMessage)
                {
                    var msg = Game1.content.LoadString("Strings/Locations:IslandNorth_CaveTool_3");
                    Game1.drawObjectDialogue(msg);
                    CanShowMessage = false;
                    Game1.delayedActions.Add(new DelayedAction(500, Reset));
                }

                return;
            }

            //set vars
            var location = __instance.Location;
            var tileLocation = __instance.TileLocation;
            var damage = GetDamage(t, -1);

            if (damage <= 0)
                return;

            //if temp data doesn't exist, it means we also have to set minutes until ready
            __instance.tempData ??= new Dictionary<string, object>();
            if (__instance.tempData.TryGetValue("Health", out _) == false)
            {
                __instance.tempData.TryAdd("Health", resource.Health);
                __instance.MinutesUntilReady = resource.Health;
            }

            if (t is not null or MeleeWeapon && t.UpgradeLevel < resource.MinToolLevel)
            {
                foreach (var sound in resource.FailSounds)
                {
                    location.playSound(sound, tileLocation);
                }
                Game1.drawObjectDialogue(string.Format(ModEntry.Help.Translation.Get("CantBreak"), t.DisplayName));
                Game1.player.jitterStrength = 1f;
                return;
            }

            if (!string.IsNullOrWhiteSpace(resource.Sound))
                location.playSound(resource.Sound, tileLocation);

            __instance.MinutesUntilReady -= damage + 1;

            if (__instance.MinutesUntilReady <= 0.0)
            {
                //shown regardless of animation
                //resource fade
                var tilePositionToTry = __instance.TileLocation;
                /*
                var temporaryAnimatedSprite = new TemporaryAnimatedSprite(0, 50f, 1, 3, new Vector2(tilePositionToTry.X * 64f, tilePositionToTry.Y * 64f), false, __instance.Flipped)
                {
                    alphaFade = 0.01f
                };
                temporaryAnimatedSprite.CopyAppearanceFromItemId(__instance.QualifiedItemId);
                Game1.Multiplayer.broadcastSprites(Game1.player.currentLocation, temporaryAnimatedSprite);*/
                //dust
                var dust = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 1600, 64, 128), tilePositionToTry * 64f+ new Vector2(0f, -64f), __instance.Flipped, 0.01f, Color.White)
                {
                    layerDepth = 0.1792f,
                    totalNumberOfLoops = 1,
                    currentNumberOfLoops = 1,
                    interval = 80f,
                    animationLength = 8
                };
                Game1.Multiplayer.broadcastSprites(Game1.player.currentLocation, dust);
                
                //do drops & destroy
                CheckDrops(resource, location, tileLocation, t);
                Destroy(__instance);

                return;
            }

            if (resource.Shake)
                __instance.shakeTimer = 100;
        }
        catch (Exception e)
        {
            Log($"Error: {e}", LogLevel.Error);
        }
    }
    
    private static void Destroy(Object o, bool onlySetDestroyable = false)
    {
        try
        {
            if (o.lightSource is not null)
            {
                var id = o.lightSource.Identifier;
                o.Location.removeLightSource(id);
            }    
        
            o.CanBeSetDown = true;
            o.CanBeGrabbed = true;
            o.IsSpawnedObject = true; //by default false IIRC
        
            if(onlySetDestroyable)
                return;
        
            //o.performRemoveAction();
            o.Location.removeObject(o.TileLocation,false);
            o = null;
        }
        catch (Exception e)
        {
            Log($"Error: {e}", LogLevel.Error);
        }
    }
    
    internal static void Pre_onExplosion(Object __instance, Farmer who)
    {
        try
        {
            if (!ModEntry.Ores.TryGetValue(__instance.ItemId, out var resource))
                return;

            if (resource == null)
                return;

            if (resource.ImmuneToBombs)
                return;
            
            //var sheetName = ItemRegistry.GetData(data.ItemDropped).TextureName;
            var where = __instance.Location;
            var tile = __instance.TileLocation;
            var num2 = Game1.random.Next(resource.MinDrops, resource.MaxDrops + 1);

            if (string.IsNullOrWhiteSpace(resource.ItemDropped))
                return;

            if (Game1.IsMultiplayer)
            {
                Game1.recentMultiplayerRandom = Utility.CreateRandom(tile.X * 1000.0, tile.Y);
                for (var index = 0; index < Game1.random.Next(2, 4); ++index)
                    CreateItemDebris(resource.ItemDropped, num2, (int)tile.X, (int)tile.Y, where);
            }
            else
            {
                CreateItemDebris(resource.ItemDropped, num2, (int)tile.X, (int)tile.Y, where);
            }

            Destroy(__instance, true);
        }
        catch (Exception e)
        {
            Log($"Error: {e}", LogLevel.Error);
        }
    }
}
