using Microsoft.Xna.Framework;
using ModdedUtilitiesNetworking.Framework.Extentions;
using Netcode;
using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModdedUtilitiesNetworking.Framework.Messages
{
    public class OutgoingMessageBase
    {

        public byte messageType;
        public long farmerID;
        public object[] data;
        public string uniqueID;

        public byte MessageType
        {
            get
            {
                return this.messageType;
            }
        }

        public long FarmerID
        {
            get
            {
                return this.farmerID;
            }
        }

        public Farmer SourceFarmer
        {
            get
            {
                return Game1.getFarmer(this.farmerID);
            }
        }

        public string UniqueID{
            get
            {
                return this.uniqueID;
            }
        }

        public ReadOnlyCollection<object> Data
        {
            get
            {
                return Array.AsReadOnly<object>(this.data);
            }
        }

        public OutgoingMessageBase(byte messageType, long farmerID, string uniqueID, params object[] data)
        {
            this.messageType = messageType;
            this.farmerID = farmerID;
            this.uniqueID = uniqueID;
            this.data = data;
        }

        public virtual OutgoingMessageBase generateOutgoingMessage(byte messageType, Farmer sourceFarmer, string uniqueID ,params object[] data)
        {
            return new OutgoingMessageBase(messageType, sourceFarmer.UniqueMultiplayerID, uniqueID ,data);
        }

        /*
        public virtual OutgoingMessageBase generateOutgoingMessage(IncomingMessage message)
        {
            OutgoingMessageBase msg = new OutgoingMessageBase(message.MessageType, message.FarmerID, message.uniqueID,new object[1]
            {
        (object) message.Data
            });
            return msg;
        }
        */

        public virtual void Write(BinaryWriter writer)
        {
            writer.Write(this.messageType);
            writer.Write(this.farmerID);
            writer.Write(this.uniqueID);
            object[] data = this.data;
            BinaryReaderWriterExtensions.WriteSkippable(writer, (Action)(() =>
            {
                foreach (object enumValue in data)
                {
                    if (enumValue is Vector2)
                    {
                        writer.Write(((Vector2)enumValue).X);
                        writer.Write(((Vector2)enumValue).Y);
                    }
                    else if (enumValue is Guid)
                        writer.Write(((Guid)enumValue).ToByteArray());
                    else if (enumValue is byte[])
                        writer.Write((byte[])enumValue);
                    else if (enumValue is bool)
                        writer.Write((bool)enumValue ? (byte)1 : (byte)0);
                    else if (enumValue is byte)
                        writer.Write((byte)enumValue);
                    else if (enumValue is int)
                        writer.Write((int)enumValue);
                    else if (enumValue is short)
                        writer.Write((short)enumValue);
                    else if (enumValue is float)
                        writer.Write((float)enumValue);
                    else if (enumValue is long)
                        writer.Write((long)enumValue);
                    else if (enumValue is string)
                        writer.Write((string)enumValue);
                    else if (enumValue is string[])
                    {
                        string[] strArray = (string[])enumValue;
                        writer.Write((byte)strArray.Length);
                        for (int index = 0; index < strArray.Length; ++index)
                            writer.Write(strArray[index]);
                    }
                    else
                    {
                        if (!(enumValue is IConvertible) || !enumValue.GetType().IsValueType)
                            throw new InvalidDataException();
                        BinaryReaderWriterExtensions.WriteEnum(writer, enumValue);
                    }
                }
            }));
        }

        public virtual OutgoingMessage convertToOutgoingMessage()
        {
            return new OutgoingMessage(this.messageType, this.farmerID, this.data);
        }

        public static void WriteFromMessage(OutgoingMessage message,BinaryWriter writer)
        {
            writer.Write(message.MessageType);
            writer.Write(message.FarmerID);
            object[] data = message.Data.ToArray();
            BinaryReaderWriterExtensions.WriteSkippable(writer, (Action)(() =>
            {
                foreach (object enumValue in data)
                {
                    if (enumValue is Vector2)
                    {
                        writer.Write(((Vector2)enumValue).X);
                        writer.Write(((Vector2)enumValue).Y);
                    }
                    else if (enumValue is Guid)
                        writer.Write(((Guid)enumValue).ToByteArray());
                    else if (enumValue is byte[])
                        writer.Write((byte[])enumValue);
                    else if (enumValue is bool)
                        writer.Write((bool)enumValue ? (byte)1 : (byte)0);
                    else if (enumValue is byte)
                        writer.Write((byte)enumValue);
                    else if (enumValue is int)
                        writer.Write((int)enumValue);
                    else if (enumValue is short)
                        writer.Write((short)enumValue);
                    else if (enumValue is float)
                        writer.Write((float)enumValue);
                    else if (enumValue is long)
                        writer.Write((long)enumValue);
                    else if (enumValue is string)
                        writer.Write((string)enumValue);
                    else if (enumValue is string[])
                    {
                        string[] strArray = (string[])enumValue;
                        writer.Write((byte)strArray.Length);
                        for (int index = 0; index < strArray.Length; ++index)
                            writer.Write(strArray[index]);
                    }
                    else
                    {
                        ModCore.processTypesToWrite(writer, (string)data[1], data[2]); //writer, stringType, data
                    }
                }
            }));

        }
    }
}

