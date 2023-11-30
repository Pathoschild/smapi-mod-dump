/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/WarpNetwork
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Network;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using WarpNetwork.models;
using SObject = StardewValley.Object;

namespace WarpNetwork
{
	class ItemHandler
	{
		private static readonly PerScreen<WarpItem> currentTotem = new();
		private static readonly PerScreen<Item> currentItem = new();
		internal static bool TryUseTotem(Farmer who, Item what)
		{
			if (what is Wand && who == Game1.player && ModEntry.config.AccessFromWand)
			{
				WarpHandler.ShowWarpMenu("_wand");
				return true;
			}
			return UseItem(who, what);
		}
		internal static bool ActivateObject(SObject obj, bool checking, Farmer who)
		{
			if (checking || !Utils.GetWarpObjects().TryGetValue(obj.ItemId, out var data))
				return false;

			Color color = data.Color.TryParseColor(out var c) ? c : Color.White;
			DoTotemWarpEffects(color, obj, false, who, (f) => WarpHandler.DirectWarp(data.Destination, data.IgnoreDisabled), true);
			return true;
		}
		private static bool UseItem(Farmer who, Item what)
		{
			Dictionary<string, WarpItem> items = Utils.GetWarpItems();
			if (items.TryGetValue(what.QualifiedItemId, out var item) || (what is SObject && items.TryGetValue(what.ItemId, out item)))
			{
				ModEntry.monitor.Log($"Totem data found for item with id '{what.QualifiedItemId}'.");
				if (item.Destination.Equals("_all", StringComparison.OrdinalIgnoreCase))
					WarpHandler.ShowWarpMenu("", item.Consume);
				else if (ModEntry.config.WarpCancelEnabled)
					RequestUseItem(item, what);
				else
					ConfirmUseItem(item, who, what);
				return true;
			}
			return false;
		}
		private static void RequestUseItem(WarpItem item, Item obj)
		{
			currentTotem.Value = item;
			currentItem.Value = obj;
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
				ConfirmUseItem(currentTotem.Value, who, currentItem.Value);
			else
				ModEntry.monitor.Log("Canceled totem warp.");
			currentTotem.Value = null;
			currentItem.Value = null;
		}
		private static void ConfirmUseItem(WarpItem item, Farmer who, Item what)
		{
			ModEntry.monitor.Log($"Totem activated! Warping {who.Name} to {item.Destination}");
			Color color = item.Color.TryParseColor(out var c) ? c : Color.White;
			DoTotemWarpEffects(color, what, item.Consume, who, (f) => WarpHandler.DirectWarp(item.Destination, item.IgnoreDisabled));
		}
		private static void DoTotemWarpEffects(Color color, Item item, bool Consume, Farmer who, Func<Farmer, bool> action, bool isCraftable = false)
		{
			who.jitterStrength = 1f;
			who.currentLocation.playSound("warrior", who.Position);
			who.faceDirection(2);
			who.canMove = false;
			who.temporarilyInvincible = true;
			who.temporaryInvincibilityTimer = -4000;
			Game1.changeMusicTrack("silence");
			who.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[2]
			{
				new FarmerSprite.AnimationFrame(57, 2000, false, false,  null, false),
				new FarmerSprite.AnimationFrame( (short) who.FarmerSprite.CurrentFrame, 0, false, false, new AnimatedSprite.endOfAnimationBehavior((f) => {
					if (action(f))
					{
						if (Consume)
						{
							item.Stack--;
							if (item.Stack is <= 0)
								who.Items[who.getIndexOfInventoryItem(item)] = null;
						}
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
				new TemporaryAnimatedSprite(0, 9999f, 1, 999, who.Position + new Vector2(0.0f, -96f), false, false, false, 0.0f)
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
				}.WithItem(item),
				new TemporaryAnimatedSprite(0, 9999f, 1, 999, who.Position + new Vector2(-64f, -96f), false, false, false, 0.0f)
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
				}.WithItem(item),
				new TemporaryAnimatedSprite(0, 9999f, 1, 999, who.Position + new Vector2(64f, -96f), false, false, false, 0.0f)
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
				}.WithItem(item));
			}
			Game1.screenGlowOnce(color, false, 0.005f, 0.3f);
			Utility.addSprinklesToLocation(who.currentLocation, who.TilePoint.X, who.TilePoint.Y, 16, 16, 1300, 20, Color.White, null, true);
		}
	}
}
