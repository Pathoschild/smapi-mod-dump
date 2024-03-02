**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# Custom questions

Las preguntas se cargan de `mistyspring.dynamicdialogues/Questions/<NPC name>`. Cuando un PNJ no tiene nada más que decir, puedes preguntarles algo.
Como los diálogos, estas necesitan una llave (se utiliza para identificar el diálogo en caso de errores).

Puedes agregar múltiples preguntas.

## Contenidos

* [Agregar preguntas](#agregar-preguntas)

  * [Ejemplo 1](#ejemplo-1)

  * [Ejemplo 2](#ejemplo-2)
-----------

## Agregar preguntas

| nombre          | obligatorio | descripción                                                |
|-----------------|-------------|------------------------------------------------------------|
| Question        | Si          | La pregunta que haces.                                     |
| Answer          | Si          | Cómo responde el PNJ.                                      |
| MaxTimesAsked\* | No          | Cuántas veces lo puedes preguntar. Si es 0, será infinito. |
| Location        | No          | La pregunta sólo aparece en este lugar.                    |
| From            | No          | La hora desde la cual se puede preguntar esto.             |
| To              | No          | Hora límite para preguntarlo.                              | |
| QuestToStart    | No          | Misión que agregar después de la pregunta.                 |
| TriggerAction   | No          | Trigger action que llamar.                                 |
| GameStateQuery  | No          | Condiciones GSQ para mostrar la pregunta.                  |

\*= Si incluyes una misión, `MaxTimesAsked` debe ser 1.

Plantilla:

```
"nombreDePregunta": {
          "Question": ,
          "Answer": ,
          "MaxTimesAsked": ,
          "Location": ,
          "From": ,
          "To": ,
          "QuestToStart": ,
          "TriggerAction": ,
          "GameStateQuery": 
        },
```

Just remove any fields you won't be using.
**Importante:** If you don't want the question to appear every day, use ContentPatcher's "When" field.

------------

### Ejemplo 1

This will add a question, which can only be asked twice (per day), and only when in Elliott's cabin.
```
{
      "Action": "EditData",
      "Target": "mistyspring.dynamicdialogues/Questions/Elliott",
      "Entries": {
        "rainyday": {
          "Question": "What do you think of rainy days?",
          "Answer": "My, they're quite gloomy....$2",
          "Location": "ElliottHouse",
          "MaxTimesAsked": 2
        }
      },
      "When":{
        "Weather":"Rain",
        "Hearts:Elliott": "{{Range: 3, 14}}"
      }
    },

```

------------

### Ejemplo 2

This will add "template" questions for sandy.
The first one can be asked forever, but the second one will only appear once.
```
{
      "Action":"EditData",
      "Target":"mistyspring.dynamicdialogues/Questions/Sandy",
      "Entries":{
        "questionTest1": {
          "Question": "Hey, this is a test question.",
          "Answer": "This is an answer."
        },
        "questionTest2": {
          "Question": "How about a limited question?",
          "Answer": "Never thought of it.",
          "MaxTimesAsked": 1
        }
      }
    }
```
