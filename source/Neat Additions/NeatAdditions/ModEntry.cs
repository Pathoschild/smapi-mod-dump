using Harmony;
using NeatAdditions.PreviewWallpaperAndFloors;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;

namespace NeatAdditions
{
	public class ModEntry : Mod
	{
		public static IModEvents Events { get; private set; }

		public override void Entry(IModHelper helper)
		{
			Events = Helper.Events;

			var config = helper.ReadConfig<ModConfig>();
			
			//Non-harmony patches
			if (config.EnableFlooringAndWallpaperPreview)
			{
				new FloorAndWallpaperPreview();
				Monitor.Log("Enabled floor and wallpaper preview", LogLevel.Info);
			}

			//Harmony patches
			var patches = new List<IPatch>();

			if (config.EnableFishingBobControlForGamepads)
				patches.Add(new GamepadCanControlFishingBob.FishingRod_TickUpdate());

			if (config.EnableFullyCycleToolbar)
				patches.Add(new FullyCycleToolbar.Game1_pressSwitchToolButton());

			if (config.CornerFlashGlitch)
				patches.Add(new CornerFlashGlitch.DialogueBox_draw());

			HarmonyInstance harmony = HarmonyInstance.Create("me.ilyaki.neatadditions");
			foreach (IPatch patch in patches)
			{
				try
				{
					patch.Patch(harmony);
					Monitor.Log($"Enabled {patch.GetPatchName()}", LogLevel.Info);
				}
				catch (Exception e)
				{
					Monitor.Log($"Could not enable {patch.GetPatchName()}", LogLevel.Error);
					Monitor.Log(e.ToString(), LogLevel.Error);
				}
			}
		}
	}
}
