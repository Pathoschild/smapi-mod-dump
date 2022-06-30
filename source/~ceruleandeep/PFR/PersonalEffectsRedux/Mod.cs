/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using System;
using System.Reflection;

namespace PersonalEffects
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Mod : StardewModdingAPI.Mod
    {
        internal static ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            Modworks.Startup(this);

            Config = helper.ReadConfig<ModConfig>();
            SaveConfig();

            NPCConfig.Load(Helper.DirectoryPath + System.IO.Path.DirectorySeparatorChar);
            if (NPCConfig.Ready)
            {
                helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
                helper.Events.GameLoop.GameLaunched += OnLaunched;

                //AddTrashLoot("Sam", 0);
                //AddTrashLoot("Jodi", 0);
                //AddTrashLoot("Haley", 1);
                //AddTrashLoot("Emily", 1);
                //AddTrashLoot("Lewis", 2);
                //AddTrashLoot("Clint", 4);
                ////saloon is one catch-all
                //AddTrashLoot("Gus", 5);
                //AddTrashLoot("Pam", 5);
                //AddTrashLoot("Emily", 5);
                //AddTrashLoot("Abigail", 5);
                //AddTrashLoot("Sebastian", 5);
                //AddTrashLoot("Marnie", 5);
                ////archeology is the other
                //AddTrashLoot("Maru", 3);
                //AddTrashLoot("Elliott", 3);
                //AddTrashLoot("Harvey", 3);
                //AddTrashLoot("Caroline", 3);
                //AddTrashLoot("Pierre", 3);
                //AddTrashLoot("Shane", 3);
            }

            ConfigLocations.Load(Helper.DirectoryPath + System.IO.Path.DirectorySeparatorChar);
            if (ConfigLocations.Ready)
            {
                Spot.Setup(helper);
            }
        }


        private void SaveConfig()
        {
            Helper.WriteConfig(Config);
        }

        private void OnLaunched(object sender, GameLaunchedEventArgs e)
        {
            SetupGMCM();
        }

        private void SetupGMCM()
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;
            
            configMenu.Unregister(ModManifest);
            configMenu.Register(ModManifest, () => Config = new ModConfig(), SaveConfig);
            configMenu.SetTitleScreenOnlyForNextOptions(ModManifest, false);

            configMenu.AddSectionTitle(ModManifest,
                () => "Creeper configuration");

            configMenu.AddBoolOption(ModManifest,
                () => Config.GiveHints,
                val => Config.GiveHints = val,
                () => "Give hints",
                () => "Give a hint at the start of each day about good places to forage"
            );
            configMenu.AddBoolOption(ModManifest,
                () => Config.BothGenders,
                val => Config.BothGenders = val,
                () => "Both genders",
                () => "Spawn both male and female personal effects for all characters"
            );
        }

        //internal void AddTrashLoot(string NPC, int can)
        //{
        //    if (NPCConfig.GetNPC(NPC) == null) return;
        //    if (!NPCConfig.GetNPC(NPC).Enabled) return;
        //    Modworks.Items.AddTrashLoot(Module, new bwdyworks.Structures.TrashLootEntry(Module, "px" + NPCConfig.GetNPC(NPC).Abbreviate() + (NPCConfig.GetNPC(NPC).HasMaleItems() ? "m" : "f") + 1, can));
        //    Modworks.Items.AddTrashLoot(Module, new bwdyworks.Structures.TrashLootEntry(Module, "px" + NPCConfig.GetNPC(NPC).Abbreviate() + (NPCConfig.GetNPC(NPC).HasMaleItems() ? "m" : "f") + 2, can));
        //}

        private static int tickUpdateLimiter;
        private static Item eatingItem;

        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            tickUpdateLimiter++;
            if (tickUpdateLimiter < 10) return;
            tickUpdateLimiter = 0;
            if (Game1.player == null) return;
            if (eatingItem is not null)
            {
                if (Game1.player.isEating) return;
                
                Modworks.Log.Trace("Dealing with consequences");
                EatForageItem(eatingItem);

                Modworks.Log.Trace("*burp*");
                eatingItem = null;
            }
            else if (Game1.player.isEating && Game1.player.itemToEat is not null)
            {
                Modworks.Log.Trace("Eating starts");
                if (Game1.player.ActiveObject == null) return;
                eatingItem = Game1.player.itemToEat;
                Modworks.Log.Trace($"Eating {Game1.player.ActiveObject.Name}");
            }
        }

        private static void EatForageItem(ISalable item)
        {
            var dn = item.DisplayName;
            string npc;
            
            if (dn is null) return;
            if (dn.EndsWith("'s Panties")) npc = dn[..dn.IndexOf("'s Panties", StringComparison.Ordinal)];
            else if (dn.EndsWith("'s Underwear")) npc = dn[..dn.IndexOf("'s Underwear", StringComparison.Ordinal)];
            else if (dn.EndsWith("'s Delicates")) npc = dn[..dn.IndexOf("'s Delicates", StringComparison.Ordinal)];
            else if (dn.EndsWith("'s Underpants")) npc = dn[..dn.IndexOf("'s Underpants", StringComparison.Ordinal)];
            else return; //not one of ours
            
            if (string.IsNullOrWhiteSpace(npc)) return;

            var npcName = NPCConfig.LookupNPC(npc);
            if (npcName == null) return;

            var npcConfig = NPCConfig.GetNPC(npcName);
            var friendship = Player.GetFriendshipPoints(npcName);

            if (friendship == 0) return; //don't reveal identities of unknown NPCs
            
            Modworks.Log.Trace($"EatForageItem: friendship {friendship}");

            Game1.showGlobalMessage("You feel like this changes your relationship with " + npcConfig.DisplayName + ".");

            if (Modworks.RNG.NextDouble() < 0.25f + (Player.GetLuckFactorFloat() * 0.75f))
            {
                Player.SetFriendshipPoints(npcName, Math.Min(2500, friendship + Modworks.RNG.Next(250)));
                Modworks.Log.Trace("Relationship increased");
            }
            else
            {
                Player.SetFriendshipPoints(npcName, Math.Max(friendship - Modworks.RNG.Next(100), 0));
                Modworks.Log.Trace("Relationship decreased");
            }
        }
    }
}