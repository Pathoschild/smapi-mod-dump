/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/predictivemods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using SObject = StardewValley.Object;

namespace ScryingOrb
{
	public abstract class Experience
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;

		public SObject orb { get; internal set; }
		public bool illuminated { get; private set; }
		public Item offering { get; internal set; }

		// Whether the experience should be available to players at present.
		public virtual bool isAvailable => true;

		public static bool Check<T> (SObject orb)
			where T : Experience, new()
		{
			try
			{
				T experience = new T { orb = orb };
				return experience.check ();
			}
			catch (Exception e)
			{
				Monitor.Log ($"{typeof (T).Name} failed: {e.Message}", LogLevel.Warn);
				return false;
			}
		}

		protected abstract bool check ();

		public static void Run<T> (SObject orb)
			where T : Experience, new()
		{
			T experience = new T { orb = orb };
			experience.run ();
		}

		public void run ()
		{
			try
			{
				doRun ();
			}
			catch (Exception e)
			{
				Monitor.Log ($"{GetType ().Name} failed: {e.Message}", LogLevel.Error);
				extinguish ();
			}
		}

		protected abstract void doRun ();

		protected Experience ()
		{
			Helper.Content.Load<Texture2D>
				(Path.Combine ("assets", "illumination.png"));
		}

		protected bool checkOffering (Type type = null, int category = 0,
			IList<int> accepted = null, IList<int> rejected = null,
			bool bigCraftable = false)
		{
			bool result = checkItem (Game1.player.CurrentItem, type: type,
				category: category, accepted: accepted, rejected: rejected,
				bigCraftable: bigCraftable);
			if (result)
				this.offering = Game1.player.CurrentItem;
			return result;
		}

		protected bool checkItem (Item item, Type type = null, int category = 0,
			IList<int> accepted = null, IList<int> rejected = null,
			bool bigCraftable = false)
		{
			if (!isAvailable)
				return false;

			type ??= typeof (SObject);
			if (item?.GetType () != type)
				return false;
			
			if (type == typeof (SObject) &&
					(item as SObject).bigCraftable.Value != bigCraftable)
				return false;
			
			if (rejected != null && rejected.Contains (item.ParentSheetIndex))
				return false;
			
			if (accepted != null && accepted.Contains (item.ParentSheetIndex))
				return true;
			
			if (category < 0 && item.Category != category)
				return false;

			return accepted == null;
		}

		protected void consumeOffering (int count = 1)
		{
			consumeItem (offering, count);
		}

		protected void consumeItem (Item item, int count = 1)
		{
			if (item == null)
				throw new ArgumentNullException (nameof (item));

			if (item.Stack > count)
				item.Stack -= count;
			else if (item.Stack == count)
				Game1.player.removeItemFromInventory (item);
			else
				throw new Exception ($"Item stack of {item.Stack} {item.Name} insufficient for count of {count}.");
		}

		protected void showRejection (string messageKey)
		{
			playSound ("fishEscape");
			showMessage (messageKey, 250);
		}

		protected void showMessage (string messageKey, int delay = 0)
		{
			showDialogues (new List<string> { Helper.Translation.Get (messageKey) },
				delay);
		}

		protected string unbreak (string dialogue)
		{
			if (Constants.TargetPlatform == GamePlatform.Android)
				return Regex.Replace (dialogue, @" *\^ *", " ").Trim ();
			else
				return dialogue;
		}

		protected void showDialogues (List<string> dialogues, int delay = 0)
		{
			// Equivalent to DelayedAction.showDialogueAfterDelay combined with
			// Game1.drawDialogueNoTyping, except that the player is locked
			// instantly before the delay and the List<string> constructor of
			// DialogueBox is used to work around the height estimation bug
			// with multi-page dialogues.

			// Pad each line of each page with a space to work around a width
			// estimation bug in the List-based DialogueBox.
			for (int i = 0; i < dialogues.Count; ++i)
				dialogues[i] = dialogues[i].Replace ("^", " ^") + " ";

			// Prepare the UI and player before the delay.
			if (Game1.activeClickableMenu != null)
			{
				Game1.activeClickableMenu.emergencyShutDown ();
			}
			Game1.player.CanMove = false;
			Game1.dialogueUp = true;

			DelayedAction.functionAfterDelay (() =>
			{
				// Display the dialogues.
				DialogueBox box = new DialogueBox (dialogues);
				Game1.activeClickableMenu = box;

				// Fix the first page height being too *short* on Android.
				if (Constants.TargetPlatform == GamePlatform.Android)
				{
					box.height = SpriteText.getHeightOfString (dialogues[0],
						box.width - 8 - 16) + 12 + 12;
				}

				// Suppress typing of dialogue, at least on first page.
				box.finishTyping ();
			}, delay);
		}

