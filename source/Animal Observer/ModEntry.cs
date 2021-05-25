/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/nofilenamed/AnimalObserver
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Network;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnimalObserver
{
    public class ModEntry : Mod
	{
		private bool m_Registered;

		private ModConfig Config;

		private EntitiesSetting Entities;


		public override void Entry(IModHelper helper)
		{
			LoadConfig();

			helper.Events.Input.ButtonsChanged += OnButtonsChanged;

			RegisterEvent();
		}

        private void LoadConfig()
        {
			Config = Helper.ReadConfig<ModConfig>();

			Entities = Config.Entities;
		}

		private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
		{
			if (Config.Keys.Reload.JustPressed())
			{
				LoadConfig();
			}

			if (Config.Keys.Toggle.JustPressed())
            {
				RegisterEvent();
			}
		}

		private List<FarmAnimal> GetAnimals(object currentLocation)
		{
			List<FarmAnimal> list = null;
			if (currentLocation is Farm farm)
			{
				if (farm.animals == null || farm.animals.Count() <= 0)
				{
					return list;
				}
				list = new List<FarmAnimal>();
				using (NetDictionary<long, FarmAnimal, NetRef<FarmAnimal>, SerializableDictionary<long, FarmAnimal>, NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>>>.ValuesCollection.Enumerator enumerator = farm.animals.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						FarmAnimal item = enumerator.Current;
						list.Add(item);
					}
					return list;
				}
			}
			if (currentLocation is AnimalHouse animalHouse)
			{
				if (animalHouse.animals != null && animalHouse.animals.Count() > 0)
				{
					list = new List<FarmAnimal>();
					foreach (FarmAnimal item2 in animalHouse.animals.Values)
					{
						list.Add(item2);
					}
				}
			}
			return list;


		}

		private Pet GetPet(GameLocation currentLocation)
		{
			foreach (NPC npc in currentLocation.characters)
			{
				if (npc is Pet pet)
				{
					return pet;
				}
			}

			return null;
		}


		public Vector2 GetOffsetForAnimal(Building home)
		{
			if (home == null)
				return Config.Offsets.Pet;

			if (home is Coop)
			{
				return Config.Offsets.CoopAnimal;
			}
			else if (home is Barn)
			{
				return Config.Offsets.BarnAnimal;
			}

			return default;
		}

		private void RegisterEvent()
		{
			if (!m_Registered)
			{
				m_Registered = true;
				Helper.Events.Display.RenderedWorld += OnRenderedWorld;

			}
            else
            {
				m_Registered = false;
				Helper.Events.Display.RenderedWorld -= OnRenderedWorld;
			}
		}

		private double m_ShowProductTimeout;
		private double m_ShowProductTime;

		private void SetupTime(ref double ms)
		{
			m_ShowProductTime = ms + Config.ShowIsHarvestableTime * 1000;
			m_ShowProductTimeout = ms + Config.ShowIsHarvestableTime * 2000;
		}

		private void OnRenderedWorld(object sender, EventArgs e)
		{
			if (!Context.IsWorldReady)
				return;

			GameLocation currentLocation = Game1.currentLocation;
			List<FarmAnimal> animals = GetAnimals(currentLocation);

			double ms = Game1.currentGameTime.TotalGameTime.TotalMilliseconds;
			float yOffsetFactor = 4f * (float)Math.Round(Math.Sin(ms / 250.0), 2);

			if (animals != null)
			{
				bool showProduct = false;
				if (Config.ShowIsHarvestable)
				{
					if (m_ShowProductTime == default)
					{
						SetupTime(ref ms);
					}

					showProduct = ms <= m_ShowProductTime;
				}

				foreach (FarmAnimal animal in animals)
				{
					if (Config.ShowIsHarvestable)
					{
						if (showProduct && animal.currentProduce > 0 && animal.age >= animal.ageWhenMature)
						{
							switch (animal.currentProduce)
							{
								case 184://Milk Cow ID
								case 186://Milk Cow ID
									DrawEntity(Game1.objectSpriteSheet, Entities.CowMilk, animal, animal.home, ref yOffsetFactor);
									break;

								case 436://Milk Goat ID
								case 438://Milk Goat ID
									DrawEntity(Game1.objectSpriteSheet, Entities.GoatMilk, animal, animal.home, ref yOffsetFactor);
									break;

								case 430: //Truffle ID
									if (Config.ShowTruffle)
									{
										DrawEntity(Game1.objectSpriteSheet, Entities.Truffle, animal, animal.home, ref yOffsetFactor);
									}
									break;



								case 440://Wool ID
									DrawEntity(Game1.objectSpriteSheet, Entities.Wool, animal, animal.home, ref yOffsetFactor);
									break;
							}

							continue;
						}

						if (ms >= m_ShowProductTimeout)
						{
							SetupTime(ref ms);
						}
					}

					if (!animal.wasPet && !animal.wasAutoPet)
					{
						DrawEntity(Game1.mouseCursors, Entities.Heart, animal, animal.home, ref yOffsetFactor);
					}
				}
			}

			if (Config.PetToo)
			{
				Pet pet = GetPet(currentLocation);
				if (pet != null && !pet.lastPetDay.Values.Any(x => x == Game1.Date.TotalDays))
				{
					DrawEntity(Game1.mouseCursors, Entities.Heart, pet, null, ref yOffsetFactor);
				}
			}



		}



		private void DrawEntity(Texture2D spriteSheet, EntitiesConfig config, Character character, Building building, ref float yOffsetFactor)
		{
			Vector2 offset = GetOffsetForAnimal(building);
			offset += character.position;

			offset += new Vector2(character.Sprite.getWidth() / 2, yOffsetFactor);

			DrawEntity(Game1.mouseCursors, Entities.Bubble, ref offset);

			offset += config.Offset;

			DrawEntity(spriteSheet, config, ref offset);
		}

		private void DrawEntity(Texture2D spriteSheet, EntitiesConfig config, ref Vector2 offset)
		{
			Game1.spriteBatch.Draw(
				spriteSheet,
				Game1.GlobalToLocal(Game1.uiViewport, offset),
				new Rectangle(config.X, config.Y, config.Width, config.Height),
				config.Color,
				config.Rotation,
				config.Origin, 
				config.Scale,
				config.SpriteEffects,
				0f);
		}
	}
}
