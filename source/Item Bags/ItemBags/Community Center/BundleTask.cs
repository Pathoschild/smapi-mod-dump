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
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
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
        /// <summary>This name is always in English. See also: <see cref="TranslatedName"/></summary>
        public string Name { get; }
        public string TranslatedName { get; }

        public int BundleIndex { get; }
        public int SpriteColorIndex { get; }
        /// <summary>The Texture position within <see cref="TextureHelpers.JunimoNoteTexture"/> where the small bundle icon is located (The little colored bag). The bag is closed.</summary>
        public Rectangle SpriteSmallIconClosedPosition { get { return new Rectangle((SpriteColorIndex % 2) * 16 * 16, 244 + (SpriteColorIndex / 2) * 16, 16, 16); } }
        /// <summary>The Texture position within <see cref="TextureHelpers.JunimoNoteTexture"/> where the small bundle icon is located (The little colored bag). The bag is opened.</summary>
        public Rectangle SpriteSmallIconOpenedPosition { get { return new Rectangle(15 * 16 + (SpriteColorIndex % 2) * 16 * 16, 244 + (SpriteColorIndex / 2) * 16, 16, 16); } }

        private int DefaultLargeIconIndex { get { return BundleIndex; } }
        private int? OverriddenLargeIconIndex { get; }
        private int ActualLargeIconIndex { get { return OverriddenLargeIconIndex ?? DefaultLargeIconIndex; } }

        public Texture2D OverriddenLargeIconTexture { get; }
        public Texture2D ActualLargeIconTexture { get { return OverriddenLargeIconTexture ?? TextureHelpers.JunimoNoteTexture; } }

        /// <summary>The Texture position within <see cref="TextureHelpers.JunimoNoteTexture"/> where the bundle's image is located (The 32x32 picture that describes the bundle), if this bundle isn't using an overridden icon.<para/>
        /// See also: <see cref="ActualLargeIconTexture"/></summary>
        public Rectangle DefaultLargeIconPosition { get { return new Rectangle((DefaultLargeIconIndex % 20) * 32, 180 + (DefaultLargeIconIndex / 20) * 32, 32, 32); } }
        public Rectangle ActualLargeIconPosition
        {
            get
            {
                if (OverriddenLargeIconTexture == null)
                {
                    return new Rectangle((ActualLargeIconIndex % 20) * 32, 180 + (ActualLargeIconIndex / 20) * 32, 32, 32);
                }
                else
                {
                    return new Rectangle((ActualLargeIconIndex % 8) * 32, (ActualLargeIconIndex / 8) * 32, 32, 32);
                }
            }
        }

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

            bool IsEnglish = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en;
            if (!IsEnglish || Entries.Last() == Name)
            {
                this.TranslatedName = Entries.Last();
                Entries.RemoveAt(Entries.Count - 1);
            }

            //TODO I think 1.6 added the BundleIndex at the end? So probably should do Entries.RemoveAt(Entries.Count - 1);

            try
            {
                if (Entries.Count <= 4)
                {
                    this.RequiredItemCount = Items.Count;
                }
                else
                {
                    //  Game version 1.6 now allows empty entries in the bundle data, such as:
                    //  "Spring Crops/O 465 20/24 1 0 188 1 0 190 1 0 192 1 0/0///Spring Crops"
                    if (int.TryParse(Entries[4], out int RequiredCt))
                        this.RequiredItemCount = RequiredCt;
                    else
                        this.RequiredItemCount = Items.Count;

                    if (Entries.Count > 5)
                    {
                        string TextureOverrideData = Entries[5];
                        try
                        {
                            if (TextureOverrideData.IndexOf(':') < 0)
                            {
                                this.OverriddenLargeIconTexture = null;
                                this.OverriddenLargeIconIndex = int.Parse(TextureOverrideData);
                            }
                            else
                            {
                                string[] OverrideParts = TextureOverrideData.Split(':');
                                this.OverriddenLargeIconTexture = Game1.content.Load<Texture2D>(OverrideParts[0]);
                                this.OverriddenLargeIconIndex = int.Parse(OverrideParts[1]);
                            }
                        }
                        catch (Exception)
                        {
                            this.OverriddenLargeIconTexture = null;
                            this.OverriddenLargeIconIndex = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.RequiredItemCount = Items.Count;
                this.OverriddenLargeIconTexture = null;
                this.OverriddenLargeIconIndex = null;

                string ErrorMsg = string.Format("Error while parsing Bundle Data: '{0}' - {1}\nBundle Bags might not display the correct data on their menu, but should still work correctly.", RawData, ex.Message);
                ItemBagsMod.ModInstance.Monitor.Log(ErrorMsg, StardewModdingAPI.LogLevel.Warn);
            }
        }

        private static HashSet<string> InvalidItemIds = new List<string>() { "639", "640", "641", "642", "643" }.ToHashSet();
        /// <summary>In the game data for bundles ("Data/Bundles.xnb"), The Bundle at "Pantry/4" contains several Item Ids which don't seem to correspond to an actual item. 
        /// "Pantry/4" (The "Animal Bundle") data contains 12 item Ids even though the Community Center menu only displays 6 different items for that bundle. No clue why.</summary>
        internal static bool IsValidItemId(string Id)
        {
            return !InvalidItemIds.Contains(Id);
        }
    }
}
