/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ichortower/SecretNoteFramework
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Menus;
using StardewValley.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using Object = StardewValley.Object;

namespace ichortower.SNF
{
    internal class Patches
    {
        public static void Apply()
        {
            Harmony harmony = new(SNF.ModId);

            PatchMethod(harmony, typeof(Object),
                    nameof(Object.performUseAction),
                    nameof(Patches.Object_performUseAction_Postfix));
            PatchMethod(harmony, typeof(GameLocation),
                    nameof(GameLocation.tryToCreateUnseenSecretNote),
                    nameof(Patches.GameLocation_tryToCreateUnseenSecretNote_Postfix));
            PatchMethod(harmony, typeof(CollectionsPage),
                    nameof(CollectionsPage.receiveLeftClick),
                    nameof(Patches.CollectionsPage_receiveLeftClick_Postfix));
            PatchMethod(harmony, typeof(CollectionsPage),
                    nameof(CollectionsPage.performHoverAction),
                    nameof(Patches.CollectionsPage_performHoverAction_Transpiler));
            ConstructorInfo collectionspage_ctor = typeof(CollectionsPage)
                    .GetConstructor(new[]{typeof(int), typeof(int), typeof(int), typeof(int)});
            harmony.Patch(original: collectionspage_ctor,
                    postfix: new HarmonyMethod(typeof(Patches),
                        nameof(Patches.CollectionsPage_ctor_Postfix)));
        }

        // only suitable for unambiguous method names
        private static void PatchMethod(Harmony harmony, Type t, string name, string patch)
        {
            string[] parts = patch.Split("_");
            string last = parts[parts.Length-1];
            if (last != "Prefix" && last != "Postfix" && last != "Transpiler") {
                Log.Error($"Skipping patch method '{patch}': bad type '{last}'");
                return;
            }
            try {
                MethodInfo m = t.GetMethod(name,
                        BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags.Instance | BindingFlags.Static);
                HarmonyMethod func = new(typeof(Patches), patch);
                if (last == "Prefix") {
                    harmony.Patch(original: m, prefix: func);
                }
                else if (last == "Postfix") {
                    harmony.Patch(original: m, postfix: func);
                }
                else if (last == "Transpiler") {
                    harmony.Patch(original: m, transpiler: func);
                }
            }
            catch (Exception e) {
                Log.Error($"Patch failed ({patch}): {e}");
            }

        }

        public static void Object_performUseAction_Postfix(
                Object __instance,
                GameLocation location,
                ref bool __result)
        {
            if (__result) {
                return;
            }
            if (!SecretModNotes.ActiveObjectIds.Contains(__instance.QualifiedItemId)) {
                return;
            }
            // pick an unseen note from active ones
            var eligible = SecretModNotes.Data.Where((kvp) => {
                return SecretModNotes.AvailableNoteIds.Contains(kvp.Key) &&
                        !ModData.HasNote(Game1.player, kvp.Key) &&
                        (kvp.Value.ObjectId ?? SecretModNotes.DefaultObjectId) ==
                        __instance.QualifiedItemId;
            }).ToList();
            if (eligible.Count == 0) {
                Log.Warn($"No unread notes found for note item '{__instance.QualifiedItemId}'");
                Game1.playSound("newRecipe");
                TemporaryAnimatedSprite puff = new(
                    rowInAnimationTexture: 5,
                    position: Game1.player.Position + new Vector2(0f, -128f+
                        (float)(Game1.player.FarmerSprite.CurrentAnimationFrame
                        .positionOffset*4)),
                    color: Color.White,
                    animationInterval: 66.67f,
                    layerDepth: Math.Max(0f, (float)(Game1.player.StandingPixel.Y+3)/10000f)
                );
                Game1.Multiplayer.broadcastSprites(Game1.currentLocation, puff);
                Game1.showGlobalMessage(TR.Get("UI.NoValidNoteData"));
                __result = true;
                return;
            }
            Random r = Utility.CreateDaySaveRandom(Game1.player.UniqueMultiplayerID, 22122);
            var chosen = r.ChooseFrom(eligible);
            ModData.AddNote(Game1.player, chosen.Key);
            LetterViewerMenu lvm = new(chosen.Value.Contents);
            FormatLetter(ref lvm, chosen.Value);
            lvm.exitFunction = delegate {
                foreach (string act in chosen.Value.ActionsOnFirstRead) {
                    if (!TriggerActionManager.TryRunAction(
                            act, out string err, out Exception e)) {
                        Log.Error($"Failed to run '{act}': {err}");
                    }
                }
            };
            Game1.activeClickableMenu = lvm;
            __result = true;
        }

