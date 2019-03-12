using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Collections.Generic;
using Modworks = bwdyworks.Modworks;
using Microsoft.Xna.Framework;
using System.IO;
using Newtonsoft.Json;

namespace Moongates
{
    public class Mod : StardewModdingAPI.Mod
    {
        internal static bool Debug = false;
        [System.Diagnostics.Conditional("DEBUG")]
        public void EntryDebug() { Debug = true; }
        internal static string Module;

        internal static MoongateNPC MoongateWax; //spring
        internal static MoongateNPC MoongateFlow; //summer
        internal static MoongateNPC MoongateWane; //fall
        internal static MoongateNPC MoongateEbb; //winter

        public List<bwdyworks.Structures.GameSpot> SpawnPoints;
        internal static string TextureMoongateLunar;
        internal static string TextureMoongateTidal;

        public override void Entry(IModHelper helper)
        {
            Module = helper.ModRegistry.ModID;
            EntryDebug();
            if (!Modworks.InstallModule(Module, Debug)) return;

            SpawnPoints = new List<bwdyworks.Structures.GameSpot>();
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            helper.Events.GameLoop.Saving += GameLoop_Saving;
            helper.Events.Player.Warped += Player_Warped;

            string filecontents = File.ReadAllText(Helper.DirectoryPath + Path.DirectorySeparatorChar + "spawns.json");
            Modworks.Events.NPCCheckAction += Events_NPCCheckAction;
            SpawnPoints = JsonConvert.DeserializeObject<List<bwdyworks.Structures.GameSpot>>(filecontents);
            TextureMoongateLunar = Helper.Content.GetActualAssetKey("MoongateLunar.png", ContentSource.ModFolder);
            TextureMoongateTidal = Helper.Content.GetActualAssetKey("MoongateTidal.png", ContentSource.ModFolder);
        }

        private void Events_NPCCheckAction(object sender, bwdyworks.Events.NPCCheckActionEventArgs args)
        {
            if (args.Cancelled) return;
            var name = args.NPC.Name;
            if (name.StartsWith("Moongate")) args.Cancelled = true;
        }

        private void Player_Warped(object sender, WarpedEventArgs e)
        {
            if (MoongateWax != null) MoongateWax.ResetForMapEntry();
            if (MoongateFlow != null) MoongateFlow.ResetForMapEntry();
            if (MoongateWane != null) MoongateWane.ResetForMapEntry();
            if (MoongateEbb != null) MoongateEbb.ResetForMapEntry();
        }

        private void GameLoop_Saving(object sender, SavingEventArgs e)
        {
            if(MoongateWax != null && MoongateWax.currentLocation != null)
                MoongateWax.currentLocation.characters.Remove(MoongateWax);
            if (MoongateFlow != null && MoongateFlow.currentLocation != null)
                MoongateFlow.currentLocation.characters.Remove(MoongateFlow);
            if (MoongateWane != null && MoongateWane.currentLocation != null)
                MoongateWane.currentLocation.characters.Remove(MoongateWane);
            if (MoongateEbb != null && MoongateEbb.currentLocation != null)
                MoongateEbb.currentLocation.characters.Remove(MoongateEbb);
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            if(MoongateWax == null) MoongateWax = new MoongateNPC("FarmHouse", "MoongateWax");
            MoongateWax.CurrentDialogue.Clear();
            var sp = SpawnPoints[Modworks.RNG.Next(SpawnPoints.Count)];
            sp.GetGameLocation().characters.Add(MoongateWax);
            MoongateWax.setTilePosition(sp.GetTileLocation());
            MoongateWax.startGlowing(Color.FromNonPremultiplied(0, 236, 222, 255),false,0.01f);

            if (MoongateFlow == null) MoongateFlow = new MoongateNPC("FarmHouse", "MoongateFlow");
            MoongateFlow.CurrentDialogue.Clear();
            sp = SpawnPoints[Modworks.RNG.Next(SpawnPoints.Count)];
            sp.GetGameLocation().characters.Add(MoongateFlow);
            MoongateFlow.setTilePosition(sp.GetTileLocation());
            MoongateFlow.startGlowing(Color.FromNonPremultiplied(179, 129, 255, 255), false, 0.01f);

            if (MoongateWane == null) MoongateWane = new MoongateNPC("FarmHouse", "MoongateWane");
            MoongateWane.CurrentDialogue.Clear();
            sp = SpawnPoints[Modworks.RNG.Next(SpawnPoints.Count)];
            sp.GetGameLocation().characters.Add(MoongateWane);
            MoongateWane.setTilePosition(sp.GetTileLocation());
            MoongateWane.startGlowing(Color.FromNonPremultiplied(236, 0, 183, 255), false, 0.01f);

            if (MoongateEbb == null) MoongateEbb = new MoongateNPC("FarmHouse", "MoongateEbb");
            MoongateEbb.CurrentDialogue.Clear();
            sp = SpawnPoints[Modworks.RNG.Next(SpawnPoints.Count)];
            sp.GetGameLocation().characters.Add(MoongateEbb);
            MoongateEbb.setTilePosition(sp.GetTileLocation());
            MoongateEbb.startGlowing(Color.FromNonPremultiplied(187, 255, 57, 255), false, 0.01f);
        }
    }
}