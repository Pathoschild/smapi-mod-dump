**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/evfredericksen/StardewSpeak**

----

# StardewSpeak

Play Stardew Valley by voice.

This mod is primarily made for people with limited use of their hands who want to play Stardew Valley without interacting with the keyboard or mouse.

## Installation

StardewSpeak can be installed just like [any other SMAPI-based mod](https://stardewvalleywiki.com/Modding:Player_Guide/Getting_Started). Download the [latest release](https://github.com/evfredericksen/StardewSpeak/releases/latest/download/StardewSpeak.0.0.12.zip) and unzip its contents into your Stardew Valley Mods folder.

Test out your microphone to ensure that it is able to clearly capture your speech. The speech engine uses about 2.5 gigabytes of RAM, so be careful about running it alongside other RAM-intensive applications.

Currently, StardewSpeak is only usable on Windows 10.

## Getting Started

Speech recognition begins automatically when Stardew Valley is launched. If you are playing in windowed mode, the microphone icon in the taskbar indicates that speech recognition is active. Additionally, an in-game notification will appear.

All menus can (or will) be navigated by voice. As an example, to load a saved game from the title menu, say `load` to enter the saved game menu, then `game` followed by the number of the game you want to play, e.g. `game three` to load the third game. If you have more than four saved games, `scroll down` and `scroll up` will click the up and down arrows. See the [menus file](docs/menus.md) for more information and a list of available menu-specific commands. 

Once in game, try saying `go to farm` to begin walking from your farmhouse interior to your farm. Saying `stop` will stop whatever your farmer is currently doing.

## Development

Documentation for developers can be found [here](docs/dev.md).

## Commands

Commands wrapped in brackets are optional, meaning that `hello [world]` will match either `hello` or `hello world`. Commands wrapped in brackets or parentheses with `|` describes alternatives. Commands wrapped in `<>` refer to a particular set of alternatives: `<direction>` refers to movement directions, `<n>`, `<x>`, and `<y>` refer to numbers, `<location>` refers to game locations, and `<item>` refers to game items.

### General
<table>
    <tr>
        <th>Command</th>
        <th>Description</th>
        <th>Example(s)</th>
    </tr>
    <tr>
        <td>&lt;direction&gt;</td>
        <td>Begin moving in a specific direction. Options are north (up), east (right), south (down), west (left), main (up and right), floor (down and right), air (down and left), and wash (up and left). Using a mnemonic with USA states: Maine in the northeast, Florida in the southeast, Arizona in the southwest and Washington in the northwest.</td>
        <td>"north"</td>
    </tr>
    <tr>
        <td>&lt;direction&gt; &lt;n&gt;</td>
        <td>Move <i>n</i> tiles in a direction and stop. Will pathfind around obstacles as long as the target tile is clear.</td>
        <td>"one two west" - move left 12 tiles</td>
    </tr>
    <tr>
        <td>navigate &lt;direction&gt;</td>
        <td>Combines the two above commands. Begin moving in a specific direction while pathfinding around terrain.</td>
        <td>"navigate east"</td>
    </tr>
    <tr>
        <td>face &lt;direction&gt;</td>
        <td>Face direction.</td>
        <td>"face east"</td>
    </tr>
    <tr>
        <td>clear (debris | stones | rocks | twigs | wood | weeds)</td>
        <td>Begin clearing weeds, stone, or wood. Saying clear debris will clear all types.</td>
        <td>
            <div>"clear debris"</div>
            <div>"clear wood"</div>
        </td>
    <tr>
        <td>(clear | mine) ore</td>
        <td>Begin mining ore and gem nodes.</td>
        <td>"clear ore"</td>
    </tr>
    <tr>
        <td>clear grass</td>
        <td>Begin clearing grass.</td>
        <td>"clear grass"</td>
    </tr>
    <tr>
        <td>chop trees</td>
        <td>Begin chopping down nearby trees.</td>
        <td>"chop trees"</td>
    </tr>
    <tr>
        <td>go to &lt;location&gt;</td>
        <td>Walk towards a game <a href="./StardewSpeak/lib/speech-client/speech-client/locations.py">location</a>.</td>
        <td>"go to mines"</td>
    </tr>
    <tr>
        <td>(go to mail box | (check | read) mail)</td>
        <td>If on the farm, go the the mailbox and press the action button</a>.</td>
        <td>
            <div>"go to mail box"</div>
            <div>"check mail"</div>
        </td>
    </tr>
    <tr>
        <td>go to shipping bin</td>
        <td>Go to the shipping bin and press action button.</td>
        <td>"go to shipping bin"</td>
    </tr>
    <tr>
        <td>go to billboard</td>
        <td>If in town, go to the billboard and press action button.</td>
        <td>"go to billboard"</td>
    </tr>
    <tr>
        <td>go to calendar</td>
        <td>If in town, go to the calendar and press action button.</td>
        <td>"go to calendar"</td>
    </tr>
    <tr>
        <td>(dig | hoe) &lt;x&gt; by &lt;y&gt;</td>
        <td>Use hoe to dig an <i>x</i> by <i>y</i> grid based on the last two directions faced.</td>
        <td>"dig three by four"</td>
    </tr>
    <tr>
        <td>start planting</td>
        <td>Start planting equipped seeds or fertilizer on available hoe dirt.</td>
        <td>"start planting"</td>
    </tr>
    <tr>
        <td>water crops</td>
        <td>Start watering nearby crops.</td>
        <td>"water crops"</td>
    </tr>
    <tr>
        <td>harvest crops</td>
        <td>Start harvesting fully grown crops.</td>
        <td>"start harvesting"</td>
    </tr>
    <tr>
        <td>pet animals</td>
        <td>Attempt to pet all animals in the current location. Will sometimes fail if the animals are clumped together or are in tight areas that make pathfinding difficult.</td>
        <td>"pet animals"</td>
    </tr>
    <tr>
        <td>milk animals</td>
        <td>Attempt to milk all cows and goats in the current location. Will sometimes fail if the animals are clumped together or are in tight areas that make pathfinding difficult.</td>
        <td>"milk animals"</td>
    </tr>
    <tr>
        <td>start fishing</td>
        <td>Cast fishing rod at maximum distance. If the cast is successful, wait for a nibble and begin reeling.</td>
        <td>"start fishing"</td>
    </tr>
    <tr>
        <td>catch fish</td>
        <td>Automatically complete fish catching minigame. Will also catch any treasure chests that appear.</td>
        <td>"catch fish"</td>
    </tr>
    <tr>
        <td>talk to &lt;npc&gt;</td>
        <td>Move to an NPC and press action button. If the player is holding a giftable item this will gift that item to the NPC. Will fail if the NPC is not in the current location.</td>
        <td>"talk to Leah"</td>
    </tr>
    <tr>
        <td>start shopping</td>
        <td>If in a store location (Pierre's General Store, Marnie's house, etc.), move to shopkeeper and press action button.</td>
        <td>"start shopping"</td>
    </tr>
    <tr>
        <td>[open | read] (quests | journal | quest log)</td>
        <td>Open journal.</td>
        <td>"read journal"</td>
    </tr>
    <tr>
        <td>go inside</td>
        <td>Go inside the nearest building, including farm buildings.</td>
        <td>"go inside"</td>
    </tr>
    <tr>
        <td>nearest &lt;item&gt; [&lt;n&gt;]</td>
        <td>Move to nearest <a href="./StardewSpeak/lib/speech-client/speech-client/items.py">item</a> by name in current location. If <i>n</i> is specified, go to the nth closest item.</td>
        <td>
            <div>"nearest chest"</div>
            <div>"nearest bee house three"</div>
        </td>
    </tr>
    <tr>
        <td>(action | check)</td>
        <td>Press action button (default x)</td>
        <td>"action"</td>
    </tr>
    <tr>
        <td>swing</td>
        <td>Use tool (default c)</td>
        <td>"swing"</td>
    </tr>
    <tr>
        <td>start swinging [tool]</td>
        <td>Hold down use tool button (default c)</td>
        <td>"start swinging"</td>
    </tr>
    <tr>
        <td>stop</td>
        <td>Stop current actions.</td>
        <td>"stop"</td>
    </tr>
    <tr>
        <td>item &lt;n&gt;</td>
        <td>Equip the nth item in the toolbar.</td>
        <td>"item seven"</td>
    </tr>
    <tr>
        <td>equip &lt;item&gt;</td>
        <td>Equip item if in inventory.</td>
        <td>
            <div>"equip pickaxe"</div>
            <div>"equip cauliflower seeds"</div>
        </td>
    </tr>
    <tr>
        <td>equip [melee] weapon</td>
        <td>Equip melee weapon if in inventory.</td>
        <td>"equip weapon"</td>
    </tr>
    <tr>
        <td>go to ladder</td>
        <td>Move to the ladder up in the mines and press the action key.</td>
        <td>"go to ladder"</td>
    </tr>
    <tr>
        <td>go to elevator</td>
        <td>Move to the elevator in the mines and press the action key.</td>
        <td>"go to elevator"</td>
    </tr>
    <tr>
        <td>ladder down</td>
        <td>Move to the nearest ladder down in the mines or skull cavern if at least one has been revealed and press the action key.</td>
        <td>"ladder down"</td>
    </tr>
    <tr>
        <td>attack</td>
        <td>Move to the nearest monster in the current location and begin swinging melee weapon. Continue until there are no more monsters in the current location.</td>
        <td>"attack"</td>
    </tr>
    <tr>
        <td>defend</td>
        <td>Wait until a monster comes into melee weapon range and swing at it. Continue until there are no more monsters in the current location.</td>
        <td>"defend"</td>
    </tr>
    <tr>
        <td>(next | cycle) toolbar</td>
        <td>Cycle the toolbar.</td>
        <td>"next toolbar"</td>
    </tr>
    <tr>
        <td>skip [cutscene | event]</td>
        <td>Skip the current cutscene.</td>
        <td>"skip cutscene"</td>
    </tr>
    <tr>
        <td>[left] click [&lt;n&gt;]</td>
        <td>Left click <i>n</i> times (default one).</td>
        <td>"click"</td>
    </tr>
    <tr>
        <td>right click [&lt;n&gt;]</td>
        <td>Right click <i>n</i> times (default one).</td>
        <td>"right click"</td>
    </tr>
    <tr>
        <td>mouse &lt;direction&gt; [&lt;n&gt;]</td>
        <td>Move the mouse <i>n</i> tiles (64 pixels). Ideally, this command and the one below will only be used sparingly when more specific commands are unavailable.</td>
        <td>"mouse down"</td>
    </tr>
    <tr>
        <td>small mouse &lt;direction&gt; [&lt;n&gt;]</td>
        <td>Move the mouse <i>n</i> pixels.</td>
        <td>"small mouse down seven"</td>
    </tr>
    <tr>
        <td>hold mouse</td>
        <td>Hold down the left mouse button.</td>
        <td>"hold mouse"</td>
    </tr>
    <tr>
        <td>release mouse</td>
        <td>Release the left mouse button if it is being held.</td>
        <td>"release mouse"</td>
    </tr>
</table>
