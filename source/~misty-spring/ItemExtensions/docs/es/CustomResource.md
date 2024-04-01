**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# Resources

Los recursos personalizados pueden ser de dos tipos: 
- Nodos (o sea, recursos chicos como la madera, malezas, menas)
- Recursos grandes (como las piedras/maderas grandes)

Para modders de C#: por defecto, los recursos se tratan como objetos. Cualquier cosa más grande que 1x1 baldosas se tratará como Resource Clump.

## Contenidos

* [Formato](#formato)
  * [Required fields](#required-fields)
  * [Optional fields](#optional-fields)
* [Debris](#debris)
  * [Pre-defined](#pre-defined-debris-types)
  * [Use item texture](#item-debris)
  * [Custom animation (advanced)](#custom-animation-for-debris-advanced)
* [Adding to stats](#adding-to-stats)
* [Ejemplos](#ejemplo)
* [Spawning](#spawning)

---

## Formato

Los recursos mod son muy extensivos, pero sólo unos cuantos campos son necesarios:

### Required fields

| nombre      | tipo     | obligatorio | descripción                                                     |
|-------------|----------|-------------|-----------------------------------------------------------------|
| Width       | `int`    | Si          | Ancho del recurso, en baldosas.                                 |
| Height      | `int`    | Si          | Alto del recurso, en baldosas.                                  |
| Health      | `int`    | Si          | Golpes antes de romperse\*.                                     |
| ItemDropped | `string` | Si          | Item soltado.                                                   |
| MinDrops    | `int`    | Si          | Cantidad mínima soltada. Si no hay máximo, siempre suelta esto. |
| Tool        | `string` | Si          | Herramienta a usar.\**                                          |

\* = Por cada golpe, se reduce NivelDeMejora + 1 (e.j el hacha de iridio quita 5 HP).
Por ejemplo: la salud de las menas de cobre es 3, los de ceniza tienen 12,y la mena radioactiva tiene 25.

\** = Puede ser cualquier clase (las clases vainilla se pueden revisar en [Data/Tools](https://stardewvalleywiki.com/Modding:Migrate_to_Stardew_Valley_1.6#Custom_tools)), "any" para usar cualquiera, o un tipo de arma (e.g Hammer, Sword, etc. Alternativamente, "Weapon" te deja usar cualquier arma).

### Campos opcionales


| nombre        | tipo                         | obligatorio | descripción                      |
|---------------|------------------------------|-------------|----------------------------------|
| MaxDrops      | `int`                        | No          | Cant. máxima soltada.            |
| ExtraItems    | `List<ExtraSpawn>`           | No          | Items extra a soltar.            |
| Debris        | `string`                     | No          | Escombros que mostrar al romper. |
| BreakingSound | `string`                     | No          | Sonido al romperse.              |
| Sound         | `string`                     | No          | Sonido al recibir golpe.         |
| AddHay        | `int`                        | No          | Heno que agregar.                |
| SecretNotes   | `bool`                       | No          | Si puede soltar notas.           |
| Shake         | `bool`                       | No          | Si se sacude al golpear.         |
| CountTowards  | `StatCounter`                | No          | Estadística que aumentar\*\*\*.  |
| MinToolLevel  | `int`                        | No          | Nivel mínimo de herramienta.     |
| Exp           | `int`                        | No          | Exp que agregar al romperse.     |
| Skill         | `string`                     | No          | Habilidad a la que dar EXP.      |
| ContextTags   | `List<string>`               | No          | Etiquetas de contexto.           |
| CustomFields  | `Dictionary<string, string>` | No          | Campos personalizados.           |
| Light         | `LightData`                  | No          | Luz de item.                     |

\*\** = Las posibles estadísticas están [aquí](#adding-to-stats).

Puedes encontrar los campos de ExtraSpawns [aquí](https://github.com/misty-spring/StardewMods/tree/main/ItemExtensions/docs/es/ExtraSpawns.md).
Puedes encontrar los campos de LightData [aquí](https://github.com/misty-spring/StardewMods/tree/main/ItemExtensions/docs/es/LightData.md).

## Escombros

Los escombros pueden ser de 3 tipos: predefinido, un objeto, o personalizado (avanzado).

### Escombros pre-definidos
- coins (monedas)
- stone (piedra)
- wood  (madera)
- boulder\*  (piedras grandes)
- stump\*  (troncos)

\* = Son las versiones "grandes" de madera y piedra. Al romperse, mostrarán la animación del recurso grande rompiéndose.

Puedes darle un tinte a los escombros pre-definidos. Sólo defínelo luego del tipo de escombro: e.j, `"Debris": "stone blue""`. Acepta códigos hex y colores en inglés (sin espacios)

### Escombros de items

Puede ser cualquier item con su id calificada- se mostrará en "trozos" como al comer un item.
Por ejemplo: `"Debris":"(O)40"`

### Escombro personalizado (avanzado)

Los escombros pueden ser una animación personalizada. Para esto, debes poner el tipo de escombro como "custom", seguido de sus parámetros:

`custom <tinte> <archivo de imagen> <x> <y> <ancho> <alto> <fotogramas> [velocidad] [desvanecer por milisegundo]`

los parámetros entre `<>` son obligatorios. Los de `[]` son opcionales

Si tu archivo tiene espacios, debes ponerlo entre comillas. e.j `custom white \"Mods/Mi imagen\" (etc...)`

Ejemplo:

`"Debris" : "custom white Mods/MiMod/Imagen 0 0 16 16 5"`

Qué ocurre aquí:
- El tinte se aplica sobre el sprite. puede ser un color en inglés sin espacios (`red`, `lightgreen`), un código hex (`#123456`), o "white" para no aplicar nada.
- El mod buscará el archivo `Mods/MyMod/CustomFile` (debes cargarlo al juego).
- Empieza a 0,0 en la imagen, y tendrá tamaño 16x16.
- Usará 5 marcos/fotogramas, iniciando desde la posición que le diste.

## Aumentar estadísticas

You can add to a few stats on breaking a resource.
These are the possible types:

| nombre          | descripción                                            |
|-----------------|--------------------------------------------------------|
| None            | Ninguno. (por defecto)                                 |
| Copper          | Cobre encontrado.                                      |
| Diamonds        | Diamantes encontrados.                                 |
| GeodesBroken    | Geodas rotas.                                          |
| Gold            | Oro encontrado.                                        |
| Iridium         | Iridio encontrado.                                     |
| Iron            | Hierro encontrado.                                     |
| MysticStones    | Piedras místicas rotas.                                |
| OtherGems       | Gemas encontradas (que no sean diamantes ni esquirlas) |
| PrismaticShards | Esquirlas prismáticas encontradas.                     |
| Stone           | Piedra recolectada                                     |
| Stumps          | Troncos gigantes cortados.                             |
| Seeds           | Semillas recolectadas.                                 |
| Weeds           | Malezas destruidas.                                    |

## Ejemplo

Esta es una mena de mod.

```jsonc
{
  "Action": "EditData",
  "Target": "Mods/mistyspring.ItemExtensions/Resources",
  "Entries": {
    "MiPiedraDeGema": {
      "Texture": "Mods\\MyMod\\Objects",
      "SpriteIndex": 1,                 //índice 1 en textura
      "Health": 10,                     //10 de salud
      "Sound": "hammer",                //hace sonido tosco al pegarle
      "BreakingSound": "stoneCrack",    //suena como piedra al romperse
      "Debris": "stone",                //muestra escombros de piedra
      "ItemDropped": "(O)GemaDeMdod",   //suelta una gema de mod al romperse
      "Tool": "Pickaxe"                 //se obtiene con el pico
      //(..etc)
    }
  }
}
```

## Generar

Puedes hacer que aparezcan en el mapa editando [Los datos de lugar](https://stardewvalleywiki.com/Modding:Location_data), o a través de FTM (sólo acepta recursos 1x1).

Si los generas con FTM, ponlos bajo forage (forraje)- el mod resolverá el resto.

Si lo que generas es más grande que 1x1 baldosas, la Id de generación debe comenzar con `ItemExtension.Clump`, así:

```jsonc
{
  "Action": "EditData",
  "Target": "Data/Locations",
  "Entries": {
    "MiLugarMod": {
      // (... datos del lugar)
      "Forage": [   //forraje generado diariamente
        {
          "Chance": 0.9,                  // 90% de seleccionar este
          "Precedence": 0,                //prioridad (menor se escoge antes)
          "Id": "ItemExtension.Clump 1",  //id de generación
          "ItemId": "MyClumpId",          //recurso generado
          "MaxItems": 1                   //esto debe ser 1 siempre
        }
      ]
    }
  }
}
```

### Región para recursos grandes

Si quieres que los recursos grandes sólo se generen en un lugar, agrega esto a los Campos Personalizados del lugar:

```
"CustomFields":{
  "mistyspring.ItemExtensions/ClumpSpawnRect":"x y ancho alto"
  }
```

Por ejemplo

```
"CustomFields":{
  "mistyspring.ItemExtensions/ClumpSpawnRect":"1 12 22 8"
  }
```

Aquí, los recursos grandes sólo aparecerán entre las coordenadas 1,12 y 23,20.
