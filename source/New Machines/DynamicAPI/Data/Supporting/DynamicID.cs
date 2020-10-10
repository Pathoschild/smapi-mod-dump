/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System;

namespace Igorious.StardewValley.DynamicAPI.Data.Supporting
{
    public class DynamicID<TEnum1, TEnum2> : DynamicID<TEnum1> where TEnum1 : struct where TEnum2 : struct
    {
        protected DynamicID(int value) : base(value) {}

        public static implicit operator DynamicID<TEnum1, TEnum2>(TEnum1 e)
        {
            return new DynamicID<TEnum1, TEnum2>(Convert.ToInt32(e));
        }

        public static implicit operator DynamicID<TEnum1, TEnum2>(TEnum2 e)
        {
            return new DynamicID<TEnum1, TEnum2>(Convert.ToInt32(e));
        }

        public static implicit operator DynamicID<TEnum1, TEnum2>(int i)
        {
            return new DynamicID<TEnum1, TEnum2>(i);
        }
    }

    public class DynamicID<TEnum> : IConvertible where TEnum : struct
    {
        protected DynamicID(int value)
        {
            Value = value;
        }

        protected int Value { get; }

        public static implicit operator DynamicID<TEnum>(TEnum e)
        {
            return new DynamicID<TEnum>(Convert.ToInt32(e));
        }

        public static implicit operator DynamicID<TEnum>(int i)
        {
            return new DynamicID<TEnum>(i);
        }

        public static implicit operator int(DynamicID<TEnum> e)
        {
            return e.Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        #region Equality Members

        private bool Equals(DynamicID<TEnum> other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is DynamicID<TEnum> && Equals((DynamicID<TEnum>)obj);
        }

        public static bool operator ==(DynamicID<TEnum> left, DynamicID<TEnum> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DynamicID<TEnum> left, DynamicID<TEnum> right)
        {
            return !Equals(left, right);
        }

        public override int GetHashCode()
        {
            return Value;
        }

        #endregion

        #region IConvertible Implementation

        TypeCode IConvertible.GetTypeCode() => TypeCode.Object;
        bool IConvertible.ToBoolean(IFormatProvider provider) => Convert.ToBoolean(Value);
        char IConvertible.ToChar(IFormatProvider provider) => Convert.ToChar(Value);
        sbyte IConvertible.ToSByte(IFormatProvider provider) => Convert.ToSByte(Value);
        byte IConvertible.ToByte(IFormatProvider provider) => Convert.ToByte(Value);
        short IConvertible.ToInt16(IFormatProvider provider) => Convert.ToInt16(Value);
        ushort IConvertible.ToUInt16(IFormatProvider provider) => Convert.ToUInt16(Value);
        int IConvertible.ToInt32(IFormatProvider provider) => Value;
        uint IConvertible.ToUInt32(IFormatProvider provider) => Convert.ToUInt32(Value);
        long IConvertible.ToInt64(IFormatProvider provider) => Convert.ToInt64(Value);
        ulong IConvertible.ToUInt64(IFormatProvider provider) => Convert.ToUInt64(Value);
        float IConvertible.ToSingle(IFormatProvider provider) => Convert.ToSingle(Value);
        double IConvertible.ToDouble(IFormatProvider provider) => Convert.ToDouble(Value);
        decimal IConvertible.ToDecimal(IFormatProvider provider) => Convert.ToDecimal(Value);
        DateTime IConvertible.ToDateTime(IFormatProvider provider) => Convert.ToDateTime(Value);
        string IConvertible.ToString(IFormatProvider provider) => Convert.ToString(Value);
        object IConvertible.ToType(Type conversionType, IFormatProvider provider) => ((IConvertible)Value).ToType(conversionType, provider);

        #endregion
    }
}
