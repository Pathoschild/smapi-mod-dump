/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mystra007/ExtendedFridgeSMAPI
**
*************************************************/

namespace ExtendedFridge
{
    internal class FridgeModConfig
    {
        public string fridgePrevPageKey {get; set;}
        public string fridgeNextPageKey {get; set;}
        public bool autoSwitchPageOnGrab {get; set;}

        public FridgeModConfig()
        {
            fridgeNextPageKey = "Right";
            fridgePrevPageKey = "Left";
            autoSwitchPageOnGrab = true;
        }
    }
}