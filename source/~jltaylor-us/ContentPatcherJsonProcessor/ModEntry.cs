/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewJsonProcessor
**
*************************************************/

// // Copyright 2022 Jamie Taylor
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

namespace ContentPatcherJsonProcessor {
    public class ModEntry : Mod {
        // This class exists so that things can do a single null check instead of several
        private class StaticVarContainer {
            public readonly IManifest Manifest;
            public readonly IMonitor Monitor;
            public readonly IJsonProcessorAPI JsonProcessorAPI;
            public readonly MethodInfo RawContentPack_TryReloadContent;
            public readonly MethodInfo ContentPack_GetFile;
            public readonly Type ContentConfigType;
            public readonly FieldInfo ContentPack_JsonHelper;
            public readonly MethodInfo JsonHelper_JsonSettings;
            // Apparently you _can_ invoke the MethodInfo saved from the transpiler, so this isn't actuall needed...
            // but keeping it around just in case that stops working for some reason.
            //public readonly MethodInfo ContentPack_ReadJsonFile;
            public StaticVarContainer(
                IManifest manifest,
                IMonitor monitor,
                IJsonProcessorAPI jsonProcessorAPI,
                MethodInfo rawContentPack_TryReloadContent,
                MethodInfo contentPack_GetFile,
                Type contentConfigType,
                FieldInfo contentPack_JsonHelper,
                MethodInfo jsonHelper_JsonSettings
                //, MethodInfo contentPack_ReadJsonFile
                ) {
                Manifest = manifest;
                Monitor = monitor;
                JsonProcessorAPI = jsonProcessorAPI;
                RawContentPack_TryReloadContent = rawContentPack_TryReloadContent;
                ContentPack_GetFile = contentPack_GetFile;
                ContentConfigType = contentConfigType;
                ContentPack_JsonHelper = contentPack_JsonHelper;
                JsonHelper_JsonSettings = jsonHelper_JsonSettings;
                //ContentPack_ReadJsonFile = contentPack_ReadJsonFile;
            }
        }
        private static StaticVarContainer? sVars = null;

        // The MethodInfo whose call we're replacing in the transpiler
        private static MethodInfo? replacedMethod = null;

        public ModEntry() {
        }

        public override void Entry(IModHelper helper) {
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) {
            PopulateSVars();
            if (sVars is not null) {
                InstallPatch();
            } else {
                Monitor.Log("Could not do all of the appropriate reflection magic to initialize.", LogLevel.Warn);
            }
        }

        /// <summary>
        /// Populates the <c>sVars</c> static variable container if all of the lookups for individual variables
        /// succeed.  Lots of tedious null checks.
        /// </summary>
        private void PopulateSVars() {
            IJsonProcessorAPI? jsonProcessorAPI = Helper.ModRegistry.GetApi<IJsonProcessorAPI>("jltaylor-us.JsonProcessor");
            if (jsonProcessorAPI is null) {
                Monitor.Log("Could not get Json Processor API (which shouldn't happen, since it's a dependency).  Incompatible version?", LogLevel.Warn);
                return;
            }
            MethodInfo? rawContentPack_TryReloadContent = AccessTools.Method("ContentPatcher.Framework.RawContentPack:TryReloadContent");
            if (rawContentPack_TryReloadContent is null) {
                Monitor.Log("couldn't get method ContentPatcher.Framework.RawContentPack:TryReloadContent", LogLevel.Debug);
                return;
            }
            // sure wish this language supported macros right about now.
            MethodInfo? getFile = AccessTools.Method("StardewModdingAPI.Framework.ContentPack:GetFile");
            if (getFile is null) {
                Monitor.Log("couldn't get method StardewModdingAPI.Framework.ContentPack:GetFile", LogLevel.Debug);
                return;
            }
            Type? contentConfigType = AccessTools.TypeByName("ContentPatcher.Framework.ConfigModels.ContentConfig");
            if (contentConfigType is null) {
                Monitor.Log("couldn't get type ContentPatcher.Framework.ConfigModels.ContentConfig", LogLevel.Debug);
                return;
            }
            FieldInfo? contentPack_JsonHelper = AccessTools.Field("StardewModdingAPI.Framework.ContentPack:JsonHelper");
            if (contentPack_JsonHelper is null) {
                Monitor.Log("couldn't get field StardewModdingAPI.Framework.ContentPack:JsonHelper", LogLevel.Debug);
                return;
            }
            MethodInfo? jsonHelper_JsonSettings = AccessTools.PropertyGetter("StardewModdingAPI.Toolkit.Serialization.JsonHelper:JsonSettings");
            if (jsonHelper_JsonSettings is null) {
                Monitor.Log("couldn't get property getter StardewModdingAPI.Toolkit.Serialization.JsonHelper:JsonSettings", LogLevel.Debug);
                return;
            }
            //MethodInfo? readJsonFileGeneric = AccessTools.Method("ContentPack:ReadJsonFile");
            //MethodInfo? readJsonFile = readJsonFileGeneric?.MakeGenericMethod(contentConfigType);
            //if (readJsonFile is null) {
            //    Monitor.Log("couldn't get method ContentPack:ReadJsonFile", LogLevel.Debug);
            //    return;
            //}
            sVars = new StaticVarContainer(
                manifest: ModManifest,
                monitor: Monitor,
                jsonProcessorAPI: jsonProcessorAPI,
                rawContentPack_TryReloadContent: rawContentPack_TryReloadContent,
                contentPack_GetFile: getFile,
                contentConfigType: contentConfigType,
                contentPack_JsonHelper: contentPack_JsonHelper,
                jsonHelper_JsonSettings: jsonHelper_JsonSettings
                //, contentPack_ReadJsonFile: readJsonFile
                );

        }
        private void InstallPatch() {
            var harmony = new Harmony("jltaylor-us.ContentPatcherJsonProcessor");
            try {
                harmony.Patch(sVars!.RawContentPack_TryReloadContent,
                    transpiler: new HarmonyMethod(GetType(), nameof(TryReloadContentTranspile)));
            } catch (Exception ex) {
                Monitor.Log("transpiler patch on content patcher failed:  " + ex.Message, LogLevel.Warn);
            }
        }

