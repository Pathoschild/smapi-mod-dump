/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bpendragon/DynamicHorses
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicHorses
{
	public class ModConfig
	{
		public List<HorseConfig> Horses { get; set; }

		public ModConfig()
		{
			HorseConfig testHorse = new HorseConfig("test", "test");
			HorseConfig epona = new HorseConfig("epona", "epona");
			Horses = new List<HorseConfig>();
			Horses.Add(testHorse);
			Horses.Add(epona);
		}
	}

	public class HorseConfig
	{
		public string Name { get; set; }
		public string XNBName { get; set; }

		public HorseConfig(string Name, string XNBName)
		{
			this.Name = Name;
			this.XNBName = XNBName;
		}

		public override string ToString()
		{
			return "Name: " + Name + ", XNB: " + XNBName;
		}
	}
}
