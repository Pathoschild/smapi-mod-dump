/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Windmill-City/InputFix
**
*************************************************/

using BsDiff;
using System.IO;

namespace Patcher
{
    public class PatchHelper
    {
        public static Stream GetPatched(Stream origin, byte[] patchdata)
        {
            Stream output = new MemoryStream();
            BinaryPatchUtility.Apply(origin, () => new MemoryStream(patchdata), output);
            return output;
        }

        public static Stream CreatePatch(byte[] origin, byte[] dest)
        {
            Stream output = new MemoryStream();
            BinaryPatchUtility.Create(origin, dest, output);
            return output;
        }
    }
}