/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HDPortraits
**
*************************************************/

using AeroCore;
using AeroCore.Utils;
using HDPortraits.Patches;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System.Reflection;

namespace HDPortraits.Integration
{
	[ModInit(WhenHasMod = "Pathoschild.LookupAnything")]
	internal class LookupAnything
	{
		private static FieldInfo isGourmand;
		private static FieldInfo npcTarget;
		internal static void Init()
		{
			var targetType = Reflection.TypeNamed("Pathoschild.Stardew.LookupAnything.Framework.Lookups.Characters.CharacterSubject");
			var target = targetType.MethodNamed("DrawPortrait");

			if (target is null)
				return;

			isGourmand = targetType.FieldNamed("IsGourmand");
			npcTarget = targetType.FieldNamed("Target");
			ModEntry.monitor.Log("Patching lookup anything...");
			ModEntry.harmony.Patch(target, new(typeof(LookupAnything).MethodNamed(nameof(Prefix))));
		}

		private static bool Prefix(SpriteBatch spriteBatch, Vector2 position, Vector2 size, object __instance, ref bool __result)
		{
			var npc = npcTarget.GetValue(__instance) as NPC;
			var gourmand = isGourmand.GetValue(__instance) as bool? ?? false;

			if (!npc.isVillager() || npc.Portrait is null || gourmand)
				return true;

			if (!ModEntry.TryGetMetadata(
				DialoguePatch.GetTextureNameSync(npc, out var has_suffix), 
				has_suffix ? null : PortraitDrawPatch.GetSuffix(npc),
				out var meta))
				return true;

			var atex = meta.TryGetTexture(out var tex) ? tex : npc.Portrait;

			spriteBatch.Draw(atex, 
				new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y), 
				meta.GetRegion(0, Game1.currentGameTime.ElapsedGameTime.Milliseconds), 
				Color.White);

			__result = true;
			return false;
		}
	}
}
