/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using Microsoft.Xna.Framework.Graphics;

using xTile;
using xTile.Layers;
using xTile.Tiles;

using StardewValley;

namespace PlayGround
{
    /// <summary>The mod entry class.</summary>
    class ModWrapper : Mod
    {
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Helper.ConsoleCommands.Add("pg_debug", "pg_debug <action>", (cmd, args) =>
            {
                switch (args[0])
                {
                    case "pooltest":
                        string mapPath = this.Helper.Content.GetActualAssetKey("map.tbin");
                        Texture2D texture = null;
                        try
                        {
                            texture = this.Helper.Content.Load<Texture2D>("steam.png");
                        }
                        catch
                        {
                            texture = null;
                        }
                        Game1.locations.Add(new CustomBathhouse(mapPath, "CustomBathhouse", texture));
                        Game1.warpFarmer("CustomBathhouse", 15, 5, false);
                        break;
                }
            });
        }
    }
}
