/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;


namespace ShowBirthdays
{
	class Birthday
	{
		// The day
		public int day;
		// List of NPCs who have a birthday that day
		private readonly List<NPC> npcs = new List<NPC>();

		// Keep track of which npc is currently shown for the day
		private int currentSpriteIndex = 0;
		// Reference to the outer class to allow error logging
		private readonly IMonitor monitor;
		private readonly IGameContentHelper contentHelper;

		public Birthday(IMonitor m, int day, IGameContentHelper h)
		{
			monitor = m;
			contentHelper = h;
			this.day = day;
		}


		internal List<NPC> GetNPCs()
		{
			return npcs;
		}


		internal void AddNPC(NPC n)
		{
			npcs.Add(n);
		}


		/// <summary>
		/// Return the NPC sprite
		/// </summary>
		/// <param name="incrementSpriteIndex">Select the next available sprite</param>
		internal Texture2D? GetSprite(bool incrementSpriteIndex)
		{
			NPC n;

			List<NPC> list = GetNPCs();

			// There is no texture if there is no NPCs
			if (list.Count == 0)
			{
				return null;
			}

			if (incrementSpriteIndex)
			{
				// Increment the index and loop it back to 0 if we reached the end
				currentSpriteIndex++;
				if (currentSpriteIndex >= list.Count)
					currentSpriteIndex = 0;
			}

			if (currentSpriteIndex < list.Count)
			{
				n = list[currentSpriteIndex];
			}
			else
			{
				monitor.Log($"Getting the NPC for the sprite index {currentSpriteIndex} failed", LogLevel.Error);
				return null;
			}

			Texture2D texture;

			// How the base game handles getting the sprite
			try
			{
				texture = contentHelper.Load<Texture2D>($"Characters\\{n.getTextureName()}");
			}
			catch (Exception ex)
			{
				monitor.Log($"Failed loading the texture for npc {n.getTextureName()}. Trying to use a backup sprite.", LogLevel.Error);
				monitor.Log(ex.ToString(), LogLevel.Error);
				texture = n.Sprite.Texture;
			}

			if (incrementSpriteIndex)
			{
				string previousCharacterName = currentSpriteIndex == 0 ? list[^1].Name : list[currentSpriteIndex - 1].Name;
				monitor.Log($"Sprite changed from {previousCharacterName} to {list[currentSpriteIndex].Name}", LogLevel.Trace);
			}

			return texture;
		}
	}
}
