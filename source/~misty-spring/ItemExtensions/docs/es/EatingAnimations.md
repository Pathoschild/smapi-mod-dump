**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# Eating animations

Puedes hacer que tus items tengan una animación personalizada al comer, o un color de bebida.

# Contenidos
* [Contenido por defecto](#contenido-por-defecto)
* [Changing drink color](#color-de-bebida)
* [Poner una animación](#poner-una-animación)
   * [Desde CustomFields](#via-custom-fields)
   * [Desde Data](#via-mod)
* [Crear animaciones nuevas](#crear-animaciones-nuevas)
    * [Formato](#formato)
    * [Fotogramas de personaje](#fotogramas-de-personaje)
    * [Ejemplo](#ejemplo)

--------------------

## Contenido por defecto

El mod viene con algunas animaciones base:
- base_eat      (comida)
- base_drink    (bebida)
- base_badfood  (comida mala)

## Color de bebida

Puedes mantener la animación de beber, pero cambiar el color de taza: Para eso, edita los Custom Fields del mod con el color que quieres. (Puede ser un código hex (e.j #123456), pero también acepta nombres en inglés (e.j lightblue)).

```jsonc
"CustomFields": {
  "mistyspring.ItemExtensions/DrinkColor": "green"
}
```

Con esto, debería funcionar.

## Poner una animación

Puedes hacerlo de dos formas: Via CustomFields, o con Mod data.

### Via custom fields

Solo agrega esto a los `CustomFields` de tu objeto:  `mistyspring.ItemExtensions/EatingAnimation` . La ID debe existir en los datos de animación. 

Para más información, ve [cómo agregar animaciones](#creating-a-new-animation), o [animaciones por defecto](#default-animations).

Alternativamente, puedes hacer que anime algo *después* de comer el objeto, usando `mistyspring.ItemExtensions/AfterEatingAnimation`

### Via mod

Puedes crear animaciones de comida personalizadas editando `Mods/mistyspring.ItemExtensions/EatingAnimations`.
El nombre/Id puede ser cualquiera, pero se recomienda que uses la Id de tu mod como prefijo (o sea, `{{ModId}}_nombreDeAnimacion`)

(Avanzado) También puedes poner una animación desde el campo `Eating` o `AfterEating` en `/Data` (del mod). Si cambias múltiples cosas, esto puede ser más útil.

## Crear animaciones nuevas

Para crear una animación, debes entender como funciona [el sprite de personaje](https://stardewvalleywiki.com/Modding:Farmer_sprite).

### Formato

Sigue el siguiente formato, **todo es opcional excepto Animation**:

| nombre        | tipo            | descripción                                             |
|---------------|-----------------|---------------------------------------------------------|
| Animation     | `FarmerFrame[]` | Fotogramas de personaje.                                |
| Food          | `FoodAnimation` | Animación de la comida.                                 |
| HideItem      | `bool`          | No muestra la comida. Mutualmente exclusivo con `Food`. |
| Emote         | `int`           | Emote que hacer luego de comer.                         |
| ShowMessage   | `string`        | Mostrar este mensaje.                                   |
| PlaySound     | `string`        | Tocar este sonido.                                      |
| PlayMusic     | `string`        | Cambiar música a esta.                                  |
| SoundDelay    | `int`           | Tiempo a esperar antes de sonido.                       |
| TriggerAction | `string`        | Id de trigger action a activar.                         |

### Fotogramas de personaje

Cada fotograma tiene este formato:

| nombre       | tipo   | obligatorio | descripción                               |
|--------------|--------|-------------|-------------------------------------------|
| Frame        | `int`  | Si          | ID en hoja de sprites.                    |
| Duration     | `int`  | Si          | Tiempo por el que mostrar (milisegundos). |
| SecondaryArm | `bool` | No          | Si usar el tipo de brazo alternativo.     |
| Flip         | `bool` | No          | Voltear horizontalmente.                  |
| HideArm      | `bool` | No          | Esconder brazos.                          |

### Animar comida

La comida se mueve por la pantalla, usando el rostro de tu personaje como punto de inicio.

Tiene estos campos:

| nombre        | tipo      | obligatorio | descripción                                                      |
|---------------|-----------|-------------|------------------------------------------------------------------|
| Duration      | `float`   | Si          | Duración total de animación.                                     |
| Motion        | `Vector2` | Si          | Coordenadas. Cómo mover en pantalla.                             |
| CustomTexture | `string`  | No          | Textura personalizada (optional)                                 |
| Frames        | `int`     | No          | Fotogramas, usado con textura personalizada.                     |
| Loops         | `int`     | No          | Veces que repetir animación completa.                            |
| Delay         | `int`     | No          | Tiempo a esperar antes de iniciar.                               |
| Scale         | `float`   | No          | Escala de comida. (e.j, 0.8 la mostrará al 80% de tamaño normal) |
| Crunch        | `bool`    | No          | Si mostrar los trozos de comida al final.                        |
| Color         | `string`  | No          | Tinte que aplicar.                                               |
| Transparency  | `float`   | No          | Qué tan transparente hacer la comida.                            |
| Rotation      | `float`   | No          | Rotación.                                                        |
| Offset        | `Vector2` | No          | Distancia de cara.                                               |
| Flip          | `bool`    | No          | Si voltear horizontalmente.                                      |
| StartSound    | `string`  | No          | Sonido que hacer al iniciar.                                     |
| EndSound      | `string`  | No          | Sonido que hacer al terminar.                                    |
| Speed         | `Vector2` | No          | Velocidad a la que mover (diferente para X e Y).                 |
| StopX         | `int`     | No          | Parar de mover horizontalmente luego de X pixeles.               |
| StopY         | `int`     | No          | Parar de mover verticalmente luego de Y pixeles.                 |

### Ejemplo

Esta es una animación personalizada.

```jsonc
{
  "Action": "EditData",
  "Target": "Mods/mistyspring.ItemExtensions/EatingAnimations",
  "Entries": {
    "{{ModId}}_te": {
      "Animation": [
        {
          "Frame":0,
          "Duration": 200,
          "SecondaryArm": false,
          "Flip": false
        },
        {
          "Frame":18,
          "Duration": 150,
          "SecondaryArm": false,
          "Flip": false
        },
        {
          "Frame":26,
          "Duration": 200,
          "SecondaryArm": false,
          "Flip": false
        },
        {
          "Frame":25,
          "Duration": 300,
          "SecondaryArm": false,
          "Flip": false
        },

        {
          "Frame":68,
          "Duration": 150,
          "SecondaryArm": false,
          "Flip": false
        },

        {
          "Frame":86,
          "Duration": 200,
          "SecondaryArm": false,
          "Flip": false
        },
        {
          "Frame":103,
          "Duration": 200,
          "SecondaryArm": false,
          "Flip": false
        },
        {
          "Frame":86,
          "Duration": 200,
          "SecondaryArm": false,
          "Flip": false
        },
        {
          "Frame":103,
          "Duration": 500,
          "SecondaryArm": false,
          "Flip": false
        }
      ],
      "Food": {
        //time - in milliseconds
        "Duration": 500,
        "Delay": 350,

        //sounds
        "StartSound": "dwop",
        "EndSound": "gulp",
        "Crunch": false,

        //position
        "Offset": "25, 40",
        "Flip": true,
        "Scale": 0.8,

        //movement
        "Speed": "0, 0.02",
        "Motion": "0.0, -0.9",
        "StopX": 0,
        "StopY": -5
      }
    }
  }
}
```