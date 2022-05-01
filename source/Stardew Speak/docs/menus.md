**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/evfredericksen/StardewSpeak**

----

# Menus

Generally, menu commands should map to what is visible on screen. For example, if a menu contains an ok button, saying `ok` will click that button. Similarly, saying `trash can` will click on any visible trash can. Adding these commands is a manual process that varies from menu to menu, so if a command is missing or not working correctly feel free to create an issue.

All menus with commands can also be navigated with the `north`, `south`, `east`, and `west` commands to move the mouse to an adjacent clickable section of the active menu. An optional number afterwards will fire the command that many times. For example, if the backpack is open with the mouse over the second item on the first row, saying `south two` will move the cursor to the second item on the third row. `Click` and `right click` will click any button underneath the mouse.

Some menus, like the new game menu, contain text boxes for <a href="../StardewSpeak/lib/speech-client/speech-client/letters.py">free form text entry</a>. Dictation is also available - saying `title hello world` will enter "Hello World", and saying `dictate hello world` will enter "hello world".

## Available Menu Commands (WIP)

<ul>
    <li> <a href="./title-menu.md">Title Menu</a> </li>
    <li> <a href="./new-game-menu.md">New Game Menu</a> </li>
    <li> <a href="./load-game-menu.md">Load Game Menu</a> </li>
</ul>
<ul>
    <li> <a href="./inventory-page.md">Inventory Page</a> </li>
    <li> <a href="./skills-page.md">Skills Page</a> </li>
    <li> <a href="./social-page.md">Social Page</a> </li>
    <li> <a href="./map.md">Map</a> </li>
    <li> <a href="./crafting-page.md">Crafting Page</a> </li>
    <li> <a href="./collections-page.md">Collections Page</a> </li>
    <li> <a href="./options-page.md">Options Page</a> </li>
    <li> <a href="./crafting-page.md">Exit Game Page</a> </li>
</ul>
<ul>
    <li> <a href="./dialogue-menu.md">Dialogue Menu</a> </li>
    <li> <a href="./letter-viewer.md">Letter Viewer</a> </li>
    <li> <a href="./shipping-bin.md">Shipping Bin</a> </li>
    <li> <a href="./shop-menu.md">Shop Menu</a> </li>
    <li> <a href="./gift-log.md">Gift Log</a> </li>
</ul>