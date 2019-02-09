namespace TehPers.FishingOverhaul.Api.Overridables {
    public class OverridableConst<T> : Overridable<T> {
        public OverridableConst(T defaultValues) : base(() => defaultValues) { }
    }
}