        // A few links to source that I was viewing at various points when figuring all of this stuff out:
        // (I.e., a dump of the browser tabs I still have open so that I can close them now)
        // https://github.com/Pathoschild/StardewMods/blob/content-patcher/1.21/ContentPatcher/Framework/RawContentPack.cs
        // https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/Framework/RawContentPack.cs
        // https://github.com/Pathoschild/SMAPI/blob/develop/src/SMAPI/Framework/ContentPack.cs
        // https://github.com/Pathoschild/SMAPI/blob/develop/src/SMAPI.Toolkit/Serialization/JsonHelper.cs#L46
        
        private static IEnumerable<CodeInstruction> TryReloadContentTranspile(IEnumerable<CodeInstruction> instructions) {
            if (sVars is null) {
                throw new InvalidOperationException("code run before required initialization");
            }
            int state = 1;
            foreach (var instruction in instructions) {
                if (state == 1 && (instruction.operand as MethodInfo)?.Name == "ReadJsonFile") {
                    //SMonitor.Log($"TryReloadContentTranspile found target instruction: {instruction}", LogLevel.Info);
                    //SMonitor.Log($"  opcode {instruction.opcode}", LogLevel.Info);
                    //SMonitor.Log($"  operand {instruction.operand}", LogLevel.Info);
                    state = 2;
                    replacedMethod = instruction.operand as MethodInfo;
                    MethodInfo m = AccessTools.Method(typeof(ModEntry), nameof(ReadJsonFileReplacement));
                    yield return new CodeInstruction(OpCodes.Call, m);
                } else {
                    yield return instruction;
                }
            }
            if (state != 2) {
                sVars.Monitor.Log("TryReloadContentTranspile did not find target instruction", LogLevel.Warn);
            }
        }

        private static object? ReadJsonFileReplacement(IContentPack contentPack, string path) {
            if (sVars is null) {
                throw new InvalidOperationException("code run before required initialization");
            }
            // This is the expression we are replacing:
            // ContentPack.ReadJsonFile<ContentConfig> ("content.json");

            // Keeping this extra code around in case I add features by having extra stuff in the manifest
            //SMonitor.Log($"mainfest extra fields: {contentPack.Manifest.ExtraFields}", LogLevel.Debug);
            //foreach (var kv in contentPack.Manifest.ExtraFields) {
            //    SMonitor.Log($"  {kv.Key} => {kv.Value.GetType()} {kv.Value}", LogLevel.Debug);
            //}
            bool doPreprocessing = contentPack.Manifest.Dependencies.Any(d => d.UniqueID == sVars.Manifest.UniqueID);

            if (!doPreprocessing) {
                return replacedMethod?.Invoke(contentPack, new object[] { path });
            }

            sVars.Monitor.Log($"doing preprocessing on {contentPack.Manifest.Name}", LogLevel.Debug);
            path = PathUtilities.NormalizePath(path);
            FileInfo? file = (FileInfo?)sVars.ContentPack_GetFile.Invoke(contentPack, new object[] { path });
            if (file is null || !file.Exists) return null;
            string fullPath = file.FullName;
            // validate
            if (string.IsNullOrWhiteSpace(fullPath))
                throw new ArgumentException("The file path is empty or invalid.", nameof(fullPath));

            // read file
            string jsonText;
            try {
                jsonText = File.ReadAllText(fullPath);
            } catch (Exception ex) when (ex is DirectoryNotFoundException or FileNotFoundException) {
                return null;
            }
            JObject json = JObject.Parse(jsonText);
            IJsonProcessor processor = sVars.JsonProcessorAPI.NewProcessor(fullPath);
            processor.Transform(json);

            object? jsonHelperInstance = sVars.ContentPack_JsonHelper.GetValue(contentPack);
            JsonSerializerSettings? serializerSettings = (JsonSerializerSettings?)sVars.JsonHelper_JsonSettings.Invoke(jsonHelperInstance, null);
            return json.ToObject(sVars.ContentConfigType, JsonSerializer.Create(serializerSettings));
        }


    }

    // ----------- Json Processor API (simple version) -----------

    public interface IJsonProcessorAPI {
        // you should always call this with includeDefaultTransformers = true
        IJsonProcessor NewProcessor(string errorLogPrefix, bool includeDefaultTransformers = true);
    }

    public interface IJsonProcessor {
        // Returns true if no errors were encountered
        bool Transform(Newtonsoft.Json.Linq.JToken tok);
    }

}

