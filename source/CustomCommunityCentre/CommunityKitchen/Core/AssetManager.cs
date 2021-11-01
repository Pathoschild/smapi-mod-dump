/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CustomCommunityCentre
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CommunityKitchen
{
    public class AssetManager : IAssetEditor, IAssetLoader
	{
		// Game content assets
		public static readonly string RootGameContentPath = PathUtilities.NormalizeAssetName(
			$@"Mods/{CommunityKitchen.ModEntry.Instance.ModManifest.UniqueID}.Assets");
		public static readonly string DeliverySpritesAssetKey = Path.Combine(RootGameContentPath, "DeliverySprites");

		// Local content assets
		internal static readonly string LocalDeliverySpritesPath = @"assets/deliverySprites";
		internal static readonly string KitchenContentPackPath = @"assets/[CCC] KitchenContentPack";

		// Asset lists
		public static readonly List<string> GameAssetKeys = new()
		{
			@"Maps/townInterior",
			@"Data/mail",
		};


		public bool CanLoad<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals(CommunityKitchen.AssetManager.DeliverySpritesAssetKey);
		}

		public T Load<T>(IAssetInfo asset)
		{
			if (asset.AssetNameEquals(CommunityKitchen.AssetManager.DeliverySpritesAssetKey))
			{
				return (T)(object)CommunityKitchen.ModEntry.Instance.Helper.Content.Load
					<Texture2D>
					($"{CommunityKitchen.AssetManager.LocalDeliverySpritesPath}.png");
			}
			return (T)(object)null;
		}

		public bool CanEdit<T>(IAssetInfo asset)
		{
			return CommunityKitchen.AssetManager.GameAssetKeys
				.Any(assetName => asset.AssetNameEquals(assetName));
		}

		public void Edit<T>(IAssetData asset)
		{
			this.Edit(asset: ref asset); // eat that, ENC0036
		}

		public void Edit(ref IAssetData asset)
		{
			if (asset.AssetNameEquals(@"Data/mail"))
			{
				var data = asset.AsDictionary<string, string>().Data;

				// Append completed mail received for all custom areas as required flags for CC completion event

				string mailId = string.Format(CustomCommunityCentre.Bundles.MailAreaCompletedFollowup, Kitchen.KitchenAreaName);
				data[mailId] = CommunityKitchen.ModEntry.i18n.Get("mail.areacompletedfollowup.gus");

				mailId = GusDeliveryService.MailSaloonDeliverySurchargeWaived;
				data[mailId] = CommunityKitchen.ModEntry.i18n.Get("mail.saloondeliverysurchargewaived");

				asset.ReplaceWith(data);
				return;
			}

			if (asset.AssetNameEquals(@"Maps/townInterior"))
			{
				if (!(Game1.currentLocation is StardewValley.Locations.CommunityCenter))
					return;

				var image = asset.AsImage();

				// Openable fridge in the kitchen
				Rectangle targetArea = Kitchen.FridgeOpenedSpriteArea; // Target some unused area of the sheet for this location
				Rectangle sourceArea = new(320, 224, targetArea.Width, targetArea.Height); // Apply base fridge sprite
				image.PatchImage(
					source: image.Data,
					sourceArea: sourceArea,
					targetArea: targetArea,
					patchMode: PatchMode.Replace);

				sourceArea = new Rectangle(0, 192, 16, 32); // Patch in opened-door fridge sprite from mouseCursors sheet
				image.PatchImage(
					source: Game1.mouseCursors2,
					sourceArea: sourceArea,
					targetArea: targetArea,
					patchMode: PatchMode.Overlay);

				// New star on the community centre area tracker wall
				sourceArea = new Rectangle(370, 705, 7, 7);
				targetArea = new Rectangle(380, 710, sourceArea.Width, sourceArea.Height);
				image.PatchImage(
					source: image.Data,
					sourceArea: sourceArea,
					targetArea: targetArea,
					patchMode: PatchMode.Replace);

				asset.ReplaceWith(image.Data);
				return;
			}
		}
	}
}
