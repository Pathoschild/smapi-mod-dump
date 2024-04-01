**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# Otras extensiones
Estos cambios se hacen al archivo `Mods/mistyspring.ItemExtensions/Data`, y usan la Id de item calificada.

## Contenidos

* [Opciones](#opciones)
* [Max stack change](#max-stack-change)
* [Held over head](#held-over-head)
* [Item light](#light)
* [OnBehavior](#on-behavior)
  * [Accepted behavior types](#accepted-types)
  * [Formato](#formato)
* [Ejemplos](#ejemplos)

--------------------

## Opciones

La extension de items se agrega al archivo /Data del mod, y tiene estas opciones:

| nombre       | tipo              | obligatorio | descripción                          |
|--------------|-------------------|-------------|--------------------------------------|
| MaximumStack | `int`             | No          | Cambia la cantidad máxima sujetable. |
| HideItem     | `bool`            | No          | No muestra el item sobre la cabeza.  |
| Light        | `LightData`       | No          | Agrega luz.                          |
| OnEquip      | `OnBehavior`\*    | No          | Acción que realizar al equipar.      |
| OnUnequip    | `OnBehavior`      | No          | Acción que realizar al desequipar.   |
| OnUse        | `OnBehavior`      | No          | Acción al usar.                      |
| OnDrop       | `OnBehavior`      | No          | Acción al botar/soltar.              |
| OnPurchase   | `OnBehavior`      | No          | Acción al comprar.                   |
| Eating       | `FarmerAnimation` | No          | Animación de comida.                 |
| AfterEating  | `FarmerAnimation` | No          | Animación después de comer.          |


\* = Para ver los campos de OnBehavior, [ve aquí](#on-behavior).


## Max stack change

Por defecto, los objetos pueden guardarse de hasta 999 cantidades (mientras otros items, como herramientas, solo aceptan 1). El mod te deja cambiar eso. 

Puedes hacerlo en dos formas: Via custom fields, o datos del mod.

### Via custom fields

Sólo agrega esto a los `CustomFields` del item:

```
"mistyspring.ItemExtensions/MaximumStack":"123" //reemplaza 123 por cantidad deseada
```

### Via mod

Esto es más útil si vas a hacer múltiples cambios.

Edita `Mods/mistyspring.ItemExtensions/Data`, y agrega el campo:

```jsonc
{
  "Action": "EditData",
  "Target": "Mods/mistyspring.ItemExtensions/Data",
  "Entries": {
    "(O)CustomObject": {
      "MaximumStack": 123   //reemplaza 123 por cantidad deseada
      //(..etc de cambios)
    }
  }
}
```

## Held over head

Por defecto, los objetos se sujetan sobre la cabeza. Este mod te deja desactivarlo.

Puedes hacerlo en dos formas: Via custom fields, o datos del mod.

### Via custom fields

Sólo agrega esto a los `CustomFields` del item:

```
"mistyspring.ItemExtensions/ShowAboveHead":"false"
```

### Via mod

Esto es más útil si vas a hacer múltiples cambios.

Edita `Mods/mistyspring.ItemExtensions/Data`, y agrega el campo:

```jsonc
{
  "Action": "EditData",
  "Target": "Mods/mistyspring.ItemExtensions/Data",
  "Entries": {
    "(O)CustomObject": {
      "HideItem": true
      //(..etc of changes)
    }
  }
}
```

## Luz
Puedes agregar una luz a objetos que no sean antorchas, editando su campo "Light" en `Mods/mistyspring.ItemExtensions/Data`.
Para ver los parámetros de luz, [ve aquí](https://github.com/misty-spring/StardewMods/tree/main/ItemExtensions/docs/es/LightData.md).

## On behavior

Puedes hacer que pasen múltiples cosas si el item cumple condiciones (e.g equipado, desequipado, vendido, etc).

### Tipos de comportamiento aceptados

Puedes usar cualquiera de estos: 

- `OnEquip` (al serequipado)
- `OnUnequip` (al ser desequipado)
- `OnUse` (al ser usado)
- `OnDrop` (al ser botado fuera del inventario)
- `OnPurchase` (al ser comprado)

### Formato

**Todos los campos son opcionales.**
Los comportamientos siguen este formato:

| nombre             | tipo                      | descripción                                       |
|--------------------|---------------------------|---------------------------------------------------|
| Message            | `string`                  | Mensaje a mostrar.                                |
| Confirm            | `string`                  | Requiere `Message`. Opción para confirmar.        |
| Reject             | `string`                  | Requiere `Message`. Opción para rechazae.         |
| ReduceBy           | `int`                     | Reducir item por esta cantidad.                   |
| PlaySound          | `string`                  | Toca un sonido.                                   |
| ChangeMoney        | `string`                  | Cambia dinero\*.                                  |
| Health             | `string`                  | Cambia salud\*.                                   |
| Stamina            | `string`                  | Cambia stamina\*.                                 |
| AddItems           | `Dictionary<string, int>` | Agrega los items listados (usa Id calificada).    |
| RemoveItems        | `Dictionary<string, int>` | Remueve los items listados (usa Id calificada).   |
| PlayMusic          | `string`                  | Cambia la música del mapa a esta. (temporal)      |
| AddQuest           | `string`                  | Agrega una misión.                                |
| AddSpecialOrder    | `string`                  | Agrega una orden.                                 |
| RemoveQuest        | `string`                  | Remueve una misión.                               |
| RemoveSpecialOrder | `string`                  | Remueve una orden.                                |
| Condition          | `string`                  | Condiciones para que el comportamiento se active. |
| TriggerAction      | `string`                  | Trigger action que llamar.                        |
| ShowNote           | `NoteData`                | (AVANZADO) Muestra una nota.\**                   |

\*= Usan un formato especial: set/add, seguido de cantidad. e.g, `add 50` para agregar 50 al valor actual
\**= This is advanced: The fields for NoteData are [found here]().

## Ejemplos

Este ejemplo es de "Start in Ginger Island". Aquí, si compras una hacha mientras estás en la isla, la hacha normal se removerá de tu inventario.

```jsonc
{
  "Action": "EditData",
  "Target": "Mods/mistyspring.ItemExtensions/Data",
  "Entries": {
    "(T)CopperAxe": {       // id (hacha de cobre)
      "OnPurchase": {       // al comprar:
        "RemoveItems": {    // remover
          "(T)Axe": 1       // 1 hacha normal
        },
        "Condition": "PLAYER_LOCATION_CONTEXT Current Island" //cuando el contexto es la isla
      }
    }
  }
}
```