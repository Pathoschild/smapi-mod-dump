/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using BirbCore.APIs;
using BirbCore.Attributes;
using BirbCore.Extensions;
using SpaceCore;
using SpaceCore.Interface;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace BirbShared;

public class BirbSkill : Skills.Skill
{
    private readonly IMargo _margoApi;

    private bool DoMargo
    {
        get
        {
            if (this._margoApi is null)
            {
                return false;
            }

            IMargo.IModConfig config = this._margoApi.GetConfig();
            return config.EnableProfessions;
        }
    }

    private Func<int, List<string>> ExtraInfo { get; }
    private Func<int, string> HoverText { get; }
    private readonly IModHelper _modHelper;
    public static Dictionary<string, Profession> KeyedProfessions { get; } = new();

    public static void Register(string id, IRawTextureData texture, IModHelper modHelper,
        Dictionary<string, object> professionTokens, Func<int, List<string>> extraInfo, Func<int, string> hoverText)
    {
        BirbSkill skill = new(id, texture, modHelper, professionTokens, extraInfo, hoverText);

        Skills.RegisterSkill(skill);

        if (skill.DoMargo)
        {
            skill._margoApi.RegisterCustomSkillForPrestige(skill.Id);
        }
    }

    private BirbSkill(string id, IRawTextureData texture, IModHelper modHelper,
        Dictionary<string, object> professionTokens, Func<int, List<string>> extraInfo,
        Func<int, string> hoverText) : base(id)
    {
        if (professionTokens.Count != 6)
        {
            Log.Error("Birb Skills only handle exactly 6 professions currently");
            return;
        }

        this._modHelper = modHelper;
        this.Icon = texture.GetTextureRect(0, 0, 16, 16);
        this.SkillsPageIcon = texture.GetTextureRect(16, 0, 10, 10);
        this.ExperienceBarColor = texture.GetColor(32, 0);
        this.ExtraInfo = extraInfo;
        this.HoverText = hoverText;
        this._margoApi = (IMargo)modHelper.ModRegistry.GetApi("DaLion.Overhaul");

        if (this.DoMargo)
        {
            this.ExperienceCurve =
            [
                100, 380, 770, 1300, 2150, 3300, 4000, 6900, 10000, 15000,
                20000, 25000, 30000, 35000, 40000, 45000, 50000, 55000, 60000, 70000
            ];
        }
        else
        {
            this.ExperienceCurve = [100, 380, 770, 1300, 2150, 3300, 4000, 6900, 10000, 15000];
        }

        int i = 0;
        foreach (string profession in professionTokens.Keys)
        {
            object tokens = professionTokens[profession];

            KeyedProfession p = new(this, profession, texture.GetTextureRect(16 * i, 16, 16, 16),
                this.DoMargo ? texture.GetTextureRect(16 * i, 32, 16, 16) : null, modHelper, tokens);

            this.Professions.Add(p);
            KeyedProfessions.Add(profession, p);
            i++;
        }

        this.ProfessionsForLevels.Add(new ProfessionPair(5, this.Professions[0], this.Professions[1]));
        this.ProfessionsForLevels.Add(new ProfessionPair(10, this.Professions[2], this.Professions[3],
            this.Professions[0]));
        this.ProfessionsForLevels.Add(new ProfessionPair(10, this.Professions[4], this.Professions[5],
            this.Professions[1]));

        modHelper.Events.Display.MenuChanged += this.DisplayEvents_MenuChanged;
    }

    public override string GetName()
    {
        return this._modHelper.Translation.Get($"skill.{this.Id}.name");
    }

    public override List<string> GetExtraLevelUpInfo(int level)
    {
        return this.ExtraInfo is null ? [] : this.ExtraInfo(level);
    }

    public override string GetSkillPageHoverText(int level)
    {
        return this.HoverText is null ? "" : this.HoverText(level);
    }


    /// <summary>
    /// Do extra work to display recipes.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DisplayEvents_MenuChanged(object sender, MenuChangedEventArgs e)
    {
        if (e.NewMenu is not SkillLevelUpMenu levelUpMenu)
        {
            return;
        }

        string skill = this._modHelper.Reflection.GetField<string>(levelUpMenu, "currentSkill").GetValue();
        if (skill != this.Id)
        {
            return;
        }

        int level = this._modHelper.Reflection.GetField<int>(levelUpMenu, "currentLevel").GetValue();

        List<CraftingRecipe> newRecipes = [];

        int menuHeight = 0;
        foreach (KeyValuePair<string, string> recipePair in CraftingRecipe.craftingRecipes)
        {
            string conditions = ArgUtility.Get(recipePair.Value.Split('/'), 4, "");
            if (!conditions.Contains(skill) || !conditions.Contains(level.ToString()))
            {
                continue;
            }

            CraftingRecipe recipe = new(recipePair.Key, isCookingRecipe: false);
            newRecipes.Add(recipe);
            Game1.player.craftingRecipes.TryAdd(recipePair.Key, 0);
            menuHeight += recipe.bigCraftable ? 128 : 64;
        }

        foreach (KeyValuePair<string, string> recipePair in CraftingRecipe.cookingRecipes)
        {
            string conditions = ArgUtility.Get(recipePair.Value.Split('/'), 3, "");
            if (!conditions.Contains(skill) || !conditions.Contains(level.ToString()))
            {
                continue;
            }

            CraftingRecipe recipe = new(recipePair.Key, isCookingRecipe: true);
            newRecipes.Add(recipe);
            if (Game1.player.cookingRecipes.TryAdd(recipePair.Key, 0) &&
                !Game1.player.hasOrWillReceiveMail("robinKitchenLetter"))
            {
                Game1.mailbox.Add("robinKitchenLetter");
            }

            menuHeight += recipe.bigCraftable ? 128 : 64;
        }

        this._modHelper.Reflection.GetField<List<CraftingRecipe>>(levelUpMenu, "newCraftingRecipes")
            .SetValue(newRecipes);

        levelUpMenu.height = menuHeight + 256 + (levelUpMenu.getExtraInfoForLevel(skill, level).Count * 64 * 3 / 4);
    }
}
