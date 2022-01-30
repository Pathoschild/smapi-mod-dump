/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dawilly/SAAT
**
*************************************************/

namespace SAAT.API
{
    /// <summary>
    /// Parameter data for creating an audio cue.
    /// </summary>
    public struct CreateAudioInfo
    {
        public string Name;
        public Category Category;
        public bool Loop;
    }
}
