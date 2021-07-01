/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/pregnancyrole
**
*************************************************/

using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

namespace PregnancyRole
{
	public class ModEntry : Mod
	{
		internal static ModEntry Instance { get; private set; }

		internal HarmonyInstance harmony { get; private set; }

		private readonly PerScreen<SkillsPageOverlay> skillsPageOverlay = new ();
		private readonly PerScreen<ProfileMenuOverlay> profileMenuOverlay = new ();

		public override void Entry (IModHelper helper)
		{
			// Make resources available.
			Instance = this;
			ModConfig.Load ();

			// Apply Harmony patches to pregnancy-related methods.
			harmony = HarmonyInstance.Create (ModManifest.UniqueID);
			WouldNeedAdoptionPatches.Apply ();
			PregnancyRolePatches.Apply ();

			// Edit pregnancy-related dialogue.
			Helper.Content.AssetEditors.Add (new DialogueEditor ());

			// Listen for game events.
			Helper.Events.GameLoop.SaveLoaded += onSaveLoaded;
		}

		public override object GetApi ()
		{
			return new Api ();
		}

		private void onSaveLoaded (object _sender, SaveLoadedEventArgs _e)
		{
			// Set up the Pregnancy Role dropdowns in the pause menu.

			if (skillsPageOverlay.Value != null)
				skillsPageOverlay.Value.Dispose ();
			skillsPageOverlay.Value = new SkillsPageOverlay ();

			if (profileMenuOverlay.Value != null)
				profileMenuOverlay.Value.Dispose ();
			profileMenuOverlay.Value = new ProfileMenuOverlay ();
		}
	}
}
