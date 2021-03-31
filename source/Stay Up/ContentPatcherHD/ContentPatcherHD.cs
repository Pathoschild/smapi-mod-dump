/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/su226/StardewValleyMods
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System.Collections.Generic;
using System.Reflection;

namespace Su226.ContentPatcherHD {
  class M {
    public static IMonitor Monitor;
    public static IModHelper Helper;
  }

  class Data {
    public string[] ScaleRequests = null;
  }

  class ContentPatcherHD : Mod, IAssetEditor, IAssetLoader {
    private ICollection<string> Requests = new HashSet<string>();
    private IDictionary<string, Texture2DWrapper> ScaledAssets = new Dictionary<string, Texture2DWrapper>();

    public override void Entry(IModHelper helper) {
      M.Monitor = Monitor;
      M.Helper = Helper;
      HarmonyInstance harmony = HarmonyInstance.Create(ModManifest.UniqueID);
      SpriteBatchOverrides.PatchAll(harmony);
      AssetDataForImageOverrides.PatchAll(harmony);
      IModInfo info = Helper.ModRegistry.Get("Pathoschild.ContentPatcher");
      Mod cp = (Mod)info.GetType().GetProperty("Mod", BindingFlags.Instance | BindingFlags.Public).GetValue(info);
      foreach (IContentPack pack in cp.Helper.ContentPacks.GetOwned()) {
        Data data = pack.ReadJsonFile<Data>("content.json");
        if (data.ScaleRequests != null) {
          foreach (string res in data.ScaleRequests) {
            Requests.Add(res);
          }
        }
      }
    }

    public bool CanEdit<T>(IAssetInfo asset) {
      return Requests.Contains(asset.AssetName);
    }

    public void Edit<T>(IAssetData asset) {
      IAssetDataForImage image = asset.AsImage();
      string name = asset.AssetName + ".4x";
      if (!ScaledAssets.ContainsKey(name) || ScaledAssets[name].Locale != asset.Locale) {
        ScaledAssets[name] = new Texture2DWrapper(image.Data, 4, name, asset.Locale);
      }
      image.ReplaceWith(ScaledAssets[name]);
    }

    public bool CanLoad<T>(IAssetInfo asset) {
      return ScaledAssets.ContainsKey(asset.AssetName);
    }

    public T Load<T>(IAssetInfo asset) {
      return (T)(object)ScaledAssets[asset.AssetName].Wrapped;
    }
  }
}
