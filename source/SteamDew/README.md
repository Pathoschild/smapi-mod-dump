**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/myuusubi/SteamDew**

----

SteamDew
===
A rewrite of Stardew Valley's networking backend using Steam Networking Sockets,
injected with SMAPI & Harmony.

Background
---

#### Why was SteamDew made?

While taking a break from coding our game, Mewnition, my friends and I would hop
on our Stardew Valley farm to relax. We experienced lots of network issues and a
quick search through Stardew Valley's Discord & subreddit showed we were not the
only ones.

These problems include:
- Frequent disconnects (~2 disconnects per hour)
- Rubber-banding of players, villagers, and cave enemies
- Items being unable to be picked up for several seconds

SteamDew was created to address these issues. Due to the limitations of what can
be done in a mod alone, our fix only applies to players on Steam. Still, this is
a good proof-of-concept of what can be done, as we have not experienced a single
disconnect with SteamDew, even after several hours of consecutive play.

#### Steam Networking Sockets

The Steam Networking Sockets library is the modern API that Steam developers are
suggested to use in their code. It uses the Steam Datagram Relay (SDR) which can
mask player's IP address when they connect to each other. Also, it is connection
oriented, which means that our code does not need to manually send heartbeats or
check for timeouts. This library is incredibly robust, and we leverage its power
for SteamDew's stability.

The original Stardew Valley code uses Galaxy's P2P Networking (yes, even for all
Steam players). This allows GoG Galaxy players to be in-game with Steam players,
but at the cost of degrading the experience for Steam only players. We have some
ideas of how to keep cross-platform play working, while also increasing network
stability. However, this is not a high priority for me, as I do not have a copy
of the game on GoG. That said, I would love to work with ConcernedApe to improve
multiplayer stability if given the opportunity!

#### LZ4 Compression

SteamDew uses pre-built LZ4 binaries from the Lightweight Java Game Library for
compressing large messages. This reduces the overall bandwidth requirements for
the game, and fixes some multiplayer issues that occur once the save files grow
larger. This can especially help when using mods that add even more data to the
save files.

In our tests, we've found the LZ4 can compress larger, 1KB+ messages to only be
25% - 50% of their original sizes. This saves considerable bandwidth, but since
LZ4 has to be loaded from an unmanaged dynamic library, it's blocked by default
when on MacOS. This can be solved by adding LZ4 to Stardew Valley's app bundle,
but that would require official integration into the game.

#### Networking Backend

Most of the networking backend implementation can be found in `SteamDew/SDKs`. I
tried to keep modding specific logic out of that folder, since they are designed
to be drop-in replacements for the original Stardew Valley networking code. That
said, once we added in the logic for allowing connections to/from Vanilla games,
it was impossible to keep it completely clean.

Here's a breakdown:

- `InviteClient.cs`: A fake client that handles Galaxy server invite codes. This
client gets the data for the Galaxy Lobby, and determines if it has the SteamDew
connection key. It then notifies our patch of `StardewValley.Menus.FarmhandMenu`
to inject the correct client.
- `SteamDewClient.cs`: The rewrite of `GalaxyNetClient` specifically for using a
Steam-based backend. It uses Steam Lobbies instead of Galaxy Lobbies, as well as
Steam's Networking Sockets
- `SteamDewServer.cs`: The rewrite of `GalaxyNetServer` for Steam. This uses the
functionality of Steam's Networking Sockets the most, taking advantage of modern
features like poll groups to isolate joining players from those who are playing.
- `SteamDewNetHelper.cs`: This is a drop-in replacement for `SteamNetHelper`, as
it actually uses Steam Lobbies directly, and instantiated our Client and Server.
Unlike `SteamNetHelper`, it does not inherit from `GalaxyNetHelper`. However, it
does use the GalaxyInstance to get the Galaxy ID, so that players can still use
their save files that used Galaxy IDs.
- `SteamDewNetUtils.cs`: These are some utility functions used by our client and
server implementations. They convert Stardew Valley's OutgoingMesssage into raw
memory that can be passed to the Steam Networking Sockets, and convert received
messages to Stardew Valley's IncomingMessage format.

