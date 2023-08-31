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
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Items.Mail;
using StardewValley;

namespace StardewArchipelago.Stardew
{
    public class StardewFurniture : StardewItem
    {
        private const string TYPE_BED = "bed";
        private const string TYPE_DOUBLE_BED = "bed double";
        private const string TYPE_FISHTANK = "fishtank";
        private const string TYPE_TV = "TV";

        public string Type { get; }
        public string TilesheetSize { get; }
        public string BoundingBoxSize { get; }
        public string Rotations { get; }
        public string PlacementRestriction { get; }

        public bool IsBed => Type.Equals(TYPE_BED, StringComparison.OrdinalIgnoreCase) ||
                             Type.Equals(TYPE_DOUBLE_BED, StringComparison.OrdinalIgnoreCase);

        public bool IsFishTank => Type.Equals(TYPE_FISHTANK, StringComparison.OrdinalIgnoreCase);

        public bool IsTV => Type.Contains(TYPE_TV, StringComparison.OrdinalIgnoreCase) ||
                             Name.Contains(TYPE_TV, StringComparison.OrdinalIgnoreCase);

        public StardewFurniture(int id, string name, string type, string tilesheetSize, string boundingBoxSize, string rotations, string price, string displayName, string placementRestriction): base(id, name, 0, displayName, "")
        {
            Type = type;
            TilesheetSize = tilesheetSize;
            BoundingBoxSize = boundingBoxSize;
            Rotations = rotations;
            PlacementRestriction = placementRestriction;
        }

        public override Item PrepareForGivingToFarmer(int amount = 1)
        {
            if (IsBed)
            {
                return new StardewValley.Objects.BedFurniture(Id, Vector2.Zero);
            }

            if (IsFishTank)
            {
                return new StardewValley.Objects.FishTankFurniture(Id, Vector2.Zero);
            }

            if (IsTV)
            {
                return new StardewValley.Objects.TV(Id, Vector2.Zero);
            }

            return new StardewValley.Objects.Furniture(Id, Vector2.Zero);
        }

        public override void GiveToFarmer(Farmer farmer, int amount = 1)
        {
            var furniture = PrepareForGivingToFarmer();
            farmer.addItemByMenuIfNecessaryElseHoldUp(furniture);
        }

        public override LetterAttachment GetAsLetter(ReceivedItem receivedItem, int amount = 1)
        {
            if (IsBed)
            {
                return new LetterActionAttachment(receivedItem, LetterActionsKeys.GiveBed, Id.ToString());
            }

            if (IsFishTank)
            {
                return new LetterActionAttachment(receivedItem, LetterActionsKeys.GiveFishTank, Id.ToString());
            }

            if (IsTV)
            {
                return new LetterActionAttachment(receivedItem, LetterActionsKeys.GiveTV, Id.ToString());
            }
            
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.GiveFurniture, Id.ToString());
        }
    }
}
