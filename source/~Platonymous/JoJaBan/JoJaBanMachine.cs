/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using PyTK.CustomElementHandler;
using StardewValley;
using System.Collections.Generic;

namespace JoJaBan
{
    class JoJaBanMachine : PySObject
    {
        public JoJaBanMachine()
        {

        }

        public JoJaBanMachine(CustomObjectData data)
            : base(data, Vector2.Zero)
        {
        }

        public JoJaBanMachine(CustomObjectData data, Vector2 tileLocation)
            : base(data, tileLocation)
        {
        }

        public override bool checkForAction(StardewValley.Farmer who, bool justCheckingForActivity = false)
        {
            if (justCheckingForActivity)
                return true;
            JoJaBanMod.startGame("", null, Vector2.Zero, "");
            return true;
        }

        public override Item getOne()
        {
            return new JoJaBanMachine(data);
        }

        public override ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
        {
            if (additionalSaveData.ContainsKey("high"))
                JoJaBanMod.highestLevel = int.Parse(additionalSaveData["high"]);

            return new JoJaBanMachine(JoJaBanMod.arcadeData);
        }

        public override Dictionary<string, string> getAdditionalSaveData()
        {
            var data = base.getAdditionalSaveData();
            data.Add("high", JoJaBanMod.highestLevel.ToString());
            return data;
        }


    }
}