#### Decoys

SMAPI's network code works by extending the Galaxy client/server, and adding its
own handlers that intercept the messages before they are sent/received. This was
problematic, since SMAPI's `InitClient(...)` and `InitServer(...)` methods could
not recognize our custom SteamDew client & server.

The decoys in `SteamDew/SDKs/Decoys` all extend the Galaxy-based client/servers,
allowing SMAPI to detect them normally. We patch SMAPI to make `InitClient(...)`
& `InitServer(...)` call our own methods, which can intercept the decoys and let
us instantiate the proper client/server that can manually use SMAPI's handlers.

#### Harmony Patches

We use Harmony transpiler patches, which can be found in `SteamDew/Patches`. The
use of transpiler patches is often discouraged, as it can break mod compatiblity
and introduce instability & crashes in general.

To keep the risks as minimal as possible, we transpiled code that we knew no mod
would be willing to patch: Stardew Valley's networking SDK & SMAPI itself. These
patches work as follows:

- `PSteamHelper/OnGalaxyStateChange.cs`: This patches `new SteamNetHelper()`, to
be replaced with `new SteamDewNetHelper()`, which drives the rest of SteamDew.
- `PGalaxySocket/UpdateLobbyPrivacy.cs`: Removes the logic that would create the
Steam lobby for a Galaxy socket. We instead inject our own method which captures
the Galaxy socket connection string and passes it to the SteamDew server.
- `PFarmhandMenu/Update.cs`: Allows our `InviteClient` to determine what type of
client to use to connect to the lobby obtained by a invite code. This patch then
injects the proper client into the menu.
- `PGameServer/Ctor.cs`: Adds both `SGalaxyNetServer` & `SteamDewServer` for use
in the server list.
- `PSMultiplayer/InitClient.cs`: This patch transpiles SMAPI itself. In order to
get the hooks that SMAPI uses, we replace their object instantiation with a call
to our own static method. This method uses the decoys to determine the client we
create. We then pass the callbacks into the constructor of the new client, which
allows us to trigger the callbacks manually in our code.
- `PSMultiplayer/InitServer.cs`: This patch works almost identically to the code
for `InitClient`, except it will obviously use our server decoys.

#### Future Ideas

- We started experiencing crashes with SteamDew when the save files for Stardew
had gotten so large that it could not fit in the network buffer used by Steam's
Networking Sockets. We introduced LZ4 compression to mitigate this problem, and
we also increased the size of the network buffer as well. Though Stardew didn't
crash when using the original Galaxy-based netcode, it likely only worked since
Galaxy's P2P networking has a bigger network buffer by default. There is a high
chance that it would also fail with enough players and a large enough save file
(such as from year 3). Our compression solution only fixes the symptom, but the
robust solution would be to rework the way Stardew Valley sends the save files.
- In theory, cross-platform play could also be reworked for more stability. The
library by Valve, the GameNetworkingSockets, is an open-source version of Steam
Networking Sockets. This could be leveraged to make cross-platform play stable,
but it would require a bigger rewrite. We have a solid idea of what needs to be
done, but it would be too difficult without support from ConcernedApe.

#### Compiling for 1.6 Alpha

Even with the multiplayer fixes that the 1.6 Alpha brings, frequent disconnects
still occur without SteamDew. As such, we have confirmed that our Harmony patch
methods will work in both Stardew Valley 1.5.6 and the 1.6 Alpha.

Simply change the `SteamDew/SteamDew.csproj` file, to set the `TargetFramework`
to `net6.0` instead of `net5.0` to compile for 1.6 specifically. Even without a
recompile, the pre-built SteamDew binary should be able to work regardless.