namespace QuickStart
{
	/// <summary>
	/// This class contains the options available for this mod.
	/// </summary>
	public class ModOptions
	{
		#region COnstants

		public static readonly int DefaultCharcoal = 20;
		public static readonly int DefaultFiber = 50;
		public static readonly int DefaultWood = 100;
		public static readonly int DefaultStone = 100;
		public static readonly int DefaultMixedSeeds = 25;
		public static readonly int DefaultClay = 10;

		#endregion

		#region Constructors

		public ModOptions()
		{
			this.IncludeCharcoal = true;
			this.IncludeFiber = true;
			this.IncludeStone = true;
			this.IncludeWood = true;
			this.IncludeMixedSeeds = true;
			this.CharcoalCount = ModOptions.DefaultCharcoal;
			this.FiberCount = ModOptions.DefaultFiber;
			this.WoodCount = ModOptions.DefaultWood;
			this.StoneCount = ModOptions.DefaultStone;
			this.MixedSeedsCount = ModOptions.DefaultMixedSeeds;
			this.IncludeBonusChest = true;
			this.IncludeClay = true;
			this.ClayCount = ModOptions.DefaultClay;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets a value indicating whether stone is included.
		/// </summary>
		public bool IncludeStone
		{
			get; set;
		}

		/// <summary>
		/// The amount of stone to include.
		/// </summary>
		public int StoneCount
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether wood is included.
		/// </summary>
		public bool IncludeWood
		{
			get; set;
		}

		/// <summary>
		/// The amount of wood to include.
		/// </summary>
		public int WoodCount
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether stone is included.
		/// </summary>
		public bool IncludeFiber
		{
			get; set;
		}

		/// <summary>
		/// The amount of fiber to include.
		/// </summary>
		public int FiberCount
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether charcoal is included.
		/// </summary>
		public bool IncludeCharcoal
		{
			get; set;
		}

		/// <summary>
		/// The amount of charcoal to include by default.
		/// </summary>
		public int CharcoalCount
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether mixed seeds are included or not.
		/// </summary>
		public bool IncludeMixedSeeds
		{
			get; set;
		}

		/// <summary>
		/// The amount of mixed seeds to include.
		/// </summary>
		public int MixedSeedsCount
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether a bonus chest is included or not.
		/// </summary>
		public bool IncludeBonusChest
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether bonus money is added to the player.
		/// </summary>
		public bool IncludeBonusMoney
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether clay is included.
		/// </summary>
		public bool IncludeClay
		{

			get; set;
		}

		/// <summary>
		/// Gets or sets the amount of clay included in the inventory.
		/// </summary>
		public int ClayCount
		{
			get; set;
		}

		#endregion
	}
}