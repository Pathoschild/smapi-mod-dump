/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JhonnieRandler/TVBrasileira
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace TVBrasileira.channels
{
    public class GloboRural : Channel
    {
        private static readonly Rectangle LivinOffTheLandArea = new(517, 361, 84, 28);
        private static IRawTextureData _globoRuralTexture;

        public GloboRural(IModHelper helper, IMonitor monitor) : base(helper, monitor)
        {
            _globoRuralTexture = Helper.ModContent.Load<IRawTextureData>("assets/globoRural.png");
            
            TargetDialogueAssets = new List<string> { "Strings/StringsFromCSFiles", "Data/TV/TipChannel" };
            TargetImageAssets = new List<string> { "LooseSprites/Cursors" };
            
            Helper.Events.Content.AssetRequested += CheckTargetDialogues;
            Helper.Events.Content.AssetRequested += CheckTargetImages;
        }
        
        protected override void SetCustomDialogues(IAssetDataForDictionary<string, string> editor, IAssetName assetName)
        {
            if (!IsChannelEnabled()) return;
            
            switch (assetName.ToString())
            {
                case "Data/TV/TipChannel":
                    editor.Data["1"] = I18n._1();
                    editor.Data["8"] = I18n._8();
                    editor.Data["15"] = I18n._15();
                    editor.Data["22"] = I18n._22();
                    editor.Data["29"] = I18n._29();
                    editor.Data["36"] = I18n._36();
                    editor.Data["43"] = I18n._43();
                    editor.Data["50"] = I18n._50();
                    editor.Data["57"] = I18n._57();
                    editor.Data["64"] = I18n._64();
                    editor.Data["71"] = I18n._71();
                    editor.Data["78"] = I18n._78();
                    editor.Data["85"] = I18n._85();
                    editor.Data["92"] = I18n._92();
                    editor.Data["99"] = I18n._99();
                    editor.Data["106"] = I18n._106();
                    editor.Data["113"] = I18n._113();
                    editor.Data["120"] = I18n._120();
                    editor.Data["127"] = I18n._127();
                    editor.Data["134"] = I18n._134();
                    editor.Data["141"] = I18n._141();
                    editor.Data["148"] = I18n._148();
                    editor.Data["155"] = I18n._155();
                    editor.Data["162"] = I18n._162();
                    editor.Data["169"] = I18n._169();
                    editor.Data["176"] = I18n._176();
                    editor.Data["183"] = I18n._183();
                    editor.Data["190"] = I18n._190();
                    editor.Data["197"] = I18n._197();
                    editor.Data["204"] = I18n._204();
                    editor.Data["211"] = I18n._211();
                    editor.Data["218"] = I18n._218();
                    editor.Data["221"] = I18n._221();
                    return;
                case "Strings/StringsFromCSFiles":
                    editor.Data["TV.cs.13111"] = I18n.TitleGloboRural();
                    editor.Data["TV.cs.13124"] = I18n.IntroGloboRural();
                    return;
            }
        }

        protected override void SetCustomImages(IAssetDataForImage editor, IAssetName assetName)
        {
            if (!IsChannelEnabled()) return;
            editor.PatchImage(_globoRuralTexture, targetArea: LivinOffTheLandArea);
        }
    }
}