namespace TehPers.Core.Api.Weighted {
    public class WeightedElement<T> : IWeightedElement<T> {
        public T Value { get; }
        private readonly double _weight;

        public WeightedElement(T elem, double weight) {
            this.Value = elem;
            this._weight = weight;
        }

        public double GetWeight() {
            return this._weight;
        }
    }
}
