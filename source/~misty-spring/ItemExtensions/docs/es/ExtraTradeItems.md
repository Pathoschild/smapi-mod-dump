**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# Extra trade items

En el juego, sólo puedes agregar 1 item para intercambio. Este mod te permite agregar más.

## Contenidos

* [Cómo agregar](#cómo-agregar)
  * [Ejemplo](#ejemplo)
  * [Editar otra tienda](#editar-otra-tienda)
* [Tips](#tips)

---

## Cómo agregar

Puedes agregar más items de intercambio editando los *Custom Fields* del mod. (Campos personalizados)

Sólo agrega esto a sus `CustomFields` en la tienda:  `mistyspring.ItemExtensions/ExtraTrades` . Debe incluir ID y cantidad.

**Importante:** "TradeItemId" también debe tener contenido para que funcione. Esto es para requerimientos _extra_.

### Ejemplo

En este ejemplo, agregamos 2 requerimientos más. **Los campos obligatorios de ShopData fueron omitidos por legibilidad**.

Aquí, las semillas de Piña se intercambiarán por: 5 savia, 1 plátano, y 2 malezas.

```json
{
  "Action": "EditData",
  "Target": "Data/Shops",
  "Entries": {
    "MiTienda": {
      "Items":[
        {
          "Id": "Pineapple seeds",
          "ItemId": "(O)833",
          "TradeItemId": "(O)92", 
          "TradeItemAmount": 5, 
          "CustomFields": {
            "mistyspring.ItemExtensions/ExtraTrades": "(O)91 1 (O)0 2"
          }
        }
      ]
    }
  }
}
```

As a result, whenever you try to buy the seeds (in MyCustomShop), it'll require the extra items. (They'll also be shown on hover, and the trade will fail if you don't have all.)


## Editar otra tienda

Para editar la tienda de alguien más, debes usar las operaciones de texto de ContentPatcher..

También debes conocer el Id de venta.
(Por ejemplo: Durante el año 1, Clint te vende el cobre más barato: la Id de esa venta es `CopperOre_YearOne`.)

Ejemplo:

Digamos que TiendaDeAlguien vende cocos de oro, y quieres que requiera más items para intercambiar. Como antes, necesitamos su Id de venta.

```jsonc
{
  "Action": "EditData",
  "Target": "Data/Shops",
  "TargetFields": [
    "TiendaDeAlguien",      // buscamos la tienda
    "Items",                // sus items
    "The_sale_id",          // y en este item
    "CustomFields"          // los campos personalizados
  ],
  "Entries": {
    "mistyspring.ItemExtensions/ExtraTrades": "(O)88 2 (O)831 1"
  }
}
```

Aquí, cada vez que `TiendaDeAlguien` venda el item, también pedirá 2 cocos y un fruto taro.

## Tips

Debes anotar la cantidad del item, incluso si es 1.


Si te acomoda, puedes separar los items por coma o doble espacio.
Así:

```jsonc
"mistyspring.ItemExtensions/ExtraTrades": "(O)91 1, (O)0 2"
```

O así:
```jsonc
"mistyspring.ItemExtensions/ExtraTrades": "(O)91 1  (O)0 2"
```
