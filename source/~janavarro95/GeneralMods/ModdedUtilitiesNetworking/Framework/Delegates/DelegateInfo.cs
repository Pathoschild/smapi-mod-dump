using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ModdedUtilitiesNetworking.Framework.Delegates.DelegateInfo;

namespace ModdedUtilitiesNetworking.Framework.Delegates
{
   
    public class DelegateInfo
    {
        public delegate void voidFunc(object obj);

        public delegate object reader(BinaryReader reader);
        public delegate void writer(BinaryWriter writer, object obj);


    }


    public class ReadWriter
    {
        reader reader;
        writer writer;
        public ReadWriter(reader reader, writer writer)
        {
            this.reader = reader;
            this.writer = writer;
        }

        public void write(BinaryWriter bWriter, object obj)
        {
            writer.Invoke(bWriter, obj);
        }

        public object read(BinaryReader bReader)
        {
            return reader.Invoke(bReader);
        }

    }
}
