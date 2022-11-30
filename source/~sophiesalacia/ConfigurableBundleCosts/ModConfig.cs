/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

namespace ConfigurableBundleCosts;

class ModConfig
{
    public JojaConfig Joja = new();
    public VaultConfig Vault = new();

    public class JojaConfig
    {
        public bool ApplyValues = true;
        public int MembershipCost = 5000;
        public int BusCost = 40000;             // button 0
        public int MinecartsCost = 15000;       // button 1
        public int BridgeCost = 25000;          // button 2
        public int GreenhouseCost = 35000;      // button 3
        public int PanningCost = 20000;         // button 4
        public int MovieTheaterCost = 500000;
    }

    public class VaultConfig
    {
        public bool ApplyValues = true;
        public int Bundle1 = 2500;
        public int Bundle2 = 5000;
        public int Bundle3 = 10000;
        public int Bundle4 = 25000;
    }
    
    public override string ToString()
    {
        return $"\nJoja values applied: {Joja.ApplyValues}" +
               $"\nMembership cost: {Joja.MembershipCost}" +
               $"\nBus Cost: {Joja.BusCost}" +
               $"\nMinecarts Cost: {Joja.MinecartsCost}" +
               $"\nBridge Cost: {Joja.BridgeCost}" +
               $"\nGreenhouse Cost: {Joja.GreenhouseCost}" +
               $"\nPanning Cost: {Joja.PanningCost}" +
               $"\nMovie Theater Cost: {Joja.MovieTheaterCost}" +
               $"\n\nVault values applied: {Vault.ApplyValues}" +
               $"\nBundle 1 Cost: {Vault.Bundle1}" +
               $"\nBundle 2 Cost: {Vault.Bundle2}" +
               $"\nBundle 3 Cost: {Vault.Bundle3}" +
               $"\nBundle 4 Cost: {Vault.Bundle4}";
    }
}
