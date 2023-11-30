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
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

#pragma warning disable IDE1006 // Naming Styles

namespace SeeAnimalPronouns;

[HarmonyPatch]
class HarmonyPatches
{
    [HarmonyPatch(typeof(FarmAnimal), nameof(FarmAnimal.getMoodMessage))]
    [HarmonyPostfix]
    public static void getMoodMessage_Postfix(FarmAnimal __instance, ref string __result)
    {
        string thirdPersonPronounWithContraction = Helpers.GetPersonalPronounFromId(__instance.myID.Value);

        if (__result.Contains("He's"))
        {
            __result = __result.Replace("He's", thirdPersonPronounWithContraction);
        }
        else
        {
            __result = __result.Replace("She's", thirdPersonPronounWithContraction);
        }
    }

    [HarmonyPatch(typeof(AnimalQueryMenu), nameof(AnimalQueryMenu.draw))]
    [HarmonyPostfix]
    public static void draw_Postfix(AnimalQueryMenu __instance, SpriteBatch b)
    {
        FarmAnimal animal;
        if (Helpers.CachedFarmAnimal is not null)
        {
            animal = Helpers.CachedFarmAnimal;
        }
        else
        {
            animal = (FarmAnimal)Helpers.animalField.GetValue(__instance);
            Helpers.CachedFarmAnimal = animal;
        }

        TextBox textBox = new(null, null, Game1.dialogueFont, Game1.textColor)
        {
            X = Game1.uiViewport.Width / 2 - 128 - 12,
            Y = __instance.yPositionOnScreen - 4 + AnimalQueryMenu.height + 20,
            Width = 256,
            Height = 192
        };
        textBox.Draw(b);

        string text = "Pronouns: " + Helpers.GetPronounStringFromId(animal.myID.Value);
        float textWidth = Game1.smallFont.MeasureString(text).X;
        float xPos = textBox.X + textBox.Width / 2 - textWidth / 2 + 8;
        float yPos = textBox.Y + 12;
        Utility.drawTextWithShadow(b, text, Game1.smallFont, new Vector2(xPos, yPos), Game1.textColor, 1f, 0.89f);

        // my textbox gets drawn over the mouse and i dont want to bother figuring out why the layering stuff doesnt work so just draw the mouse again over the textbox
        __instance.drawMouse(b);
    }

}

public class Helpers
{
    internal static Dictionary<string, PronounSet> BasePronounsDict;    // Stores base pronoun sets - they/them, she/her, xe/xem, etc
    internal static Dictionary<string, PronounSet> PronounsDict;        // Stores base pronoun sets as well as combo sets - he/they, xe/she, etc - dynamically generated from base pronoun sets
    internal static bool BasePronounsDictDirty = false;

    internal static string CachedThirdPersonPronoun;
    internal static string CachedPronounString;
    internal static FarmAnimal CachedFarmAnimal;

    internal static Random Random = new();

    internal static FieldInfo animalField = AccessTools.Field(typeof(AnimalQueryMenu), "animal");

    internal static string GetPersonalPronounFromId(long id)
    {
        if (PronounsDict is null || BasePronounsDictDirty)
            GeneratePronounsDict();

        if (CachedThirdPersonPronoun is not null)
            return CachedThirdPersonPronoun;

        int index = Math.Abs(id.GetHashCode()) % PronounsDict.Count;
        List<string> thirdPersonPronounsWithContraction = PronounsDict.ElementAt(index).Value.ThirdPersonPronounWithContraction;

        CachedThirdPersonPronoun = thirdPersonPronounsWithContraction[Random.Next(thirdPersonPronounsWithContraction.Count)];
        return CachedThirdPersonPronoun;
    }

    internal static string GetPronounStringFromId(long id)
    {
        if (PronounsDict is null || BasePronounsDictDirty)
            GeneratePronounsDict();

        if (CachedPronounString is not null)
            return CachedPronounString;

        int index = Math.Abs(id.GetHashCode()) % PronounsDict.Count;
        CachedPronounString = PronounsDict.ElementAt(index).Key;
        return CachedPronounString;
    }


    private static void GeneratePronounsDict()
    {
        BasePronounsDictDirty = false;
        BasePronounsDict = Globals.GameContent.Load<Dictionary<string, PronounSet>>(Globals.BasePronounsAssetPath);
        PronounsDict = new Dictionary<string, PronounSet>(BasePronounsDict);
        IEnumerable<string> anyThirdPersonPronounsWithContraction = new List<string>();

        foreach (var pronouns in BasePronounsDict)
        {
            if (pronouns.Value.ExcludeFromCombos)
                continue;

            anyThirdPersonPronounsWithContraction = anyThirdPersonPronounsWithContraction.Concat(pronouns.Value.ThirdPersonPronounWithContraction);

            foreach (var otherPronouns in BasePronounsDict)
            {
                if (pronouns.Key == otherPronouns.Key || otherPronouns.Value.ExcludeFromCombos)
                    continue;

                KeyValuePair<string, PronounSet> ComboSet = GenerateComboSet(pronouns, otherPronouns);
                PronounsDict.Add(ComboSet.Key, ComboSet.Value);
            }
        }

        PronounSet anyPronouns = new(anyThirdPersonPronounsWithContraction.ToList(), true);
        PronounsDict.Add("any/all", anyPronouns);

        Log.Trace($"All pronoun options generated:\n{string.Join("\n", PronounsDict.Select(kvp => $"{{{kvp.Key} | {kvp.Value}}}"))}");

    }

    private static KeyValuePair<string, PronounSet> GenerateComboSet(KeyValuePair<string, PronounSet> pronouns, KeyValuePair<string, PronounSet> otherPronouns)
    {
        string key = pronouns.Key.Split('/')[0] + "/" + otherPronouns.Key.Split('/')[0];
        List<string> thirdPersonPronounsWithContraction = pronouns.Value.ThirdPersonPronounWithContraction.Concat(otherPronouns.Value.ThirdPersonPronounWithContraction).ToList();
        PronounSet comboSet = new(thirdPersonPronounsWithContraction, true);
        return new KeyValuePair<string, PronounSet>(key, comboSet);
    }

    internal static void ClearCache()
    {
        CachedThirdPersonPronoun = null;
        CachedPronounString = null;
        CachedFarmAnimal = null;
    }

    public class PronounSet
    {
        public List<string> ThirdPersonPronounWithContraction;
        public bool ExcludeFromCombos = false;

        public PronounSet(List<string> thirdPersonPronounWithContraction, bool excludeFromCombos)
        {
            ThirdPersonPronounWithContraction = thirdPersonPronounWithContraction;
            ExcludeFromCombos = excludeFromCombos;
        }

        public override string ToString()
        {
            return $"({string.Join(", ", ThirdPersonPronounWithContraction)})";
        }
    }
}

#pragma warning restore IDE1006 // Naming Styles
