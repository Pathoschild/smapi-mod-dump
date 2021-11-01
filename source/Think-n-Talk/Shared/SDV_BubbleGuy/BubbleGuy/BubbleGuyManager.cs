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

//using StardewWeb.Utilities;

namespace SDV_Speaker.Speaker
{

    class BubbleGuyManager
    {
        private readonly BubbleRecorder Recorder;
        private readonly string sSpirteDirectory;
        public bool IsBubbleVisible = false;
        private string sRecordingLastLocation = "";
        private int iRecordingLastX;
        private int iRecordingLastY;
        public IModHelper oHelper;
        public IMonitor oMonitor;
        public List<SpeakerItem> CurrentRecording => Recorder.CurrentRecording;
        public Dictionary<string, List<SpeakerItem>> Recordings => Recorder.Recordings;


        public BubbleGuyManager(string sSavePath, string sSpriteDir, IModHelper helper, IMonitor monitor)
        {
            oHelper = helper;
            oMonitor = monitor;
            Recorder = new BubbleRecorder(sSavePath, oHelper);
            sSpirteDirectory = sSpriteDir;
            if (Game1.IsMasterGame)
            {
                //
                //  the master game will handle the cleanups
                //
                helper.Events.GameLoop.Saving += GameLoop_Saving;
                helper.Events.Multiplayer.PeerDisconnected += Multiplayer_PeerDisconnected;
            }
            helper.Events.Player.Warped += Player_Warped;
        }

        private void Multiplayer_PeerDisconnected(object sender, PeerDisconnectedEventArgs e)
        {
            //
            //  clean up any BubbleGuys left by a disconnected peer
            //
            RemoveBubbleGuy(true, false, BubbleGuyStatics.BubbleGuyPrefix + e.Peer.PlayerID.ToString());
        }

        private void GameLoop_Saving(object sender, SavingEventArgs e)
        {
            RemoveBubbleGuy(false, true);
        }

        public void AddBubbleGuy(bool isThink, string sText)
        {
            RemoveBubbleGuy(false, false);
            Game1.currentLocation.characters.Add(new BubbleGuy(isThink, sText, sSpirteDirectory));
            IsBubbleVisible = true;
        }
        public void RemoveBubbleGuy(bool bAllLocations, bool bIsSave)
        {
            RemoveBubbleGuy(bAllLocations, bIsSave, BubbleGuyStatics.BubbleGuyName);
        }
        public void RemoveBubbleGuy(bool bAllLocations, bool bIsSave, string sBubbleGuyId)
        {
#if DEBUG
            oMonitor.Log($"removing BubbleGuy. name '{BubbleGuyStatics.BubbleGuyName}'", LogLevel.Info);
#endif
            if (bAllLocations) {
                foreach (GameLocation gl in Game1.locations)
                {
                    List<NPC> lDelete = new List<NPC> { };
                    foreach (NPC oNpc in gl.characters)
                    {
                        if (oNpc.name.Value==sBubbleGuyId)
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
                if (Game1.IsMultiplayer && Game1.IsMasterGame && bIsSave)
                {
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
#if DEBUG
                    oMonitor.Log($"Characters: {string.Join(", ", Game1.currentLocation.characters.Select(p => p.name))}",LogLevel.Info);
#endif
                    if (Game1.currentLocation.getCharacterFromName(sBubbleGuyId) is NPC oGuy)
                    {
                        Game1.currentLocation.characters.Remove(oGuy);
                    }
                }
            }
            IsBubbleVisible = false;
        }
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
