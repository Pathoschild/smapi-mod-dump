[b][size=6]Stardew Multiplayer Server Mod[/size][/b]

[size=4]This is a "Headless Server" mod allowing the host and thus the game world to
 remain online always, much the way Minecraft and other online games 
function. It accomplishes this by automating the Host's day-to-day 
activities. AKA a "Bot" that plays the game for you when you are away.[/size]
[size=6]

[/size][size=6][size=6]How to Install:[/size][/size][size=6]
[/size]
[list]
[*][size=3]Install SMAPI: [url=https://www.nexusmods.com/stardewvalley/mods/2400]https://www.nexusmods.com/stardewvalley/mods/2400[/url]﻿[/size]
[*][size=3]Place the "Stardew Multiplayer Server Mod" folder inside the "Mods" folder in your StardewValley game directory.[/size]
[*][size=3]Launch the game via "StardewModdingAPI.exe".[/size]
[/list]
[size=6]
How to Use:[/size]
   ﻿
[list]
[*]Change your settings to what you prefer in the config.json file in the "Stardew Multiplayer Server Mod" folder before you start the game.
[*][size=3]Host a multiplayer game like you normally would. The server mod will start automatically when the game starts.[/size]
[*][size=3]﻿[/size][size=3][size=3]Pres the "F9" button on your keyboard to turn server mode on/off. (For when you want to play the host yourself)[/size][/size]
[*][size=3][b]Warning![/b] - The server will hang on the save screen if the game is minimized in [/size]
[size=3]windows. It doesn't have to have focus, you can go do other things, but 
IT CAN NOT BE MINIMIZED! (Not sure about Mac/Linux)[/size][/list][size=6]
Settings[/size]

[list]
[*][size=3]Edit the "config.json" file in the "Stardew Multiplayer Server Mod" folder to set your options.[/size]
[/list][size=3]   ﻿
      ﻿﻿"serverHotKey": "F9",   ﻿   ﻿   ﻿   ﻿   ﻿    (which button to start/stop server)
   ﻿   ﻿"profitmargin" : 100,    ﻿   ﻿   ﻿              (Profit Margin in percent)   
   ﻿   ﻿"upgradeHouse": 3,                     ﻿(Upgrades the main house. 3 is max level, 0 will do nothing)   
[size=3]   ﻿   ﻿"petname": "Qwerty",   ﻿   ﻿   ﻿       ﻿(what is petname? must be 10 characters or less, can change at any time)
   ﻿   ﻿"farmcavechoicemushrooms": true,      [size=3][size=3]([size=3][size=3]make farm cave mushroom, false will be bats, choose before run)[/size][/size][/size][/size]   [size=3]      ﻿   [/size]
   ﻿   ﻿"communitycenterrun": true,   ﻿   ﻿         ﻿(host will attempt CC run,  false will do Joja run)[/size]
   ﻿   ﻿"timeOfDayToSleep": 2200,       ﻿         ﻿﻿(when does the host sleep in military time, up to 2600 for 2 A.M.)
   ﻿   ﻿"lockPlayerChests": true,   ﻿   ﻿              (while servermode is on, players can not access the chests or inventory in other
   ﻿   ﻿   ﻿   ﻿   ﻿   ﻿   ﻿   ﻿   ﻿   ﻿   ﻿   ﻿    player's cabins.)         
   ﻿   ﻿"clientsCanPause": false,[size=3]      ﻿   ﻿      ﻿(type !pause/!unpause into chat[/size])
   ﻿   ﻿"copyInviteCodeToClipboard": true,   ﻿   ﻿(copies current invite code to your computer's clipboard once a minute)
   ﻿   ﻿"festivalsOn": true,   ﻿   ﻿   ﻿   ﻿   ﻿   ﻿(host will go to festivals)
   ﻿   ﻿"<eventname>CountDownConfig": 60,      (how long in seconds from the start of the festival until it's main event)   ﻿   ﻿
   ﻿   ﻿"[size=3]<eventname>[/size]TimeOut": 120,   ﻿   ﻿   ﻿   (how long in seconds until the connection resets to prevent lockup from an AFK player.
   ﻿   ﻿   ﻿    ﻿   ﻿   ﻿   ﻿   ﻿   ﻿   ﻿         Most festivals start this timer after their final cutscene ends. Fair, Spiritseve, and 
   ﻿   ﻿   ﻿   ﻿   ﻿   ﻿   ﻿   ﻿   ﻿   ﻿          Winterstar start this timer as soon as the festival starts)
 [size=3][size=6]Chat Commands
[size=3]  Clients can type these into chat if the game gets stuck.[/size][/size]

[/size][list]
[*][size=3][u][b][/b][/u]!sleep - tries to trigger the sleep command if it's past the config sleep time (default 10PM)[/size]
[*][size=3]!festival - tries to trigger the go-to-festival command if it is a festival day[/size]
[*][size=3]!event - tries to trigger the command to start the main event of a festival[/size]
[*][size=3]!leave - will attempt to leave a festival [/size]
[*][size=3]﻿﻿!unstick - will warp the host inside/outside the farmhouse (which generally [/size]
[size=3]seems to get through any bugs. Sometimes it takes a few warps, 
especially for festivals.)[/size][/list]
   ﻿
[size=6]Accessing the Server[/size]

[/size][list]
[*][size=3][u][b]Friends List:[/b][/u] Your Steam or GoG friends should be able to see your game and join at will the same way they would normally.[/size]
[*][size=3][u][b]Direct IP:[/b][/u] If you want to set a up direct IP access over the internet "Join LAN Game" you need to open (or forward) port 24642. Or use [url=https://www.nexusmods.com/stardewvalley/mods/2993]Server Port Changer[/url]﻿ to choose a different port.Then give people your external IP.[/size]
[*][size=3][u][b]Invite Code[/b][/u]: Invite Code connections are routed through Steam/GoG . I've provided [/size]
[size=3]two methods for delivering your game's current invite code to players:[/size][*][size=3][list]
[*][size=3][b][u][size=3][b][u] Invite Code[/u][/b][/size] Auto Copy/Paste:[/u][/b] The server will copy the most up-to-date invite code to the clipboard[/size][size=3] (on by default in the config.json file) whenever it changes. You can [/size][size=3]then use a macro program of your choice to paste that code into the chat[/size][size=3] service of your choice so that your non-steam friends can always have [/size][size=3]access to the most up-to-date invite code even when you are not there. [/size][size=3]For your convenience I've included an [url=https://autohotkey.com/]AutoHotkey [/url]script under the [url=https://www.nexusmods.com/stardewvalley/mods/2677?tab=files]Files Tab> Optional Files[/url]﻿ here on Nexus that you can use. Run the Game Server>Run the [url=https://autohotkey.com/]AutoHotKey[/url]﻿ Script>Open Discord or other chat service and click into the chatbox[/size]
[size=3] of that service. The current invite code for your game will be pasted and sent every two minutes. Do not close the chat window of your chat service or click out of the chat box or it will not work. When the game 
server is turned off it will no longer copy the key so be sure to turn 
off AutoHotKey as well.[/size][*][size=3][u][b]Invite Code Bot[/b][/u]: ﻿[/size][size=3][size=3][size=3][size=3]The server will copy the invite code to an "InviteCode.txt" file in the same [/size][/size][/size][/size][size=3][size=3][size=3]folder as the mod. You can use this to make a bot for a chat [/size][/size][/size]
[size=3][size=3][size=3]service/website/etc. I've provided the code for a node.js Discord bot in
 the "Discord Bots" section at the bottom of this page.[/size][/size][/size][/list][/size][size=3][/size]



[/list][size=6]
Features
[/size]
[list]
[*][size=4]Only the host needs to install the mod[/size]
[/list][list]
[*][size=4]Host Teleports to bed and Auto-Sleeps every day (default 10P.M., can change [/size]
[size=4]in config.json) so that other players can keep playing without the host 
having to be at their computer. Other players can go to bed when they're
 ready and won't be stuck waiting on the host.[/size][*][size=4]Checks Mail every day[/size]
[*][size=4]Does host set up (cave/pet/house/etc)[/size]
[*][size=4]Locks client chests to everyone but cabin owner (optional)[/size]
[*][size=4]Clicks "OK" on the end of day Shipping Menu allowing the game to save and sync with the clients.[/size]
[*][size=4]Pauses the game when all clients disconnect (except on Holidays to prevent [/size]
[size=4]lock-up). Resumes when at least 1 client rejoins. This prevents the farm
 from falling into disrepair and years of game time passing with no one 
around.[/size][*][size=4][size=4]Freezes Relationship Heart Loss for Host while "server" is running[/size][/size]
[*][size=4][size=4]Full Festival Support! - Events like Egg Hunt, Ice Fishing, etc begin 1 [/size][/size]
[size=4][size=4]minute after festival starts so players have some time to look around. 
Host will not go to sleep until after Festival time window is over. 
Clients can choose to go to festival or not, server will still work. All
 clients must go to festival to trigger it to start just like normal 
game-play.[/size][/size][*][size=4][size=4]Full Event Support! - Grampa Evaluation, Kent arrival, all cutscenes, etc etc[/size][/size]
[size=4][size=4] will all click through if they pop-up allowing the server to stay 
online and not get locked up.[/size][/size][*][size=4][size=4]Festival and Event Warnings sent to clients via chat.[/size][/size]
[*][size=4][size=4]Times Out and resets the server connection to prevent an AFK'er or the game's[/size][/size]
[size=4][size=4] own connection bugs from locking up the game indefinitely(A 2 minute 
warning is sent out in chat before the end of the appropriate festivals 
to let everyone know to exit or they will be kicked). [/size][/size][*][size=4][size=4]Turns off "Pause When Game Window Is Inactive" so you no longer have to.[/size][/size]
[*][size=4][size=4]Config file for various settings[/size][/size]
[*][size=4][size=4]Copies most up-to-date Invite Code to clipboard[/size][/size]
[*][size=4][size=4]Copies most up-to-date Invite Code to "InviteCode.txt" file inside the "Stardew Multiplayer Server Mod" mod folder[/size][/size]
[*][size=4][size=4][size=3]Joja run will buy upgrades going from lowest cost to highest every time you have double the cost saved up. E.G. Joja[/size][/size][/size][size=4][size=4][size=3] Minecarts cost 15,000g so it will buy them when you have 30,000g[/size][/size][/size]
[/list][size=5][size=6]

[/size][/size]