**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# LightData

La información de luz se usa en 2 lados: En recursos custom, o como extension de objeto (En el archivo `/Data` del mod).

## Contenidos

* [Formato](#formato)
* [Ejemplo](#ejemplo)

---

## Formato

`LightData` sigue este formato:

| nombre       | tipo     | obligatorio | descripción                                                                  |
|--------------|----------|-------------|------------------------------------------------------------------------------|
| Size         | `float`  | Si          | Tamaño de brillo, en baldosas. Por defecto 1.2                               |
| Hex          | `string` | No          | Código hex para color de luz, tiene prioridad sobre RGB.                     |
| R            | `int`    | No          | Valor rojo del color. Requiere G, B.                                         |
| G            | `int`    | No          | Valor verde del color. Requiere R, B.                                        |
| B            | `int`    | No          | Valor azul del color. Requiere G, R.                                         |
| Transparency | `float`  | No          | Transparencia, puede estar entre 1.0 y 0.x (no puede ser 0). Por defecto 0.9 |

## Ejemplo

Aquí, hacemos un recurso que brilla.

```jsonc
{
  "Action": "EditData",
  "Target": "Mods/mistyspring.ItemExtensions/Resources",
  "Entries": {
    "{{ModId}}_GemStone": {
      "Name": "{{ModId}}_GemStone",
      "SpriteIndex": 1,
      "Texture": "Mods\\{{ModId}}\\Objects",
      "Health": 10,
      "Sound": "hammer",
      "BreakingSound": "stoneCrack",
      "ItemDropped": "(O){{ModId}}_Gem",
      "Tool": "Pickaxe",
      "MinDrops": 1,
      "MaxDrops": 3,
      "ExtraItems": [
        {
          "Id": "(O)107", //dinosaur egg
          "Chance": 0.9 //50%
        },
        {

        }
      ],
      "Debris": "(O)84", //"stone"
      "ContextTags": [
        "color_purple"
      ]
    }
  }
}
```
