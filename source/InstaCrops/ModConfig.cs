/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Brian-Wuest/Instacrops
**
*************************************************/

namespace InstaCrops
{
	/// <summary>
	/// This class contains the mod configuration values.
	/// </summary>
	public class ModConfig
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ModConfig"/> class.
		/// </summary>
		public ModConfig()
		{
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the percentage chance of growth for a crop.
		/// </summary>
		public int ChanceForGrowth { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether to use the random chance processing.
		/// </summary>
		public bool UseRandomChance { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Validates the configuration options.
		/// </summary>
		public void ValidateConfigOptions()
		{
			if (this.ChanceForGrowth < 0)
			{
				this.ChanceForGrowth = 0;
			}
			else if (this.ChanceForGrowth > 100)
			{
				this.ChanceForGrowth = 100;
			}
		}

		#endregion
	}
}
