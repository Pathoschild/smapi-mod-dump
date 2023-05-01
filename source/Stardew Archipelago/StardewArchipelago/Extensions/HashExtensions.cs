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
using System.Security.Cryptography;
using System.Text;

namespace StardewArchipelago.Extensions
{
    public static class HashExtensions
    {
        public static int GetHash(this string text)
        {
            if (text == null)
            {
                return 0;
            }

            using HashAlgorithm algorithm = SHA256.Create();
            var bytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(text));
            var intValue = BitConverter.ToInt32(bytes, 0);
            return intValue;
        }
    }
}
