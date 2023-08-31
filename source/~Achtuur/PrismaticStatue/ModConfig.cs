/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Integrations;
using PrismaticStatue.Utility;
using StardewModdingAPI;
using System;
using System.Collections.Generic;

namespace PrismaticStatue;


internal class ModConfig
{
    private const int MaxAllowedStatues = 10;

    /// <summary>
    /// Factor for slow option, after 5 statues a 50% speedup is gained.
    /// </summary>
    private static readonly float SlowFactor = (float)Math.Pow(0.5f, 1f / 5f);

    /// <summary>
    /// Factor for medium option, after 3 statues a 50% speedup is gained.
    /// </summary>
    private static readonly float MediumFactor = (float)Math.Pow(0.5f, 1f / 3f);

    /// <summary>
    /// Factor for fast option, after 2 statues a 50% speedup is gained.
    /// </summary>
    private static readonly float FastFactor = (float)Math.Pow(0.5f, 1f / 2f);

    /// <summary>
    /// Maximum number of statues until no more speedup is provided, defaults to 5
    /// </summary>
    public int MaxStatues { get; set; }

    /// <summary>
    /// Factor that is applied to speedup formula, lower values = less diminishing returns. Defaults to 5.
    /// </summary>
    public float StatueSpeedupFactor { get; set; }

    public SButton OverlayButton { get; set; }

    private int TableTime { get; set; }

    public ModConfig()
    {
        this.MaxStatues = 5;
        this.StatueSpeedupFactor = MediumFactor;
        this.TableTime = 300;
        OverlayButton = SButton.L;
    }

    /// <summary>
    /// Constructs config menu for GenericConfigMenu mod
    /// </summary>
    /// <param name="instance"></param>
    public void createMenu()
    {
        // get Generic Mod Config Menu's API (if it's installed)
        var configMenu = ModEntry.Instance.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null)
            return;

        // register mod
        configMenu.Register(
            mod: ModEntry.Instance.ModManifest,
            reset: () => ModEntry.Instance.Config = new ModConfig(),
            save: () =>
            {
                ModEntry.Instance.Helper.WriteConfig(ModEntry.Instance.Config);
                // Re-register
                configMenu.Unregister(ModEntry.Instance.ModManifest);
                ModEntry.Instance.Config.createMenu();
            }
        );
        /// General settings header
        configMenu.AddSectionTitle(
            mod: ModEntry.Instance.ModManifest,
            text: I18n.CfgSection_General,
            tooltip: null
        );

        configMenu.AddKeybind(
           mod: ModEntry.Instance.ModManifest,
           name: I18n.CfgOverlaybutton_Name,
           tooltip: I18n.CfgOverlaybutton_Desc,
           getValue: () => this.OverlayButton,
           setValue: value => this.OverlayButton = value
        );

        /// max statue
        configMenu.AddNumberOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgMaxstatues_Name,
            tooltip: I18n.CfgMaxstatues_Desc,
            min: 1,
            max: MaxAllowedStatues,
            interval: 1,
            getValue: () => this.MaxStatues,
            setValue: val => this.MaxStatues = val
        );

        /// statue factor
        configMenu.AddTextOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgStatuespeedupfactor_Name,
            tooltip: I18n.CfgStatuespeedupfactor_Desc,
            getValue: () => this.StatueSpeedupFactor.ToString(),
            setValue: val => this.StatueSpeedupFactor = float.Parse(val),
            allowedValues: new string[] { SlowFactor.ToString(), MediumFactor.ToString(), FastFactor.ToString() },
            formatAllowedValue: DisplayStatueFactorValues
        );


        /// Speedup table section
        configMenu.AddSectionTitle(
            mod: ModEntry.Instance.ModManifest,
            text: I18n.CfgSection_Speeduptable,
            tooltip: I18n.CfgSection_Speeduptable_Desc
        );

        /// test time
        configMenu.AddTextOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgTabletest_Name,
            tooltip: I18n.CfgTabletest_Desc,
            getValue: () => this.TableTime.ToString(),
            setValue: val => this.TableTime = int.Parse(val),
            allowedValues: new string[] { "30", "120", "200", "300", "480", "540", "4000", "6000", "8000", "10000" },
            formatAllowedValue: DisplayTableTimeValues
        );


        // Get table values
        List<string> statue_table_times = new List<string>();
        for (int i = 0; i <= this.MaxStatues; i++)
        {
            int minutes_left = SpedUpMachineWrapper.SpeedUpFunction(this.TableTime, i);
            float speedup_percentage = (float)Math.Round(1.0 + this.TableTime / minutes_left, 2);
            statue_table_times.Add($"{Formatter.FormatNStatues(i)}: {Formatter.FormatMinutes(minutes_left)} ({speedup_percentage}x faster)");
        }


        // Create paragraphs for table values
        foreach (string entry in statue_table_times)
        {
            configMenu.AddParagraph(
                mod: ModEntry.Instance.ModManifest,
                text: () => entry
            );
        }

    }

    private static string DisplayStatueFactorValues(string expgain_option)
    {
        if (expgain_option == FastFactor.ToString())
            return "Fast";
        if (expgain_option == MediumFactor.ToString())
            return "Normal";
        if (expgain_option == SlowFactor.ToString())
            return "Slow";

        return "Something went wrong... :(";
    }

    private static string DisplayTableTimeValues(string tabletime_option)
    {
        string item_name;
        switch (tabletime_option)
        {
            case "30":
                item_name = "Copper Bar";
                break;
            case "120":
                item_name = "Iron Bar";
                break;
            case "200":
                item_name = "Cheese";
                break;
            case "300":
                item_name = "Gold Bar";
                break;
            case "480":
                item_name = "Iridum Bar";
                break;
            case "540":
                item_name = "Radioactive Bar";
                break;
            case "4000":
                item_name = "Jelly/Pickles/Aged Roe";
                break;
            case "6000":
                item_name = "Juice (keg)";
                break;
            case "8000":
                item_name = "Crystalarium (Diamond)";
                break;
            case "10000":
                item_name = "Wine (keg)";
                break;
            default:
                item_name = "Copper Bar";
                break;
        }

        return $"{item_name} ({Formatter.FormatMinutes(int.Parse(tabletime_option))})";
    }

    public static string DisplayAsPercentage(float value)
    {
        return Math.Round(100f * value, 2).ToString() + "%";
    }
}


