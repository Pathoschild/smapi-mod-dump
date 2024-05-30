#!/bin/bash

refasmer -v -c -p -O "." "/home/towk/.local/share/Steam/steamapps/common/Stardew Valley/MonoGame.Framework.dll"
refasmer -v -c -p -O "." "/home/towk/.local/share/Steam/steamapps/common/Stardew Valley/Stardew Valley.dll"
refasmer -v -c -p -O "." "/home/towk/.local/share/Steam/steamapps/common/Stardew Valley/StardewModdingAPI.dll"
refasmer -v -c -p -O "." "/home/towk/.local/share/Steam/steamapps/common/Stardew Valley/StardewValley.GameData.dll"
refasmer -v -c -p -O "." "/home/towk/.local/share/Steam/steamapps/common/Stardew Valley/xTile.dll"
refasmer -v -c -p -O "./smapi-internal" "/home/towk/.local/share/Steam/steamapps/common/Stardew Valley/smapi-internal/SMAPI.Toolkit.CoreInterfaces.dll"
cp "/home/towk/.local/share/Steam/steamapps/common/Stardew Valley/smapi-internal/SMAPI.Toolkit.CoreInterfaces.xml" "./smapi-internal"
refasmer -v -c -p -O "./Mods/ProjectFluent" "/home/towk/.local/share/Steam/steamapps/common/Stardew Valley/Mods/ProjectFluent/Linguini.Bundle.dll"
refasmer -v -c -p -O "./Mods/ProjectFluent" "/home/towk/.local/share/Steam/steamapps/common/Stardew Valley/Mods/ProjectFluent/Linguini.Shared.dll"
refasmer -v -c -p -O "./Mods/ProjectFluent" "/home/towk/.local/share/Steam/steamapps/common/Stardew Valley/Mods/ProjectFluent/Linguini.Syntax.dll"
refasmer -v -c -p -O "./Mods/ProjectFluent" "/home/towk/.local/share/Steam/steamapps/common/Stardew Valley/Mods/ProjectFluent/ProjectFluent.dll"
