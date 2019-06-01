using xTile;

namespace BeyondTheValleyExpansion
{
    /// <summary> Interface to provide an API for Jessebot.BeyondtheValley </summary>
    public interface IBeyondtheValleyAPI
    {
        /// <summary> Load a new asset instead of the Default/Content Pack edit </summary>
        /// <param name="replaceFile"> the file to replace relative to "Jessebot.BeyondtheValley"'s root folder </param>
        /// <param name="newMap"> the new map asset </param>
        void LoadNewAsset(string replaceFile, Map newMap);
    }
}
