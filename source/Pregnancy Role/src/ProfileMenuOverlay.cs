using StardewValley;
using StardewValley.Menus;

namespace PregnancyRole
{
	internal class ProfileMenuOverlay : DropdownOverlay
	{
		public ProfileMenuOverlay ()
		{
			// Align the label and dropdown below the favorite items.
			int xOffset = 64 - 12 + 400 + 125 + (IsAndroid ? 150 : 200);
			int yOffset = IClickableMenu.borderWidth + 32 + Game1.daybg.Height +
				32 + 96 + 56 + 48 + (IsAndroid ? -32 : 64 + 48);
			setOffset (xOffset, yOffset);
		}

		protected override int roleIndex
		{
			get
			{
				return (int) Model.GetPregnancyRole (getTarget ());
			}
			set
			{
				Model.SetPregnancyRole (getTarget (), (Role) value);
			}
		}

		protected override bool shouldRender
		{
			get
			{
				NPC npc = getTarget ();
				if (npc == null || npc.getSpouse () != Game1.player ||
						!Model.BaseGameNPCs.Contains (npc.Name))
					return false;
				
				return Helper.Reflection.GetField<int>
					(Game1.activeClickableMenu, "_currentCategory", false)
					?.GetValue () == 0; // "<Name>'s Favorites" page
			}
		}

		private NPC getTarget ()
		{
			if (!(Game1.activeClickableMenu is ProfileMenu pm))
				return null;
			return pm.GetCharacter () as NPC;
		}
	}
}
