/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/Think-n-Talk
**
*************************************************/


using System.Collections.Generic;
using StardewValley;
using StardewModdingAPI.Events;
using StardewModdingAPI;
using System.Linq;
using StardewModHelpers;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using HarmonyLib;
using System;
using StardewValley.Menus;
//using StardewWeb.Utilities;

namespace SDV_Speaker.Speaker
{

    public class BubbleGuyManager
    {
        private readonly BubbleRecorder Recorder;
        private StardewBitmap sbThinkBubble = null;
        private StardewBitmap sbTalkBubble = null;
        private Texture2D txThinkBubble = null;
        private Texture2D txTalkBubble = null;
        private readonly string sSpirteDirectory;
        public bool IsBubbleVisible = false;
        private string sRecordingLastLocation = "";
        private int iRecordingLastX;
        private int iRecordingLastY;
        public IModHelper oHelper;
        public IMonitor oMonitor;
        public List<SpeakerItem> CurrentRecording => Recorder.CurrentRecording;
        public Dictionary<string, List<SpeakerItem>> Recordings => Recorder.Recordings;


        public BubbleGuyManager(string sSavePath, string sSpriteDir, IModHelper helper, IMonitor monitor, bool bIsMaster)
        {
            IsMaster = bIsMaster;
            oHelper = helper;
            oMonitor = monitor;
            Recorder = new BubbleRecorder(sSavePath);
            sSpirteDirectory = sSpriteDir;
            if (((Game1.IsMultiplayer && Game1.IsMasterGame) || (!Game1.IsMultiplayer)) && bIsMaster)
            {
                //
                //  the master game will handle the cleanups
                //
                helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
                helper.Events.GameLoop.Saving += GameLoop_Saving;
                helper.Events.Multiplayer.PeerDisconnected += Multiplayer_PeerDisconnected;
                monitor.Log("Bubble guy master game hooks added.", LogLevel.Info);
            }
            if (bIsMaster) helper.Events.Player.Warped += Player_Warped;
            //
            //  load default or custom bubble backgrounds
            //
            if (File.Exists(Path.Combine(sSpirteDirectory, "custom_think_bubble.png")))
            {
                monitor.Log("Using custom Think bubble", LogLevel.Info);
                sbThinkBubble = new StardewBitmap(Path.Combine(sSpirteDirectory, "custom_think_bubble.png"));
            }
            else
            {
                sbThinkBubble = new StardewBitmap(Path.Combine(sSpirteDirectory, "think_bubble.png"));
            }
            if (File.Exists(Path.Combine(sSpirteDirectory, "custom_talk_bubble.png")))
            {
                monitor.Log("Using custom Talk bubble", LogLevel.Info);
                try
                {
                    sbTalkBubble = new StardewBitmap(Path.Combine(sSpirteDirectory, "custom_talk_bubble.png"));
                }
                catch (Exception ex)
                {
                    monitor.Log("Failed to load custom talk bubble, using default.  See logs for details", LogLevel.Info);
                    monitor.Log($"Custome talk bubble error: {ex}", LogLevel.Debug);
                    sbTalkBubble = new StardewBitmap(Path.Combine(sSpirteDirectory, "talk_bubble.png"));
                }
            }
            else
            {
                sbTalkBubble = new StardewBitmap(Path.Combine(sSpirteDirectory, "talk_bubble.png"));
            }
            txTalkBubble = sbTalkBubble.Texture();
            txThinkBubble = sbThinkBubble.Texture();
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            BubbleGuyStatics.SetBubbleGuyId();
        }

        public void StartBubbleChat(string sModId)
        {
            //
            //  add hooks to chat box 
            //  for in game bubble creation
            //
            BubbleChat.Initialize(this);

            Harmony harmony = new Harmony(sModId);

            harmony.Patch(
          original: AccessTools.Method(typeof(ChatBox), "runCommand", new Type[] { typeof(string) }),
          prefix: new HarmonyMethod(typeof(BubbleChat), nameof(BubbleChat.RunCommand))
          );

            oMonitor.Log($"Bubblechat Harmony patch applied", LogLevel.Info);
        }
        public bool IsMaster { get; private set; }
        private void Multiplayer_PeerDisconnected(object sender, PeerDisconnectedEventArgs e)
        {
            //
            //  clean up any BubbleGuys left by a disconnected peer
            //
            RemoveBubbleGuy(true, false, BubbleGuyStatics.BubbleGuyPrefix + e.Peer.PlayerID.ToString());
        }

