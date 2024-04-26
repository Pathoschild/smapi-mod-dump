/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

namespace stardew_access.Utils;

/// <summary>
/// A circular queue which dequeues the first element when enqueueing an element on a full queueu.
/// </summary>
public class BoundedQueue<T>
{
    T[] _queue;
    int _front;
    int _rear;
    bool _duplicacy;

    /// <summary>
    /// The size or maximum limit of the queue
    /// </summary>
    public int Size { get; private set; }

    /// <summary>
    /// Number of elements in the queue
    /// </summary>
    public int Count
    {
        get
        {
            if (IsEmpty()) return 0;
            if (IsFull()) return Size;

            return _rear >= _front
                ? _rear - _front + 1
                : Size - (_front - _rear - 1);
        }
    }

    /// <param name="size">The maximum limit of the queue.</param>
    /// <param name="allowDuplicacy">If false, it will not enqueue the element if it is equal to the last element.</param>
    public BoundedQueue(int size, bool allowDuplicacy)
    {
        Size = size;
        _duplicacy = allowDuplicacy;
        _queue = new T[Size];
        _front = _rear = -1;
    }

    /// <summary>
    /// Add the element from the rear.
    /// If the queue is full, dequeue the first element and then enqueue the new element.
    /// </summary>
    /// <param name="val">The element to be enqueued.</param>
    public void Add(T val)
    {
        if (val is null) return;
        if (val is string str && string.IsNullOrWhiteSpace(str)) return;

        if (IsEmpty())
        {
            _front = _rear = 0;
            _queue[_rear] = val;
            return;
        }

        if (!_duplicacy && val.Equals(_queue[_rear])) return;

        if (IsFull()) _front = NextIndex(_front);
        _rear = NextIndex(_rear);
        _queue[_rear] = val;
    }

    /// <summary>
    /// Remove the first element.
    /// </summary>
    /// <returns>The removed element.</returns>
    public T Remove()
    {
        if (IsEmpty()) return default(T)!;

        T deleted = _queue[_front];
        _front = _front == _rear ? -1 : NextIndex(_front);
        _rear = _front == -1 ? -1 : _rear;

        return deleted;
    }

    public bool IsEmpty() => _front is -1 && _rear is -1;

    public bool IsFull() => (_rear + 1) % Size == _front;

    private int NextIndex(int index, int interval = 1) => (index + interval) % Size;

    private int PreviousIndex(int index, int interval = 1) => (index - (interval % Size)) < 0 ? Size + index - (interval % Size) : index - (interval % Size);

    public T this[Index index]
    {
        get => index.IsFromEnd
            ? _queue[PreviousIndex(_rear, interval: index.Value - 1)]
            : _queue[NextIndex(_front, interval: index.Value)];
    }
}
