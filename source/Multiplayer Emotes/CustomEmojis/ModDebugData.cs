
using CustomEmojis.Framework.Utilities;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ModDebugData {
	
	public bool IsHost { get; set; }

	public ModDebugData() {

		IsHost = false;

	}

	public bool ActAsHost() {
		IsHost = !IsHost;
		return IsHost;
	}

}
