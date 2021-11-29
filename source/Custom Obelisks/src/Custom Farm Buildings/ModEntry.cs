/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/XxHarvzBackxX/Custom-Obelisks
**
*************************************************/

using System;
using System.Collections.Generic;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using System.Reflection;
using Object = StardewValley.Object;
using Microsoft.Xna.Framework;

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    internal static ModEntry Instance { get; private set; }
    internal static Harmony harmonye { get; private set; }
	internal static JAAPI JAAPIInstance { get; set; } 
    private static List<ModData> Data { get; set; } = new List<ModData> { new ModData(new Obelisk[] { }, new Totem[] { }) };
	internal static IMonitor Monitor1;

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
		Monitor1 = Monitor;
        Instance = this;
        harmonye = new Harmony(ModManifest.UniqueID);
        var harmony = new Harmony(this.ModManifest.UniqueID); // this could spawn a gateway to hell
			harmony.Patch(
			   original: AccessTools.Method(typeof(Building), "obeliskWarpForReal"),
			   prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.Building_obeliskWarpForReal_Prefix))
			);
		harmony.Patch(
		   original: AccessTools.Method(typeof(Object), "totemWarpForReal"),
		   prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.Object_totemWarpForReal_Prefix))
		);
		foreach (IContentPack contentPack in this.Helper.ContentPacks.GetOwned())
        {
            this.Monitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}");
            if (!contentPack.HasFile("content.json"))
            {
                Monitor.Log($"{contentPack.Manifest.Name} {contentPack.Manifest.Version} is missing a \"content.json\" file.", LogLevel.Error);
                contentPack.WriteJsonFile("content.json", new ModData(new Obelisk[] { new Obelisk() }, new Totem[] { new Totem() }));
            }
            else
            {
                ModData modData = contentPack.ReadJsonFile<ModData>("content.json");
                Data.Add(modData);
            }
        }
		helper.Events.GameLoop.Saved += OnSaved;
		helper.Events.GameLoop.Saving += OnSaving;
		helper.Events.Display.MenuChanged += OnMenuChanged;
        helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
        helper.Events.Input.ButtonsChanged += Input_ButtonsChanged;
	}

    private void Input_ButtonsChanged(object sender, ButtonsChangedEventArgs e)
    {
		foreach (SButton button in e.Pressed)
		{
			if (button == SButton.MouseRight)
			{
				foreach (ModData m in Data)
				{
					foreach (Totem t in m.Totems)
					{
						if (Game1.player.ActiveObject?.ParentSheetIndex == JAAPIInstance.GetObjectId(t.Name))
                        {
							Object o = Game1.player.ActiveObject;
							try
							{
								Helper.Reflection.GetMethod(o, "totemWarp", true).Invoke(Game1.player);
							}
                            catch (Exception ex)
                            {
								Monitor1.Log($"Failed in {nameof(Input_ButtonsChanged)} while trying to invoke method 'Object.totemWarp':\n{ex}",
						LogLevel.Error);
								Monitor1.Log(ex.StackTrace, LogLevel.Trace);
							}
						}
					}
				}
			}
		}
    }

    private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
    {
		if (Helper.ModRegistry.GetApi<JAAPI>("spacechase0.JsonAssets") != null)
		{
			JAAPIInstance = Helper.ModRegistry.GetApi<JAAPI>("spacechase0.JsonAssets");
		}
    }

    private static void OnMenuChanged(object sender, MenuChangedEventArgs e)
	{
		UpdateMenu(e.NewMenu);
		Monitor1.Log($"OnMenuChanged called.");
	}

	private static void OnSaved(object sender, SavedEventArgs e)
	{
		RestoreAll();
	}

	private static void OnSaving(object sender, SavingEventArgs e)
	{
		SanitizeAll();
	}

	private static bool Building_obeliskWarpForReal_Prefix(Building __instance)
	{
        foreach (ModData m in Data)
        {
            foreach (Obelisk o in m.Obelisks)
            {
				Monitor1.Log($"{o.Name} reached warp patch.");
				try
                {
                    if (__instance.buildingType.Value.Equals(o.InternalName))
                    {
                        Game1.warpFarmer(o.WhereToWarp,
                            o.ToX, o.ToY, flip: false);
                    }
                }
                catch (Exception e)
                {
                    Monitor1.Log($"Failed in {nameof(Building_obeliskWarpForReal_Prefix)}:\n{e}",
                        LogLevel.Error);
                    Monitor1.Log(e.StackTrace, LogLevel.Trace);
                }
            }
        }
		return true;
	}
	private static bool Object_totemWarpForReal_Prefix(Object __instance)
	{
		foreach (ModData m in Data)
		{
			foreach (Totem t in m.Totems)
			{
				try
				{
					if (t.Name != "???")
					{
						if (__instance.ParentSheetIndex == JAAPIInstance.GetObjectId(t.Name))
						{
							Game1.warpFarmer(t.WhereToWarp, t.ToX, t.ToY, flip: false);
							if (!t.PersistItem)
							{
								Game1.player.ActiveObject = null;
							}
							return true;
						}
					}
				}
				catch (Exception e)
				{
					Monitor1.Log($"Failed in {nameof(Building_obeliskWarpForReal_Prefix)}:\n{e}",
						LogLevel.Error);
					Monitor1.Log(e.StackTrace, LogLevel.Trace);
				}
			}
		}
		return true;
	}
	// Add the obelisk to the Wizard's construction menu.
	public static void UpdateMenu(IClickableMenu menu)
	{
		Monitor1.Log($"Inside updatemenu.");
		foreach (ModData m in Data)
		{
			foreach (Obelisk o in m.Obelisks)
			{
				if (menu is CarpenterMenu cMenu && IsContentPresent(o) && o.Name != "???")
				{
					if (Instance.Helper.Reflection.GetField<bool>(cMenu, "magicalConstruction").GetValue())
					{
						var blueprints = Instance.Helper.Reflection.GetField<List<BluePrint>>(cMenu, "blueprints").GetValue();
						blueprints.Add(new BluePrint(o.InternalName));
						Monitor1.Log($"Added {o.InternalName} to Wizard's construction menu",
							LogLevel.Trace);
					}
				}
			}
		}
	}

	// Change the obelisk to an Earth Obelisk for save files.
	public static void SanitizeAll()
	{
		if (!Game1.player.IsMainPlayer)
			return;

		foreach (var building in Game1.getFarm().buildings)
		{
			foreach (ModData m in Data)
			{
				foreach (Obelisk o in m.Obelisks)
				{
					if (building != null &&
						building.buildingType.Value == o.InternalName)
					{
						building.buildingType.Value = "Earth Obelisk";
						// Store the original type in modData.
						if (!building.modData.ContainsKey(o.InternalName.GetHashCode().ToString()))
							building.modData.Add(o.InternalName.GetHashCode().ToString(), o.InternalName);
						Monitor1.Log($"Replaced {o.InternalName} at {building.tileX}, {building.tileY} with 'Earth Obelisk' for temporary sanitisation.",
							LogLevel.Trace);
					}
				}
			}
		}
	}

	// Change placeholder Earth Obelisks back to obelisks.
	public static void RestoreAll()
	{
		foreach (ModData m in Data)
		{
			foreach (Obelisk o in m.Obelisks)
			{
				if (!Game1.player.IsMainPlayer || !IsContentPresent(o))
					return;
				foreach (var building in Game1.getFarm().buildings)
				{
					if (building != null &&
						building.buildingType.Value == "Earth Obelisk" &&
						building.modData.ContainsKey(o.InternalName) &&
						building.modData[o.InternalName] == o.InternalName)
					{
						building.buildingType.Value = o.InternalName;
						Monitor1.Log($"Restored {o.InternalName} at {building.tileX}, {building.tileY}");
					}
				}
			}
		}
	}

	// Check if the needed content files have been patched.
	private static bool IsContentPresent(Obelisk o)
	{
		Dictionary<string, string> blueprintDict = Game1.content.Load<Dictionary<string, string>>(PathUtilities.NormalizePath("Data/Blueprints"));
		if (!blueprintDict.ContainsKey(o.InternalName))
		{
			Monitor1.Log($"{o.InternalName}: Content not present.");
			return false;
		}

		var texture = Game1.content.Load<Texture2D>(PathUtilities.NormalizePath("Buildings/" + o.InternalName));
		Monitor1.Log($"{o.InternalName}: Content is present. Content == null = {texture == null}.");
		return texture != null;
	}

	public class ModData
	{
		public Obelisk[] Obelisks = new Obelisk[] { new Obelisk() };
		public Totem[] Totems = new Totem[] { new Totem() };
        public ModData(Obelisk[] obelisks, Totem[] totems)
        {
			if (obelisks != null)
			{
				Obelisks = obelisks;
			}
			else
			{
				Obelisks = new Obelisk[] { new Obelisk() };
			}

			if (totems != null)
			{
				Totems = totems;
			}
			else
            {
				Totems = new Totem[] { new Totem() };
            }
        }
	}
	public class Totem
    {
		public string Name { get; set; }
		public string WhereToWarp { get; set; }
		public int ToX { get; set; }
		public int ToY { get; set; }
		public bool PersistItem { get; set; }
		public Totem()
		{
			Name = "???";
			WhereToWarp = "???";
			ToX = 0;
			ToY = 0;
			PersistItem = false;
		}
	}
	public class Obelisk
    {
		public string Name { get; set; }
        public string InternalName { get; set; }
		public string WhereToWarp { get; set; }
		public int ToX { get; set; }
		public int ToY { get; set; }
		public Obelisk()
        {
			Name = "???";
            InternalName = "???";
			WhereToWarp = "???";
			ToX = 0;
			ToY = 0;
        }
    }
}