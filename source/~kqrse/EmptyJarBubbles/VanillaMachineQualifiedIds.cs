/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods
**
*************************************************/

namespace EmptyJarBubbles; 

// List of qualified IDs for all machines in Machines/Data
// that have Trigger: "ItemPlacedInMachine", except for the crab pot.

public struct VanillaMachineQualifiedIds {
    public const string BoneMill = "(BC)90";
    public const string Cask = "(BC)163";
    public const string CharcoalKiln = "(BC)114";
    public const string Loom = "(BC)17";
    public const string Furnace = "(BC)13";
    public const string Deconstructor = "(BC)265";
    public const string Keg = "(BC)12";
    public const string Jar = "(BC)15";
    public const string CheesePress =  "(BC)16";
    public const string RecyclingMachine = "(BC)20";
    public const string MayonnaiseMachine = "(BC)24";
    public const string OilMaker = "(BC)19";
    public const string Incubator = "(BC)101";
    public const string OstrichIncubator = "(BC)254";
    public const string SlimeIncubator = "(BC)156";
    public const string SlimeEggPress = "(BC)158";
    public const string Crystalarium = "(BC)21";
    public const string SeedMaker = "(BC)25";
    public const string GeodeCrusher = "(BC)182";
    public const string WoodChipper = "(BC)211";
    public const string BaitMaker = "(BC)BaitMaker";
    public const string Dehydrator = "(BC)Dehydrator";
    public const string HeavyFurnace = "(BC)HeavyFurnace";
    public const string Anvil = "(BC)Anvil";
    public const string FishSmoker = "(BC)FishSmoker";
    public const string CrabPot = "(O)710";

    public static List<string> AsList() {
        return new List<string> {
            BoneMill,
            Cask,
            CharcoalKiln,
            Loom,
            Furnace,
            Deconstructor,
            Keg,
            Jar,
            CheesePress,
            RecyclingMachine,
            MayonnaiseMachine,
            OilMaker,
            Incubator,
            OstrichIncubator,
            SlimeIncubator,
            SlimeEggPress,
            Crystalarium,
            SeedMaker,
            GeodeCrusher,
            WoodChipper,
            BaitMaker,
            Dehydrator,
            HeavyFurnace,
            Anvil,
            FishSmoker,
            CrabPot
        };
    }
}