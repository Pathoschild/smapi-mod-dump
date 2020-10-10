/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

namespace Entoarox.AdvancedLocationLoader.Configs
{
    internal class Redirect
    {
        /*********
        ** Accessors
        *********/
#pragma warning disable CS0649
        public string FromFile;
        public string ToFile;
#pragma warning restore CS0649


        /*********
        ** Public methods
        *********/
        public override string ToString()
        {
            return $"Redirect({this.FromFile} => {this.ToFile}{')'}";
        }
    }
}
