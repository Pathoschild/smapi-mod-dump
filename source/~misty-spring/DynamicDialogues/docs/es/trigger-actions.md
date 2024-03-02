**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# Nuevos trigger actions

## Contenidos
* [Agregar EXP](#agregar-experiencia)
* [Hacer que PNJs hablen](#diálogo-de-pnjs)
* [Iniciar eventos](#iniciar-eventos)
* [Notificaciones](#mandar-notificaciones)

------------------

## Agregar experiencia
`mistyspring.dynamicdialogues_AddExp <player> <skill> <amt>`

Agrega EXP a la habilidad dada.

La habilidad debe ser vainilla:
- `farming` (granja)
- `fishing` (pesca)
- `foraging` (recolección)
- `mining` (minería)
- `combat` (combate)
- `luck` (suerte)

### Ejemplo

`mistyspring.dynamicdialogues_AddExp All farming 20`

Para todes los jugadores, se agregarán 20 puntos a la habilidad de granja.

-----------------

## Diálogo de PNJs
`mistyspring.dynamicdialogues_Speak <NPC> <key> [shouldOverride]`

Parecido al `speak` de eventos, pero usa una llave de los diálogos del PNJ. Si `shouldOverride` es true, se forzará sobre cualquier diálogo/menu.

### Ejemplo
`mistyspring.dynamicdialogues_Speak Krobus Fri false`
Aquí, Krobus dice su línea de los viernes (`Fri`: "..."), mientras que no haya ningún menu abierto.

------------------

## Iniciar eventos

`mistyspring.dynamicdialogues_DoEvent <eventID> <location> [preconditions] [checkseen] [reset_if_not_played]`

Inicia un evento. __Sólo puede llamarse__ desde trigger actions __al inicio del día.__

### Parameters

| nombre              | type   | requerido | descripción                                                                                            |
|---------------------|--------|-----------|--------------------------------------------------------------------------------------------------------|
| eventID             | string | si        | ID del evento.                                                                                         |
| location            | string | si        | Lugar donde ocurre el evento.                                                                          |
| preconditions       | bool   | no        | Si revisar condiciones. Por defecto es `true` (verdadero).                                             |
| checkseen           | bool   | no        | Revisa si el evento ya fue visto. Por defecto es `true` (verdadero).                                   |
| reset_if_not_played | bool   | no        | El *trigger action* se reiniciará si este evento no es visto (e.j el día termina antes de que puedas). |

**Ejemplo**

`mistyspring.dynamicdialogues_DoEvent MyCustomEvent ScienceHouse false true true`

Aquí, `MyCustomEvent` ocurrirá en la casa de Robin (ScienceHouse), sin revisar las condiciones de evento.

-----------------

## Mandar notificaciones
`mistyspring.dynamicdialogues_SendNotification <id> <check if already sent>`

Manda una notificación. Debe existir en el archivo de notificaciones.

### Ejemplo
`mistyspring.dynamicdialogues_SendNotification MiNotificacion true`

Aquí, el mod busca MiNotificacion en `mistyspring.dynamicdialogues/Notifs`, y lo envía *si es que* aún no fue vista.
