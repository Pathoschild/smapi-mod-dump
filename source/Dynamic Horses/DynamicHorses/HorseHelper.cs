using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using System;
using System.Collections.Generic;
using System.IO;

namespace DynamicHorses
{
	class HorseHelper
	{
		private Dictionary<string, string> Horses;
		private ModEntry baseMod;
		private IMonitor Monitor;
		public HorseHelper(Dictionary<string, string> Horses, ModEntry sender)
		{
			baseMod = sender;
			this.Horses = Horses;
			this.Monitor = baseMod.Monitor;
		}

		public void FindTheHorse()
		{
			using (List<GameLocation>.Enumerator enumerator = Game1.locations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					GameLocation current = enumerator.Current;
					for (int i = 0; i < current.characters.Count; i++)
					{
						if (current.characters[i] is Horse)
						{
							try
							{
								Horse PlayerHorse = (Horse)current.characters[i];
								if (PlayerHorse.name.Length > 0)
								{
									LoadSprite(PlayerHorse);
								}
							}
							catch (Exception ex)
							{
								Monitor.Log("Error BPEN418: That was not actually a horse, please send the following stack trace to Bpendragon", LogLevel.Error);
								Monitor.Log(ex.StackTrace, LogLevel.Trace);
							}
						}
					}
				}
			}
		}

		private void LoadSprite(Horse playerHorse)
		{
			try
			{
				if (Horses.ContainsKey(playerHorse.name))
				{
					
					baseMod.Monitor.Log($"Attempting to retexture {playerHorse.name} with {Horses[playerHorse.name]}.xnb");
					baseMod.Monitor.Log($"Current Directory: {baseMod.Helper.DirectoryPath}");
					Monitor.Log($"Looking for {Path.Combine(baseMod.Helper.DirectoryPath, "Horses", Horses[playerHorse.name] + ".xnb")}");
					if (!File.Exists(Path.Combine(baseMod.Helper.DirectoryPath,"Horses",  Horses[playerHorse.name] + ".xnb")))
					{
						throw new FileNotFoundException();
					}
					Monitor.Log("Ok we got past the first check");
					string tempPath = Path.Combine("Stardew Valley", baseMod.RelativePath, "Horses", Horses[playerHorse.name]);
					Monitor.Log(tempPath);
					string tempPath2 = ".." + tempPath;
					Monitor.Log(tempPath2);
					playerHorse.sprite = new AnimatedSprite(Game1.content.Load<Texture2D>(tempPath2), 0, 32, 32);
					playerHorse.sprite.textureUsesFlippedRightForLeft = true;
					playerHorse.sprite.loop = true;
					
				}
			}
			catch (Exception ex)
			{
				Monitor.Log($"File {Horses[playerHorse.name]}.xnb Not Found \nLeaving Default texture.", LogLevel.Error);
			}
		}
	}
}
