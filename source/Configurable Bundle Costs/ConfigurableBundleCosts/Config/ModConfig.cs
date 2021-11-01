/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/dtomlinson-ga/ConfigurableBundleCosts
**
*************************************************/

// Copyright (C) 2021 Vertigon
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://www.gnu.org/licenses/.


using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfigurableBundleCosts
{
	/// <summary>
	/// Utilized by SMAPI to store configurable values. Can be modified by hand or by use of GMCM.
	/// </summary>
	public class ModConfig
	{
		public JojaConfig Joja = new();
		public VaultConfig Vault = new();

		public class JojaConfig : SubConfig
		{
			public int membershipCost = 5000;
			public int busCost = 40000;         // button 0
			public int minecartsCost = 15000;   // button 1
			public int bridgeCost = 25000;      // button 2
			public int greenhouseCost = 35000;  // button 3
			public int panningCost = 20000;     // button 4
			public int movieTheaterCost = 500000;
		}

		public class VaultConfig : SubConfig
		{
			public int bundle1 = 2500;
			public int bundle2 = 5000;
			public int bundle3 = 10000;
			public int bundle4 = 25000;
		}

		public class SubConfig
		{
			public bool applyValues = true;
		}

		public override string ToString()
		{
			return $"\nJoja values applied: {Joja.applyValues}" +
				$"\nMembership cost: {Joja.membershipCost}" +
				$"\nBus Cost: {Joja.busCost}" +
				$"\nMinecarts Cost: {Joja.minecartsCost}" +
				$"\nBridge Cost: {Joja.bridgeCost}" +
				$"\nGreenhouse Cost: {Joja.greenhouseCost}" +
				$"\nPanning Cost: {Joja.panningCost}" +
				$"\nMovie Theater Cost: {Joja.movieTheaterCost}" +
				$"\n\nVault values applied: {Vault.applyValues}" +
				$"\nBundle 1 Cost: {Vault.bundle1}" +
				$"\nBundle 2 Cost: {Vault.bundle2}" +
				$"\nBundle 3 Cost: {Vault.bundle3}" +
				$"\nBundle 4 Cost: {Vault.bundle4}";
		}

		public static FieldInfo GetMatchingField(string fieldName)
		{
			List<FieldInfo> jojaFields = typeof(JojaConfig).GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
			List<FieldInfo> vaultFields = typeof(VaultConfig).GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();

			if (jojaFields.Any(x => x.Name.ToLower() == fieldName))
			{
				return jojaFields.Find(x => x.Name.ToLower() == fieldName);
			}

			if (vaultFields.Any(x => x.Name == fieldName))
			{
				return vaultFields.Find(x => x.Name.ToLower() == fieldName);
			}

			return null;
		}

		public static void SaveConfigData()
		{
			Globals.Helper.Data.WriteSaveData("Saved_Config", Globals.CurrentValues);
		}
	}

	public class ContentPackConfig
	{
		public JojaConfig Joja = new();
		public VaultConfig Vault = new();

		public class JojaConfig : SubConfig
		{
			public int? membershipCost = null;
			public int? busCost = null;         // button 0
			public int? minecartsCost = null;   // button 1
			public int? bridgeCost = null;      // button 2
			public int? greenhouseCost = null;  // button 3
			public int? panningCost = null;     // button 4
			public int? movieTheaterCost = null;
		}

		public class VaultConfig : SubConfig
		{
			public int? bundle1 = null;
			public int? bundle2 = null;
			public int? bundle3 = null;
			public int? bundle4 = null;
		}

		public static FieldInfo GetMatchingField(string fieldName)
		{
			List<FieldInfo> jojaFields = typeof(JojaConfig).GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
			List<FieldInfo> vaultFields = typeof(VaultConfig).GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();

			if (jojaFields.Any(x => x.Name.ToLower() == fieldName))
			{
				return jojaFields.Find(x => x.Name.ToLower() == fieldName);
			}

			if (vaultFields.Any(x => x.Name == fieldName))
			{
				return vaultFields.Find(x => x.Name.ToLower() == fieldName);
			}

			return null;
		}

		public class SubConfig
		{
			public bool? applyValues = null;
		}
	}
}