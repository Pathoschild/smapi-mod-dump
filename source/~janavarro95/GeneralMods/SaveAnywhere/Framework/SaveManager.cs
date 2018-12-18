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
        ** Properties
        *********/
        /// <summary>Simplifies access to game code.</summary>
        private readonly IReflectionHelper Reflection;

        /// <summary>A callback invoked when data is loaded.</summary>
        private readonly Action OnLoaded;

        /// <summary>SMAPI's APIs for this mod.</summary>
        private readonly IModHelper Helper;

        /// <summary>The full path to the player data file.</summary>
        private string SavePath => Path.Combine(this.Helper.DirectoryPath, "data", $"{Constants.SaveFolderName}.json");

        /// <summary>Whether we should save at the next opportunity.</summary>
        private bool WaitingToSave;

        /// <summary> Currently displayed save menu (null if no menu is displayed) </summary>
        private NewSaveGameMenu currentSaveMenu;



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

        }

        private void empty(object o, EventArgs args)
        {

        }

        /// <summary>Perform any required update logic.</summary>
        public void Update()
        {
            // perform passive save
            if (this.WaitingToSave && Game1.activeClickableMenu == null)
            {
                currentSaveMenu = new NewSaveGameMenu();
                currentSaveMenu.SaveComplete += CurrentSaveMenu_SaveComplete;
                Game1.activeClickableMenu = currentSaveMenu;
                this.WaitingToSave = false;
            }
        }

        /// <summary>
        ///     Event function for NewSaveGameMenu event SaveComplete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentSaveMenu_SaveComplete(object sender, EventArgs e)
        {
            currentSaveMenu.SaveComplete -= CurrentSaveMenu_SaveComplete;
            currentSaveMenu = null;
            //AfterSave.Invoke(this, EventArgs.Empty);
        }

        /// <summary>Clear saved data.</summary>
        public void ClearData()
        {
            File.Delete(this.SavePath);
            this.RemoveLegacyDataForThisPlayer();
        }

        /// <summary>Initiate a game save.</summary>
        public void BeginSaveData()
        {
            
            // save game data
            Farm farm = Game1.getFarm();
            if (farm.shippingBin.Any())
            {
                
                Game1.activeClickableMenu = new NewShippingMenu(farm.shippingBin, this.Reflection);
                farm.shippingBin.Clear();
                farm.lastItemShipped = null;
                this.WaitingToSave = true;
            }
            else
            {
                currentSaveMenu = new NewSaveGameMenu();
                currentSaveMenu.SaveComplete += CurrentSaveMenu_SaveComplete;
                Game1.activeClickableMenu = currentSaveMenu;
            }
                

            // get data
            PlayerData data = new PlayerData
            {
                Time = Game1.timeOfDay,
                Characters = this.GetPositions().ToArray(),
                IsCharacterSwimming = Game1.player.swimming.Value
            };

            // save to disk
            // ReSharper disable once PossibleNullReferenceException -- not applicable
            Directory.CreateDirectory(new FileInfo(this.SavePath).Directory.FullName);
            this.Helper.WriteJsonFile(this.SavePath, data);

            // clear any legacy data (no longer needed as backup)1
            this.RemoveLegacyDataForThisPlayer();
        }

        /// <summary>Load all game data.</summary>
        public void LoadData()
        {
            // get data
            PlayerData data = this.Helper.ReadJsonFile<PlayerData>(this.SavePath);
            if (data == null)
                return;

            // apply
            Game1.timeOfDay = data.Time;
            this.ResumeSwimming(data);
            this.SetPositions(data.Characters);
            this.OnLoaded?.Invoke();

            // Notify other mods that load is complete
            //AfterLoad.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Checks to see if the player was swimming when the game was saved and if so, resumes the swimming animation.
        /// </summary>
        /// <param name="data"></param>
        public void ResumeSwimming(PlayerData data)
        {
            try
            {
                if (data.IsCharacterSwimming == true)
                {
                    Game1.player.changeIntoSwimsuit();
                    Game1.player.swimming.Value = true;
                }
            }
            catch (Exception err)
            {
                err.ToString();
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
                if (map == ""|| map==null) map = player.currentLocation.Name; //This is used to account for maps that share the same name but have a unique ID such as Coops, Barns and Sheds.
                Point tile = player.getTileLocationPoint();
                int facingDirection = player.facingDirection;

                yield return new CharacterData(CharacterType.Player, name, map, tile, facingDirection);
            }

            // NPCs (including horse and pets)
            foreach (NPC npc in Utility.getAllCharacters())
            {
                CharacterType? type = this.GetCharacterType(npc);
                if (type == null)
                    continue;
                if (npc == null || npc.currentLocation == null) continue;
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
                CharacterData data = positions.FirstOrDefault(p => p.Type == CharacterType.Player && p.Name == Game1.player.Name);
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
