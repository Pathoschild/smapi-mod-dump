/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bcmpinc/StardewHack
**
*************************************************/

using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System.Reflection.Emit;

namespace StardewHack.FlexibleArms
{
    public class ModConfig {
        public float MaxRange = 1.4f;
    }

    public class ModEntry : HackWithConfig<ModEntry, ModConfig>
    {
        public override void HackEntry(IModHelper helper)
        {
            I18n.Init(helper.Translation);
            Patch((Character c) => c.GetToolLocation(Vector2.Zero, false), Character_GetToolLocation);
            Patch(() => Game1.pressUseToolButton(), Game1_pressUseToolButton);
        }
        protected override void InitializeApi(IGenericModConfigMenuApi api) {
            api.AddNumberOption(mod: ModManifest, name: I18n.RangeName, tooltip: I18n.RangeTooltip, getValue: () => config.MaxRange, setValue: (float val) => config.MaxRange = val, min: 1, max: 5);
        }

        void Character_GetToolLocation() {
            AllCode().Replace(
                Instructions.Ldarg_0(),
                Instructions.Ldarg_1(),
                Instructions.Ldarg_2(),
                Instructions.Call(typeof(ModEntry), nameof(GetToolLocation), typeof(Character), typeof(Vector2), typeof(bool)),
                Instructions.Ret()
            );
        }

		public static Vector2 GetToolLocation(Character c, Vector2 target_position, bool ignoreClick) {
			if (!ignoreClick && !target_position.Equals(Vector2.Zero) && c.Name.Equals(Game1.player.Name)) {
                var player_position = Game1.player.getStandingPosition();
                var delta = target_position - player_position;
                var range = getConfig().MaxRange * 64.0f;
				if (delta.LengthSquared() <= range * range) {
					return target_position;
				} else { 
                    delta.Normalize();
				    return player_position + delta * range;
                }
			}
			Rectangle boundingBox = c.GetBoundingBox();
			return c.FacingDirection switch {
				0 => new Vector2(boundingBox.X + boundingBox.Width / 2, boundingBox.Y - 48), 
				1 => new Vector2(boundingBox.X + boundingBox.Width + 48, boundingBox.Y + boundingBox.Height / 2), 
				2 => new Vector2(boundingBox.X + boundingBox.Width / 2, boundingBox.Y + boundingBox.Height + 48), 
				3 => new Vector2(boundingBox.X - 48, boundingBox.Y + boundingBox.Height / 2), 
				_ => c.getStandingPosition(), 
			};
		}

        void Game1_pressUseToolButton() {
            var code = FindCode(
                // Vector2 toolLocation = Game1.player.GetToolLocation(position);
                Instructions.Call_get(typeof(Game1), nameof(Game1.player)),
                OpCodes.Ldloc_0,
                OpCodes.Ldfld,
                OpCodes.Ldc_I4_0,
                Instructions.Callvirt(typeof(Character), nameof(Character.GetToolLocation), typeof(Vector2), typeof(bool)),
                OpCodes.Stloc_S,

	            // Game1.player.FacingDirection = Game1.player.getGeneralDirectionTowards(new Vector2((int)toolLocation.X, (int)toolLocation.Y));
                Instructions.Call_get(typeof(Game1), nameof(Game1.player)),
                Instructions.Call_get(typeof(Game1), nameof(Game1.player)),

                OpCodes.Ldloc_S,
                OpCodes.Ldfld,
                OpCodes.Conv_I4,
                OpCodes.Conv_R4,

                OpCodes.Ldloc_S,
                OpCodes.Ldfld,
                OpCodes.Conv_I4,
                OpCodes.Conv_R4,

                OpCodes.Newobj
            );
            code.Replace(
                Instructions.Call_get(typeof(Game1), nameof(Game1.player)),
                Instructions.Call_get(typeof(Game1), nameof(Game1.player)),
                code[1],
                code[2]
            );
        }
    }
}

