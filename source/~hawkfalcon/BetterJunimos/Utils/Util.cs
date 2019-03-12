using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Objects;
using SObject = StardewValley.Object;

namespace BetterJunimos.Utils {
    public class Util {
        internal const int DefaultRadius = 8;
        internal const int UnpaidRadius = 3;
        public const int CoffeeId = 433;

        public static int MaxRadius;

        internal static ModConfig Config;
        internal static IReflectionHelper Reflection;
        internal static JunimoAbilities Abilities;
        internal static JunimoPayments Payments;

        public static Guid GetHutIdFromHut(JunimoHut hut) {
            return Game1.getFarm().buildings.GuidOf(hut);
        }

        public static JunimoHut GetHutFromId(Guid id) {
            return Game1.getFarm().buildings[id] as JunimoHut;
        }

        public static void AddItemToChest(Farm farm, Chest chest, SObject item) {
            Item obj = chest.addItem(item);
            if (obj == null)
                return;
            Vector2 pos = chest.TileLocation;
            for (int index = 0; index < obj.Stack; ++index)
                Game1.createObjectDebris(item.ParentSheetIndex, (int)pos.X + 1, (int)pos.Y + 1, -1, item.Quality, 1f, farm);
        }

        public static void RemoveItemFromChest(Chest chest, Item item) {
            if (Config.FunChanges.InfiniteJunimoInventory) { return; }
            item.Stack--;
            if (item.Stack == 0) {
                chest.items.Remove(item);
            }
        }

        public static void SpawnJunimoAtHut(JunimoHut hut) {
            Vector2 pos = new Vector2((float)hut.tileX.Value + 1, (float)hut.tileY.Value + 1) * 64f + new Vector2(0.0f, 32f);
            SpawnJunimoAtPosition(pos, hut, hut.getUnusedJunimoNumber());
        }

        public static void SpawnJunimoAtPosition(Vector2 pos, JunimoHut hut, int junimoNumber) {
            if (hut == null) return;
            Farm farm = Game1.getFarm();
            JunimoHarvester junimoHarvester = new JunimoHarvester(pos, hut, junimoNumber);
            farm.characters.Add((NPC)junimoHarvester);
            hut.myJunimos.Add(junimoHarvester);

            if (Game1.isRaining) {
                var alpha = Reflection.GetField<float>(junimoHarvester, "alpha");
                alpha.SetValue(Config.FunChanges.RainyJunimoSpiritFactor);
            }
            if (!Utility.isOnScreen(Utility.Vector2ToPoint(pos), 64, farm))
                return;
            farm.playSound("junimoMeep1");
        }

        public static void SendMessage(string msg) {
            if (!Config.Other.ReceiveMessages) return;

            Game1.addHUDMessage(new HUDMessage(msg, 3) {
                noIcon = true,
                timeLeft = HUDMessage.defaultTime
            });
        }

        public static void SpawnParticles(Vector2 pos) {
            Multiplayer multiplayer = Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
            multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite[1] {
                new TemporaryAnimatedSprite(17, new Vector2(pos.X * 64f, pos.Y * 64f), Color.White, 7, Game1.random.NextDouble() < 0.5, 125f, 0, -1, -1f, -1, 0)
            });
            multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite[1] {
                new TemporaryAnimatedSprite(14, new Vector2(pos.X * 64f, pos.Y * 64f), Color.White, 7, Game1.random.NextDouble() < 0.5, 50f, 0, -1, -1f, -1, 0)
            });
        }
    }
}
