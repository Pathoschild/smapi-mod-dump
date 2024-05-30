**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/BinaryLip/ScheduleViewer**

----

[← back to readme](README.md)

This document is for mod authors who'd like to add custom tile areas. **For players, see the [main readme](README.md) instead.**

## Custom Tile Areas

Tile areas are used by this mod to enhance aspects of the UI and are split into 2 types: Access and General Tile Areas.

- Access Tile Areas - used to determine if an NPC is in an area the farmer might not have access to. Most notably an NPC's bedroom but
  also other areas that have a friendship check like Caroline's Sun Room. Right now the check is hard-coded to 2 hearts.
- General Tile Areas - used to specify sub-areas inside a location and are displayed in parenthesis next to the location name in the schedule.
  Examples for the Town location include the Ice Cream Stand, Playground, Square, and Water Fountain

### Tile Area Object

<table>
<thead>
  <tr>
    <th>Property</th>
    <th>Description</th>
    <th>Type</th>
    <th>Access</th>
    <th>General</th>
  </tr>
</thead>
<tbody>
  <tr>
    <td><code>DisplayName</code></td>
    <td>
      the name of the tile area to display to the user<br>
      Note: if the string starts with <code>String\</code> then the mod will use <code>Game1.content.LoadString(DisplayName)</code> as the name of the tile area <br>
      ex: <code>"Abigail's Bedroom"</code>, <code>"Ice Cream Stand"</code><br>
    </td>
    <td><code>string</code></td>
    <td>Required</td>
    <td>Required</td>
  </tr>
  <tr>
    <td><code>Location</code></td>
    <td>
      the internal name of the game location<br>
      ex: <code>"SeedShop"</code>, <code>"SamHouse"</code>, <code>"SebastianRoom"</code>
    </td>
    <td><code>string</code></td>
    <td>Required</td>
    <td>Required</td>
  </tr>
  <tr>
    <td><code>Npcs</code></td>
    <td>
      a list of NPCs to check the farmer's friendship status against to enter the area<br>
      <ul><li>the farmer must have at least 2 hearts with one name in the list</li></ul>
      ex: <code>["Abigail"]</code>, <code>["Caroline", "Pierre"]</code>
    </td>
    <td><code>string[]</code></td>
    <td>Required</td>
    <td>Not used</td>
  </tr>
  <tr>
    <td><code>TileRectangle</code></td>
    <td>
      a rectangle of tiles for the tile area<br>
      &nbsp;&nbsp;<b>"X":</b> x-coordinate of the top left tile<br>
      &nbsp;&nbsp;<b>"Y":</b> y-coordinate of the top left tile<br>
      &nbsp;&nbsp;<b>"Height":</b> height of the rectangle<br>
      &nbsp;&nbsp;<b>"Width":</b>  width of the rectangle<br>
      ex: <code>{ "X": 1, "Y": 2, "Height": 3, "Width": 4 }</code>
    </td>
    <td>
<code>{<br>
&nbsp;&nbsp;"X":&nbsp;number,<br>
&nbsp;&nbsp;"Y":&nbsp;number,<br>
&nbsp;&nbsp;"Height":&nbsp;number,<br>
&nbsp;&nbsp;"Width":&nbsp;number<br>
}</code>
    </td>
    <td>Optional</td>
    <td>Optional</td>
  </tr>
  <tr>
    <td><code>Tiles<code></td>
    <td>
      a list of individual tiles for the tile area<br>
      ex: <code>[{ "X": 0, "Y": 0 }, { "X": 1, "Y": 0 }]</code>
    </td>
    <td>
<code>{<br>
&nbsp;&nbsp;"X":&nbsp;number,<br>
&nbsp;&nbsp;"Y":&nbsp;number<br>
}[]
</code>
    </td>
    <td>Optional</td>
    <td>Optional</td>
  </tr>
  <tr>
    <td><code>OverrideLocationName</code></td>
    <td>
      if true <code>DisplayName</code> overrides the <code>Location</code>'s display name<br>
      if false <code>DisplayName</code> gets added to the end of the <code>Location</code>'s display name in parenthesis<br>
    </td>
    <td><code>boolean</code></td>
    <td>Optional<br>if no tiles<br>otherwise<br>not used</td>
    <td>Optional</td>
  </tr>
</tbody>
</table>

### Adding a Custom Tile Area

To add a Custom Tile Area, add an entry to the `Mods/BinaryLip.ScheduleViewer/TileAreas`
asset using [Content Patcher](https://stardewvalleywiki.com/Modding:Content_Patcher)
or [SMAPI's content API](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Content):

```json
{
  "Action": "EditData",
  "Target": "Mods/BinaryLip.ScheduleViewer/TileAreas",
  "Entries": {
    // Ex: General Tile Area
    "{{ModId}}_TownBenches": {
      "DisplayName": "Town Benches",
      "Location": "Town",
      "TileRectangle": {
        "X": 41,
        "Y": 77,
        "Width": 6,
        "Height": 3
      }
    },
    // Ex: Access Tile Area
    "{{ModId}}_SamBedroom": {
      "Npcs": [ "Sam" ],
      "DisplayName": "Sam's Bedroom",
      "Location": "SamHouse",
      "TileRectangle": {
        "X": 14,
        "Y": 12,
        "Width": 10,
        "Height": 7
      },
      "Tiles": [
        { "X": 13, "Y": 12 },
        { "X": 13, "Y": 13 },
        { "X": 12, "Y": 12 },
        { "X": 12, "Y": 13 },
        { "X": 12, "Y": 14 }
      ]
    },
    // Ex: Access Tile Area for whole location
    "{{ModId}}_ElliottBedroom": {
      "Npcs": [ "Elliott" ],
      "DisplayName": "Elliott's Bedroom",
      "Location": "ElliottHouse"
    },
    // Ex: Access Tile Area for whole location
    //   + overrides the location's display name
    //   + uses a translation for the DisplayName
    "{{ModId}}_CarolineSunroom": {
      "Npcs": [ "Caroline" ],
      "DisplayName": "{{i18n:LocationName.Sunroom}}",
      "Location": "Sunroom",
      "OverrideLocationName": true
    }
  }
}
```

You can add as many Custom Tile Areas as you like but the entry names must be unique. An easy way to ensure that is by prepending the entry name with the `{{ModId}}` CP token. For more examples of Custom Tile Areas see [SVETileAreas/content.json](/SVETileAreas/content.json)

### Translations

You can use Content Patcher's built-in translation feature to provide translations for `DisplayName`. See the [CP translations guide](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide/translations.md) for more info.
