**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# Búsquedas de objeto
`objectHunt <key>`

Te deja crear búsquedas de objeto, como el festival del huevo o el evento de Haley.

## Contenidos
* [Formato](#formato)
    * [AfterSequenceBehavior](#aftersequencebehavior)
* [Ejemplo](#ejemplo)
    * [Plantilla](#plantilla)
* [Usar el comando](#usar-el-comando)



----------


## Formato

Los objetos usan un modelo propio:

| nombre       | tipo                    | requerido | descripción                                       |
|--------------|-------------------------|-----------|---------------------------------------------------|
| Timer        | `int`                   | no        | Límite de tiempo para la búsqueda.                |
| OnFailure    | `AfterSequenceBehavior` | no        | Qué hacer si jugador/a pierde (requiere `Timer`). |
| OnSuccess    | `AfterSequenceBehavior` | no        | Qué hacer si jugador/a completa la búsqueda.      |
| CanPlayerRun | `bool`                  | no        | Si jugador/a puede correr. Por defecto `true`.    |
| Objects      | `List<ObjectData>`      | si        | Objetos que usar en búsqueda.                     |

### AfterSequenceBehavior

Los parámetros para OnFailure/OnSuccess son los mismos:

| nombre        | tipo     | requerido | descripción                         |
|---------------|----------|-----------|-------------------------------------|
| Mail          | `string` | false     | Correo que enviar.                  |
| ImmediateMail | `bool`   | false     | Si el correo debe ser recibido hoy. |
| Flag          | `string` | false     | Flag to set.                        |
| Energy        | `int`    | false     | Energía que agregar/disminuir.      |
| Health        | `int`    | false     | Salud que agregar/disarming.        |

### Ejemplo
Para agregar tu propia búsqueda, edita `mistyspring.dynamicdialogues/Commands/objectHunt`.

Aquí, creamos una llamada "simple_test".
```json
{
  "Action": "EditData",
  "Target": "mistyspring.dynamicdialogues/Commands/objectHunt",
  "Entries": {
    "simple_test": {
      "CanPlayerRun": true, //puede correr
      "OnSuccess": {
        "Energy": "-50" //al terminar pierde 50 energía
      },
      "Objects": [
        {
          "ItemId": "(O)541",
          "X": 20,
          "Y": 6
        },
        {
          "ItemId": "(O)543",
          "X": 21,
          "Y": 6
        }
      ]
    }
  }
}
```

Luego, lo llamamos en el evento con `objectHunt simple_test`

Items 541 and 543 serán puestos en el mapa, y perderás 50 de energía al completar la búsqueda.

### Plantilla

```jsonc
"tu_llave_aquí": {
    "Timer": 0,
    "CanPlayerRun": true,
    "OnSuccess": {
        "Mail":"",
        "ImmediateMail":"",
        "Flag":"",
        "Energy": "",
        "Health":""
    },
    "OnFailure": {
        "Mail":"",
        "ImmediateMail":"",
        "Flag":"",
        "Energy": "",
        "Health":""
    },
    "Objects": [
        {
            "ItemId": "(O)",
            "X": "",
            "Y": ""
        },
        {
            "ItemId": "(O)",
            "X": "",
            "Y": ""
        },
        {
            "ItemId": "(O)",
            "X": "",
            "Y": ""
        }
    ]
}
```

### Usar el comando

En tu evento, sólo agrega `objectHunt tu_llave_aquí` y la búsqueda iniciará.
