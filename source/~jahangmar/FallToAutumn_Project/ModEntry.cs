/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jahangmar/StardewValleyMods
**
*************************************************/

// Copyright (c) 2020 Jahangmar
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;
using StardewModdingAPI;

namespace FallToAutumn
{
    public class ModEntry : Mod, IAssetEditor
    {
        const string StringsFromCSFiles = "Strings/StringsFromCSFiles";
        const string ObjectInformation = "Data/ObjectInformation";
        const string Pierre = "Characters/Dialogue/Pierre";
        const string TV = "Data/TV/TipChannel";

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return (asset.AssetNameEquals(StringsFromCSFiles)
                || asset.AssetNameEquals(ObjectInformation)
                || asset.AssetNameEquals(Pierre)
                || asset.AssetNameEquals(TV));
        }

        public override void Entry(IModHelper helper)
        {

        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals(StringsFromCSFiles))
            {
                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                data["Utility.cs.5682"] = "Autumn";
                data["Utility.cs.5746"] = "Autumn (The Smell Of Mushroom)";
                data["Utility.cs.5748"] = "Autumn (Ghost Synth)";
                data["Utility.cs.5750"] = "Autumn (Raven's Descent)";
                data["Seeds.cs.14237"] = "Autumn Mix";
                data["fall"] = "autumn";
            }
            else if (asset.AssetNameEquals(ObjectInformation))
            {
                IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;
                foreach (int id in new List<int>(data.Keys))
                {
                    string[] array = data[id].Split('/');
                    string descr = array[5];
                    if (descr.Contains("falling"))
                        continue;

                    data[id] = data[id].Replace("fall", "autumn");

                    if (id == 497)//fall seeds
                    {
                        data[id] = data[id].Replace("Fall", "Autumn"); //the word "Fall" appears as an internal reference in other places
                    }
                }
            }
            else if (asset.AssetNameEquals(Pierre))
            {
                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                data["fall_Mon"] = data["fall_Mon"].Replace("Fall", "Autumn");
            }
            else if (asset.AssetNameEquals(TV))
            {
                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                data["53"] = data["53"].Replace("Fall", "Autumn");
                foreach (string id in new List<string>() { "36", "67", "78", "116", "186"})
                {
                    data[id] = data[id].Replace("fall", "autumn");
                }
            }
        }
    }
}
