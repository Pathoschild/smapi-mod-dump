/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/congha22/foodstore
**
*************************************************/

using StardewModdingAPI;
using System.Collections.Generic;

namespace MarketTown
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public bool EnablePrice { get; set; } = false;
        public bool EnableTip { get; set; } = false;
        public bool TipWhenNeaBy { get; set; } = true;
        public bool RushHour { get; set; } = true;
        public bool EnableDecor { get; set; } = true;
        public bool DisableChat { get; set; } = false;
        public bool DisableChatAll { get; set; } = false;

        public int MinutesToHungry { get; set; } = 240;
        public float MoveToFoodChance { get; set; } = 0.005f;
        public float MaxDistanceToFind { get; set; } = 40;
        public float MaxDistanceToEat { get; set; } = 4f;

        public float LoveMultiplier { get; set; } = -1f;
        public float LikeMultiplier { get; set; } = -1f;
        public float NeutralMultiplier { get; set; } = -1f;
        public float DislikeMultiplier { get; set; } = -1f;
        public float HateMultiplier { get; set; } = -1f;

        public float TipLove { get; set; } = -1f;
        public float TipLike { get; set; } = -1f;
        public float TipNeutral { get; set; } = -1f;
        public float TipDislike { get; set; } = -1f;
        public float TipHate { get; set; } = -1f;

        public float InviteComeTime { get; set; } = 1000;
        public float InviteLeaveTime { get; set; } = 2000;
        public bool EnableVisitInside { get; set; } = true;
        public int DialogueTime { get; set; } = 1;
        public bool AllowRemoveNonFood { get; set; } = false;

        public bool DisableKidAsk { get; set; } = false;
        public bool EnableSaleWeapon { get; set; } = true;
        public bool RandomPurchase { get; set; } = false;
        public int SignRange { get; set; } = 0;


        public bool DoorEntry { get; set; } = true;
        public float ShedVisitChance { get; set;} = 0.2f;
        public int MaxShedCapacity { get; set; } = 5;
        public int TimeStay { get; set; } = 230;
        public int OpenHour { get; set; } = 800;
        public int CloseHour { get;set; } = 2200;

        public float ShedMoveToFoodChance { get; set; } = 0.2f;
        public int ShedMinuteToHungry { get; set; } = 90;

        public float KidAskChance { get; set; } = 0.2f;

        public bool BusWalk { get; set; } = true;

        public SButton ModKey { get; set; } = SButton.LeftAlt;
        public int MaxNPCOrdersPerNight { get; set; } = 3;
        public float PriceMarkup { get; set; } = 3f;
        public float OrderChance { get; set; } = 0.02f;
        public float LovedDishChance { get; set; } = 0.8f;
        public List<string> RestaurantLocations { get; set; } = new List<string>()
        {
            "Shed",
            "Big Shed"
        };
        public int NPCCheckTimer { get; set; } = 1;
        public float MuseumPriceMarkup { get; set; } = 1.0f;
        public bool MultiplayerMode { get; set; } = false;

    }
}
