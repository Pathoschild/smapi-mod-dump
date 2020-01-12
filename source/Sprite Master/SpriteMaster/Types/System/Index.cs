#nullable enable

using System.Runtime.CompilerServices;

#if NETSTANDARD2_1
[assembly: TypeForwardedTo(typeof(System.Index))]
#else
namespace System {
  /// <summary>Represent a type can be used to index a collection either from the start or the end.</summary>
  /// <remarks>
  /// Index is used by the C# compiler to support the new index syntax
  /// <code>
  /// int[] someArray = new int[5] { 1, 2, 3, 4, 5 } ;
  /// int lastElement = someArray[^1]; // lastElement = 5
  /// </code>
  /// </remarks>
  public readonly struct Index : IEquatable<Index> {
    public readonly Type _originalType;
    private readonly long _value;

    /// <summary>Construct an Index using a value and indicating if the index is from the start or from the end.</summary>
    /// <param name="value">The index value. it has to be zero or positive number.</param>
    /// <param name="fromEnd">Indicating if the index is from the start or from the end.</param>
    /// <remarks>
    /// If the Index constructed from the end, index value 1 means pointing at the last element and index value 0 means pointing at beyond last element.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Index (sbyte value, bool fromEnd) {
      if (value < 0) {
        throw new ArgumentOutOfRangeException(nameof(value), "value must be non-negative");
      }

      _originalType = typeof(sbyte);

      if (fromEnd)
        _value = ~value;
      else
        _value = value;
    }

    /// <summary>Construct an Index using a value and indicating if the index is from the start or from the end.</summary>
    /// <param name="value">The index value. it has to be zero or positive number.</param>
    /// <param name="fromEnd">Indicating if the index is from the start or from the end.</param>
    /// <remarks>
    /// If the Index constructed from the end, index value 1 means pointing at the last element and index value 0 means pointing at beyond last element.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Index (short value, bool fromEnd) {
      if (value < 0) {
        throw new ArgumentOutOfRangeException(nameof(value), "value must be non-negative");
      }

      _originalType = typeof(short);

