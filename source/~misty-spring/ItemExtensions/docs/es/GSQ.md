**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----


-------------

## Mejoras de herramienta

`mistyspring.dynamicdialogues_ToolUpgrade <jugador/a> <herramienta> [min] [max] [recursivo]`

Revisa que la herramienta esté en el rango de mejora. Debe estar en el inventario (si `recursivo` es verdadero, también busca en cofres).

Por defecto: Min es 0, Max es 4 y recursivo es false/falso (a menos que se indiquen otros valores).

### Valores de mejora
| nombre | equivalente |
|--------|-------------|
| 0      | Sin mejoras |
| 1      | Cobre       |
| 2      | Hierro      |
| 3      | Oro         |
| 4      | Iridio      |

### Ejemplo

`mistyspring.dynamicdialogues_ToolUpgrade Any Axe 1 3 true`

Esto revisa si cualquier jugador/a tiene una hacha de cobre, hierro u oro.

### Advertencias
- Si `recursivo` es falso y la herramienta pedida no está en el inventario, el resultado será `false` (falso).
- `recursivo` revisa quién usó la herramienta por última vez. Si ninguna de las hachas tienen al usuario, también devolverá falso/false.