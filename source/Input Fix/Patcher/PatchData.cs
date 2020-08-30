using System.IO;
using System.Security.Cryptography;

namespace Patcher
{
    public class PatchData
    {
        public byte[] Source_SHA1;
        public byte[] Dest_SHA1;
        public byte[] patch;
        public string version;

        private PatchData()
        {
        }

        public PatchData(string origin, string dest, string version)
        {
            this.version = version;
            byte[] b_ori = File.ReadAllBytes(origin);
            byte[] b_dest = File.ReadAllBytes(dest);
            Source_SHA1 = SHA1.Create().ComputeHash(b_ori);
            Dest_SHA1 = SHA1.Create().ComputeHash(b_dest);

            Stream patch = PatchHelper.CreatePatch(b_ori, b_dest);
            patch.Position = 0;//reset
            this.patch = new byte[patch.Length];
            patch.Read(this.patch, 0, (int)patch.Length);
        }

        public static PatchData GetDataFrom(Stream data)
        {
            var patchdata = new PatchData();
            patchdata.version = StreamHelper.ReadString(data);
            patchdata.Source_SHA1 = StreamHelper.ReadByte(data);
            patchdata.Dest_SHA1 = StreamHelper.ReadByte(data);
            patchdata.patch = StreamHelper.ReadByte(data);
            return patchdata;
        }

        public Stream ToStream()
        {
            var result = new MemoryStream();
            StreamHelper.WriteString(result, version);
            StreamHelper.WriteByte(result, Source_SHA1);
            StreamHelper.WriteByte(result, Dest_SHA1);
            StreamHelper.WriteByte(result, patch);
            return result;
        }
    }
}