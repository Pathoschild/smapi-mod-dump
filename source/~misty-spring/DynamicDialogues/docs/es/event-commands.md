**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----


# Comandos de evento

## Contenidos

* [Agregar EXP](#add-skill-experience)
* [Incluir comandos](#insertar)
* [Cambiar salud/energía](#cambiar-salud--energía)
* [Relación con PNJ](#relacion-con-pnj)
* [Nuevos comandos de final](#comandos-de-final)
  * [house / farmhouse](#house--farmhouse)
  * [lastSleepLocation](#last-sleep-location)
  * [warp](#warp)
* [Condiciones para comandos](#if--else-condiciones)
  * [Parámetros](#parámetros)
* [Escenas](#escenas)
  * [Agregar una escena](#agregar-una-escena)
  * [Remover una escena](#removing-una-escena)


----------

## Agregar EXP
`addExp <name> <amount>`

Agrega EXP a una habilidad. Sólo afecta al jugador que ve el evento.

La habilidad debe ser vainilla:
- `farming` (granja)
- `fishing` (pesca)
- `foraging` (recolección)
- `mining` (minería)
- `combat` (combate)
- `luck` (suerte)

**Ejemplo**

Para agregar 20 puntos a combate: `addExp combat 20`

----------

## Insertar

`append <key>`

Inserta comandos en el evento actual. Los comandos deben existir como una entrada en el archivo actual de `Data/Events/`.

Es más útil cuando se usa con [if/else](#if--else) (e.j, acciones que sólo pasan en un evento si tienes X correo o algo más).

### Ejemplo (usando if/else)
```jsonc
{
  "Action": "EditData",
  "Target": "Data/Events/IslandNorth",
  "Entries": {
    "CustomEvent/f Abigail 250": "continue/40 25/Abigail 38 26 2 farmer 41 33 0/skippable/if PLAYER_HAS_MAIL Current MyCustomFlag##append continuation##append alt/emote Abigail 16/end",
    "continuation":"pause 1500/move farmer 0 -5 0",
    "alt":"pause 1200/playsound crash/jump Abigail"
  }
}
```

Aquí, el evento se ramifica momentáneamente: Si tienes el correo MyCustomFlag, se usa `continuation`. Si no, se usa `alt`.

### Explicación
El evento 'inicial' debe seguir el formato normal (o sea, iniciar con `música/cámara/actores` y tener un comando `end` al final). En nuestro caso iniciamos con `continue/40 25/Abigail 38 26 2 farmer 41 33 0`.

Sin embargo, los comandos insertados con `append` **no** siguen este formato, sólo requieren los comandos a llamar.

----------

## Cambiar salud / energía. 

`health <set/add> <amount>`
`stamina <set/add> <amount>`

Te permite alterar salud o energía. Si el resultado es 0 o menos, se cambia a 1.

También la puedes rellenar usando `health reset` o `stamina reset`.

**Ejemplo 1**

`health set 50`

Cambia la salud a 50.

**Ejemplo 2**

Digamos que te queda 90 de energía. Si un evento usa `stamina - 100`, se cambiará a 1.

(Esto es porque el juego no permite energía/salud negativa).

**Seudónimos**

Puedes usar estos seudónimos para cambiar el valor:

| nombre | seudónimo                |
|--------|--------------------------|
| set    | cualquiera (por defecto) |
| add    | `+`, `more`              |
| reduce | `-`, `less`              |
| reset  | ninguno                  |

Ambos `health = 10` y `health set 10` cambian la salud a 10.

----------

## Relaciones con PNJ
`setDating <who> [breakup]`

Cambia la relación con un PNJ a romántica. Si `breakup` es `true`, terminará contigo con los efectos normales de una ruptura.

(Importante: Si no están solteros, esto no tiene efecto.)

----------
## Comandos de final


### House / farmhouse
`end house` / `end farmhouse`

Ends the event and returns to the farm/farmhouse (as given), on the default entry point.

### Last sleep location
`end lastSleepLocation`

Ends the event and returns to the last sleeping location, on the last slept bed.

### Warp
`end warp <location> [x] [y]`
Ends the event and warps to desired location. If no x/y are given, the default entry point will be used.

----------

## If / Else (condiciones)
`if <GSQ>##<consequence>##[alternative]`

Puedes hacer que un comando solo aparezca bajo ciertas condiciones usando [Game State Queries](https://stardewvalleywiki.com/Modding:Game_state_queries):

**\*Importante: Diálogos dentro del comando deben tener doble barra** (o sea, `\\\"` en vez de `\"`).

### Parámetros
| nombre      | requerido | descripción                                                                                |
|-------------|-----------|--------------------------------------------------------------------------------------------|
| GSQ         | si        | La condición [game state query](https://stardewvalleywiki.com/Modding:Game_state_queries). |
| Consequence | si        | Comando que usar si aplican condiciones.                                                   |
| Alternative | no        | Usado si las condiciones NO aplican.                                                       |

**Ejemplo**

`if PLAYER_COMBAT_LEVEL Current 5 ## speak Abigail \\\"Wow, ¡eres fuerte!\\\"`

En este caso, si el nivel del jugador es mayor a 5, Abigail hablará. Si el nivel es menor a 5, no ocurre nada.

----------
## Escenas
Event scenes work just like vanilla scenes (for example, Caroline Tea or the Onions event.)

### Agregar a Scene


First, you must load the scene to `mistyspring.dynamicdialogues/Scenes/<name-of-scene>`. 

Files with a height of **112** will be automatically centered on the screen.


**Ejemplo:**

```
{
  "Action": "Load",
  "Target": "mistyspring.dynamicdialogues/Scenes/MyScene",
  "FromFile": "assets/Scenes/MyScene.png"
}
```

Now that the scene is loaded, we can use it for events.

| field        | requerido | descripción                                                       |
|--------------|----------|-------------------------------------------------------------------|
| AddScene     | si      | The command.                                                      |
| FileName     | si      | The name of the scene to load.                                    |
| ID           | si      | Number assigned to this scene. Recommended to use mod's UniqueID. |
| Frames\*     | no       | Used for animations. How many frames this scene has.              |
| Milliseconds | no       | Used for animations. How many milliseconds each frame will have.  |

\*= If the scene is animated, each frame must have the same size (e.g 100 width). A 5-frame scene should use a file with a width of 500.

**Ejemplo:**

Let's say our scene is a 112x112 image.

`/AddScene MyScene 12195`

This will add the scene *immediately.* 
(For a fade effect, add `/fade/viewport -300 -300/` before this command)

----------

### Removing a Scene

To remove a scene, simply use the ID we assigned.

`/RemoveScene <ID>`

This will immediately remove it from the event.

**Ejemplo:**
`/fade/RemoveScene 12195/unfade`

First, the scene will fade out. Then, the scene will be removed.
When the game unfades, our scene will be gone.
