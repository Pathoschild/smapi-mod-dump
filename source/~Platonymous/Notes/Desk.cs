/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using PyTK.CustomElementHandler;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System.Collections.Generic;

namespace Notes
{
    class Desk : PySObject
    {
        public Desk()
        {

        }

        public Desk(CustomObjectData data)
            : base(data, Vector2.Zero)
        {
        }

        public Desk(CustomObjectData data, Vector2 tileLocation)
            : base(data, tileLocation)
        {
        }

        public override Item getOne()
        {
            return new Desk(data) { TileLocation = Vector2.Zero };
        }

        public override ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
        {
            if (additionalSaveData["id"] == "na")
                additionalSaveData["id"] = "Notes.Desk";

            CustomObjectData data = CustomObjectData.collection[additionalSaveData["id"]];
            return new Desk(CustomObjectData.collection[additionalSaveData["id"]], (replacement as Chest).TileLocation);
        }

        public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        {
            if (justCheckingForActivity)
                return true;

            Game1.activeClickableMenu = (IClickableMenu)new NoteMenu(new NamingMenu.doneNamingBehavior((s) => {

                if (s.Length > 0) {
                    if (s.Length > 100)
                        s = s.Substring(0, 97) + "...";

                    Note note = (Note) NotesMod.Note.getObject();
                    note.text = s;
                    Game1.exitActiveMenu();
                    Game1.player.addItemByMenuIfNecessary(note);
                }

            }), NotesMod.NoteInfo, "");
            return true;
        }
    }
}
