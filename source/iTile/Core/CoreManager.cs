/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ha1fdaew/iTile
**
*************************************************/

#pragma warning disable IDE0074 // Use compound assignment

using iTile.Client.UI.Impl;
using iTile.Core.Logic;
using iTile.Core.Config;
using iTile.Core.Logic.SaveSystem;
using iTile.Core.Logic.Network;

namespace iTile.Core
{
    public class CoreManager : Manager
    {
        public static readonly int defaultNotificationTime = 200;
        private static CoreManager instance;
        public AssetsManager assetsManager;
        public UIManager uiManager;
        public SaveManager saveManager;
        public NetworkManager netManager;
        public TileMode tileMode;
        public ModConfig config;

        public static CoreManager Instance
            => instance ?? (instance = new CoreManager());

        public override void Init()
        {
            InitConfig();
            assetsManager = new AssetsManager();
            uiManager = new UIManager();
            saveManager = new SaveManager();
            netManager = new NetworkManager();
            tileMode = new TileMode();
        }

        private void InitConfig()
        {
            config = Helper.ReadConfig<ModConfig>();
        }

        public static void ShowNotification(int time, string text)
        {
            Instance.uiManager?.ShowNotification(time, text);
        }

        public static string SelectedLayer
            => Instance.uiManager.controlPanel.layersPanel.SelectedLayer;
    }
}