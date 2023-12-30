/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/GiftWrapper
**
*************************************************/

using Microsoft.Xna.Framework;
using Colour = Microsoft.Xna.Framework.Color;

namespace GiftWrapper.Data
{
	public record Definitions
	{
		public float AddedFriendship;
		public int GiftValue;
		public int WrapValue;
		public int[] HitCount;
		public int HitShake;
		public string HitSound;
		public string LastHitSound;
		public string OpenSound;
		public string RemoveSound;
		public string InvalidGiftStringPath;
		public string InvalidGiftSound;
		public int EventConditionId;
		public int CategoryNumber;
		public Colour CategoryTextColour;
		public Colour SecretTextColour;
		public Colour? DefaultTextColour;
		public string WrapItemTexture;
		public Rectangle WrapItemSource;
	}
}
