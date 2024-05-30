/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
using System.Linq;
using System.Reflection;

namespace Multi_User_Chest
{
    public class ModEntry : Mod
    {
        private readonly PerScreen<Vector2?> Tile = new();
        private FieldInfo currentLidFrame;

        public override void Entry(IModHelper helper) //I'm commited to doing this without harmony at this point
        {
            helper.Events.Input.ButtonPressed += OnButtonDown;

            helper.Events.World.ObjectListChanged += OnObjectListChanged;
            helper.Events.Display.MenuChanged += OnMenuChanged;

            currentLidFrame = typeof(Chest).GetField("currentLidFrame", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        private void OnButtonDown(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsMultiplayer || Game1.getOnlineFarmers().Count <= 1 || Game1.activeClickableMenu != null) 
                return;

            if (e.Button.IsActionButton())
            {
                Vector2 tile = e.Cursor.GrabTile;

                if (Helper.ModRegistry.IsLoaded("spacechase0.ExtendedReach")) 
                    tile = e.Cursor.Tile;

                var OatTOrig = Game1.player.currentLocation?.getObjectAtTile((int)tile.X, (int)tile.Y);
                if (OatTOrig is null || OatTOrig is not Chest c)
                    return;
                int lidFrame = (int)currentLidFrame.GetValue(c);

                DelayedAction.functionAfterDelay(() =>
                {
                    var OatT = Game1.player.currentLocation?.getObjectAtTile((int)tile.X, (int)tile.Y); //Check after delay for the chest object
                    if (OatT?.QualifiedItemId == OatTOrig?.QualifiedItemId && OatT is Chest c && c.playerChest.Value && c.GetMutex().IsLocked())
                    {
                        if (lidFrame != c.startingLidFrame.Value)
                            Game1.playSound("openChest");
                        c.ShowMenu();
                        Tile.Value = c.TileLocation;
                    }
                }, 250);
            }
        }

        private void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            if (!e.IsCurrentLocation)
                return;
            if (Tile.Value is not null && e.Removed.Any(x => x.Key == Tile.Value))
                Game1.activeClickableMenu = null;
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is null && Tile.Value is not null)
                Tile.Value = null;
        }

        /*private void OnUpdateTicked(object sender, UpdateTickedEventArgs e) //Close the active chest menu if the chest is replaced \\This was the dumbest fucking thing I wrote in a minute
        {
        }*/
    }
}
