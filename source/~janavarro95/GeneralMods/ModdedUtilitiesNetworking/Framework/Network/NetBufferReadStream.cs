using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModdedUtilitiesNetworking.Framework.Network
{
    public class NetBufferReadStream : Stream
    {
        private long offset;
        public NetBuffer Buffer;

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override long Length
        {
            get
            {
                return ((long)this.Buffer.LengthBits - this.offset) / 8L;
            }
        }

        public override long Position
        {
            get
            {
                return (this.Buffer.Position - this.offset) / 8L;
            }
            set
            {
                this.Buffer.Position = this.offset + value * 8L;
            }
        }

        public NetBufferReadStream(NetBuffer buffer)
        {
            this.Buffer = buffer;
            this.offset = buffer.Position;
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            this.Buffer.ReadBytes(buffer, offset, count);
            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (origin == SeekOrigin.Begin)
                this.Position = offset;
            else if (origin == SeekOrigin.Current)
                this.Position = this.Position + offset;
            else if (origin == SeekOrigin.End)
                this.Position = this.Length + offset;
            return this.Position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
