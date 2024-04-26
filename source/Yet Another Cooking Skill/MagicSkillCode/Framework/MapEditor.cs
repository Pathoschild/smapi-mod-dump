/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using Microsoft.Xna.Framework;
using SpaceShared;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Log = BirbCore.Attributes.Log;
using xTile;
using xTile.Layers;
using xTile.Tiles;
using MagicSkillCode.Core;

namespace MagicSkillCode.Framework
{
    /// <summary>An asset editor which makes map changes for Magic.</summary>
    public class MapEditor
    {
        /*********
        ** Fields
        *********/
        /// <summary>The mod configuration.</summary>
        private readonly Config Config;

        /// <summary>The SMAPI API for loading content assets.</summary>
        private readonly IModContentHelper Content;

        /// <summary>Whether the player has Stardew Valley Expanded installed.</summary>
        private readonly bool HasStardewValleyExpanded;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="config">The mod configuration.</param>
        /// <param name="content">The SMAPI API for loading content assets.</param>
        /// <param name="hasStardewValleyExpanded">Whether the player has Stardew Valley Expanded installed.</param>
        public MapEditor(Config config, IModContentHelper content, bool hasStardewValleyExpanded)
        {
            this.Config = config;
            this.Content = content;
            this.HasStardewValleyExpanded = hasStardewValleyExpanded;
        }

        public bool TryEdit(AssetRequestedEventArgs e)
        {
            // add altar
            if (e.NameWithoutLocale.IsEquivalentTo($"Maps/{Config.AltarLocation}"))
            {
                e.Edit(asset =>
                {
                    (int altarX, int altarY) = this.GetAltarPosition();
                    Map altar = this.Content.Load<Map>("assets/altar.tmx");
                    asset.AsMap().PatchMap(altar, targetArea: new Rectangle(altarX, altarY, 3, 3));
                }, AssetEditPriority.Late);

                return true;
            }

            // add radio to Wizard's tower
            else if (e.NameWithoutLocale.IsEquivalentTo($"Maps/{Config.RadioLocation}"))
            {
                e.Edit(asset =>
                {
                    Map map = asset.AsMap().Data;

                    // get buildings layer
                    Layer buildingsLayer = map.GetLayer("Buildings");
                    if (buildingsLayer == null)
                    {
                        Log.Warn("Can't add radio to Wizard's tower: 'Buildings' layer not found.");
                        return;
                    }

                    // get front layer
                    Layer frontLayer = map.GetLayer("Front");
                    if (frontLayer == null)
                    {
                        Log.Warn("Can't add radio to Wizard's tower: 'Front' layer not found.");
                        return;
                    }

                    // get tilesheet
                    TileSheet tilesheet = map.GetTileSheet("untitled tile sheet");
                    if (tilesheet == null)
                    {
                        Log.Warn("Can't add radio to Wizard's tower: main tilesheet not found.");
                        return;
                    }

                    // add radio
                    (int radioX, int radioY) = this.GetRadioPosition();
                    frontLayer.Tiles[radioX, radioY] = new StaticTile(frontLayer, tilesheet, BlendMode.Alpha, 512);
                    (buildingsLayer.Tiles[radioX, radioY] ?? frontLayer.Tiles[radioX, radioY]).Properties["Action"] = "MagicRadio";
                }, AssetEditPriority.Late);
                return true;
            }

            return false;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Get the tile position on which to place the altar.</summary>
        private (int x, int y) GetAltarPosition()
        {
            int x = Config.AltarX;
            int y = Config.AltarY;

            if (x < 0)
                x = 36;
            if (y < 0)
                y = 15;

            return (x, y);
        }

        /// <summary>Get the tile position on which to place the radio.</summary>
        private (int x, int y) GetRadioPosition()
        {
            int x = Config.RadioX;
            int y = Config.RadioY;

            if (x < 0)
                x = this.HasStardewValleyExpanded ? 5 : 1;
            if (y < 0)
                y = this.HasStardewValleyExpanded ? 23 : 5;

            return (x, y);
        }
    }
}
