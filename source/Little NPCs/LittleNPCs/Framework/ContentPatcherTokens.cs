/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mus-candidus/LittleNPCs
**
*************************************************/

using System.Linq;
using System.Collections.Generic;


namespace LittleNPCs.Framework {
    internal static class ContentPatcherTokens {
        public static void Register(ModEntry modEntry) {
            var api = modEntry.Helper.ModRegistry.GetApi<ContentPatcher.IContentPatcherAPI>("Pathoschild.ContentPatcher");

            api.RegisterToken(modEntry.ModManifest, "FirstLittleNPCName", () => {
                string name = new LittleNPCInfo(0, modEntry.Monitor).Name;

                modEntry.Monitor.Log($"FirstLittleNPCName() returns {name}");

                return name.ToTokenReturnValue();
            });

            api.RegisterToken(modEntry.ModManifest, "FirstLittleNPCDisplayName", () => {
                string displayName = new LittleNPCInfo(0, modEntry.Monitor).DisplayName;

                modEntry.Monitor.Log($"FirstLittleNPCDisplayName() returns {displayName}");

                return displayName.ToTokenReturnValue();
            });

            api.RegisterToken(modEntry.ModManifest, "FirstLittleNPCGender", () => {
                string gender = new LittleNPCInfo(0, modEntry.Monitor).Gender;

                modEntry.Monitor.Log($"FirstLittleNPCGender() returns {gender}");

                return gender.ToTokenReturnValue();
            });

            api.RegisterToken(modEntry.ModManifest, "SecondLittleNPCName", () => {
                string name = new LittleNPCInfo(1, modEntry.Monitor).Name;

                modEntry.Monitor.Log($"SecondLittleNPCName() returns {name}");

                return name.ToTokenReturnValue();
            });

            api.RegisterToken(modEntry.ModManifest, "SecondLittleNPCDisplayName", () => {
                string displayName = new LittleNPCInfo(1, modEntry.Monitor).DisplayName;

                modEntry.Monitor.Log($"SecondLittleNPCDisplayName() returns {displayName}");

                return displayName.ToTokenReturnValue();
            });

            api.RegisterToken(modEntry.ModManifest, "SecondLittleNPCGender", () => {
                string gender = new LittleNPCInfo(1, modEntry.Monitor).Gender;

                modEntry.Monitor.Log($"SecondLittleNPCGender() returns {gender}");

                return gender.ToTokenReturnValue();
            });
        }

        private static IEnumerable<string> ToTokenReturnValue(this string value) {
            // Create an IEnumerable from value as required by CP.
            return string.IsNullOrEmpty(value) ? Enumerable.Empty<string>()
                                               : Enumerable.Repeat(value, 1);
        }
    }
}
