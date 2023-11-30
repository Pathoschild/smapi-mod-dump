/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mus-candidus/LittleNPCs
**
*************************************************/

using System;
using System.Linq;
using System.Collections.Generic;

using StardewModdingAPI;
using StardewModdingAPI.Utilities;


namespace LittleNPCs.Framework {
    internal static class ContentPatcherTokens {
        /// <summary>
        /// Core implementation of a CP token that returns a single unbounded value.
        /// Implementation details are provided by function objects.
        /// </summary>
        private class TokenCore {
            /// <summary>Function called by <code>IsReady()</code>.</summary>
            private Func<bool> isReady_;

            /// <summary>Function called by <code>UpdateContext()</code>.</summary>
            private Func<bool> updateContext_;

            /// <summary>Function called by <code>GetValues()</code>.</summary>
            private Func<string, IEnumerable<string>> getValues_;

            /// <summary>Flag that determines whether input is required.</summary>
            private bool requiresInput_;

            public TokenCore(Func<bool> isReady, Func<bool> updateContext, Func<string, IEnumerable<string>> getValues, bool requiresInput) {
                isReady_       = isReady;
                updateContext_ = updateContext;
                getValues_     = getValues;
                requiresInput_ = requiresInput;
            }

            /// <summary>Get whether the values may change depending on the context.</summary>
            public bool IsMutable() => true;

            /// <summary>Get whether the token allows an input argument (e.g. an NPC name for a relationship token).</summary>
            public bool AllowsInput() => requiresInput_;

            /// <summary>Whether the token requires an input argument to work, and does not provide values without it (see <see cref="AllowsInput"/>).</summary>
            public bool RequiresInput() => requiresInput_;
 
            /// <summary>Whether the token may return multiple values for the given input.</summary>
            /// <param name="input">The input argument, if applicable.</param>
            public bool CanHaveMultipleValues(string input = null) => false;
        
            /// <summary>Get whether the token always chooses from a set of known values for the given input. Mutually exclusive with <see cref="HasBoundedRangeValues"/>.</summary>
            /// <param name="input">The input argument, if applicable.</param>
            /// <param name="allowedValues">The possible values for the input.</param>
            public bool HasBoundedValues(string input, out IEnumerable<string> allowedValues) {
                allowedValues = null;

                return false;
            }
            
            /// <summary>Update the values when the context changes.</summary>
            /// <returns>Returns whether the value changed, which may trigger patch updates.</returns>
            public bool UpdateContext() => updateContext_();
            

            /// <summary>Get whether the token is available for use.</summary>
            public bool IsReady() => isReady_();

            /// <summary>Get the current values.</summary>
            /// <param name="input">The input argument, if applicable.</param>
            public IEnumerable<string> GetValues(string input) => getValues_(input);
        }

        private class TokenImplementation {
            private LittleNPCInfo[] cachedLittleNPCs_ = new LittleNPCInfo[2];

