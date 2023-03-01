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
    public class Palmirinha : Channel
    {
        private static readonly Rectangle QueenOfSauceArea = new(602, 361, 84, 28);
        private static IRawTextureData _palmirinhaTexture;
        
        public Palmirinha(IModHelper helper, IMonitor monitor) : base(helper, monitor)
        {
            _palmirinhaTexture = Helper.ModContent.Load<IRawTextureData>("assets/palmirinha.png");
            
            TargetDialogueAssets = new List<string> { "Strings/StringsFromCSFiles" };
            TargetImageAssets = new List<string> { "LooseSprites/Cursors" };
            
            Helper.Events.Content.AssetRequested += CheckTargetDialogues;
            Helper.Events.Content.AssetRequested += CheckTargetImages;
        }
        
        protected override void SetCustomDialogues(IAssetDataForDictionary<string, string> editor, IAssetName assetName)
        {
            if (!IsChannelEnabled()) return;
            editor.Data["TV.cs.13114"] = I18n.TitlePalmirinha();
            editor.Data["TV.cs.13117"] = I18n.RerunPalmirinha();
            editor.Data["TV.cs.13127"] = I18n.IntroPalmirinha();
            editor.Data["TV.cs.13151"] = I18n.LearnedPalmirinha();
            editor.Data["TV.cs.13153"] = I18n.OutroPalmirinha();
        }

        protected override void SetCustomImages(IAssetDataForImage editor, IAssetName assetName)
        {
            if (!IsChannelEnabled()) return;
            editor.PatchImage(_palmirinhaTexture, targetArea: QueenOfSauceArea);
        }
    }
}