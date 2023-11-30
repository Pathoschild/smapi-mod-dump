**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/CJBok/SDV-Mods**

----

**CJB Item Spawner** is a [Stardew Valley](http://stardewvalley.net/) mod that lets you spawn items
with just a few clicks:

![](screenshot.png)

Compatible with Stardew Valley 1.5.5+ on Linux, macOS, and Windows.

## Install
1. [Install the latest version of SMAPI](https://smapi.io/).
2. [Install this mod from Nexus mods](http://www.nexusmods.com/stardewvalley/mods/93).
3. Run the game using SMAPI.

## Use
Press `i` on your keyboard (configurable) to open the menu.

In the menu you can...
* Change item quality by clicking the quality button.
* Change category filters by clicking the dropdown, with `left`/`right` on a keyboard, or with `left trigger`/`right trigger` on a controller.
* Scroll items by clicking the arrows, with your mouse scroll wheel, with `up`/`down` on your keyboard, or with the right thumbstick on a controller.
* Search by pointing at the search box and typing an item name.

## Configure
The mod will work fine out of the box, but you can tweak its settings by editing the `config.json`
file. These are the available settings.

<table>
<tr>
  <th>setting</th>
  <th>default</th>
  <th>what it affects</th>
</tr>
<tr>
  <td><code>ShowMenuKey</code></td>
  <td><code>i</code></td>
  <td>

The keybind which opens the menu (see [valid key bindings](https://stardewvalleywiki.com/Modding:Player_Guide/Key_Bindings)).
This supports multi-key bindings.

  </td>
</tr>
<tr>
  <td><code>AllowProblematicItems</code></td>
  <td><code>false</code></td>
  <td>

Whether to show items which may cause bugs or crashes when spawned.

  </td>
</tr>
<tr>
  <td><code>ReclaimPriceInMenuTrashCan</code></td>
  <td><code>true</code></td>
  <td>

When you [upgrade your trash can](https://stardewvalleywiki.com/Trash_Cans) to reclaim part of the
price of destroyed items, whether that upgrade applies in the item spawner menu too.

  </td>
</tr>
<tr>
  <td><code>HideCategories</code></td>
  <td><em>none</em></td>
  <td>

The categories to hide in the UI. Items in these categories will not be accessible through the
spawn menu. You can see the category keys in the `assets/categories` file.

For example, this will hide tools and weapons:
```json
"HideCategories": [ "filter.equipment-tools", "filter.equipment-weapons" ]
```

  </td>
</tr>
</table>

## FAQs
### What are 'problematic items'?
You can [edit `config.json`](#configure) to enable problematic items, which are normally hidden
because they cause in-game issues, glitches, or crashes. These are the items considered problematic:

item | issue
---- | -----
Crystal Chair | (Unreleased version.) Glitches and causes crashes once rotated / picked up.
Incubator | Can't be removed once placed.
Locked Door | Can't be removed once placed.
Mushroom Box | Can't be removed once placed (unless you install the [Craftable Mushroom Boxes](https://www.nexusmods.com/stardewvalley/mods/2899) mod _before_ spawning them).
Obelisk | Once placed, it runs a glitched animation when the player is near that eventually starts switching between the Obelisk and a chicken statue sprite. Picking it up after that point can cause an immediate game crash or result in an unremovable inventory item.
Wicked Statue | (Lit eyes version.) Can't be removed once placed.

## See also
* [Release notes](release-notes.md)