            public TokenImplementation(ModEntry modEntry) {
                var api = modEntry.Helper.ModRegistry.GetApi<ContentPatcher.IContentPatcherAPI>("Pathoschild.ContentPatcher");

                api.RegisterToken(modEntry.ModManifest, "FirstLittleNPCName",
                    new TokenCore(
                        () => cachedLittleNPCs_[0]?.LoadedFrom != LittleNPCInfo.LoadState.None,
                        () => UpdateFirstLittleNPC(modEntry.Monitor),
                        (unused) => cachedLittleNPCs_[0].Name.ToTokenReturnValue(),
                        false
                    )
                );

                api.RegisterToken(modEntry.ModManifest, "FirstLittleNPCDisplayName",
                    new TokenCore(
                        () => cachedLittleNPCs_[0]?.LoadedFrom != LittleNPCInfo.LoadState.None,
                        () => UpdateFirstLittleNPC(modEntry.Monitor),
                        (unused) => cachedLittleNPCs_[0].DisplayName.ToTokenReturnValue(),
                        false
                    )
                );

                api.RegisterToken(modEntry.ModManifest, "FirstLittleNPCGender",
                    new TokenCore(
                        () => cachedLittleNPCs_[0]?.LoadedFrom != LittleNPCInfo.LoadState.None,
                        () => UpdateFirstLittleNPC(modEntry.Monitor),
                        (unused) => cachedLittleNPCs_[0].Gender.ToTokenReturnValue(),
                        false
                    )
                );
                
                api.RegisterToken(modEntry.ModManifest, "FirstLittleNPCBirthSeason",
                    new TokenCore(
                        () => cachedLittleNPCs_[0]?.LoadedFrom != LittleNPCInfo.LoadState.None,
                        () => UpdateFirstLittleNPC(modEntry.Monitor),
                        (unused) => cachedLittleNPCs_[0].Birthday.Season.ToTokenReturnValue(),
                        false
                    )
                );

                api.RegisterToken(modEntry.ModManifest, "FirstLittleNPCBirthDay",
                    new TokenCore(
                        () => cachedLittleNPCs_[0]?.LoadedFrom != LittleNPCInfo.LoadState.None,
                        () => UpdateFirstLittleNPC(modEntry.Monitor),
                        (unused) => cachedLittleNPCs_[0].Birthday.Day.ToString().ToTokenReturnValue(),
                        false
                    )
                );

                api.RegisterToken(modEntry.ModManifest, "FirstLittleNPCAge",
                    new TokenCore(
                        () => cachedLittleNPCs_[0]?.LoadedFrom != LittleNPCInfo.LoadState.None,
                        () => UpdateFirstLittleNPC(modEntry.Monitor),
                        (unused) => (SDate.Now().Year - cachedLittleNPCs_[0].Birthday.Year).ToString().ToTokenReturnValue(),
                        false
                    )
                );
                
                api.RegisterToken(modEntry.ModManifest, "SecondLittleNPCName",
                    new TokenCore(
                        () => cachedLittleNPCs_[1]?.LoadedFrom != LittleNPCInfo.LoadState.None,
                        () => UpdateSecondLittleNPC(modEntry.Monitor),
                        (unused) => cachedLittleNPCs_[1].Name.ToTokenReturnValue(),
                        false
                    )
                );

                api.RegisterToken(modEntry.ModManifest, "SecondLittleNPCDisplayName",
                    new TokenCore(
                        () => cachedLittleNPCs_[1]?.LoadedFrom != LittleNPCInfo.LoadState.None,
                        () => UpdateSecondLittleNPC(modEntry.Monitor),
                        (unused) => cachedLittleNPCs_[1].DisplayName.ToTokenReturnValue(),
                        false
                    )
                );

                api.RegisterToken(modEntry.ModManifest, "SecondLittleNPCGender",
                    new TokenCore(
                        () => cachedLittleNPCs_[1]?.LoadedFrom != LittleNPCInfo.LoadState.None,
                        () => UpdateSecondLittleNPC(modEntry.Monitor),
                        (unused) => cachedLittleNPCs_[1].Gender.ToTokenReturnValue(),
                        false
                    )
                );
                
                api.RegisterToken(modEntry.ModManifest, "SecondLittleNPCBirthSeason",
                    new TokenCore(
                        () => cachedLittleNPCs_[1]?.LoadedFrom != LittleNPCInfo.LoadState.None,
                        () => UpdateSecondLittleNPC(modEntry.Monitor),
                        (unused) => cachedLittleNPCs_[1].Birthday.Season.ToTokenReturnValue(),
                        false
                    )
                );

                api.RegisterToken(modEntry.ModManifest, "SecondLittleNPCBirthDay",
                    new TokenCore(
                        () => cachedLittleNPCs_[1]?.LoadedFrom != LittleNPCInfo.LoadState.None,
                        () => UpdateSecondLittleNPC(modEntry.Monitor),
                        (unused) => cachedLittleNPCs_[1].Birthday.Day.ToString().ToTokenReturnValue(),
                        false
                    )
                );

                api.RegisterToken(modEntry.ModManifest, "SecondLittleNPCAge",
                    new TokenCore(
                        () => cachedLittleNPCs_[1]?.LoadedFrom != LittleNPCInfo.LoadState.None,
                        () => UpdateSecondLittleNPC(modEntry.Monitor),
                        (unused) => (SDate.Now().Year - cachedLittleNPCs_[1].Birthday.Year).ToString().ToTokenReturnValue(),
                        false
                    )
                );

                api.RegisterToken(modEntry.ModManifest, "FirstLittleNPC",
                    new TokenCore(
                        () => cachedLittleNPCs_[0]?.LoadedFrom != LittleNPCInfo.LoadState.None,
                        () => UpdateFirstLittleNPC(modEntry.Monitor),
                        (input) => {
                            return (input switch {
                                "Name"        => cachedLittleNPCs_[0].Name,
                                "DisplayName" => cachedLittleNPCs_[0].DisplayName,
                                "Gender"      => cachedLittleNPCs_[0].Gender,
                                "BirthSeason" => cachedLittleNPCs_[0].Birthday.Season,
                                "BirthDay"    => cachedLittleNPCs_[0].Birthday.Day.ToString(),
                                "Age"         => (SDate.Now().Year - cachedLittleNPCs_[0].Birthday.Year).ToString(),
                                _             => string.Empty
                            }).ToTokenReturnValue();
                        },
                        true
                    )
                );

                api.RegisterToken(modEntry.ModManifest, "SecondLittleNPC",
                    new TokenCore(
                        () => cachedLittleNPCs_[1]?.LoadedFrom != LittleNPCInfo.LoadState.None,
                        () => UpdateSecondLittleNPC(modEntry.Monitor),
                        (input) => {
                            return (input switch {
                                "Name"        => cachedLittleNPCs_[1].Name,
                                "DisplayName" => cachedLittleNPCs_[1].DisplayName,
                                "Gender"      => cachedLittleNPCs_[1].Gender,
                                "BirthSeason" => cachedLittleNPCs_[1].Birthday.Season,
                                "BirthDay"    => cachedLittleNPCs_[1].Birthday.Day.ToString(),
                                "Age"         => (SDate.Now().Year - cachedLittleNPCs_[1].Birthday.Year).ToString(),
                                _             => string.Empty
                            }).ToTokenReturnValue();
                        },
                        true
                    )
                );
            }

            private bool UpdateFirstLittleNPC(IMonitor monitor) {
                var littleNPC = new LittleNPCInfo(0, monitor);
                if (littleNPC is not null && !littleNPC.Equals(cachedLittleNPCs_[0])) {
                    cachedLittleNPCs_[0] = littleNPC;

                    monitor.Log($"FirstLittleNPC updated: {cachedLittleNPCs_[0]}");

                    return true;
                }

                return false;
            }

            private bool UpdateSecondLittleNPC(IMonitor monitor) {
                var littleNPC = new LittleNPCInfo(1, monitor);
                if (littleNPC is not null && !littleNPC.Equals(cachedLittleNPCs_[1])) {
                    cachedLittleNPCs_[1] = littleNPC;

                    monitor.Log($"SecondLittleNPC updated: {cachedLittleNPCs_[1]}");

                    return true;
                }

                return false;
            }
        }

        public static void Register(ModEntry modEntry) {
            new TokenImplementation(modEntry);
        }

        private static IEnumerable<string> ToTokenReturnValue(this string value) {
            // Create an IEnumerable from value as required by CP.
            return string.IsNullOrEmpty(value) ? Enumerable.Empty<string>()
                                               : Enumerable.Repeat(value, 1);
        }
    }
}
