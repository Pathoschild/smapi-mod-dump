/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ha1fdaew/iTile
**
*************************************************/

using StardewModdingAPI;
using StardewValley;

namespace iTile.Core.Logic.SaveSystem
{
    public class SaveManager : Manager
    {
        public const string saveProfileKeyPrefix = "SaveProfile_";
        public SaveProfile session;

        public string SaveProfileKey
            => string.Concat(saveProfileKeyPrefix, iTile.ModID);

        public SaveManager()
        {
            Init();
        }

        public override void Init()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            Helper.Events.GameLoop.Saving += OnSaving;
            Helper.Events.Player.Warped += OnWarped;
        }

        private void OnWarped(object sender, StardewModdingAPI.Events.WarpedEventArgs e)
        {
            session.GetLocationProfileSafe(e.NewLocation).ApplyAllTiles();
        }

        private void OnSaving(object sender, StardewModdingAPI.Events.SavingEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;

            Helper.Data.WriteSaveData(SaveProfileKey, session);
        }

        private void OnSaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                session = GetSessionSaveData();
                InitLocationProfiles();
                InitLPsDataOnLoad();
            }
        }

        public void InitSessionExternal(SaveProfile session)
        {
            this.session = session;
            InitLPsDataOnLoad();
        }

        private void InitLPsDataOnLoad()
        {
            foreach (LocationProfile lp in session.locs)
            {
                lp.LoadInitialTiles();
                lp.ApplyAllTiles();
            }
        }

        private void InitLocationProfiles()
        {
            foreach (GameLocation loc in Game1.locations)
            {
                session.GetLocationProfileSafe(loc);
            }
        }

        private SaveProfile GetSessionSaveData()
        {
            SaveProfile session = Helper.Data.ReadSaveData<SaveProfile>(SaveProfileKey);
            if (session == null)
                session = new SaveProfile(false);
            return session;
        }
    }
}