		protected void playSound (string soundName, int delay = 0)
		{
			DelayedAction.playSoundAfterDelay (soundName, delay,
				Game1.currentLocation);
		}

		protected TemporaryAnimatedSprite showAnimation (string textureName,
			Rectangle sourceRect, float interval, int length, int loops,
			int delay = 0)
		{
			if (orb == null) return null;

			Vector2 position = new Vector2 (orb.TileLocation.X,
				orb.TileLocation.Y - (sourceRect.Height / (float) sourceRect.Width));
			position *= Game1.tileSize;

			float layerDepth = (float) (((orb.TileLocation.Y + 1.0) * 64.0 / 10000.0)
				+ 9.99999974737875E-05);

			TemporaryAnimatedSprite sprite = new TemporaryAnimatedSprite
				(textureName, sourceRect, interval, length, loops, position,
				false, false, layerDepth, 0f, Color.White, 64f / sourceRect.Width,
				0f, 0f, 0f, false);
			DelayedAction.addTemporarySpriteAfterDelay (sprite,
				Game1.currentLocation, delay);

			return sprite;
		}

		protected LightSource illuminate (int r = 153, int g = 217, int b = 234)
		{
			// Update the status.
			if (illuminated)
				return orb?.lightSource;
			illuminated = true;

			// Switch to the special mouse cursor.
			++ModEntry.Instance.OrbsIlluminated;

			// The rest requires an actual orb positioned in the world.
			if (orb == null || orb.TileLocation.Equals (Vector2.Zero))
				return null;

			// Replace any existing light source.
			removeLightSource ();

			// Calculate and create the light source.
			Vector2 position = new Vector2 ((orb.TileLocation.X * 64f) + 32f,
				(orb.TileLocation.Y * 64f) - 32f);
			Color color = new Color (255 - r, 255 - g, 255 - b) * 2f;
			int identifier = (int) ((orb.TileLocation.X * 2000f) +
				orb.TileLocation.Y);
			orb.lightSource = new LightSource (LightSource.cauldronLight,
				position, 1f, color, identifier);

			// If a real Scrying Orb, the light source is now effective.
			if (ModEntry.Instance.IsScryingOrb (orb))
			{
				// Switch the orb to its illuminated sprite, unless the lighting
				// is not in the blue range.
				if (b > r && b > g)
				{
					TemporaryAnimatedSprite sprite = showAnimation
						(Helper.Content.GetActualAssetKey
							(Path.Combine ("assets", "illumination.png")),
						new Rectangle (0, 0, 16, 16), 200f, 5, 9999);
					sprite.id = identifier;
				}
			}
			else
			{
				// If not a real orb, add the light source directly to the game.
				Game1.currentLightSources.Add (orb.lightSource);
			}

			return orb.lightSource;
		}

		protected void extinguish ()
		{
			// Update the status.
			if (!illuminated)
				return;
			illuminated = false;

			// Restore the regular mouse cursor.
			--ModEntry.Instance.OrbsIlluminated;

			// Remove any light source from any actual orb.
			removeLightSource ();
		}

		internal void transferIllumination (Experience from)
		{
			if (!from.illuminated || illuminated)
				return;
			from.extinguish ();
			illuminate ();
		}

		private void removeLightSource ()
		{
			if (orb?.lightSource == null)
				return;
			if (ModEntry.Instance.IsScryingOrb (orb))
			{
				Game1.currentLocation.removeTemporarySpritesWithID
					(orb.lightSource.Identifier);
				Game1.currentLocation.removeLightSource (orb.lightSource.Identifier);
			}
			else
			{
				Game1.currentLightSources.Remove (orb.lightSource);
			}
			orb.lightSource = null;
		}
	}
}