        public static void GameLocation_tryToCreateUnseenSecretNote_Postfix(
                GameLocation __instance,
                Farmer who,
                ref Object __result)
        {
            if (__instance.currentEvent?.isFestival == true) {
                return;
            }
            if (who == null || !who.hasMagnifyingGlass ||
                    who.secretNotesSeen.Count == 0) {
                return;
            }
            // 50% chance to abort if vanilla already rolled a note
            if (__result != null && Game1.random.NextBool()) {
                return;
            }
            string context = __instance.GetLocationContextId().ToLower();
            var eligible = SecretModNotes.Data.Where((kvp) => {
                if (!SecretModNotes.AvailableNoteIds.Contains(kvp.Key)) {
                    return false;
                }
                string lc = kvp.Value.LocationContext;
                if (lc.StartsWith("!")) {
                    return context != lc.Substring(1).Trim().ToLower();
                }
                string[] locs = lc.Split(",")
                        .Select(s => s.Trim().ToLower()).ToArray();
                return Array.IndexOf(locs, context) >= 0;
            }).ToList();
            var unseen = eligible.Where(kvp => !ModData.HasNote(who, kvp.Key)).ToList();
            int notesHeld = 0;
            foreach (string id in SecretModNotes.ActiveObjectIds) {
                notesHeld += who.Items.CountId(id);
            }
            if (unseen.Count <= notesHeld) {
                return;
            }
            float frac = (float)(unseen.Count - notesHeld - 1) /
                    (float)Math.Max(1, eligible.Count - 1);
            // if vanilla didn't roll a note, cut the initial note chance in
            // half, since this gives you a second chance to get one
            float denom = (__result == null ? 2.0f : 1.0f);
            float noteChance = Utility.Lerp(GameLocation.LAST_SECRET_NOTE_CHANCE,
                    GameLocation.FIRST_SECRET_NOTE_CHANCE / denom, frac);
            if (!Game1.random.NextBool(noteChance)) {
                return;
            }
            Random r = Utility.CreateDaySaveRandom(who.UniqueMultiplayerID, 22122);
            string obj = r.ChooseFrom(unseen).Value.ObjectId ??
                    SecretModNotes.DefaultObjectId;
            Log.Trace($"Generating note item with id {obj}");
            __result = ItemRegistry.Create<Object>(obj);
        }

        public static void CollectionsPage_ctor_Postfix(
                CollectionsPage __instance)
        {
            // plenty of magic numbers in here. not ideal, but (probably)
            // better than trying to transpile this in
            if (!__instance.collections.ContainsKey(6)) {
                return;
            }
            int rowItems = 10;
            int index = 0;
            int baseX = __instance.xPositionOnScreen + IClickableMenu.borderWidth +
                    IClickableMenu.spaceToClearSideBorder;
            int baseY = __instance.yPositionOnScreen + IClickableMenu.borderWidth +
                    IClickableMenu.spaceToClearTopBorder - 16;
            var list = __instance.collections[6].Last();
            if (list.Count > 0) {
                index = (list.Count / rowItems + 1) * rowItems;
            }
            foreach (var entry in SecretModNotes.Data) {
                string id = entry.Key;
                if (!SecretModNotes.AvailableNoteIds.Contains(id) &&
                        !ModData.HasNote(Game1.player, id)) {
                    continue;
                }
                int xPos = baseX + (index % rowItems) * 68;
                int yPos = baseY + (index / rowItems) * 68;
                if (yPos > __instance.yPositionOnScreen + __instance.height - 128) {
                    var nl = new List<ClickableTextureComponent>();
                    __instance.collections[6].Add(nl);
                    list = __instance.collections[6].Last();
                    index = 0;
                    xPos = baseX;
                    yPos = baseY;
                }
                var noteData = entry.Value;
                var itemData = ItemRegistry.GetDataOrErrorItem(
                        noteData.ObjectId ?? SecretModNotes.DefaultObjectId);
                bool hasNote = ModData.HasNote(Game1.player, id);
                list.Add(new ClickableTextureComponent(
                    name: $"{id} {hasNote}",
                    bounds: new Rectangle(xPos, yPos, 64, 64),
                    label: null,
                    hoverText: makeHoverText(noteData),
                    texture: itemData.GetTexture(),
                    sourceRect: itemData.GetSourceRect(),
                    scale: 4f,
                    drawShadow: hasNote));
                ++index;
            }
            SavedVanillaTexture = __instance.secretNoteImageTexture;
        }

