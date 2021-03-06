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
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace SundropNPCTest
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            SubTypePatcher.Patch<NPC, SundropNPC>();
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
        }

        private void GameLoop_DayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            Game1.removeCharacterFromItsLocation("Pierre");
            Game1.getLocationFromName("SeedShop").addCharacter(new SundropNPC(new AnimatedSprite("Characters\\Pierre", 0, 16, 32), new Vector2(256f, 1088f), "SeedShop", 2, "Pierre", datable: false, null, Game1.content.Load<Texture2D>("Portraits\\Pierre")));
            Game1.getCharacterFromName("Pierre").resetForNewDay(Game1.dayOfMonth);
        }
    }
}
