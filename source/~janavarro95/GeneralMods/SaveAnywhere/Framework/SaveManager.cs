/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Omegasis.SaveAnywhere.Framework.Models;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Monsters;

namespace Omegasis.SaveAnywhere.Framework
{
    /// <summary>Provides methods for saving and loading game data.</summary>
    public class SaveManager
    {
        /*********
        ** Fields
        *********/
        /// <summary>Simplifies access to game code.</summary>
        private readonly IReflectionHelper Reflection;

        /// <summary>A callback invoked when data is loaded.</summary>
        private readonly Action OnLoaded;

        /// <summary>SMAPI's APIs for this mod.</summary>
        private readonly IModHelper Helper;

        /// <summary>The relative path to the player data file.</summary>
        private string RelativeDataPath => Path.Combine("data", $"{Constants.SaveFolderName}.json");

        /// <summary>Whether we should save at the next opportunity.</summary>
        private bool WaitingToSave;

        /// <summary> Currently displayed save menu (null if no menu is displayed) </summary>
        private NewSaveGameMenuV2 currentSaveMenu;


        public Dictionary<string, Action> beforeCustomSavingBegins;
        public event EventHandler beforeSave;

        public Dictionary<string, Action> afterCustomSavingCompleted;
        public event EventHandler afterSave;

        public Dictionary<string, Action> afterSaveLoaded;
        public event EventHandler afterLoad;

        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="helper">SMAPI's APIs for this mod.</param>
        /// <param name="reflection">Simplifies access to game code.</param>
        /// <param name="onLoaded">A callback invoked when data is loaded.</param>
        public SaveManager(IModHelper helper, IReflectionHelper reflection, Action onLoaded)
        {
            this.Helper = helper;
            this.Reflection = reflection;
            this.OnLoaded = onLoaded;

            this.beforeCustomSavingBegins = new Dictionary<string, Action>();
            this.afterCustomSavingCompleted = new Dictionary<string, Action>();
            this.afterSaveLoaded = new Dictionary<string, Action>();

        }

        private void empty(object o, EventArgs args) { }

        /// <summary>Perform any required update logic.</summary>
        public void Update()
        {
            // perform passive save
            if (this.WaitingToSave && Game1.activeClickableMenu == null)
            {
                this.currentSaveMenu = new NewSaveGameMenuV2();
                this.currentSaveMenu.SaveComplete += this.CurrentSaveMenu_SaveComplete;
                Game1.activeClickableMenu = this.currentSaveMenu;
                this.WaitingToSave = false;
            }
        }

        /// <summary>Event function for NewSaveGameMenu event SaveComplete</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void CurrentSaveMenu_SaveComplete(object sender, EventArgs e)
        {
            this.currentSaveMenu.SaveComplete -= this.CurrentSaveMenu_SaveComplete;
            this.currentSaveMenu = null;
            SaveAnywhere.RestoreMonsters();
            if (this.afterSave != null)
            {
                this.afterSave.Invoke(this, EventArgs.Empty);
            }
            
            foreach (var v in this.afterCustomSavingCompleted)
            {
                v.Value.Invoke();
            }
        }

        /// <summary>Clear saved data.</summary>
        public void ClearData()
        {
            if (File.Exists(Path.Combine(this.Helper.DirectoryPath, this.RelativeDataPath)))
            {
                File.Delete(Path.Combine(this.Helper.DirectoryPath, this.RelativeDataPath));
            }
            this.RemoveLegacyDataForThisPlayer();
        }

        /// <summary>
        /// Checks to see if a custom save file exists for the player.
        /// </summary>
        /// <returns></returns>
        public bool saveDataExists()
        {
            return File.Exists(Path.Combine(this.Helper.DirectoryPath, this.RelativeDataPath));
        }

        /// <summary>Initiate a game save.</summary>
        public void BeginSaveData()
        {
            if (this.beforeSave != null)
            {
                this.beforeSave.Invoke(this, EventArgs.Empty);
            }
            foreach(var v in this.beforeCustomSavingBegins)
            {
                v.Value.Invoke();
            }

            SaveAnywhere.Instance.cleanMonsters();

            // save game data
            Farm farm = Game1.getFarm();
            if (farm.getShippingBin(Game1.player)!=null)
            {

                //Game1.activeClickableMenu = new NewShippingMenu(farm.getShippingBin(Game1.player), this.Reflection);
                Game1.activeClickableMenu = new NewShippingMenuV2(farm.getShippingBin(Game1.player));
                //farm.getShippingBin(Game1.player).Clear();
                farm.lastItemShipped = null;
                this.WaitingToSave = true;
            }
            else
            {
                this.currentSaveMenu = new NewSaveGameMenuV2();
                this.currentSaveMenu.SaveComplete += this.CurrentSaveMenu_SaveComplete;
                Game1.activeClickableMenu = this.currentSaveMenu;
            }


            // save data to disk
            PlayerData data = new PlayerData
            {
                Time = Game1.timeOfDay,
                Characters = this.GetPositions().ToArray(),
                IsCharacterSwimming = Game1.player.swimming.Value
            };
            this.Helper.Data.WriteJsonFile(this.RelativeDataPath, data);

            // clear any legacy data (no longer needed as backup)
            this.RemoveLegacyDataForThisPlayer();
        }

