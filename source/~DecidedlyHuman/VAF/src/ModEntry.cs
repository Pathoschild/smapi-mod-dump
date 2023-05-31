/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using VAF.Models;
using VAFPackFormat.Models;

namespace VAF
{
    public class VAF : Mod
    {
        private List<Pack> packs = new List<Pack>();

        private Dictionary<string, List<VoLine>> mappedVoiceovers = new Dictionary<string, List<VoLine>>();

        // private Dictionary<string, VoFile> mappedVoiceovers = new Dictionary<string, VoFile>();
        private IModHelper helper;

        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
            this.helper = helper;

            Stopwatch timer = new Stopwatch();
            timer.Start();

            foreach (IContentPack pack in this.Helper.ContentPacks.GetOwned())
            {
                string[] packDirectories = Directory.GetDirectories(pack.DirectoryPath);
                Pack newPack = new Pack();

                if (packDirectories.Length > 0)
                {
                    foreach (string dir in packDirectories)
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(dir);
                        string voJsonPath = Path.Combine(directoryInfo.FullName, "vo.json");
                        string relativeJsonPath = Path.GetRelativePath(pack.DirectoryPath, voJsonPath);

                        if (File.Exists(voJsonPath))
                        {
                            // We know the pack has a JSON at this point.
                            // newPack.AddCharacterVo(helper.Data.ReadJsonFile<CharacterVo>(relativeJsonPath));
                            newPack.AddCharacterVo(pack.ReadJsonFile<CharacterVo>(relativeJsonPath), pack.DirectoryPath,
                                pack.Manifest);
                        }
                    }
                }

                // foreach (string directory in Directory.GetDirectories(pack.DirectoryPath))
                // {
                //     directories.Add(new DirectoryInfo(directory));
                // }

                if (!newPack.IsEmpty)
                    this.packs.Add(newPack);
                else
                    this.Monitor.Log(
                        $"Pack {pack.Manifest.Name} contained no characters, or had incorrectly named JSON files.",
                        LogLevel.Error);
            }

            // foreach (Pack pack in this.packs)
            // {
            //     foreach (CharacterVo vo in pack.CharacterVoiceovers)
            //     {
            //         foreach (Voiceover voiceover in vo.Voiceover)
            //         {
            //             foreach (VoFile voFile in voiceover.VoFiles)
            //             {
            //                 // if (!voiceover.DialoguePath.Contains("rainy", StringComparison.OrdinalIgnoreCase))
            //                 // {
            //                 // If the key contains a pipe, we know we should be looking for multiple files.
            //                 if (voFile.Key.Contains("|"))
            //                 {
            //                     // Split the key by the pipe character to get all desired keys.
            //                     string[] splitKeys = voFile.Key.Split("|");
            //
            //                     // For every split key,
            //                     foreach (string key in splitKeys)
            //                     {
            //                         VoFile newVo = new VoFile();
            //                         newVo.File = voFile.File.Replace("{key}", key);
            //                         newVo.Key = voFile.Key;
            //
            //                         this.mappedVoiceovers[voiceover.DialoguePath] = newVo;
            //                     }
            //
            //                     continue;
            //                 }
            //                 // }
            //
            //                 voFile.File = Path.Combine(pack.PackDirectory, voFile.File);
            //                 this.mappedVoiceovers[voiceover.DialoguePath] = voFile;
            //             }
            //         }
            //     }
            // }

            foreach (Pack pack in this.packs)
            {
                foreach (CharacterVo vo in pack.CharacterVoiceovers)
                {
                    foreach (Voiceover voiceover in vo.Voiceover)
                    {
                        IAssetName assetName = this.helper.GameContent.ParseAssetName(voiceover.DialoguePath);

                        foreach (VoFile voFile in voiceover.VoFiles)
                        {
                            List<VoLine> newLines = new List<VoLine>();

                            // if (!voiceover.DialoguePath.Contains("rainy", StringComparison.OrdinalIgnoreCase))
                            // {

                            // If the key contains a pipe, we know we should be looking for multiple files.
                            if (voFile.Key.Contains("|"))
                            {
                                // Split the key by the pipe character to get all desired keys.
                                string[] splitKeys = voFile.Key.Split("|");

                                // For every split key,
                                foreach (string key in splitKeys)
                                {
                                    string filePath = voFile.File.Replace("{key}", key);

                                    newLines.Add(new VoLine(assetName, key, filePath, pack.OwningMod));
                                }

                                continue;
                            }
                            // }

                            // Does the file contain our {key} tag?
                            if (voFile.File.Contains("{key}"))
                            {
                                voFile.File = voFile.File.Replace("{key}", voFile.Key);
                            }

                            // If not, we simply set our file path as appropriate.
                            string path = Path.Combine(pack.PackDirectory, voFile.File);
                            newLines.Add(new VoLine(assetName, voFile.Key, path, pack.OwningMod));

                            foreach (VoLine line in newLines)
                            {
                                if (!this.mappedVoiceovers.ContainsKey(assetName.BaseName))
                                    this.mappedVoiceovers.Add(assetName.BaseName, new List<VoLine>());

                                this.mappedVoiceovers[assetName.BaseName].Add(line);
                            }
                        }
                    }
                }
            }

