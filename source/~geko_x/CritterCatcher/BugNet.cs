/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/geko_x/stardew-mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewValley.Objects;
using SObject = StardewValley.Object;
using StardewValley.Tools;
using StardewValley;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Netcode;


namespace CritterCatcher {
	//class BugNet : PlatoTool<GenericTool> {

	//	public static int TileIndex { get; set; }
	//	public bool InUse = false;
	//	public static Texture2D Texture;

	//	public BugNet(): base() {

	//	}

	//	internal static void LoadTextures(IPlatoHelper helper) {
	//		if (Texture == null) {
	//			Texture = helper.ModHelper.Content.Load<Texture2D>(@"assets/bugnet2.png");
	//		}
	//	}

	//	public override string DisplayName {
	//		get => Helper.ModHelper.Translation.Get("name_bugnet");
	//		set {

	//		}
	//	}

	//	public override string Name {
	//		get {
	//			return DisplayName;
	//		}
	//		set {

	//		}
	//	}

	//	public override string getDescription() {
	//		return Helper.ModHelper.Translation.Get("desc_bugnet");
	//	}

	//	public static Tool GetNew(IPlatoHelper helper) {
	//		Tool newTool = new GenericTool();

	//		newTool.initialParentTileIndex.Value = TileIndex;
	//		newTool.currentParentTileIndex.Value = TileIndex;
	//		newTool.instantUse.Value = false;
	//		newTool.indexOfMenuItemView.Value = TileIndex;
	//		newTool.netName.Set("Geko_X:CritterCatcher:Bugnet");

	//		return newTool;
	//	}

	//	public override Item getOne() {
	//		throw new NotImplementedException();
	//	}

	//	protected override string loadDescription() {
	//		return getDescription();
	//	}

	//	protected override string loadDisplayName() {
	//		return DisplayName;
	//	}

	//	public override bool CanLinkWith(object linkedObject) {
	//		return false;
	//	}

	//	public override NetString GetDataLink(object linkedObject) {
	//		if (linkedObject is Tool t)
	//			return t.netName;
	//		return null;
	//	}
	//}
}
