/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/WarpNetwork
**
*************************************************/

using AeroCore;
using AeroCore.API;
using AeroCore.Utils;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Network;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using WarpNetwork.models;

namespace WarpNetwork
{
    [ModInit]
    class ItemHandler
    {
        private static readonly PerScreen<WarpItem> currentTotem = new();
        private static readonly PerScreen<string> currentID = new();
        internal static void Init()
        {
            ModEntry.AeroAPI.UseItemEvent += TryUseTotem;
            ModEntry.AeroAPI.UseObjectEvent += ActivateObject;
        }
        internal static void TryUseTotem(IUseItemEventArgs ev)
        {
            if (ev.IsHandled || !ev.NormalGameplay)
                return;

            if (ev.IsTool)
            {
                if (ev.Item is Wand)
                {
                    ev.ConsumeItem = false;
                    ev.IsHandled = true;
                    WarpHandler.ShowWarpMenu("_wand");
                }
            } else if (UseItem(Game1.player, ev.ItemStringID))
            {
                ev.ConsumeItem = false; //manage it manually if its a totem
                ev.IsHandled = true;
            }
        }
        internal static void ActivateObject(IUseObjectEventArgs ev)
        {
            if (ev.IsHandled || ev.IsChecking || !Utils.GetWarpObjects().TryGetValue(ev.ObjectStringID, out var data))
                return;

            ev.IsHandled = true;
            Color color = data.Color.TryParseColor(out var c) ? c : Color.White;
            DoTotemWarpEffects(color, ev.ObjectStringID, false, ev.Who, (f) => WarpHandler.DirectWarp(data.Destination, data.IgnoreDisabled), true);
        }
        private static bool UseItem(Farmer who, string id)
        {
            Dictionary<string, WarpItem> items = Utils.GetWarpItems();
            var aid = id.StartsWith("(O)") ? id[3..] : id;
            if (items.TryGetValue(id, out var item) || items.TryGetValue(aid, out item))
            {
                if (item.Destination.Equals("_all", StringComparison.OrdinalIgnoreCase))
                    WarpHandler.ShowWarpMenu("", item.Consume);
                else if (ModEntry.config.WarpCancelEnabled)
                    RequestUseItem(item, id);
                else
                    ConfirmUseItem(item, who, id);
                return true;
            }
            return false;
        }
        private static void RequestUseItem(WarpItem item, string id)
        {
            currentTotem.Value = item;
            currentID.Value = id;
            if (item.Destination.Equals("_return", StringComparison.OrdinalIgnoreCase))
                if (WarpHandler.wandLocation.Value is not null)
                    Game1.currentLocation.createQuestionDialogue(ModEntry.i18n.Get("ui-usereturn"), Game1.currentLocation.createYesNoResponses(), AnswerRequest);
                else
                    WarpHandler.ShowFailureText();
            else if (Utils.GetWarpLocations().TryGetValue(item.Destination, out var dest))
                Game1.currentLocation.createQuestionDialogue(ModEntry.i18n.Get("ui-usetotem", dest), Game1.currentLocation.createYesNoResponses(), AnswerRequest);
            else
                ModEntry.monitor.Log($"Totem could not warp to '{item.Destination}', destination is not defined and does not exist!", LogLevel.Warn);
        }
        private static void AnswerRequest(Farmer who, string key)
        {
            if (key == "Yes")
                ConfirmUseItem(currentTotem.Value, who, currentID.Value);
            currentTotem.Value = null;
            currentID.Value = null;
        }
        private static void ConfirmUseItem(WarpItem item, Farmer who, string id)
        {
            Color color = item.Color.TryParseColor(out var c) ? c : Color.White;
            DoTotemWarpEffects(color, id, item.Consume, who, (f) => WarpHandler.DirectWarp(item.Destination, item.IgnoreDisabled));
        }
        private static void DoTotemWarpEffects(Color color, string id, bool Consume, Farmer who, Func<Farmer, bool> action, bool isCraftable = false)
        {
            if (!int.TryParse(id, out int index))
                index = Utils.GetDeterministicHashCode(id);
            who.jitterStrength = 1f;
            who.currentLocation.playSound("warrior", NetAudio.SoundContext.Default);
            who.faceDirection(2);
            who.canMove = false;
            who.temporarilyInvincible = true;
            who.temporaryInvincibilityTimer = -4000;
            Game1.changeMusicTrack("none", false, Game1.MusicContext.Default);
            who.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[2]
            {
                new FarmerSprite.AnimationFrame(57, 2000, false, false,  null, false),
                new FarmerSprite.AnimationFrame( (short) who.FarmerSprite.CurrentFrame, 0, false, false, new AnimatedSprite.endOfAnimationBehavior((f) => {
                    if (action(f))
                    {
                        if(Consume)
                            who.reduceActiveItemByOne();
                    } else
                    {
                        who.temporarilyInvincible = false;
                        who.temporaryInvincibilityTimer = 0;
                    }
                }), true)
            }, null);
            if (!isCraftable) // no support for bigcraftables until 1.6. otherwise you'll get random sprites
            {
                // reflection
                Multiplayer mp = ModEntry.helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
                // --
                mp.broadcastSprites(who.currentLocation,
                new TemporaryAnimatedSprite(index, 9999f, 1, 999, who.Position + new Vector2(0.0f, -96f), false, false, false, 0.0f)
                {
                    motion = new Vector2(0.0f, -1f),
                    scaleChange = 0.01f,
                    alpha = 1f,
                    alphaFade = 0.0075f,
                    shakeIntensity = 1f,
                    initialPosition = who.Position + new Vector2(0.0f, -96f),
                    xPeriodic = true,
                    xPeriodicLoopTime = 1000f,
                    xPeriodicRange = 4f,
                    layerDepth = 1f
                },
                new TemporaryAnimatedSprite(index, 9999f, 1, 999, who.Position + new Vector2(-64f, -96f), false, false, false, 0.0f)
                {
                    motion = new Vector2(0.0f, -0.5f),
                    scaleChange = 0.005f,
                    scale = 0.5f,
                    alpha = 1f,
                    alphaFade = 0.0075f,
                    shakeIntensity = 1f,
                    delayBeforeAnimationStart = 10,
                    initialPosition = who.Position + new Vector2(-64f, -96f),
                    xPeriodic = true,
                    xPeriodicLoopTime = 1000f,
                    xPeriodicRange = 4f,
                    layerDepth = 0.9999f
                },
                new TemporaryAnimatedSprite(index, 9999f, 1, 999, who.Position + new Vector2(64f, -96f), false, false, false, 0.0f)
                {
                    motion = new Vector2(0.0f, -0.5f),
                    scaleChange = 0.005f,
                    scale = 0.5f,
                    alpha = 1f,
                    alphaFade = 0.0075f,
                    delayBeforeAnimationStart = 20,
                    shakeIntensity = 1f,
                    initialPosition = who.Position + new Vector2(64f, -96f),
                    xPeriodic = true,
                    xPeriodicLoopTime = 1000f,
                    xPeriodicRange = 4f,
                    layerDepth = 0.9988f
                });
            }
            Game1.screenGlowOnce(color, false, 0.005f, 0.3f);
            Utility.addSprinklesToLocation(who.currentLocation, who.getTileX(), who.getTileY(), 16, 16, 1300, 20, Color.White, null, true);
        }
    }
}
