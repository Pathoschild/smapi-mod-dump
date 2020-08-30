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