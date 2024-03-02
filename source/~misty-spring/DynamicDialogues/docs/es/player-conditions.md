**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# Condiciones de jugador/a
Condiciones extra, relacionado al jugador/a. Se usa en notificaciones y dialogo.

## Parametros



| nombre         | tipo   | descripción             |
|----------------|--------|-------------------------|
| Hat            | string | ID del gorro.           |
| Shirt          | string | ID de polera.           |
| Pants          | string | ID de pantalón.         |
| Boots          | string | ID de botas.            |
| Rings          | string | ID de anillo(s).\*      |
| Inventory      | string | Items en inventario.\*  |
| GameStateQuery | string | Una *game state query.* |

------------

## Usar condiciones

Puedes agregar más condiciones al diálogo, como [visto antes](#condiciones).

Por ejemplo:
```
"Conditions": {
          "Shirt": "1009",
          "Rings": "520 OR 521"
}
```
Este diálogo *sólo* pasará si tu jugador/a lleva la polera con ID 1009, y los anillos 520 o 521.

Se pueden indicar objetos de mod.

## Múltiples objetos (anillos/inventario)
Algunos campos -específicamente, anillos e items- aceptan múltiples condiciones. Pueden ser "AND" u "OR".

Por ejemplo:
```
"Conditions": {
          "Inventory": "(O)680 OR (O)413 OR (O)437"
}
```

El diálogo ocurre si tienes cualquiera de estos items: 680, 413, or 437.

Al contrario de los anillos, <u>**Inventory** requiere la **ID completa.**</u>

<br>

También puedes especificar "ambos, o este" usando comillas:
```
"Conditions": {
          "Inventory": "\"(O)182 AND (W)9\" OR (F)3"
}
```
El mod lo interpretará como "`(O)182` y `(W)9`", o sólo `(F)3`.

No hay un límite para cuántos items puedes solicitar.