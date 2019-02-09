namespace TehPers.Core.Multiplayer.Synchronized {
    public interface ISynchronizedWrapper<T> : ISynchronized {
        T Value { get; set; }
    }
}