        private void GameLoop_Saving(object sender, SavingEventArgs e)
        {
            //
            //  clean game before save, remove any bubbles
            //
#if LOG_DEBUG
            oMonitor.Log("GameLoop_Saving", LogLevel.Info);
#endif
            //
            //  todo: check for passout flag so only have to call all locations
            //          when the player passes out
            //
            RemoveBubbleGuy(true, true);
        }

        public void AddBubbleGuy(bool isThink, string sText)
        {
            //
            //  clear out any previous bubbles
            //
            RemoveBubbleGuy(false, false);
            //
            //  if recording, add to current recording
            //
            if (Recorder.Status == BubbleRecorder.RecorderStatus.Recording)
            {
                Recorder.CurrentRecording.Add(new SpeakerItem { IsClear = string.IsNullOrEmpty(sText), IsThink = isThink, Location = Game1.currentLocation.NameOrUniqueName, Text = sText, TileX = Game1.player.getTileX(), TileY = Game1.player.getTileY() });
            }
            //
            //  add new bubble guy
            //
            if (Game1.eventUp)
            {
                //  do not add bubble guy, needs work
                //
                //Game1.CurrentEvent.actors.Add(new BubbleGuy(isThink, sText, (isThink ? sbThinkBubble.Texture() : sbTalkBubble.Texture())));
            }
            else
            {
                Game1.currentLocation.characters.Add(new BubbleGuy(isThink, sText, isThink ? txThinkBubble : txTalkBubble));
                IsBubbleVisible = true;
            }
        }
        public void RemoveBubbleGuy(bool bAllLocations, bool bIsSave)
        {
            RemoveBubbleGuy(bAllLocations, bIsSave, BubbleGuyStatics.BubbleGuyName);
        }
        public void RemoveBubbleGuy(bool bAllLocations, bool bIsSave, string sBubbleGuyId)
        {
#if LOG_DEBUG
            oMonitor.Log($"removing BubbleGuy. name '{BubbleGuyStatics.BubbleGuyName}'", LogLevel.Info);
#endif
            if (bAllLocations)
            {
                //
                //  all locations with a playerid is called 
                //  when a farmhand (peer) quits.
                //
                foreach (GameLocation gl in Game1.locations)
                {
                    List<NPC> lDelete = new List<NPC> { };
                    foreach (NPC oNpc in gl.characters)
                    {
                        if (oNpc.name.Value == sBubbleGuyId)
                        {
                            lDelete.Add(oNpc);
                        }
                    }
                    foreach (NPC oDel in lDelete)
                    {
                        gl.characters.Remove(oDel);
                    }
                }
            }
            else
            {
                if (!bIsSave)
                {
                    //
                    //  if recording, add clear mark to recording
                    //
                    if (Recorder.Status == BubbleRecorder.RecorderStatus.Recording)
                    {
                        SpeakerItem oNewItem = new SpeakerItem
                        {
                            IsThink = false,
                            IsClear = true,
                            Location = Game1.currentLocation.NameOrUniqueName,
                            TileX = Game1.player.getTileX(),
                            TileY = Game1.player.getTileY()
                        };
                        Recorder.CurrentRecording.Add(oNewItem);
                    }
                }
                if (Game1.IsMultiplayer && Game1.IsMasterGame && bIsSave)
                {
                    //
                    //  if host, clear the game of all bubble guys before the save
                    //
                    foreach (GameLocation gl in Game1.locations)
                    {
                        List<NPC> lDelete = new List<NPC> { };
                        foreach (NPC oNpc in gl.characters)
                        {
                            if (oNpc.name.Value.StartsWith(BubbleGuyStatics.BubbleGuyPrefix))
                            {
                                lDelete.Add(oNpc);
                            }
                        }
                        foreach (NPC oDel in lDelete)
                        {
                            gl.characters.Remove(oDel);
                        }
                    }
                }
                else
                {
#if LOG_DEBUG
                    oMonitor.Log($"Location: {Game1.currentLocation.name.Value}, Characters: {string.Join(", ", Game1.currentLocation.characters.Select(p => p.name))}", LogLevel.Info);
#endif
                    //
                    //  remove any existing bubble guys if they exist
                    //
                    if (Game1.currentLocation.getCharacterFromName(sBubbleGuyId) is NPC oGuy)
                    {
                        Game1.currentLocation.characters.Remove(oGuy);
                    }
                }
            }
            IsBubbleVisible = false;
        }
        public void SetThinkBubble(Texture2D thinkbubble)
        {
            sbThinkBubble = new StardewBitmap(thinkbubble);
        }
        public void SetTalkBubble(Texture2D talkbubble)
        {
            sbTalkBubble = new StardewBitmap(talkbubble);
        }
        public Texture2D GetTalkBubble() => sbTalkBubble.Texture();
        public Texture2D GetThinkBubble() => sbThinkBubble.Texture();
        public BubbleRecorder.RecorderStatus RecorderStatus => Recorder.Status;
        public bool LoadSave(string sSaveName)
        {
            Recorder.CurrentRecording = Recorder.Recordings[sSaveName];

            return true;
        }
        public void LoadRecordings()
        {
            Recorder.LoadRecordings();
        }
        public bool SaveRecording(string sSaveName)
        {
            Recorder.SaveRecording(sSaveName);
            return true;
        }
        public bool SaveRecordings()
        {
            Recorder.SaveRecordings();

            return true;
        }
        public void Play()
        {
            foreach (SpeakerItem oitem in Recorder.CurrentRecording)
            {
                oitem.MarkHit = false;
            }
            oHelper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
            Recorder.Play();
        }
        public void Record()
        {
            Recorder.Record();
        }

