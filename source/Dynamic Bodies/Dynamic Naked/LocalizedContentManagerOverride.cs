/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ribeena/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Threading;

using DynamicBodies.Patches;

namespace DynamicBodies
{

	

	public class LocalizedContentManagerOverride : StardewValley.LocalizedContentManager
	{
		public static ModEntry modEntry;
		public Farmer who;
		public LocalizedContentManagerOverride(IServiceProvider serviceProvider, string rootDirectory, CultureInfo currentCulture, Farmer farmer)
			: base(serviceProvider, rootDirectory, currentCulture)
		{
			who = farmer;
		}

		public LocalizedContentManagerOverride(IServiceProvider serviceProvider, string rootDirectory)
			: this(serviceProvider, rootDirectory, Thread.CurrentThread.CurrentUICulture, null)
		{
		}

		

		public override T Load<T>(string assetName)
		{
			modEntry.Monitor.Log($"Intervening base of {assetName} for {who.Name}", LogLevel.Debug);

			//modEntry.Monitor.Log($"Intervening base of {assetName} for {who.Name}", LogLevel.Debug);
			if (assetName.StartsWith("Characters\\Farmer\\farmer_base")
				|| assetName.StartsWith("Characters\\Farmer\\farmer_base_bald")
				|| assetName.StartsWith("Characters/Farmer/farmer_base!")
				|| assetName.StartsWith("Characters\\Farmer\\farmer_girl")
				|| assetName.StartsWith("Characters\\Farmer\\farmer_girl_bald"))
			{
				if (who != null)
				{
					//Return the cached version for that character
					//modEntry.Monitor.Log($"ContentManager Replacing farmer sprite base with cached of {assetName}.", LogLevel.Debug);
					//Texture2D toReturn = ModEntry.GetFarmerBaseSprite(who, assetName);

					Texture2D source_texture = FarmerRendererPatched.GetFarmerBaseSprite(who, assetName);
					Texture2D toReturn = new Texture2D(Game1.graphics.GraphicsDevice, source_texture.Width, source_texture.Height);
					Color[] data = new Color[source_texture.Width * source_texture.Height];
					source_texture.GetData(data, 0, data.Length);
					toReturn.SetData(data);
					//JA fix
					//who.FarmerRenderer.recolorShoes(who.shoes);

					if (toReturn != null)
					{
						return (T)(object)toReturn;
					} else
                    {
						modEntry.Monitor.Log($"Oops, not there.", LogLevel.Debug);
					}
				}
			}
			return modEntry.Helper.GameContent.Load<T>(assetName);
			//return this.Load<T>(assetName, LocalizedContentManager.CurrentLanguageCode);
		}


		public virtual LocalizedContentManagerOverride CreateTemporary(ModEntry context, Farmer farmer)
		{
			modEntry = context;
			who = farmer;
			return new LocalizedContentManagerOverride(base.ServiceProvider, base.RootDirectory, this.CurrentCulture, farmer);
		}
	}

}
