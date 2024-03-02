**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

## Agregar notificaciones

Las notificaciones son cargadas desde `mistyspring.dynamicdialogues/Notifs`.

| nombre        | requerido | descripción                                                                                                                  |
|---------------|-----------|------------------------------------------------------------------------------------------------------------------------------|
| Time          | (*)       | Hora a la que agregar notificación.                                                                                          |
| Location      | (*)       | Nombre de mapa.                                                                                                              |
| Message       | si        | Mensaje.                                                                                                                     |
| IsBox         | no        | Si es `true`, la notificación será "de caja".                                                                                |
| Sound         | no        | Sonido que hacer. ([ver IDs de sonido](https://docs.google.com/spreadsheets/d/18AtLClQPuC96rJOC-A4Kb1ZkuqtTmCRFAKn9JJiFiYE)) |
| Conditions    | no        | [Condiciones](https://github.com/misty-spring/DynamicDialogues/blob/main/docs/es/player-conditions.md) que usar.             |
| TriggerAction | no        | El *trigger action* que llamar.                                                                                              |

`(*)` = Como los diálogos, necesitas una hora o un lugar (o ambos) para que el diálogo cargue.

**Plantilla**:

```
        "ejemplo": {
          "Time": "",
          "Location": "",
          "Message": "",
          "IsBox": false,
          "Sound": "",
          "Conditions": "",
          "TriggerAction": "", 
          }


```
**Importante:** Si no quieres que un diálogo aparezca todos los días, usa las condiciones When de ContentPatcher ("When"), o [GSQ](https://stardewvalleywiki.com/Modding:Game_state_queries).

Ejemplo:

```
{
      "Action": "EditData",
      "Target": "mistyspring.dynamicdialogues/Notifs",
      "Entries": {
        "ejemplo_caja": {
          "Location": "Farm", //en la granja
          "Message": "El clima está feo hoy...",
          "IsBox": true
        }
      },
      "When":{
        "Weather":"Rain, Storm" //si clima es lluvia o trueno
      }
    },
```
