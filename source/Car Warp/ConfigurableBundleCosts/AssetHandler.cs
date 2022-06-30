/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace ConfigurableBundleCosts;

internal class AssetHandler
{
    public static Dictionary<string, string> BundleData;

    private static Texture2D CdFont;
    private static readonly Vector2 BusOnesDigit = new(130, 59);
    private static readonly Vector2 MinecartsOnesDigit = new(301, 59);
    private static readonly Vector2 BridgeOnesDigit = new(131, 89);
    private static readonly Vector2 GreenhouseOnesDigit = new(300, 89);
    private static readonly Vector2 PanningOnesDigit = new(129, 119);
    private static readonly Vector2 PanningOnesDigitGerman = new(130, 127);

    internal static void LoadOrEditAssets(object sender, AssetRequestedEventArgs e)
    {
        if (e.Name.IsEquivalentTo(Globals.FontContentPath))
            e.LoadFromModFile<Texture2D>("Assets/cdFont.png", AssetLoadPriority.Medium);
        else if (e.Name.IsEquivalentTo("LooseSprites/JojaCDForm"))
            e.Edit(EditJojaCdForm);
        else if (e.Name.IsEquivalentTo("Data/Bundles"))
            e.Edit(EditBundleData);
        else if (e.Name.IsEquivalentTo("Data/ExtraDialogue"))
            e.Edit(EditExtraDialogue);
    }

    private static void EditJojaCdForm(IAssetData asset)
    {
        if (!Globals.Config.Joja.ApplyValues)
            return;

        IAssetDataForImage cdForm = asset.AsImage();

        Dictionary<string, string> bundlePrices = GetCurrentBundlePrices();

        foreach (KeyValuePair<string, string> pair in bundlePrices)
        {
            CdFont ??= Globals.GameContent.Load<Texture2D>(Globals.FontContentPath);

            // get number of digits in value
            int numDigits = pair.Value.Length;

            Rectangle sourceRect, destRect;
            Vector2 destStartPos = GetDestStartPos(pair.Key); // position on form to start overwriting from

            // need to erase existing numbers if new number is shorter than the original
            if (numDigits < 5)
            {
                // get dots from cdFont.png
                sourceRect = new Rectangle(70, 0, 35, 11);
                // get ten-thousands position of destination - slightly different positions on left and right side of form
                int offset = pair.Key is "minecarts" or "greenhouse" ? 34 : 33;
                // get position to overwrite using offset
                destRect = new Rectangle((int) destStartPos.X - offset, (int) destStartPos.Y, 35, 11);
                // overwrite with dots
                cdForm.PatchImage(CdFont, sourceRect, destRect);
            }

            // start at last index in pair.Value; decrement until at first index
            for (int k = pair.Value.Length - 1; k >= 0; k--)
            {
                // if number is negative, need to set position of '-' manually on tilesheet
                int digit = pair.Value[k] == '-' ? 15 : int.Parse(pair.Value[k].ToString());

                // get current digit position in number
                int currentPos = numDigits - (k + 1);

                // create source rectangle by adjusting starting x position by tilesheet position determined earlier
                sourceRect = new Rectangle(7 * digit, 0, 7, 11);

                // get destination rectangle by adjusting start position by current digit position
                destRect = new Rectangle((int) destStartPos.X - (7 * currentPos), (int) destStartPos.Y, 7, 11);

                // overwrite current position with digit
                cdForm.PatchImage(CdFont, sourceRect, destRect);
            }
        }
    }

    private static void EditBundleData(IAssetData asset)
    {
        IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
        CultureInfo culture = CultureInfo.GetCultureInfo(Globals.Helper.Translation.Locale);

        int bundle1 = Globals.Config.Vault.Bundle1;
        int bundle2 = Globals.Config.Vault.Bundle2;
        int bundle3 = Globals.Config.Vault.Bundle3;
        int bundle4 = Globals.Config.Vault.Bundle4;

        string bundle1String = bundle1.ToString("#,###", culture);
        string bundle2String = bundle2.ToString("#,###", culture);
        string bundle3String = bundle3.ToString("#,###", culture);
        string bundle4String = bundle4.ToString("#,###", culture);

        foreach (string key in data.Keys.ToArray())
        {
            if (!key.StartsWith("Vault"))
                continue;

            string[] field = data[key].Split('/');

            switch (key)
            {
                case "Vault/23":
                {
                    field[0] = bundle1String;              // name field
                    field[2] = $"-1 {bundle1} {bundle1}";  // value field
                    break;
                }
                case "Vault/24":
                {
                    field[0] = bundle2String;
                    field[2] = $"-1 {bundle2} {bundle2}";
                    break;
                }
                case "Vault/25":
                {
                    field[0] = bundle3String;
                    field[2] = $"-1 {bundle3} {bundle3}";
                    break;
                }
                case "Vault/26":
                {
                    field[0] = bundle4String;
                    field[2] = $"-1 {bundle4} {bundle4}";
                    break;
                }
                default:
                {
                    field[0] = "";
                    field[2] = "";
                    break;
                }
            }

            data[key] = string.Join("/", field);
        }
        
        BundleData = new Dictionary<string, string>(data);
    }

    private static void EditExtraDialogue(IAssetData asset)
    {
        if (!Globals.Config.Joja.ApplyValues ||
            (Globals.Config.Joja.MovieTheaterCost == 500000 && Globals.Config.Joja.MembershipCost == 5000))
            return;

        IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
        CultureInfo culture = CultureInfo.GetCultureInfo(Globals.Helper.Translation.Locale);

        string newValue = Globals.Config.Joja.MovieTheaterCost.ToString("#,###", culture);
        string joinCost = Globals.Config.Joja.MembershipCost.ToString("#,###", culture);

        data["Morris_BuyMovieTheater"] = Globals.Helper.Translation.Get("Morris_BuyMovieTheater", new { buyValue = newValue });
        data["Morris_TheaterBought"] = Globals.Helper.Translation.Get("Morris_TheaterBought", new { buyValue = newValue });
        data["Morris_WeekendGreeting_MembershipAvailable"] = Globals.Helper.Translation.Get("Morris_WeekendGreeting_MembershipAvailable", new { membershipCost = joinCost });
        data["Morris_FirstGreeting_MembershipAvailable"] = Globals.Helper.Translation.Get("Morris_FirstGreeting_MembershipAvailable", new { membershipCost = joinCost });
    }

    private static Dictionary<string, string> GetCurrentBundlePrices()
    {
        return new Dictionary<string, string>
        {
            ["bus"] = Globals.Config.Joja.BusCost.ToString(),
            ["minecarts"] = Globals.Config.Joja.MinecartsCost.ToString(),
            ["bridge"] = Globals.Config.Joja.BridgeCost.ToString(),
            ["greenhouse"] = Globals.Config.Joja.GreenhouseCost.ToString(),
            ["panning"] = Globals.Config.Joja.PanningCost.ToString()
        };
    }

    private static Vector2 GetDestStartPos(string key)
    {
        return key switch
        {
            "bus" => BusOnesDigit,
            "minecarts" => MinecartsOnesDigit,
            "bridge" => BridgeOnesDigit,
            "greenhouse" => GreenhouseOnesDigit,
            "panning" => Globals.Helper.Translation.Locale.ToLower().Equals("de-de")
                ? PanningOnesDigitGerman
                : PanningOnesDigit,
            _ => new Vector2(-1, -1)
        };
    }
}
