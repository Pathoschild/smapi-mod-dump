**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# Semillas variadas

El mod te permite agregar semillas variadas (como las que [el juego tiene](https://es.stardewvalleywiki.com/Semillas_variadas)). Esto también incluye la semilla "principal" (el item que usas), pero puede ser excluido.

## Contenidos

* [Cómo agregar](#how-to-add)
* [Via Custom Fields](#via-customfields)
  * [Ejemplo](#ejemplo)
* [Via Mod](#via-mod)
  * [Formato](#formato)
  * [Ejemplo](#ejemplo-1)
* [Excluir semilla principal](#excluir-semilla-principal) 

---

## Cómo agregar

You can add custom mixed seeds in two ways: Via custom fields and by editing the mod. The latter has more options (like conditions, weight, etc.)

## Via CustomFields

Just add this to your Object's `CustomFields`:  `mistyspring.ItemExtensions/MixedSeeds` . The seed IDs must be separated by spaces.

### Ejemplo

En este ejemplo, agregamos una nueva semilla a [Data/Objects](https://stardewvalleywiki.com/Modding:Migrate_to_Stardew_Valley_1.6#Define_a_custom_item). **Los campos requeridos de Objects fueron omitidos para ser legible**.

Esto agrega las semillas: frutilla, carambola, y mora.

```jsonc
{
  "Action": "EditData",
  "Target": "Data/Objects",
  "Entries": {
    "MiSemillaMod": {
      //(...seed data)
      "CustomFields": {
        "mistyspring.ItemExtensions/MixedSeeds": "745 486 481"
      }
    }
  }
}
```

Como resultado, cada vez que plantes `MiSemillaMod`, hay un 75% de que obtengas algo más (25% por cada semilla variada).

Esto es porque el mod toma todas las semillas "posibles":
```txt
MiSemillaMod 745 486 481
```
Y escoge una aleatoriamente.

## Via Mod

Para agregar semillas mixtas a través del mod, edita `Mods/mistyspring.ItemExtensions/MixedSeeds` . La clave debe ser la Id de tu semilla, y el valor una lista `MixedSeedData` con todas las opciones.

### Formato

`MixedSeedData` tiene este formato:

| nombre    | tipo     | obligatorio | descripción                            |
|-----------|----------|-------------|----------------------------------------|
| ItemId    | `string` | Si          | Id en Data/Objects.                    |
| Condition | `string` | No          | Consulta al estado de juego.           |
| HasMod    | `string` | No          | Sólo agregar si se instala este mod.   |
| Weight    | `int`    | No          | Cantidad de veces que agregar a lista. |

### Ejemplo

Este es un ejemplo de semillas variadas. Agregamos semillas posibles a MiSemillaMod (el item **debe** existir en Data/Objects).
Esto agregará las siguientes semillas a la lista: frutilla, carambola, y mora.

```jsonc
{
  "Action": "EditData",
  "Target": "Mods/mistyspring.ItemExtensions/MixedSeeds",
  "Entries": {
    "MiSemillaMod": [
      {
        "ItemId": "745",
        "Weight": 3
      },
      {
        "ItemId": "472",
        "HasMod": "UnMod.QueRequiere"
      },
      {
        "ItemId": "486",
        "Condition": "YEAR 2"
      },
      {
        "ItemId": "481"
      }
    ]
  }
}
```

Esto es lo que ocurre:

- Las semillas de frutilla se agregarán 3 veces.
- Las semillas de carambola sólo se agregarpan si es el año 2 o más.
- Las semillas de mora sólo se agregarpan si tienes `UnMod.QueRequiere`.

Si todas esas condiciones se cumplen, la lista de semillas se vería así:

```txt
MiSemillaMod 745 745 745 486 481
```

Pero digamos que es el año 1 y no tienes `UnMod.QueRequiere`. Entonces se verá así:

```txt
MiSemillaMod 745 745 745
```

De esa lista, se escoge 1 aleatoriamente.

(Si una condicion no es cumplida para x semilla, esa semilla específica se ignorará).

## Excluir semilla principal

La semilla que plantas se agrega automáticamente a los posibles resultados. Si no quieres eso, agrega esto a los "CustomFields" de su objeto:

```
 "mistyspring.ItemExtensions/AddMainSeed": "0"
```

(De la misma forma, si quieres agregarla más de una vez, cambia el número a esa cantidad (e.j, 2 para agregarla otra vez)).