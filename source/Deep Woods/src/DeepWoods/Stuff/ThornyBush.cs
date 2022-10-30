/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/maxvollmer/DeepWoodsMod
**
*************************************************/

using DeepWoodsMod.API.Impl;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.TerrainFeatures;
using static DeepWoodsMod.DeepWoodsSettings;

namespace DeepWoodsMod
{
    public class ThornyBush : DestroyableBush
    {
        public ThornyBush()
            : base()
        {
            minAxeLevel = Settings.Objects.Bush.ThornyBushMinAxeLevel;
        }

        public ThornyBush(Vector2 tileLocation, GameLocation location)
            : base(tileLocation, Bush.smallBush, location)
        {
            minAxeLevel = Settings.Objects.Bush.ThornyBushMinAxeLevel;
        }

        public override bool performUseAction(Vector2 tileLocation, GameLocation location)
        {
            base.performUseAction(tileLocation, location);
            DamageFarmer(Game1.player, location);
            return true;
        }

        public override void doCollisionAction(Rectangle positionOfCollider, int speedOfCollision, Vector2 tileLocation, Character who, GameLocation location)
        {
            if (who is Farmer farmer)
            {
                DamageFarmer(farmer, location);
            }
        }

        private void DamageFarmer(Farmer who, GameLocation location)
        {
            who.takeDamage(GetDamage(location as DeepWoods), false, null);
        }

        private int GetDamage(DeepWoods deepWoods)
        {
            int level = deepWoods?.level.Value ?? 1;
            return (1 + level / 10) * Settings.Objects.Bush.ThornyBushDamagePerLevel;
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 positionOnScreen, Vector2 tileLocation, float scale, float layerDepth)
        {
            var backupTexture = Bush.texture;
            Bush.texture = new System.Lazy<Texture2D>(() => DeepWoodsTextures.Textures.InfestedBushes);
            base.drawInMenu(spriteBatch, positionOnScreen, tileLocation, scale, layerDepth);
            Bush.texture = backupTexture;
        }

        public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
        {
            var backupTexture = Bush.texture;
            Bush.texture = new System.Lazy<Texture2D>(() => DeepWoodsTextures.Textures.InfestedBushes);
            base.draw(spriteBatch, tileLocation);
            Bush.texture = backupTexture;
        }
    }
}
