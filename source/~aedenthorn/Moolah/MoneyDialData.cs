/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using StardewModdingAPI.Utilities;
using System;
using System.Numerics;

namespace Moolah
{
    public class MoneyDialData
    {
        public BigInteger previousTarget = new();
        public BigInteger currentValue = new();
        public BigInteger flipSpeed = new();
        public BigInteger soundTime = new();
        public BigInteger moneyShineTimer = new();
        public BigInteger moneyMadeAccumulator = new();

        public void Reset()
        {
            previousTarget = 0;
            currentValue = 0;
            flipSpeed = 0;
            soundTime = 0;
            moneyShineTimer = 0;
            moneyMadeAccumulator = 0;
        }
    }
}