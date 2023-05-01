/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/RealSweetPanda/SaveAnywhereRedux
**
*************************************************/

namespace SaveAnywhere.Framework.Model
{
    public class PlayerData
    {
        public int Time { get; init; }
        public BuffData[] OtherBuffs { get; init; }

        public BuffData DrinkBuff { get; init; }

        public BuffData FoodBuff { get; init; }
        public PositionData[] Position { get; init; }

        public bool IsCharacterSwimming { get; init; }
    }
}