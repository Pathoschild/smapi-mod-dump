**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# Visitas personalizadas

Este mod te deja agregar visitas personalizadas. Se hace editando el archivo de visitas.

## Valores aceptados

Nombre|Tipo|Requerido| Descripción
-----|---|---------|--------- 
From | `int` | true | La hora a la que el PNJ llega.
To | `int` | false | La hora a la que el PNJ se va. Si está vacío, se usa la hora por defecto.
MustBeExact | `bool` | false| Si es `true`, la visita debe ser exacta. E.j: Si From es 900, la visita sólo ocurrirá si estás en la granja a las 9:00, y 9:10 (etc) no harán nada.
EntryBubble | `string` | false | Texto que mostrar sobre su cabeza al entrar.
EntryQuestion | `string` | false | Si "Pedir permiso" está activado, usarán este texto.
EntryDialogue | `string` | false | Diálogo usado al entrar.
ExitDialogue | `string` | false | Diálogo usado al salir.
Dialogues | `List<string>` | false| Diálogos usado durante la visita.
Extras | `ExtraBehavior` | false | Comportamiento extra para PNJ.

### Comportamiento extra
Es puramente opcional, y tiene estos parámetros:
Nombre|Tipo| Descripción
-----|---|--------- 
Force|bool| La visita ocurrirá aunque no estés en la granja.
Mail|string|ID del correo que enviar post-visita.
GameStateQuery|string|Condiciones extra para que la visita ocurra. Ve [GSQ en la wiki](https://stardewvalleywiki.com/Modding:Game_state_queries).


## Ejemplo:

```
{
  "LogName": "Agregar visita de George",
  "Action":"EditData",
  "Target":"mistyspring.farmhousevisits/Schedules",
  "Entries": {
    "George": 
    {
      "From": 700,
      "To": 1000,
      "EntryDialogue": "Hola, @. Qué hacen los jóvenes como tú?#$b#Vine a visitar.",
      "ExitDialogue": "Hace frío...#$b#Tengo que irme. Adiós, @.",
      "Dialogues":
      [
        "¿Qué es eso?#$b#¿Una pintura tuya?", 
        "Esta casa no está tan mal...", 
        "%George observa mientras trabajas.",
      ]
    },
  "When":{
    "Hearts:George": 5
    }
  }
```

Plantilla vacía:
```
{

  "LogName": "Add visit",
  "Action":"EditData",
  "Target":"mistyspring.farmhousevisits/Schedules",
  "Entries": {
    "name_of_character": {
      "From": ,
      "To": ,
      "EntryBubble": "",
      "EntryQuestion: "",
      "EntryDialogue": "",
      "ExitDialogue": "",
      "Dialogues":
      [
        "", 
        "", 
        "",
      ],
      "Extras":{
	"Force": false,
	"Mail": null,
	"GSQ": null
      }
    }
  }
```
