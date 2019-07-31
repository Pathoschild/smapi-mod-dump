### Overview of DataFiles Folder

Each game location/area can be configured to setup where panning spots can show up.  
There is a limit on where spots can should up, so you can't just have a spot in the middle of land.  
The files are used to finetune each area to limit unreachable panning spots being created.  
The file is formated as a list of X and Y coordinates like:
[
  "9, 36",
  "9, 37"
]

If a game location file is deleted then the next time a player goes to the that area the file will be recreated. 

For example, if you are using a custom area like a Big Coop with a fishing pond, then you will need to delete the Big Coop.json and next time Stardew Valley is started then panning spots might show up in the modded Big Coop.  It does depend on a few things like if the water is considered open water or not, but the mod scans for valid panning spots if there is no data file. 
