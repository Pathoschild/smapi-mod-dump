using System.Collections.Generic;

namespace TehPers.CoreMod.Api.Conflux.Collections {
    public interface ISliceableList<T> : IList<T>, ISliceable<T> { }
}