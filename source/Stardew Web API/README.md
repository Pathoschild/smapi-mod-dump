**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/zunderscore/StardewWebApi**

----

# Stardew Valley Web API

A mod for Stardew Valley that provides access to some game basic information, actions, and events via an HTTP interface listening at `http://localhost:7882/`.

It's my first game mod, so be gentle.

## Items

### Item Info

- `/api/v1/items`: A basic list of all items registered in the game
- `/api/v1/items/id/{itemId}`: Get an item by its fully qualified ID (e.g. `(F)1365`)
- `/api/v1/items/type/{itemType}`: A basic list of all items of the given type (e.g. `F` for furniture)
- `/api/v1/items/type/{itemType}/id/{itemId}`: Get an item by its type and individual ID (e.g. `F` for furniture type and `1365` for individual ID)

## Mods

### Mod Info

- `/api/v1/mods`: Gets a list of all installed mods
- `/api/v1/mods/{modId}`: Get info on a specific mod by its unique mod ID (e.g. `zunderscore.StardewWebApi`)

## NPCs

### NPC Info

- `/api/v1/npcs`: Gets a basic list of all named NPCs
- `/api/v1/npcs/name/{name}`: Gets a specific NPC by name
- `/api/v1/npcs/birthday/{season}/{day}`: Gets a list of all NPCs whose birthday is on the specified day of the specified season
- `/api/v1/npcs/pets`: Gets a list of all pet NPCs

## Player

### Player Info

- `/api/v1/player`: Information about the current player

### Player Actions

- `/api/v1/player/actions/refillEnergy`: Fully refills the player's energy/stamina
- `/api/v1/player/actions/passOut`: Fully drains the player's energy/stamina, causing them to pass out
- `/api/v1/player/actions/fullyHeal`: Fully refills the player's health
- `/api/v1/player/actions/knockOut`: Fully drains the player's health, causing them to be knocked out/die
- `/api/v1/player/actions/giveMoney/{amount}`: Gives money to/takes money away from the player
- `/api/v1/player/actions/giveItem/name/{itemName}`: Attempts to add the specified item to the player's inventory, using the item's display name
- `/api/v1/player/actions/giveItem/id/{itemId}`: Same as above, but using the item's fully qualified ID (e.g. `(F)1365`) instead
- `/api/v1/player/actions/warp/{location}`: Warps the player to the specified location
- `/api/v1/player/actions/petFarmAnimal/{name}`: Pets the named farm animal (i.e. chickens, cows, etc., but not pets like dogs, cats, or turtles)

## UI

### UI Actions

- `/api/v1/ui/actions/showHudMessage`: Show a HUD message in the bottom left corner
- `/api/v1/ui/actions/showHudMesssage/item`: Show a HUD message with an item icon
- `/api/v1/ui/actions/showHudMessage/large`: Show a larger HUD message with text wrapping and no icon box
- `/api/v1/ui/actions/showChatMessage`: Show a chat message

## World

### World Info

- `/api/v1/world`: Information about the world, including in-game date and farm weather

### World Actions

- `/api/v1/world/actions/playSound`: Plays a sound in game

## Events

Opening a WebSocket connection to `/events` will allow another application to listen for some basic in-game events.

When events occur, all connected WebSocket connections are sent a broadcast message containing an event object. This object has two possible properties:
- `event`: The name of the event, as listed below
- `data`: An optional property that includes any relevant data about the event (e.g. `TimeChanged` will send values including `oldTime` and `newTime`)

### WebSocket Events

- `Connected`: When an application connects to the `/events` endpoint. This event is NOT broadcast to all connections, but rather sent ONLY to the current connection.

### Game Loop Events

- `SaveLoaded`: The player loads a save
- `Saved`: The player saves the game
- `ReturnedToTitle`: The player returns to the title screen
- `DayStarted`: A new day has started
- `DayEnding`: The current day is ending
- `TimeChanged`: The time has changed (e.g. from 6:00 AM to 6:10 AM)
- `PlayerInventoryChanged`: The player's inventory has changed in any way
- `PlayerLevelChanged`: The level of one of the player's skills has changed 
- `PlayerWarped`: The player has warped to a different map location, including normal travel between areas (e.g. walking from the farm to the bus stop)

### Relationship Events

- `FriendshipIncreased`: The player's friendship with an NPC has increased
- `MultipleFriendshipsIncreased`: The player's friendship with multiple NPCs has increased
- `FriendshipDecreased`: The player's friendship with an NPC has decreased
- `MultipleFriendshipsDecreased`: The player's friendship with multiple NPCs has decreased
- `PlayerStartedDating`: The player has started dating an NPC
- `PlayerStoppedDating`: The player is no longer dating an NPC
- `PlayerEngaged`: The player has become engaged to an NPC
- `PlayerNoLongerEngaged`: The player is no longer engaged to an NPC
- `PlayerMarried`: The player has married an NPC
- `PlayerDivorced`: The player has divorced an NPC

### Festival Events

- `FestivalStarted`: A festival has started
- `FestivalEnded`: A festival has ended
