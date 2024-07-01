/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buffs;
using StardewValley.GameData.Objects;
using StardewValley.Menus;
using StardewValley.TokenizableStrings;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace GlowBuff
{
    internal static class Patches
    {
        private static ModEntry ctx;

        private static string GlowId => $"{ctx.ModManifest.UniqueID}/Glow";

        private static string GlowTextureId => $"{ctx.ModManifest.UniqueID}/GlowTexture";

        private static string GlowRadiusId => $"{ctx.ModManifest.UniqueID}/GlowRadius";

        private static string GlowColorId => $"{ctx.ModManifest.UniqueID}/GlowColor";

        private static string GlowDurationId => $"{ctx.ModManifest.UniqueID}/GlowDuration";

        private static string GlowBuffNameId => $"{ctx.ModManifest.UniqueID}/DisplayName";

        private static string GlowBuffDescriptionId => $"{ctx.ModManifest.UniqueID}/Description";

        private static FieldInfo BuffManagerPlayerField;

        public static void Patch(ModEntry context)
        {
            BuffManagerPlayerField = AccessTools.Field(typeof(BuffManager), "Player");

            ctx = context;

            Harmony harmony = new(ctx.ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.doneEating)),
                prefix: new(typeof(Patches), nameof(Farmer_DoneEating_Prefix)),
                postfix: new(typeof(Patches), nameof(Farmer_DoneEating_Postfix))
            );

            harmony.Patch(
                original: AccessTools.PropertySetter(typeof(Character), nameof(Character.currentLocation)),
                prefix: new(typeof(Patches), nameof(Farmer_CurrentLocation_Prefix)),
                postfix: new(typeof(Patches), nameof(Farmer_CurrentLocation_Postfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.Update)),
                postfix: new(typeof(Patches), nameof(Farmer_Update_Postfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(BuffManager), nameof(BuffManager.Update)),
                postfix: new(typeof(Patches), nameof(BuffManager_Update_Postfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(LightSource), nameof(LightSource.Draw)),
                transpiler: new(typeof(Patches), nameof(LightSource_Draw_Transpiler))
            );

            harmony.Patch(
                original: AccessTools.FirstMethod(typeof(IClickableMenu), x => x.Name == nameof(IClickableMenu.drawHoverText) && x.GetParameters().Any(y => y.ParameterType == typeof(StringBuilder))),
                transpiler: new(typeof(Patches), nameof(IClickableMenu_DrawHoverText_Transpiler))
            );
        }

        private static void Farmer_DoneEating_Prefix(Farmer __instance, ref Item __state) => __state = __instance.itemToEat;

        private static void Farmer_DoneEating_Postfix(Farmer __instance, Item __state)
        {
            if (__state is null || !DataLoader.Objects(Game1.content).TryGetValue(__state.ItemId, out var objectData))
                return;

            if (ctx.FarmerToLightSourceMap.TryGetValue(__instance.UniqueMultiplayerID, out var lightSourceId) && ctx.LightSourceMap.TryGetValue(lightSourceId, out var lightSource) && lightSource.Source == (objectData.IsDrink ? "drink" : "food"))
                ModEntry.ClearLightSource(lightSourceId, __instance);

            if (objectData.Buffs.FirstOrDefault(x => x.Id == $"{ctx.ModManifest.UniqueID}.Glow") is { } buff)
                ReadFromBuffData(buff, __instance, objectData);
            else if (objectData.CustomFields?.ContainsKey(GlowId) ?? false)
                ReadFromObjectData(objectData, __instance);
        }

        private static void Farmer_CurrentLocation_Prefix(Character __instance, ref GameLocation __state) => __state = __instance.currentLocation;

        private static void Farmer_CurrentLocation_Postfix(Character __instance, GameLocation __state)
        {
            if (__instance is not Farmer f)
                return;
            ModEntry.OnNewLocation(f, Game1.player.currentLocation, __state);
        }

        private static void Farmer_Update_Postfix(Farmer __instance, GameLocation location)
        {
            if (!ctx.FarmerToLightSourceMap.TryGetValue(__instance.UniqueMultiplayerID, out int lightSourceId))
                return;
            Vector2 offset = __instance.shouldShadowBeOffset ? __instance.drawOffset : Vector2.Zero;
            location.repositionLightSource(lightSourceId, new Vector2(__instance.Position.X + 21f, __instance.Position.Y) + offset);
        }

        private static void BuffManager_Update_Postfix(BuffManager __instance, GameTime time)
        {
            var owner = BuffManagerPlayerField.GetValue(__instance) as Farmer ?? Game1.player;
            if (!Game1.shouldTimePass() || !ctx.FarmerToLightSourceMap.TryGetValue(owner.UniqueMultiplayerID, out int lightSourceId) || ctx.LightSourceMap[lightSourceId].Duration == -2)
                return;
            ctx.LightSourceMap[lightSourceId].Duration -= time.ElapsedGameTime.Milliseconds;
            if (ctx.LightSourceMap[lightSourceId].Duration <= 0)
                ModEntry.ClearLightSource(lightSourceId, owner);
        }

        private static IEnumerable<CodeInstruction> LightSource_Draw_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            CodeMatcher matcher = new(instructions, generator);

            matcher.End().MatchStartBackwards([
                new(OpCodes.Callvirt, AccessTools.Method(typeof(SpriteBatch), nameof(SpriteBatch.Draw), [typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float)]))
            ]).InsertAndAdvance([
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, AccessTools.Method(typeof(Patches), nameof(DrawLight)))
            ]).RemoveInstruction();

            return matcher.Instructions();
        }

        private static IEnumerable<CodeInstruction> IClickableMenu_DrawHoverText_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            CodeMatcher matcher = new(instructions, generator);

            //CodeInstruction startInsert = new(OpCodes.)
            matcher.End().MatchEndBackwards([
                new(OpCodes.Ldloc_S),
                new(OpCodes.Ldc_I4_S),
                new(OpCodes.Bne_Un),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldsfld, AccessTools.Field(typeof(Game1), nameof(Game1.mouseCursors)))
            ]).Advance(-1).InsertAndAdvance([
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldarg_2),
                new(OpCodes.Ldloc_S, 5),
                new(OpCodes.Ldloca_S, 6),
                new(OpCodes.Ldarg_S, 9),
                new(OpCodes.Call, AccessTools.Method(typeof(Patches), nameof(TryDrawGlowBuffHoverIcon)))
            ]);

            matcher.MatchEndForward([
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldarg_S),
                new(OpCodes.Ldloc_S),
                new(OpCodes.Ldelem_Ref),
                new(OpCodes.Ldarg_2)
            ]).InsertAndAdvance([
                new(OpCodes.Ldarg_S, 9),
                new(OpCodes.Call, AccessTools.Method(typeof(Patches), nameof(GetBuffDurationString)))
            ]);

            matcher.MatchStartBackwards([
                new(OpCodes.Ldloc_2),
                new(OpCodes.Ldc_I4_4),
                new(OpCodes.Add),
                new(OpCodes.Stloc_2)
            ]).InsertAndAdvance([
                new(OpCodes.Ldarg_S, 9),
                new(OpCodes.Ldloca_S, 2),
                new(OpCodes.Call, AccessTools.Method(typeof(Patches), nameof(TryUpdateHoverBoxHeight)))
            ]);

            matcher.MatchStartForward([
                new(OpCodes.Stloc_1),
                new(OpCodes.Ldloc_S),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Add),
                new(OpCodes.Stloc_S)
            ]).Advance(1).InsertAndAdvance([
                new(OpCodes.Ldarg_S, 9),
                new(OpCodes.Ldarg_2),
                new(OpCodes.Ldloc_S, 17),
                new(OpCodes.Ldloca_S, 1),
                new(OpCodes.Call, AccessTools.Method(typeof(Patches), nameof(TryUpdateHoverBoxWidth)))
            ]);

            return matcher.Instructions();
        }

        private static void DrawLight(SpriteBatch b, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth, LightSource source)
        {
            //This was pure hell to figure it, so if you like the fancy colors, go thank these people (in no particular order):
            //Rokugin \\Special thanks for the prismatic colors
            //Adradis / Audri
            //Khloe Leclair
            //Atravita
            //Abagaianye
            //Ichor \\Biggest thanks for making this piece of junk actually run decent
            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            if (!ctx.LightSourceMap.TryGetValue(source.Identifier, out var data))
            {
                b.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
                return;
            }

            b.Draw(texture, position, sourceRectangle, data.PrismaticColor ? Utility.GetPrismaticColor() : color, rotation, origin, scale, effects, layerDepth);
            
            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, transformMatrix: Game1.game1.useUnscaledLighting ? Matrix.CreateScale(Game1.options.zoomLevel) : Matrix.Identity);
        }

        private static void ReadFromObjectData(ObjectData data, Farmer who)
        {
            var fieldData = ReadDataFromFields(data.CustomFields);

            int id = ctx.ModManifest.UniqueID.Length + (int)who.UniqueMultiplayerID;
            ctx.LightSourceMap[id] = new() { TextureId = fieldData.Texture, Radius = fieldData.Radius, Color = fieldData.Color, Duration = fieldData.Duration, PrismaticColor = fieldData.Prismatic, Source = data.IsDrink ? "drink" : "food" };
            ctx.FarmerToLightSourceMap[who.UniqueMultiplayerID] = id;
            ModEntry.OnNewLocation(who, who.currentLocation, null);

            who.applyBuff(new Buff($"{ctx.ModManifest.UniqueID}.Glow", data.IsDrink ? "drink" : "food", TokenParser.ParseText(data.DisplayName), fieldData.Duration, displayName: fieldData.DisplayName, description: fieldData.Description));
        }

        private static void ReadFromBuffData(ObjectBuffData data, Farmer who, ObjectData source)
        {
            ReadData fieldData;
            if (data.CustomFields is not null && data.CustomFields.Count > 0)
                fieldData = ReadDataFromFields(data.CustomFields);
            else
                fieldData = ReadDataFromFields(source.CustomFields);

            if (data.Duration != Buff.ENDLESS && fieldData.Duration == Buff.ENDLESS)
                fieldData.Duration = data.Duration;
            if (fieldData.Duration != Buff.ENDLESS)
                fieldData.Duration *= Game1.realMilliSecondsPerGameMinute;

            int id = ctx.ModManifest.UniqueID.Length + (int)who.UniqueMultiplayerID;
            ctx.LightSourceMap[id] = new() { TextureId = fieldData.Texture, Radius = fieldData.Radius, Color = fieldData.Color, Duration = fieldData.Duration, PrismaticColor = fieldData.Prismatic, Source = source.IsDrink ? "drink" : "food" };
            ctx.FarmerToLightSourceMap[who.UniqueMultiplayerID] = id;
            ModEntry.OnNewLocation(who, who.currentLocation, null);

            Texture2D? iconTexture = null;
            if (!string.IsNullOrWhiteSpace(data.IconTexture))
            {
                try
                {
                    iconTexture = Game1.content.Load<Texture2D>(data.IconTexture);
                }
                catch (Exception ex)
                {
                    ctx.Monitor.Log($"Tried to load custom buff icon ('{data.IconTexture}') but it could not be loaded", LogLevel.Error);
                    ctx.Monitor.Log($"[{nameof(ReadFromBuffData)}] {ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
                }
            }

            who.applyBuff(new Buff($"{ctx.ModManifest.UniqueID}.Glow", source.IsDrink ? "drink" : "food", TokenParser.ParseText(source.DisplayName), fieldData.Duration, iconTexture, data.IconSpriteIndex, new(data.CustomAttributes), data.IsDebuff, fieldData.DisplayName, fieldData.Description));
        }

        private static ReadData ReadDataFromFields(Dictionary<string, string> fields)
        {
            ReadData data = new();

            if (fields.TryGetValue(GlowTextureId, out string? textureStr))
                data.Texture = int.Parse(textureStr);
            if (fields.TryGetValue(GlowRadiusId, out string? radiusStr))
                data.Radius = int.Parse(radiusStr);
            if (fields.TryGetValue(GlowColorId, out string? colorStr))
            {
                if (colorStr.Trim().ToLower() == "prismatic")
                    data.Prismatic = true;
                else
                {
                    var c = ColorParser.Read(colorStr);
                    data.Color = new(c.R ^ 255, c.G ^ 255, c.B ^ 255, c.A);
                }
            }
            if (fields.TryGetValue(GlowDurationId, out string? durationStr))
                data.Duration = int.Parse(durationStr);
            if (fields.TryGetValue(GlowBuffNameId, out string? displayName))
                data.DisplayName = displayName;
            if (fields.TryGetValue(GlowBuffDescriptionId, out string? description))
                data.Description = description;

            data.DisplayName = string.Format(data.DisplayName, data.Radius);

            if (data.Duration != Buff.ENDLESS)
                data.Duration *= Game1.realMilliSecondsPerGameMinute;

            return data;
        }

        private static void TryUpdateHoverBoxHeight(Item hoverItem, ref int height)
        {
            if (hoverItem is null || !Game1.objectData.TryGetValue(hoverItem.ItemId, out var data))
                return;
            for (int i = 0; i < (data.Buffs?.Count ?? 0); i++)
            {
                if (data.Buffs![i].BuffId != $"{ctx.ModManifest.UniqueID}.Glow")
                    continue;
                height += 39;
            }
        }

        private static void TryUpdateHoverBoxWidth(Item hoverItem, SpriteFont font, int horizontalBuffer, ref int width)
        {
            if (hoverItem is null || !Game1.objectData.TryGetValue(hoverItem.ItemId, out var data))
                return;
            for (int i = 0; i < (data.Buffs?.Count ?? 0); i++)
            {
                if (data.Buffs![i].BuffId != $"{ctx.ModManifest.UniqueID}.Glow")
                    continue;
                width = (int)Math.Max(width, font.MeasureString(GetDisplayName(data, data.Buffs[i])).X + horizontalBuffer);
            }
        }

        private static void TryDrawGlowBuffHoverIcon(SpriteBatch b, SpriteFont font, int x, ref int y, Item hoverItem)
        {
            if (hoverItem is null || !Game1.objectData.TryGetValue(hoverItem.ItemId, out var data))
                return;
            for (int i = 0; i < (data.Buffs?.Count ?? 0); i++)
            {
                if (data.Buffs![i].BuffId != $"{ctx.ModManifest.UniqueID}.Glow")
                    continue;
                Utility.drawWithShadow(b, ctx.HoverIcon, new(x + 16 + 4, y + 16), new(0, 0, 10, 10), Color.White, 0f, Vector2.Zero, 3f, false, .95f);
                if (data.Buffs[i].CustomFields.TryGetValue(GlowRadiusId, out string? radius) || data.CustomFields.TryGetValue(GlowRadiusId, out radius))
                    Utility.drawTextWithShadow(b, $"+{radius}  {GetDisplayName(data, data.Buffs[i])}", font, new(x + 16 + 34 + 4, y + 16), Game1.textColor);
                y += 39;
            }
        }

        private static string GetDisplayName(ObjectData data, ObjectBuffData buffData)
        {
            if (!buffData.CustomFields.TryGetValue(GlowBuffNameId, out string? displayName) && !data.CustomFields.TryGetValue(GlowBuffNameId, out displayName))
                displayName = ctx.Helper.Translation.Get("Buff.DefaultName");
            return string.Format(displayName, "");
        }

        private static string GetBuffDurationString(string current, Item hoverItem)
        {
            if (hoverItem is null || !Game1.objectData.TryGetValue(hoverItem.ItemId, out var data))
                return current;
            for (int i = 0; i < (data.Buffs?.Count ?? 0); i++)
            {
                if (data.Buffs![i].BuffId != $"{ctx.ModManifest.UniqueID}.Glow")
                    continue;
                var duration = data.Buffs[i].Duration;
                if (duration != -2)
                {
                    var minuteSecondDuration = Utility.getMinutesSecondsStringFromMilliseconds(duration * Game1.realMilliSecondsPerGameMinute);
                    if (minuteSecondDuration == current)
                        return current;
                    return $"{current}, {minuteSecondDuration}";
                }
            }
            return current;
        }

        private record ReadData
        {
            public int Texture { get; set; } = 1;

            public int Radius { get; set; } = 2;

            public Color Color { get; set; } = new(0, 50, 170);

            public int Duration { get; set; } = Buff.ENDLESS;

            public bool Prismatic { get; set; } = false;

            public string DisplayName { get; set; } = ctx.Helper.Translation.Get("Buff.DefaultName");

            public string Description { get; set; } = ctx.Helper.Translation.Get("Buff.DefaultDescription");
        } 
    }
}
