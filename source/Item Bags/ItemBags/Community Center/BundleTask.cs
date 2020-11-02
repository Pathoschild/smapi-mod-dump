/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-ItemBags
**
*************************************************/

using ItemBags.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemBags.Community_Center
{
    /// <summary>Represents a single bundle in the Community Center, requiring 1 or more <see cref="BundleItem"/>s to complete.</summary>
    public class BundleTask
    {
        public BundleRoom Room { get; }
        public string Name { get; }
        /// <summary>May be null if using the game's default language. Check <see cref="Name"/> if so.</summary>
        public string TranslatedName { get; }

        public int BundleIndex { get; }
        public int SpriteColorIndex { get; }
        /// <summary>The Texture position within <see cref="TextureHelpers.JunimoNoteTexture"/> where the small bundle icon is located (The little colored bag). The bag is closed.</summary>
        public Rectangle SpriteSmallIconClosedPosition { get { return new Rectangle((SpriteColorIndex % 2) * 16 * 16, 244 + (SpriteColorIndex / 2) * 16, 16, 16); } }
        /// <summary>The Texture position within <see cref="TextureHelpers.JunimoNoteTexture"/> where the small bundle icon is located (The little colored bag). The bag is opened.</summary>
        public Rectangle SpriteSmallIconOpenedPosition { get { return new Rectangle(15 * 16 + (SpriteColorIndex % 2) * 16 * 16, 244 + (SpriteColorIndex / 2) * 16, 16, 16); } }
        public int SpriteIconIndex { get { return BundleIndex; } }
        /// <summary>The Texture position within <see cref="TextureHelpers.JunimoNoteTexture"/> where the bundle's image is located (The 32x32 picture that describes the bundle)</summary>
        public Rectangle SpriteLargeIconPosition { get { return new Rectangle((SpriteIconIndex % 20) * 32, 180 + (SpriteIconIndex / 20) * 32, 32, 32); } }

        /// <summary>May be null (EX: The Missing bundle in the Abandoned JojaMart)</summary>
        public BundleReward Reward { get; }

        /// <summary>The # of <see cref="BundleItem"/>s that must be fulfilled to finish this <see cref="BundleTask"/>. (Most Bundles require all Items to be completed)</summary>
        public int RequiredItemCount { get; }
        public bool AreAllItemsRequired { get { return RequiredItemCount == Items.Count; } }
        public ReadOnlyCollection<BundleItem> Items { get; }

        public bool IsCompleted { get { return Items.Count(x => x.IsCompleted) >= RequiredItemCount; } }

        /// <param name="RawData">The raw data string from the game's bundle content. EX: "Spring Foraging/O 495 30/16 1 0 18 1 0 20 1 0 22 1 0/0".<para/>
        /// This format is described here: <see cref="https://stardewvalleywiki.com/Modding:Bundles"/></param>
        public BundleTask(BundleRoom Room, int BundleIndex, string RawData)
        {
            this.Room = Room;
            this.BundleIndex = BundleIndex;

            List<string> Entries = RawData.Split('/').ToList();
            this.Name = Entries[0];

            if (string.IsNullOrEmpty(Entries[1]))
                this.Reward = null;
            else
            {
                try { this.Reward = new BundleReward(this, Entries[1]); }
                catch (Exception ex)
                {
                    string ErrorMsg = string.Format("Error while parsing Bundle Reward: '{0}' - {1}", Entries[1], ex.Message);
                    ItemBagsMod.ModInstance.Monitor.Log(ErrorMsg, StardewModdingAPI.LogLevel.Error);
                    this.Reward = null;
                }
            }

            this.Items = Entries[2].Split(' ').Split(3).Select(x => string.Join(" ", x)).Select(x => new BundleItem(this, x)).ToList().AsReadOnly();
            this.SpriteColorIndex = int.Parse(Entries[3]);
            if (Entries.Count > 4)
            {
                int RequiredCount;
                if (int.TryParse(Entries[4], out RequiredCount))
                {
                    this.RequiredItemCount = RequiredCount;
                    if (Entries.Count > 5)
                    {
                        this.TranslatedName = Entries[5];
                    }
                }
                else
                {
                    this.RequiredItemCount = Items.Count;
                    this.TranslatedName = Entries[4];
                }
            }
            else
            {
                this.RequiredItemCount = Items.Count;
            }
        }

        private static HashSet<int> InvalidItemIds = new HashSet<int>(new List<int>() { 639, 640, 641, 642, 643 });
        /// <summary>In the game data for bundles ("Data/Bundles.xnb"), The Bundle at "Pantry/4" contains several Item Ids which don't seem to correspond to an actual item. 
        /// "Pantry/4" (The "Animal Bundle") data contains 12 item Ids even though the Community Center menu only displays 6 different items for that bundle. No clue why.</summary>
        internal static bool IsValidItemId(int Id)
        {
            return !InvalidItemIds.Contains(Id);
        }
    }
}
