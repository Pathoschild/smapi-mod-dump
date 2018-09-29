using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModdedUtilitiesNetworking.Framework.Network
{
    
        public class NetBufferWriteStream : Stream
        {
            private int offset;
            public NetBuffer Buffer;

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

            public NetBufferWriteStream(NetBuffer buffer)
            {
                this.Buffer = buffer;
                this.offset = buffer.LengthBits;
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
                if (origin == SeekOrigin.Begin)
                    this.Position = offset;
                else if (origin == SeekOrigin.Current)
                    this.Position = this.Position + offset;
                else if (origin == SeekOrigin.End)
                    throw new NotSupportedException();
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

