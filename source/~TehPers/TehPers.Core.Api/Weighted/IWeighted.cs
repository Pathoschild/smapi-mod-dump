namespace TehPers.Core.Api.Weighted {
    /// <summary>Defines a weighted chance for an object, allowing easy weighted choosing of a random element from a list of the object.</summary>
    public interface IWeighted {
        /// <summary>Returns the positive weighted chance of this object being selected (in comparison to other objects).</summary>
        double GetWeight();
    }
}
