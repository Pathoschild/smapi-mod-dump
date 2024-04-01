/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;

namespace CustomPictureFrames
{
    public class CustomPictureFramesApi
    {
        public Texture2D GetFrameTexture(string frameName, int index)
        {
            if (!ModEntry.pictureDict.ContainsKey(frameName))
            {
                ModEntry.SMonitor.Log($"Frame {frameName} not found");
                return null;
            }
            if(ModEntry.pictureDict[frameName].Count < index)
            {
                ModEntry.SMonitor.Log($"Index {index} for frame {frameName} not found");
                return null;
            }
            return ModEntry.pictureDict[frameName][index];
        }
    }
}