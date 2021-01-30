/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AairTheGreat/StardewValleyMods
**
*************************************************/

### Overview of DataFiles Folder

Each game location/area can be configured to setup where panning spots can show up.  
There is a limit on where spots can should up, so you can't just have a spot in the middle of land.  
The files are used to finetune each area to limit unreachable panning spots being created.  
Each area can be configured to limit the number of panning spots are gathered and the start and end times the mod will create panning spots. 

The ore spots is formated as a list of X and Y coordinates.

The file is formated like: 
{
  "FileVersion": 1,
  "AreaName": "Caldera",
  "NumberOfOreSpotsPerDay": 50,
  "StartTime": 600,
  "EndTime": 2600,
  "CustomTreasure": false,
  "OreSpots": [
    "5, 19",
    "5, 23",
    "5, 24",
    "5, 25",
    ]
}

Notes: 
- CustomTreasure is a placeholder for future work.
- If the config.json maxNumberOfOrePointsGathered value is lower than what is in the area file, then it will use the config.json value.

If a game location file is deleted then the next time a player goes to the that area the file will be recreated. 

For example, if you are using a custom area like a Big Coop with a fishing pond, then you will need to delete the Big Coop.json 
and next time Stardew Valley is started then panning spots might show up in the modded Big Coop.  

It does depend on a few things like if the water is considered open water or not, but the mod scans for valid panning spots if there is no data file. 
