using System;
using System.IO;
using Lidgren.Network;

namespace StardustCore.NetCode
{
    public class NetBufferWriteStream : Stream
    {
        private int offset;
        public NetBuffer Buffer;

        public NetBufferWriteStream(NetBuffer buffer)
        {
            this.Buffer = buffer;
            this.offset = buffer.LengthBits;
        }

        public override bool CanRead
        {
            get
            {
                return false;
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
                return true;
            }
        }

        public override long Length
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override long Position
        {
            get
            {
                return (long)((this.Buffer.LengthBits - this.offset) / 8);
            }
            set
            {
                this.Buffer.LengthBits = (int)((long)this.offset + value * 8L);
            }
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    this.Position = offset;
                    break;
                case SeekOrigin.Current:
                    this.Position += offset;
                    break;
                case SeekOrigin.End:
                    throw new NotSupportedException();
            }
            return this.Position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.Buffer.Write(buffer, offset, count);
        }
    }
}
