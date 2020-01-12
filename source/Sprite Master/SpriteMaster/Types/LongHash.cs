namespace SpriteMaster.Types {
	public static class LongHash {
		public const ulong Null = 0UL;

		public static ulong GetLongHashCode<T>(this T obj) {
			if (obj is ILongHash hashable) {
				return hashable.GetLongHashCode();
			}
			return From(obj);
		}

		public static ulong From (int hashCode) {
			switch (hashCode) {
				case 0:
					return 0UL;
				case -1:
					return ulong.MaxValue;
			}

			return unchecked((ulong)(uint)hashCode | (((ulong)~(uint)hashCode) << 32));
		}

		public static ulong From (ILongHash obj) {
			return obj.GetLongHashCode();
		}

		public static ulong From<T> (T obj) {
			if (obj is ILongHash hashable) {
				return hashable.GetLongHashCode();
			}
			return From(obj.GetHashCode());
		}
	}

	public interface ILongHash {
		public ulong GetLongHashCode ();
	}
}
