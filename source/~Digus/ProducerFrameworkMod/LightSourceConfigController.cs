using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ProducerFrameworkMod.ContentPack;
using StardewValley;
using Object = StardewValley.Object;

namespace ProducerFrameworkMod
{
    public class LightSourceConfigController
    {
        public static void CreateLightSource(Object producer, Vector2 tileLocation, LightSourceConfig lightSourceConfig)
        {
            producer.lightSource = new LightSource(
                lightSourceConfig.TextureIndex
                , new Vector2(tileLocation.X * 64.0f + 32.0f + lightSourceConfig.OffsetX, tileLocation.Y * 64f + lightSourceConfig.OffsetY)
                , lightSourceConfig.Radius
                , lightSourceConfig.Color * lightSourceConfig.ColorFactor
                , GenerateIdentifier(tileLocation)
                , LightSource.LightContext.None
                , 0L
            );
        }

        public static int GenerateIdentifier(Vector2 tileLocation)
        {
            return (int)((double)tileLocation.X * 2000.0 + (double)tileLocation.Y);
        }
    }
}
