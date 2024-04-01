**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# Item actions

## Contenidos
* [Descripción](#descripción)
* [Cómo usar](#how-to-use)
* [Parámetros](#parameters)
* [Ejemplo](#ejemplo)

## Descripción

Las acciones de item pasan al hacer dos objetos interactuar. Esto ocurre dentro del menu: sujetas un item, y haces clic derecho sobre otro.

Si el item sujetado tiene "comportamiento personalizado" para el que le haces clic, será llamado.

## Parámetros
| nombre            | tipo           | requerido | descripción                                                    |
|-------------------|----------------|-----------|----------------------------------------------------------------|
| TargetId          | `string`       | si        | El ítem que afectar. Usa ID calificada.                        |
| RemoveAmount      | `int`          | no        | Cuántos se remueven (por ítem reemplazado). Por defecto, 0.    |
| ReplaceBy         | `string`       | no        | El ítem por que se reemplaza. Usa la ID calificada.            |
| RetainAmount      | `bool`         | no        | Si mantener la cantidad al reemplazar. Por defecto es `true`   |
| RetainQuality     | `bool`         | no        | Si mantener la calidad de item al reemplazar. Por defecto true |
| AddContextTags    | `List<string>` | no        | Etiquetas de contexto que agregar.                             |
| RemoveContextTags | `List<string>` | no        | Etiquetas de contexto que remover.                             |
| QualityChange     | `string`       | no        | Cómo cambiar la calidad. (E.j +1)                              |
| PriceChange       | `string`       | no        | Cómo cambiar precio. (E.j 5x = 5 x precio original)            |
| TextureIndex      | `int`          | no        | Cambia el índice en la imagen de textura.                      |
| PlaySound         | `string`       | no        | Toca un sonido.                                                |
| TriggerActionID   | `string`       | no        | Trigger action que llamar.                                     |
| Conditions        | `string`       | no        | Game state query que revisar.                                  |

## Cómo usar

Edita `mistyspring.ItemExtensions/MenuActions`, la llave es el ítem a usar, y la lista tiene cada item con comportamiento personalizado.

## Ejemplo

Aquí, agregamos comportamiento para la katana de lava.
Cuando le hagas click derecho a un huevo XXL café, el huevo se freirá.

Esto ocurrirá para todos los huevos en la pila, y hará un sonido de fuego.

```
{
    "Action": "EditData",
    "Target": "mistyspring.ItemExtensions/MenuActions",
    "Entries": {
        "(W)9": [
            {
                "TargetID": "(O)182", //huevo
                "ReplaceBy": "(O)194", //huevo frito
                "RetainAmount": true,
                "RetainQuality": false,
                "PlaySound": "fireball"
            }
        ]
    }
}
```