        /// <summary>Load all game data.</summary>
        public void LoadData()
        {
            // get data
            PlayerData data = this.Helper.Data.ReadJsonFile<PlayerData>(this.RelativeDataPath);
            if (data == null)
                return;

            // apply
            Game1.timeOfDay = data.Time;
            this.ResumeSwimming(data);
            this.SetPositions(data.Characters);
            this.OnLoaded?.Invoke();
            if (this.afterLoad != null)
            {
                this.afterLoad.Invoke(this, EventArgs.Empty);
            }
            foreach (var v in this.afterSaveLoaded)
            {
                v.Value.Invoke();
            }

            // Notify other mods that load is complete
            //AfterLoad.Invoke(this, EventArgs.Empty);
        }

        /// <summary>Checks to see if the player was swimming when the game was saved and if so, resumes the swimming animation.</summary>
        public void ResumeSwimming(PlayerData data)
        {
            try
            {
                if (data.IsCharacterSwimming)
                {
                    Game1.player.changeIntoSwimsuit();
                    Game1.player.swimming.Value = true;
                }
            }
            catch
            {
                //Here to allow compatability with old save files.
            }
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Get the current character positions.</summary>
        private IEnumerable<CharacterData> GetPositions()
        {
            // player
            {
                var player = Game1.player;
                string name = player.Name;
                string map = player.currentLocation.uniqueName.Value; //Try to get a unique name for the location and if we can't we are going to default to the actual name of the map.
                if (string.IsNullOrEmpty(map))
                    map = player.currentLocation.Name; //This is used to account for maps that share the same name but have a unique ID such as Coops, Barns and Sheds.
                Point tile = player.getTileLocationPoint();
                int facingDirection = player.facingDirection;

                yield return new CharacterData(CharacterType.Player, name, map, tile, facingDirection);
            }

            // NPCs (including horse and pets)
            foreach (NPC npc in Utility.getAllCharacters())
            {
                CharacterType? type = this.GetCharacterType(npc);
                if (type == null || npc?.currentLocation == null)
                    continue;
                string name = npc.Name;
                string map = npc.currentLocation.Name;
                Point tile = npc.getTileLocationPoint();
                int facingDirection = npc.facingDirection;

                yield return new CharacterData(type.Value, name, map, tile, facingDirection);
            }
        }

        /// <summary>Reset characters to their saved state.</summary>
        /// <param name="positions">The positions to set.</param>
        /// <returns>Returns whether any NPCs changed position.</returns>
        private void SetPositions(CharacterData[] positions)
        {
            // player
            {
                CharacterData data = positions.FirstOrDefault(p => p.Type == CharacterType.Player && p.Name.Equals(Game1.player.Name));
                if (data != null)
                {
                    Game1.player.previousLocationName = Game1.player.currentLocation.Name;
                    //Game1.player. locationAfterWarp = Game1.getLocationFromName(data.Map);
                    Game1.xLocationAfterWarp = data.X;
                    Game1.yLocationAfterWarp = data.Y;
                    Game1.facingDirectionAfterWarp = data.FacingDirection;
                    Game1.fadeScreenToBlack();
                    Game1.warpFarmer(data.Map, data.X, data.Y, false);
                    Game1.player.faceDirection(data.FacingDirection);
                }
            }

            // NPCs (including horse and pets)
            foreach (NPC npc in Utility.getAllCharacters())
            {
                // get NPC type
                CharacterType? type = this.GetCharacterType(npc);
                if (type == null)
                    continue;

                // get saved data
                CharacterData data = positions.FirstOrDefault(p => p.Type == type && p.Name == npc.Name);
                if (data == null)
                    continue;

                // update NPC
                Game1.warpCharacter(npc, data.Map, new Point(data.X, data.Y));
                npc.faceDirection(data.FacingDirection);
            }
        }

        /// <summary>Get the character type for an NPC.</summary>
        /// <param name="npc">The NPC to check.</param>
        private CharacterType? GetCharacterType(NPC npc)
        {
            if (npc is Monster)
                return null;
            if (npc is Horse)
                return CharacterType.Horse;
            if (npc is Pet)
                return CharacterType.Pet;
            return CharacterType.Villager;
        }

        /// <summary>Remove legacy save data for this player.</summary>
        private void RemoveLegacyDataForThisPlayer()
        {
            DirectoryInfo dataDir = new DirectoryInfo(Path.Combine(this.Helper.DirectoryPath, "Save_Data"));
            DirectoryInfo playerDir = new DirectoryInfo(Path.Combine(dataDir.FullName, Game1.player.Name));
            if (playerDir.Exists)
                playerDir.Delete(recursive: true);
            if (dataDir.Exists && !dataDir.EnumerateDirectories().Any())
                dataDir.Delete(recursive: true);
        }
    }
}
