**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/ylsama/RightClickMoveMode**

----

# RightClickMoveMode
  A simple mod for Stadrew valley that player can use right click to move
  
*Feature:*
- Right-click or Holding right mouse  will make player move 
- Right-click too far from player will no longer perform any action 

- Press *G* for On/Off the Right mouse move mode (auto On)
  
# Extended mouse function mode
  Add some function that should come along with the game  
  
*Feature:*
- Add Ctrl + Wheel to Zoom in and Zoom out 
- Add Right Alt + Enter to change Window mode to Full screen mode (Left alt + Enter to fullscreen is already the game feature) 

- Press *H* for On/off Extended mouse function mode (auto On)
  
# Build

- Install NET 5.0 (Need manual download)
- Currently using VS2022, VS2019 to build with current source have no problem

> I have to create new project using VS2019 Community for NET 5.0 support
  
# Version
## Current : Update project sln to NET 5.0 and Harmony 2
-  After a while, I found out that I can't even complie the code any more
-  Refactor some of my code, but mainly change still forcus on library change

## 1.1 : Source code release version
-  Extended mouse function mode added (press *H* to On/Off, auto On when open)
-  Organize code for futher update

## Pre source code release
   This not come with the souce code, but you can have \*.dll file from https://www.nexusmods.com/stardewvalley/mods/2614 along with Zip file

### 1.0.2
- Fix bug where you can't uses right-mouse nomally (eg: Can't split item in inventory, etc...) 
- Slightly smooth movement
- Change right-click interaction when clicking to far from player

### 1.0.1
- Turn off debug mode

### 1.0
- Out pre-release

### 0.1
- Pre-release

