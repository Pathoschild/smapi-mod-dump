namespace TehPers.CoreMod.Api.ContentLoading {
    public interface IContentSource {
        string Path { get; }
        T Load<T>(string path);
    }
}