        public static Texture2D SavedVanillaTexture = null;

        private static string makeHoverText(SecretModNoteData noteData)
        {
            if (!String.IsNullOrEmpty(noteData.NoteImageTexture) &&
                    noteData.NoteImageTextureIndex >= 0) {
                return $"!image \"{noteData.NoteImageTexture}\"" +
                        " " + noteData.NoteImageTextureIndex +
                        " " + (noteData.Title ?? "???");
            }
            string parsedText = Game1.parseText(Utility.ParseGiftReveals(noteData.Contents)
                    .TrimStart(' ', '^').Replace("^", Environment.NewLine)
                    .Replace("@", Game1.player.Name), Game1.smallFont, 512);
            string[] split = parsedText.Split(Environment.NewLine);
            int max = 15;
            if (split.Length > max) {
                parsedText = string.Join(Environment.NewLine,
                        split.Where((line, i) => i < max)).Trim() +
                        Environment.NewLine + "(...)";
            }
            return (noteData.Title ?? "???") + Environment.NewLine +
                    Environment.NewLine + parsedText;
        }

        /*
         * This is called by the transpiler to reset the vanilla note image
         * texture (before potentially setting it below).
         * Shared state, oh well
         */
        private static void resetNoteTexture(CollectionsPage instance)
        {
            instance.secretNoteImageTexture = SavedVanillaTexture;
        }