            timer.Stop();

            string packOrPacks = this.packs.Count > 1 ? "packs" : "pack";
            this.Monitor.Log($"Took {timer.ElapsedMilliseconds}ms to load {this.packs.Count} {packOrPacks}.",
                LogLevel.Info);

            // foreach (var voiceovers in this.mappedVoiceovers)
            // {
            //     SoundEffect dynamicSoundEffect = SoundEffect.FromFile(voiceovers.Value.File);
            //     voiceovers.Value.Cue = new CueDefinition(
            //         $"{voiceovers.Value.Path}:{voiceovers.Value.Key}",
            //         dynamicSoundEffect,
            //         Game1.audioEngine.GetCategoryIndex("Sound"));
            //     Game1.soundBank.AddCue(voiceovers.Value.Cue);
            // }

            // timer.Restart();
            //
            // foreach (Pack pack in this.packs)
            // {
            //     foreach (CharacterVo characterVo in pack.CharacterVoiceovers)
            //     {
            //         foreach (Voiceover vo in characterVo.Voiceover)
            //         {
            //             foreach (VoFile voFile in vo.VoFiles)
            //             {
            //                 string filePath = voFile.File;
            //
            //                 if (voFile.File.Contains("{key}"))
            //                 {
            //                     filePath.Replace("{key}", voFile.Key);
            //                 }
            //
            //                 int partCount = 1;
            //                 while (File.Exists($"{filePath}_{partCount}"))
            //                 {
            //                     partCount++;
            //                 }
            //             }
            //         }
            //     }
            // }

            // foreach (DirectoryInfo dir in directories)
            // {
            //     string voJsonPath = Path.Combine(dir.FullName, "vo.json");
            //     string relativeJsonPath = Path.GetRelativePath()
            //
            //     if (File.Exists(voJsonPath))
            //     {
            //         helper.Data.ReadJsonFile<CharacterVo>(Path.GetRelativePath())
            //     }
            // }
        }

        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is DialogueBox dialogueBox)
            {
                string dialoguePath = dialogueBox.characterDialogue.TranslationKey.Split(":")[0].Replace("\\", "/");
                IAssetName dialogue = this.helper.GameContent.ParseAssetName(dialoguePath);

                // if (this.mappedVoiceovers.ContainsKey(dialogue.BaseName))
                // {
                //     this.Monitor.Log("Dict contains this dialogue key.", LogLevel.Info);
                // }

                // this.Monitor.Log($"Speaking NPC: {dialogueBox.characterDialogue.speaker.Name}", LogLevel.Info);
                // this.Monitor.Log($"Dialogue key: {dialogueBox.characterDialogue.TranslationKey}", LogLevel.Info);
                //
                // if (Game1.CurrentEvent != null)
                // {
                //     this.Monitor.Log($"Event asset: {Game1.CurrentEvent.fromAssetName}", LogLevel.Info);
                //     this.Monitor.Log($"Event key: {Game1.CurrentEvent.id}", LogLevel.Info);
                //     this.Monitor.Log($"Event current command: {Game1.CurrentEvent.eventCommands[Game1.CurrentEvent.currentCommand]}", LogLevel.Info);
                //
                //     if (Game1.CurrentEvent.eventCommands[Game1.CurrentEvent.currentCommand].Contains("speak"))
                //     {
                //         this.Monitor.Log($"Current dialogue contains speaking.");
                //     }
                // }
                // else
                // {
                //     this.Monitor.Log($"Dialogue speaker: {dialogueBox.characterDialogue.speaker.Name}", LogLevel.Info);
                //     this.Monitor.Log($"Dialogue path: {Utilities.Dialogue.GetDialoguePath(dialogueBox.characterDialogue)}", LogLevel.Info);
                //     this.Monitor.Log($"Dialogue key: {Utilities.Dialogue.GetDialogueKey(dialogueBox.characterDialogue)}", LogLevel.Info);
                // }

                // foreach (string command in Game1.CurrentEvent.eventCommands)
                // {
                //     this.Monitor.Log($"Event command: {command}", LogLevel.Info);
                // }

                // int dialogueCount = 1;
                // foreach (string dialogue in dialogueBox.characterDialogue.dialogues)
                //     this.Monitor.Log($"Dialogue {dialogueCount++}: {dialogue}", LogLevel.Info);
                //
                // foreach (string splitDialogue in dialogueBox.characterDialoguesBrokenUp)
                //     this.Monitor.Log($"Split dialogue part: {splitDialogue}", LogLevel.Info);
            }
        }
    }
}
