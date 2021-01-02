---MoveAnnoyingCursor---
Moves the cursor to the edge when the menu is closed.

Can be enabled / disabled with the "U" key.
(The key can be changed with "Config.json EnableKey")

You can set the destination of the cursor with "Moveto" in "Config.json"
Default:"UL"
"Moveto" is Combination of "U"(Up),"D"(Down),"L"(Left) and "R"(Right)
Other characters are ignored.

The "Config.json" is created at startup.
---Fixed ver1.1.1---
support WindowMode.
The order of "Config.json Moveto" is now free.(Example "UL"="LU" "DR"="RD")

---add ver1.1.0---
"Config.json Moveto" is two character.(default "UL")
first character is "U"(Up) or "D"(Down) and other character(no move).
second character is "L"(Left) or "R"(Right) and other character(no move).