        /*
         * This is called by the transpiler in order to parse out the in-band
         * image command from makeHoverText, above.
         * It returns the final hover text, which should just be the title.
         */
        private static string applyHoverText(CollectionsPage instance, string text)
        {
            string[] split = ArgUtility.SplitBySpaceQuoteAware(text);
            if (!split[0].Equals("!image")) {
                return text;
            }
            int index = -1;
            if (split.Length < 4 || !int.TryParse(split[2], out index)) {
                Log.Warn($"Malformed image note: '{text}'");
                return text;
            }
            instance.secretNoteImageTexture = Game1.temporaryContent.Load
                    <Texture2D>(split[1]);
            instance.secretNoteImage = index;
            return string.Join(" ", split.Skip(3));
        }

        
        public static IEnumerable<CodeInstruction>
            CollectionsPage_performHoverAction_Transpiler(
                IEnumerable<CodeInstruction> instructions,
                ILGenerator generator,
                MethodBase original)
        {
            Label backToDefault = generator.DefineLabel();
            Label finalStore = generator.DefineLabel();
            FieldInfo hoverTextField = typeof(ClickableTextureComponent)
                    .GetField("hoverText", BindingFlags.Public | BindingFlags.Instance);
            MethodInfo stringIsNullOrEmpty = typeof(String)
                    .GetMethod("IsNullOrEmpty", BindingFlags.Public | BindingFlags.Static);
            MethodInfo applyHoverText = typeof(Patches)
                    .GetMethod("applyHoverText", BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo resetNoteTexture = typeof(Patches)
                    .GetMethod("resetNoteTexture", BindingFlags.NonPublic | BindingFlags.Static);

            List<CodeInstruction> injection = new() {
                new(OpCodes.Ldloc_S, (short)4),
                new(OpCodes.Ldfld, hoverTextField),
                new(OpCodes.Call, stringIsNullOrEmpty),
                new(OpCodes.Brtrue_S, backToDefault),
                new(OpCodes.Ldloc_S, (short)4),
                new(OpCodes.Ldfld, hoverTextField),
                new(OpCodes.Call, applyHoverText),
                new(OpCodes.Br_S, finalStore),
                new(OpCodes.Ldarg_0), // this one gets the label
                new(OpCodes.Call, resetNoteTexture),
            };
            // make sure this index matches the commented entry from injection
            injection[injection.Count-2].labels.Add(backToDefault);

            List<CodeInstruction> codes = instructions.ToList();
            List<CodeInstruction> modified = new();
            for (int i = 0; i < codes.Count; ++i) {
                var instr = codes[i];
                if (i < 2 || i+4 >= codes.Count ||
                        codes[i-2].opcode != OpCodes.Ldarg_0 ||
                        codes[i-1].opcode != OpCodes.Ldarg_0 ||
                        codes[i].opcode != OpCodes.Ldloc_S) {
                    modified.Add(instr);
                    continue;
                }
                codes[i+4].labels.Add(finalStore);
                modified.AddRange(injection);
                modified.Add(instr);
            }
            return modified;
        }


        public static void CollectionsPage_receiveLeftClick_Postfix(
                CollectionsPage __instance,
                int x, int y)
        {
            if (__instance.currentTab != 6 ||
                    __instance.letterviewerSubMenu != null) {
                return;
            }
            var clicked = __instance.collections[6][__instance.currentPage]
                    .Where(c => c.containsPoint(x, y)).ToList();
            if (clicked.Count == 0) {
                return;
            }
            string[] parts = ArgUtility.SplitBySpace(clicked[0].name);
            if (!Convert.ToBoolean(parts[1]) ||
                    !SecretModNotes.Data.ContainsKey(parts[0])) {
                return;
            }
            var noteData = SecretModNotes.Data[parts[0]];
            LetterViewerMenu lvm = new(noteData.Contents);
            FormatLetter(ref lvm, noteData);
            lvm.isFromCollection = true;
            __instance.letterviewerSubMenu = lvm;
        }


        private static void FormatLetter(ref LetterViewerMenu lvm,
                SecretModNoteData note)
        {
            if (note.NoteTexture != null) {
                try {
                    lvm.letterTexture = Game1.temporaryContent.Load
                            <Texture2D>(note.NoteTexture);
                }
                catch {
                    Log.Error($"Missing letter texture asset: '{note.NoteTexture}'");
                }
            }
            if (note.NoteTextureIndex >= 0) {
                lvm.whichBG = note.NoteTextureIndex;
            }
            if (note.NoteTextColor != null) {
                string val = note.NoteTextColor;
                if (val.StartsWith("rgb(")) {
                    try {
                        string[] split = ArgUtility.SplitQuoteAware(
                                val.Substring(4, val.IndexOf(")")-4), ',');
                        lvm.customTextColor = new Color(int.Parse(split[0]),
                                int.Parse(split[1]), int.Parse(split[2]));
                    }
                    catch {
                        Log.Error($"Could not apply rgb text color '{val}'");
                    }
                }
                else {
                    string fakefmt = $"[textcolor {val}]";
                    _ = lvm.ApplyCustomFormatting(fakefmt);
                }
            }
            if (note.NoteImageTexture != null) {
                try {
                    lvm.secretNoteImageTexture = Game1.temporaryContent.Load
                            <Texture2D>(note.NoteImageTexture);
                }
                catch {
                    Log.Error($"Missing image texture asset: '{note.NoteImageTexture}'");
                }
            }
            if (note.NoteImageTextureIndex >= 0) {
                lvm.secretNoteImage = note.NoteImageTextureIndex;
            }
        }
    }
}