        public void Stop()
        {
            if (Recorder.Status == BubbleRecorder.RecorderStatus.Playing)
            {
                oHelper.Events.GameLoop.UpdateTicked -= GameLoop_UpdateTicked;
            }
            Recorder.Stop();
        }
        public void DeleteRecordingItem(SpeakerItem oDelete)
        {
            Recorder.DeleteItemFromCurrentRecording(oDelete);
        }
        public void UpdateItemInCurrentRecording(SpeakerItem oOldValue, SpeakerItem oNewVal)
        {
            Recorder.UpdateItemInCurrentRecording(oOldValue, oNewVal);
        }
        private void Player_Warped(object sender, WarpedEventArgs e)
        {

            if (e.OldLocation.getCharacterFromName(BubbleGuyStatics.BubbleGuyName) != null)
            {

                NPC oOld = e.OldLocation.getCharacterFromName(BubbleGuyStatics.BubbleGuyName);
                e.OldLocation.characters.Remove(oOld);
                if (e.NewLocation.getCharacterFromName(BubbleGuyStatics.BubbleGuyName) == null)
                {
                    e.NewLocation.characters.Add(oOld);
                }
            }
        }


        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (string.IsNullOrEmpty(sRecordingLastLocation) || sRecordingLastLocation != Game1.currentLocation.name.Value
                || Game1.player.getTileX() != iRecordingLastX || Game1.player.getTileY() != iRecordingLastY)
            {
                sRecordingLastLocation = Game1.currentLocation.name.Value;
                iRecordingLastX = Game1.player.getTileX();
                iRecordingLastY = Game1.player.getTileY();

                foreach (SpeakerItem oItem in Recorder.CurrentRecording)
                {
                    if (!oItem.MarkHit && oItem.Location == sRecordingLastLocation && oItem.TileX == iRecordingLastX && oItem.TileY == iRecordingLastY)
                    {
                        RemoveBubbleGuy(false, false);

                        if (!oItem.IsClear && oItem.Text != null)
                        {
                            Game1.currentLocation.characters.Add(new BubbleGuy(oItem.IsThink, oItem.Text, sSpirteDirectory));
                        }
                        oItem.MarkHit = true;
                    }
                }
            }
        }

    }
}
