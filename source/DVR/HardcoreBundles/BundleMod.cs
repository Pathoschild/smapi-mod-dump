using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace HardcoreBundles
{
    public class ModEntry : Mod, IAssetEditor
    {
        public static ModEntry Instance;
        public static IDictionary<string, string> VanillaRewards = new Dictionary<string, string>();

        public override void Entry(IModHelper helper)
        {
            Instance = this;

            helper.Events.GameLoop.SaveLoaded += SaveLoaded;
            helper.Events.GameLoop.Saving += Saving;
            helper.Events.GameLoop.ReturnedToTitle += TitleReturn;

            helper.Events.GameLoop.UpdateTicked += UpdateTicked;

            new CommunityCenterManager(this.Helper, this.Monitor);
        }

        private void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            var abi = Game1.currentMinigame;
            if (abi != null && abi.GetType().Name == "AbigailGame")
            {
                var fi = abi.GetType().GetField("playerInvincibleTimer", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                fi.SetValue(abi, 99);
                Helper.Reflection.GetField<bool>(abi, "spreadPistol").SetValue(true);
                var dict = Helper.Reflection.GetField<Dictionary<int, int>>(abi, "activePowerups").GetValue();
                dict[2] = 100;
                var wt = abi.GetType().GetField("waveTimer", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                var wtime = (int)wt.GetValue(abi);
                if (wtime > 5000)
                {
                    wt.SetValue(abi, 4900);
                }
                Helper.Reflection.GetField<int>(abi, "coins").SetValue(100);
            }
        }

        const string saveKey = "hardcore-bundles";

        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            GameState.Current = Helper.Data.ReadSaveData<GameState>(saveKey);
            if (GameState.Current == null)
            {
                GameState.Current = new GameState();
            }
            Invalidate();
            var data = Bundles.Data();
            if (GameState.Current.Activated)
            {
                Helper.Content.Load<Dictionary<string, string>>("Data/Bundles", ContentSource.GameContent);
                foreach (var bundle in Bundles.List)
                {
                    bundle.Perk?.EnableIfCompleted();
                }
            }
#if DEBUG
            Bundles.SaveMarkdown();
#endif
        }

        private void Saving(object sender, SavingEventArgs e)
        {
            Helper.Data.WriteSaveData(saveKey, GameState.Current);
        }

        private void TitleReturn(object sender, ReturnedToTitleEventArgs e)
        {
            if (GameState.Current.Activated)
            {
                foreach (var bundle in Bundles.List)
                {
                    bundle.Perk?.Disable();
                }
            }
            GameState.Current = null;
            Invalidate();
        }

        private IList<string> assetsToEdit = new List<string>
        {
            "Data/Bundles",
            "Strings/UI",
            "LooseSprites/JunimoNote",
        };

        public void Invalidate()
        {
            foreach (var a in assetsToEdit)
            {
                Helper.Content.InvalidateCache(a);
            }
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (GameState.Current == null || !GameState.Current.Activated) return false;

            foreach (var a in assetsToEdit)
            {
                if (asset.AssetNameEquals(a)) return true;
   
            }
            return false;
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data/Bundles"))
            {
                asset.AsDictionary<string, string>().ReplaceWith(Bundles.Data());
                Bundles.Fix(false);
            }
            if (asset.AssetNameEquals("Strings/UI"))
            {
                Func<string, Translation> t = Helper.Translation.Get;
                var dict = asset.AsDictionary<string, string>().Data;
                foreach (var room in new string[] { "Boiler", "Crafts", "Pantry", "Vault", "FishTank" })
                {
                    var key = "JunimoNote_Reward" + room;
                    var r = room;
                    if (room == "Crafts" || room == "Boiler")
                    {
                        r += "Room";
                    }
                    VanillaRewards[r] = dict[key];
                    dict[key] = t(key);
                }
            }
            if (asset.AssetNameEquals("LooseSprites/JunimoNote"))
            {
                var tex = Helper.Content.Load<Texture2D>("assets/icons.png", ContentSource.ModFolder);
                asset.AsImage().PatchImage(tex, targetArea: new Rectangle(0, 180, 640, 64));
            }
        }
    }

}
