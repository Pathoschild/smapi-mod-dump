Scroll To Zoom - 1.3

Zoom In and Out Using The Mouse.

::REQUIREMENTS::
• SMAPI >= 2.6-beta
• Stardew Valley >= 1.3

::FEATURES::
• Define Min\Max Zoom
• Define a modifier key.
• Suppress default behavior for modifier key.
• Define how much you zoom in which each scroll tick.

::CHANGELOG::
1.3
Fix Null Refernence Exception caused by Functionality Level lookup

1.2 
Allowed for whitelisting of Menus to enable zoom on.
Currently supports three functionality levels [None - 0, Mostly - 1, Fully - 2]

Fully Functional:
ShopMenu
Billboard
GameMenu
CarpenterMenu
LetterViewMenu
LevelUpMenu

Mostly Functional:
TitleMenu
- Mouse jumps on zoom.
DialogBox
- Mouse jumps on zoom.
MineElevatorMenu
LoadGameMenu
- Zooming causes cursor to jump off screen.
CoopMenu 
- Zooming causes cursor to jump off screen
GeodeMenu 
- Mouse jumps on zoom.
MuseumMenu 
- Window flicks back from the corner on zoom.
PurchaseAnimalsMenu 
- Mouse jumps on zoom.
QuestLog 
- Mouse jumps on zoom.

Known Broken:
ItemGrabMenu (Chests, Shipping Box, etc.) - Elements shift around on zoom.
AboutMenu - Elements shift on zoom.
JunimoNoteMenu (Bundles)
- X button doesn't adjust position on zoom. 
- Secondary screens after selecting Bundles have horrible duplication issues.
- Secondary screens don't fire MenuChanged events and therefore cannot be handled at all.

1.1
• Fix Light Overlay not resizing with zoom.
• Fix Crash related to scrolling not checking item bounds while in a IClickableMenu.
• Disabled Zooming In\Out on Clickable Menus as the Game doesn't have an easy way to compensate.

1.0
• Initial Release