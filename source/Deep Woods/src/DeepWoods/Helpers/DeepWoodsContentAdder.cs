/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/maxvollmer/DeepWoodsMod
**
*************************************************/


using static DeepWoodsMod.DeepWoodsSettings;
using static DeepWoodsMod.DeepWoodsGlobals;
using System.Collections.Generic;
using StardewModdingAPI.Events;
using DeepWoodsMod.API.Impl;
using StardewModdingAPI.Enums;
using Microsoft.Xna.Framework.Graphics;
using xTile;

namespace DeepWoodsMod.Helpers
{
    public class DeepWoodsContentAdder
    {
        public static void OnLoadStageChanged(object sender, LoadStageChangedEventArgs e)
        {
            switch (e.NewStage)
            {
                case LoadStage.CreatedInitialLocations:
                case LoadStage.SaveAddedLocations:
                    DeepWoodsManager.AddMaxHut();
                    break;
            }
        }

        public static void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            //ModEntry.Log("Bla Name: " + e.NameWithoutLocale, StardewModdingAPI.LogLevel.Error);

            if (e.NameWithoutLocale.StartsWith("Maps/DW_Redirect_SMAPI_To_Content_Root/"))
            {
                e.LoadFrom(() => ModEntry.GetHelper().GameContent.Load<Texture2D>(e.NameWithoutLocale.ToString().Substring("Maps/DW_Redirect_SMAPI_To_Content_Root/".Length)), AssetLoadPriority.Medium);
            }
            else 
            if (e.NameWithoutLocale.IsEquivalentTo("Data/mail"))
            {
                e.Edit(a => EditMail(a.AsDictionary<string, string>().Data));
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Blueprints"))
            {
                e.Edit(a => EditBlueprints(a.AsDictionary<string, string>().Data));
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/NPCDispositions"))
            {
                e.Edit(a => EditNPCDispositions(a.AsDictionary<string, string>().Data));
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/NPCGiftTastes"))
            {
                e.Edit(a => EditNPCGiftTastes(a.AsDictionary<string, string>().Data));
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Strings/SpeechBubbles"))
            {
                e.Edit(a => EditNPCSpeechBubbles(a.AsDictionary<string, string>().Data));
            }
            else if (e.NameWithoutLocale.IsEquivalentTo($"Maps/DeepWoodsMaxHouse"))
            {
                e.LoadFromModFile<Map>("assets/Maps/DeepWoodsMaxHouse.tmx", AssetLoadPriority.Medium);
            }
            /*
            else if (e.NameWithoutLocale.IsEquivalentTo($"Characters/DeepWoodsMax"))
            {
                e.LoadFrom(() => { return DeepWoodsTextures.Textures.MaxCharacter; }, AssetLoadPriority.Medium);
            }
            else if (e.NameWithoutLocale.IsEquivalentTo($"Portraits/DeepWoodsMax"))
            {
                e.LoadFrom(() => { return DeepWoodsTextures.Textures.MaxPortrait; }, AssetLoadPriority.Medium);
            }
            else if (e.NameWithoutLocale.IsEquivalentTo($"Characters/schedules/DeepWoodsMax"))
            {
                e.LoadFromModFile<Dictionary<string, string>>("assets/schedules/DeepWoodsMax.json", AssetLoadPriority.Medium);
            }
            else if (e.NameWithoutLocale.IsEquivalentTo($"Characters/Dialogue/DeepWoodsMax"))
            {
                e.LoadFromModFile<Dictionary<string, string>>("assets/dialogue/DeepWoodsMax.json", AssetLoadPriority.Medium);
            }
            */
            else if (e.NameWithoutLocale.IsEquivalentTo($"Buildings/{WOODS_OBELISK_BUILDING_NAME}"))
            {
                e.LoadFrom(() => { return DeepWoodsTextures.Textures.WoodsObelisk; }, AssetLoadPriority.Medium);
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Maps/deepWoodsLakeTilesheet"))
            {
                e.LoadFrom(() => { return DeepWoodsTextures.Textures.LakeTilesheet; }, AssetLoadPriority.Medium);
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Maps/deepWoodsInfestedOutdoorsTileSheet"))
            {
                e.LoadFrom(() => { return DeepWoodsTextures.Textures.InfestedOutdoorsTilesheet; }, AssetLoadPriority.Medium);
            }
        }

        public static void EditMail(IDictionary<string, string> mailData)
        {
            mailData.Add(WOODS_OBELISK_WIZARD_MAIL_ID, I18N.WoodsObeliskWizardMailMessage);
        }

        public static void EditBlueprints(IDictionary<string, string> bluePrintsData)
        {
            string itemsRequired = "";

            foreach (var item in Settings.Objects.WoodsObelisk.ItemsRequired)
            {
                itemsRequired += item.Key + " " + item.Value + " ";
            }

            bluePrintsData.Add(
                WOODS_OBELISK_BUILDING_NAME,
                "" + itemsRequired.Trim() + "/3/2/-1/-1/-2/-1/null/" + I18N.WoodsObeliskDisplayName + "/" + I18N.WoodsObeliskDescription + "/Buildings/none/48/128/-1/null/Farm/" + Settings.Objects.WoodsObelisk.MoneyRequired + "/true"
            );
        }

        public static void EditNPCDispositions(IDictionary<string, string> npcDispositionsData)
        {
            //npcDispositionsData.Add("DeepWoodsMax", "adult/polite/shy/positive/female/non-datable/Null/Other/spring 1//DeepWoodsMaxHouse 16 17/Max");
        }

        public static void EditNPCGiftTastes(IDictionary<string, string> npcGiftTasteData)
        {
            //npcGiftTasteData.Add("DeepWoodsMax", "This is amazing! Thank you!//Oh what a lovely gift. It reminds me of the forest.//Sure, I can put this in my recycling, no problem.//Why would you do that? Please don't do that again.//Thanks?/ ");
        }

        public static void EditNPCSpeechBubbles(IDictionary<string, string> npcSpeechBubblesData)
        {
            /*
            npcSpeechBubblesData.Add("DeepWoods_DeepWoodsMax_Greeting", "Hallu :3");
            npcSpeechBubblesData.Add("DeepWoods_DeepWoodsMax_RareGreeting", "Blub!");
            npcSpeechBubblesData.Add("DeepWoods_DeepWoodsMax_CloseFriends", "Yay, it's you!");
            */
        }
    }
}
