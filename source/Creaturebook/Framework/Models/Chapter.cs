/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KediDili/Creaturebook
**
*************************************************/

using StardewModdingAPI;
using System;

namespace Creaturebook.Framework.Models
{

    public class Chapter : IEquatable<Chapter>
    {
        public enum Categories
        {
            Animals,
            Plants,
            Monsters,
            Magical,
            Other
        }

        public enum Style
        {
            ImageNText,
            AllText
        }

        public string Title;

        public string Folder;

        public string CreatureNamePrefix;

        public string Author;

        public Categories Category;

        public Style PageStyle;

        public Creature[] Creatures;

        public Set[] Sets;

        public bool EnableSets;

        public IContentPack FromContentPack;

        public string RewardItem;

        public string PackID;
        public Chapter(Chapter old)
        {
            if (old is not null)
            {
                FromContentPack = old.FromContentPack;
                PackID = FromContentPack.Manifest.UniqueID;
                RewardItem = old.RewardItem;
                CreatureNamePrefix = old.CreatureNamePrefix;
                Author = old.Author;
                Category = old.Category;
                Creatures = old.Creatures;
                Sets = old.Sets;
                EnableSets = old.EnableSets;
                Folder = old.Folder;
                Title = old.Title;
            }
        }

        public static Chapter FindChapter(string packID, string prefix, Creature creature)
        {
            for (int i = 0; i < ModEntry.Chapters.Count; i++)
                for (int f = 0; f < ModEntry.Chapters[i].Creatures.Length; f++)
                    if (ModEntry.Chapters[i].PackID == packID && ModEntry.Chapters[i].Creatures[f].DetailedEquality(creature) && ModEntry.Chapters[i].CreatureNamePrefix == prefix)
                        return ModEntry.Chapters[i];
            return null;
        }

        public static bool operator ==(Chapter right, Chapter left)
        {
            if (right.CreatureNamePrefix == left.CreatureNamePrefix && right.PackID == left.PackID)
                return true;
            return false;
        }

        public static bool operator !=(Chapter right, Chapter left)
        {
            if (right?.CreatureNamePrefix == left.CreatureNamePrefix && right.PackID == left.PackID)
                return false;
            return true;
        }

        public bool DetailedEquality(Chapter other)
        {
            if (this == other)
                if (Category == other.Category && Title == other.Title && Folder == other.Folder && Author == other.Author)
                    return true;
            return false;
        }
        public bool Equals(Chapter other)
        {
            if (CreatureNamePrefix == other.CreatureNamePrefix && PackID == other.PackID)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Chapter);
        }
    }
}
