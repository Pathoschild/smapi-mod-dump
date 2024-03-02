**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# Nuevos game state queries
## Contenidos
* [Items usados](#items-usados)
* [Mejoras de herramienta](#mejoras-de-herramienta)

-------------

## Items usados
`mistyspring.dynamicdialogues_PlayerWearing <player> <type> <item> [only worn]`

Revisa si jugador/a está usando el item dado. Puede ser un gorro, polera, anillo, pantalón, o zapato/bota.

### Parámetros
| nombre    | requerido | alias                                                                                              |
|-----------|-----------|----------------------------------------------------------------------------------------------------|
| player    | si        | [Jugador/a](https://stardewvalleywiki.com/Modding:Game_state_queries#Target_player) a revisar.     |
| type      | si        | El [tipo de item](https://stardewvalleywiki.com/Modding:Migrate_to_Stardew_Valley_1.6#Item_types). |
| item      | si        | La ID de item.                                                                                     |
| only worn | no        | Si debería ignorar diferencias con el item mostrado\*. falso por defecto.                          |

`*` = A veces, el pantalón o polera mostrado es diferente del que usas. En esos casos, se da prioridad al mostrado (a menos que este valor sea `true`/verdadero).

### Tipos de item aceptados

| nombre | alias                |
|--------|----------------------|
| (H)    | `Hat` (gorro)        |
| (S)    | `Shirt` (polera)     |
| (R)    | `Ring` (anillo)      |
| (P)    | `Pants` (pantalones) |
| (B)    | `Boots` (botas)      |

### Ejemplos
 
- `mistyspring.dynamicdialogues_PlayerWearing Current Hat GorroDeMod` 

Revisará si el gorro del jugador/a actual tiene la ID GorroDeMod.


- `mistyspring.dynamicdialogues_PlayerWearing Current (S) PoleraDeMod false`

Revisa si el jugador/a actual está usando una polera con ID PoleraDeMod. Si la polera mostrada es diferente a la puesta, será ignorado.