      if (fromEnd)
        _value = ~value;
      else
        _value = value;
    }

    /// <summary>Construct an Index using a value and indicating if the index is from the start or from the end.</summary>
    /// <param name="value">The index value. it has to be zero or positive number.</param>
    /// <param name="fromEnd">Indicating if the index is from the start or from the end.</param>
    /// <remarks>
    /// If the Index constructed from the end, index value 1 means pointing at the last element and index value 0 means pointing at beyond last element.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Index (int value, bool fromEnd) {
      if (value < 0) {
        throw new ArgumentOutOfRangeException(nameof(value), "value must be non-negative");
      }

      _originalType = typeof(int);

      if (fromEnd)
        _value = ~value;
      else
        _value = value;
    }

    /// <summary>Construct an Index using a value and indicating if the index is from the start or from the end.</summary>
    /// <param name="value">The index value. it has to be zero or positive number.</param>
    /// <param name="fromEnd">Indicating if the index is from the start or from the end.</param>
    /// <remarks>
    /// If the Index constructed from the end, index value 1 means pointing at the last element and index value 0 means pointing at beyond last element.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Index (long value, bool fromEnd) {
      if (value < 0) {
        throw new ArgumentOutOfRangeException(nameof(value), "value must be non-negative");
      }

      _originalType = typeof(long);

      if (fromEnd)
        _value = ~value;
      else
        _value = value;
    }

    /// <summary>Construct an Index using a value and indicating if the index is from the start or from the end.</summary>
    /// <param name="value">The index value. it has to be zero or positive number.</param>
    /// <param name="fromEnd">Indicating if the index is from the start or from the end.</param>
    /// <remarks>
    /// If the Index constructed from the end, index value 1 means pointing at the last element and index value 0 means pointing at beyond last element.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Index (byte value, bool fromEnd) {
      if (value < 0) {
        throw new ArgumentOutOfRangeException(nameof(value), "value must be non-negative");
      }

      _originalType = typeof(byte);

      if (fromEnd)
        _value = ~value;
      else
        _value = value;
    }

    /// <summary>Construct an Index using a value and indicating if the index is from the start or from the end.</summary>
    /// <param name="value">The index value. it has to be zero or positive number.</param>
    /// <param name="fromEnd">Indicating if the index is from the start or from the end.</param>
    /// <remarks>
    /// If the Index constructed from the end, index value 1 means pointing at the last element and index value 0 means pointing at beyond last element.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Index (ushort value, bool fromEnd) {
      if (value < 0) {
        throw new ArgumentOutOfRangeException(nameof(value), "value must be non-negative");
      }

      _originalType = typeof(ushort);

      if (fromEnd)
        _value = ~value;
      else
        _value = value;
    }

    /// <summary>Construct an Index using a value and indicating if the index is from the start or from the end.</summary>
    /// <param name="value">The index value. it has to be zero or positive number.</param>
    /// <param name="fromEnd">Indicating if the index is from the start or from the end.</param>
    /// <remarks>
    /// If the Index constructed from the end, index value 1 means pointing at the last element and index value 0 means pointing at beyond last element.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Index (uint value, bool fromEnd) {
      if (value < 0) {
        throw new ArgumentOutOfRangeException(nameof(value), "value must be non-negative");
      }

      _originalType = typeof(uint);

      if (fromEnd)
        _value = ~value;
      else
        _value = value;
    }

    /// <summary>Construct an Index using a value and indicating if the index is from the start or from the end.</summary>
    /// <param name="value">The index value. it has to be zero or positive number.</param>
    /// <param name="fromEnd">Indicating if the index is from the start or from the end.</param>
    /// <remarks>
    /// If the Index constructed from the end, index value 1 means pointing at the last element and index value 0 means pointing at beyond last element.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Index (ulong value, bool fromEnd) {
      if (value < 0) {
        throw new ArgumentOutOfRangeException(nameof(value), "value must be non-negative");
      }

      _originalType = typeof(ulong);

      unchecked {
        if (fromEnd)
          _value = ~(long)value;
        else
          _value = (long)value;
      }
    }

    // The following private constructors mainly created for perf reason to avoid the checks
    private Index (long value, Type type) {
      _originalType = type;
      _value = value;
    }

    /// <summary>Create an Index pointing at first element.</summary>
    public static Index Start => new Index(0, typeof(int));

    /// <summary>Create an Index pointing at beyond last element.</summary>
    public static Index End => new Index(~0, typeof(int));

    /// <summary>Create an Index from the start at the position indicated by the value.</summary>
    /// <param name="value">The index value from the start.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Index FromStart (long value, Type type) {
      if (value < 0) {
        throw new ArgumentOutOfRangeException(nameof(value), "value must be non-negative");
      }

      return new Index(value, type);
    }

    /// <summary>Create an Index from the start at the position indicated by the value.</summary>
    /// <param name="value">The index value from the start.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Index FromStart (long value) {
      return FromStart(value, typeof(int));
    }

    /// <summary>Create an Index from the end at the position indicated by the value.</summary>
    /// <param name="value">The index value from the end.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Index FromEnd (long value, Type type) {
      if (value < 0) {
        throw new ArgumentOutOfRangeException(nameof(value), "value must be non-negative");
      }

      return new Index(~value, type);
    }

    /// <summary>Create an Index from the end at the position indicated by the value.</summary>
    /// <param name="value">The index value from the end.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Index FromEnd (long value) {
      return FromEnd(value, typeof(int));
    }

    /// <summary>Returns the index value.</summary>
    public long Value {
      get {
        if (_value < 0)
          return ~_value;
        else
          return _value;
      }
    }

    /// <summary>Indicates whether the index is from the start or the end.</summary>
    public bool IsFromEnd => _value < 0;

    /// <summary>Calculate the offset from the start using the giving collection length.</summary>
    /// <param name="length">The length of the collection that the Index will be used with. length has to be a positive value</param>
    /// <remarks>
    /// For performance reason, we don't validate the input length parameter and the returned offset value against negative values.
    /// we don't validate either the returned offset is greater than the input length.
    /// It is expected Index will be used with collections which always have non negative length/count. If the returned offset is negative and
    /// then used to index a collection will get out of range exception which will be same affect as the validation.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long GetOffset (int length) {
      var offset = _value;
      if (IsFromEnd) {
        // offset = length - (~value)
        // offset = length + (~(~value) + 1)
        // offset = length + value + 1

        offset += length + 1;
      }
      return offset;
    }

    /// <summary>Indicates whether the current Index object is equal to another object of the same type.</summary>
    /// <param name="value">An object to compare with this object</param>
    public override bool Equals (object? value) => value is Index && _value == ((Index)value)._value;

    /// <summary>Indicates whether the current Index object is equal to another Index object.</summary>
    /// <param name="other">An object to compare with this object</param>
    public bool Equals (Index other) => _value == other._value;

    /// <summary>Returns the hash code for this instance.</summary>
    public override int GetHashCode () => _value.GetHashCode();

    /// <summary>Converts integer number to an Index.</summary>
    public static implicit operator Index (sbyte value) => FromStart(value, typeof(sbyte));

    /// <summary>Converts integer number to an Index.</summary>
    public static implicit operator Index (short value) => FromStart(value, typeof(short));

    /// <summary>Converts integer number to an Index.</summary>
    public static implicit operator Index (int value) => FromStart(value, typeof(int));

    /// <summary>Converts integer number to an Index.</summary>
    public static implicit operator Index (long value) => FromStart(value, typeof(long));

    /// <summary>Converts integer number to an Index.</summary>
    public static implicit operator Index (byte value) => FromStart(value, typeof(byte));

    /// <summary>Converts integer number to an Index.</summary>
    public static implicit operator Index (ushort value) => FromStart(value, typeof(ushort));

    /// <summary>Converts integer number to an Index.</summary>
    public static implicit operator Index (uint value) => FromStart(value, typeof(uint));

    /// <summary>Converts integer number to an Index.</summary>
    public static implicit operator Index (ulong value) => FromStart(unchecked((long)value), typeof(ulong));

    /// <summary>Converts the value of the current Index object to its equivalent string representation.</summary>
    public override string ToString () {
      if (IsFromEnd)
        return "^" + ((ulong)Value).ToString();

      return ((ulong)Value).ToString();
    }
  }
}
#endif
