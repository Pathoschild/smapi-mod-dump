using Harmony;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace StackToNearbyChests
{
	class ModEntry : Mod
	{
		internal static ModConfig Config { get; private set; }

		public override void Entry(IModHelper helper)
		{
			Config = helper.ReadConfig<ModConfig>();

			ButtonHolder.ButtonIcon = helper.Content.Load<Texture2D>(@"icon.png");
			
			Patch.PatchAll(HarmonyInstance.Create("me.ilyaki.StackToNearbyChests"));
		}
	}
}
