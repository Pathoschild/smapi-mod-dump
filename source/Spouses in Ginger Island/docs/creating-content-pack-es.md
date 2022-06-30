**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/SpousesIsland**

----

### Creando un paquete de contenido
El método para crear paquetes de contenido es como el de cualquier framework: necesitas un manifest.json y un content.json.
[Aquí se encuentra una plantilla del content.json que necesitarás.](https://github.com/misty-spring/SpousesIsland/blob/main/docs/content_template.json).

## Contenido

* [Explicación](#explicacion)

  * [Datos](#datos)

  * [Diálogo](#dialogo)

  * [Rutina](#rutina)

* [Traduciendo el mod](#traducciones)

## Explicación
Spouses' Island parcha los archivos del juego de dos maneras:
- Los diálogos siempre se agregan, sin importar el día.
- Las rutinas ("Characters/schedules") sólo se editan en "días de visita" (días donde los personajes irán a la isla).

En los días de visita, la(s) pareja(s) del personaje caminarán hacia la tienda de Willy (a las 6:20). Al llegar, entran al cuarto del fondo (de la misma manera que los NPC hacen cuando visitan la playa). Luego de esto, camina hacia la casa de la isla.
Desde ese punto, todo lo que el personaje haga es configurable por el modder (e.j., a qué hora y a dónde ir). Los personajes vuelven a la casa a las 21:50 y proceden a dormir en la cama del jugador.

Puedes crear contenido para múltiples personajes en el mismo paquete. ([Ejemplo](https://github.com/misty-spring/SpousesIsland/blob/main/docs/example-contentpack.json))

### Datos
Spouses' Island's usa los siguientes datos:

nombre | descripción
-----|------------
Name | El nombre del personaje que se va a usar/modificar.
ArrivalPosition | La posición donde el personaje se pondrá cuando llegue a la isla, usa tres valores (coordenada x, coordenada y, a dónde mirar). Para más información, puedes ver [este artículo de la wiki](https://stardewvalleywiki.com/Modding:Schedule_data#Schedule_points).
ArrivalDialogue | El diálogo que dirá el personaje al terminar de caminar.
Location Name | El nombre del mapa a donde el personaje irá. __Valores permitidos:__ Cualquier mapa de la isla.
Location Time | La hora a la que el personaje _comenzará_ a moverse. __Valores permitidos:__ Entre las 10:30 y 21:40.
Location Position | La posición en la que el personaje se pondrá al llegar (similar a `ArrivalPosition`, pero para un lugar que tú indiques). __Valores permitidos:__ Cualquiera, mientras esté dentro del rango del mapa (ver [este link](https://stardewvalleywiki.com/Modding:Maps#Tile_coordinates) para más información.)
Location Dialogue | El diálogo que dirá el personaje en ese lugar.

Hay tres listas llamadas `Location<número>`: de esas, **sólo la tercera es opcional** (de lo contrario, el formato sería muy complicado.)

Si quieres un ejemplo de cómo se vería un paquete de contenido, [puedes ver este](https://github.com/misty-spring/SpousesIsland/blob/main/docs/example-contentpack.json). (Tiene dos rutinas- una con y sin traducciones).

### Diálogo
El diálogo sigue el mismo formato que los diálogos del juego. Para más información, puedes ver [Modding Dialogue](https://stardewvalleywiki.com/Modding:Dialogue#Format) en la wiki.
El diálogo es agregado cuando el juego solicita el archivo (e.j. si el juego quiere cargar `"Characters/Dialogue/Krobus"`, se editará y luego se le pasa al juego).

Por ejemplo:
```json
"ArrivalDialogue" : "Do you think we can explore this volcano?$0#$b#Willy said we shouldn't get close..$2#$b#But I still brought my sword.$1",
```
No necesitas agregar una clave de diálogo; el mod hará eso internamente.

### Rutina
La rutina sigue la misma convención que las del juego base.
Para manejar eso por ti, el mod necesita la siguiente información: 
- Nombre (Del mapa o lugar al que tu pareja irá).
- Hora (la hora a la cual el personaje comenzará a moverse).
- Posición (la coordenada a la que llegarán).
Para más información sobre cómo funcionan las rutinas, [ve aquí](https://stardewvalleywiki.com/Modding:Schedule_data#Schedule_points).

## Traducciones
Para traducir tu mod, sólo agrega una entrada a "Translations":
```json
"Translations": [
    {
        "Key":"",
        "Arrival":"",
        "Location1":"",
        "Location2":"",
        "Location3":""
    }
]
```
Cada "{}" cuenta como una traducción diferente. Puedes agregar múltiples para un sólo paquete de contenido, y tiene soporte para idiomas mod. (Sólo necesitas agregar el diálogo aquí)

Ejemplo:
```json
"Translations": [
    {
        "Key":"es",
        "Arrival":"Hola, @. El clima aquí es muy cálido.#$b#¿Harás algo más tarde?",
        "Location1":"¿Crees que podríamos quedarnos hasta mañana, @? $1",
        "Location2":"%Spouse está observando el atardecer.",
        "Location3":"Se está haciendo tarde...$0"
    }
]
```
El mod reconocerá que es una traducción al español, y la agregará al idioma respectivo.
Las traducciones siguen el mismo formato de diálogo que el del juego.
Para más información en los idiomas que el juego tiene, ve [aquí](https://github.com/misty-spring/SpousesIsland/blob/main/docs/languagecodes-es.md).
