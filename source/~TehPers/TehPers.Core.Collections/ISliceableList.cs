using System.Collections.Generic;

namespace TehPers.Core.Collections {
    public interface ISliceableList<T> : IList<T>, ISliceable<T> { }
}