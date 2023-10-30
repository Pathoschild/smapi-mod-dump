/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Items.Mail;
using StardewValley;
using Object = StardewValley.Object;

namespace StardewArchipelago.Stardew
{
    public class BigCraftable : StardewItem
    {
        public int Edibility { get; private set; }
        public string ObjectType { get; private set; }
        public string Category { get; private set; }
        public bool Outdoors { get; private set; }
        public bool Indoors { get; private set; }
        public int Fragility { get; private set; }

        public BigCraftable(int id, string name, int sellPrice, int edibility, string objectType, string category, string description, bool outdoors, bool indoors, int fragility, string displayName)
        : base(id, name, sellPrice, displayName, description)
        {
            Edibility = edibility;
            ObjectType = objectType;
            Category = category;
            Outdoors = outdoors;
            Indoors = indoors;
            Fragility = fragility;

            if (Name == "Rarecrow")
            {
                var pattern = @"\((\d) of \d\)"; // (# of 8)
                var match = Regex.Match(Description, pattern);
                var rarecrowNumber = match.Groups[1].Value;
                Name += $" #{rarecrowNumber}";
            }
        }

        public static string ConvertToApName(Object salableItem)
        {
            if (salableItem.Name != "Rarecrow")
            {
                return salableItem.Name;
            }

            var rarecrowNumber = GetRarecrowNumber(salableItem);
            return $"{salableItem.Name} #{rarecrowNumber}";
        }

        private static int GetRarecrowNumber(Object salableItem)
        {
            return salableItem.ParentSheetIndex switch
            {
                110 => 1,
                113 => 2,
                126 => 3,
                136 => 4,
                137 => 5,
                138 => 6,
                139 => 7,
                140 => 8,
                _ => throw new ArgumentException($"{salableItem.Name} is not a recognized rarecrow!")
            };
        }

        public override Item PrepareForGivingToFarmer(int amount = 1)
        {
            var bigCraftable = new Object(Vector2.Zero, Id);
            bigCraftable.Stack = amount;
            return bigCraftable;
        }

        public override Item PrepareForRecovery()
        {
            throw new NotImplementedException();
        }

        public override void GiveToFarmer(Farmer farmer, int amount = 1)
        {
            var bigCraftable = PrepareForGivingToFarmer();
            farmer.addItemByMenuIfNecessaryElseHoldUp(bigCraftable);
        }

        public override LetterAttachment GetAsLetter(ReceivedItem receivedItem, int amount = 1)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.GiveBigCraftable, Id.ToString());
        }
    }
}
