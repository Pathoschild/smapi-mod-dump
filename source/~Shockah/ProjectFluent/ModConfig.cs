/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Shockah.ProjectFluent.ContentPatcher;

namespace Shockah.ProjectFluent
{
	internal class ModConfig
	{
		public ContentPatcherPatchingMode ContentPatcherPatchingMode { get; set; } = ContentPatcherPatchingMode.PatchFluentToken;
		public string CurrentLocaleOverride { get; set; } = "";
		public bool DeveloperMode { get; set; } = false;
	}
}