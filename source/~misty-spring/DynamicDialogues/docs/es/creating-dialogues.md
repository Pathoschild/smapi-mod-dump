**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

## Contenidos
* [Agregar diálogos](#agregar-diálogos)
  * [From-To (condición de tiempo)](#usar-from-to)
  * [Remover diálogo si PNJ se va](#usar-clearonmove)
  * [Quitar diálogo anterior](#usar-override)
  * [Agregar una animación](#usar-animaciones)


## Agregar diálogos

Para agregar diálogos, edita `mistyspring.dynamicdialogues/Dialogues/<namehere>`. 
Debe tener una llave única para asegurar que otro parche no lo sobreescriba.

| nombre        | requerido | descripción                                                                                                                        |
|---------------|-----------|------------------------------------------------------------------------------------------------------------------------------------|
| Time          | (\*)      | Hora en la que agregar diálogo.                                                                                                    |
| From          | (\*\*)    | Horario mínimo.                                                                                                                    |
| To            | (\*\*)    | Horario máximo.                                                                                                                    |
| Location      | (\*)      | Nombre de mapa donde debe estar el PNJ.                                                                                            |
| Dialogue      | Si        | Texto a mostrar.                                                                                                                   |
| ClearOnMove   | No        | Si su valor es `true`, el diálogo se removerá cuando el PNJ se mueva.                                                              |
| Override      | No        | Remueve diálogo previo.                                                                                                            |
| Force         | No        | Mostrará el diálogo aunque no estés en el lugar requerido.                                                                         |
| IsBubble      | No        | `true`/`false`. El diálogo será una burbuja sobre su cabeza.                                                                       |
| Jump          | No        | Si el valor es `true`, el PNJ salta.                                                                                               |
| Shake         | No        | Sacude por la cantidad de milsegundos (e.j Shake 1000 para 1 segundo).                                                             |
| Emote         | No        | Mostrará el ([emote dado](https://docs.google.com/spreadsheets/d/18AtLClQPuC96rJOC-A4Kb1ZkuqtTmCRFAKn9JJiFiYE/edit#gid=693962458)) |
| FaceDirection | No        | Cambia la dirección en la que mira: `0`(↑), `1`(→), `2`(↓) o `3` (←).                                                              |
| Animation     | No        | Animación de personaje.                                                                                                            |
| Conditions    | No        | Condiciones para activar el diálogo.                                                                                               |
| TriggerAction | No        | ID de la *trigger action* que usar.                                                                                                |

`*`= Necesitas usar o time o location (o ambos) para que el diálogo cargue.

`**` = Mutualmente exclusivo con "Time". Úsalo si sólo quieres que un diálogo se muestre cuando el jugador está presente.

### Condiciones

Todos los valores en `Conditions` son opcionales. Para ver un ejemplo, [ve aquí](#usar-condiciones).

### Plantilla
```
"nombreDeTuEdit": {
          "Time": ,
          "From": ,
          "To": ,
          "Location": ,
          "Dialogue": ,
          "Override": ,
          "Force": ,
          "ClearOnMove": ,
          "IsBubble": ,
          "Emote": ,
          "Shake": ,
          "Jump": ,
          "Conditions": ,
          "TriggerAction": ,
        },
```

Debes remover los campos que no usarás.

**Importante:** Si no quieres que un diálogo aparezca todos los días, usa las condiciones When de ContentPatcher's "When", o [GSQ](https://stardewvalleywiki.com/Modding:Game_state_queries).

------------

## Ejemplos

### Usar From-To

From-To sólo aplica los cambios si el jugador/a está presente dentro del rango de tiempo.
El rango puede ser cualquiera entre 610 y 2550.

_"¿Por qué no antes/después?"_: El mod agrega diálogo cuando el tiempo cambia.
- Cuando un día inicia (6 AM), aún no hay cambios de tiempo.
- A las 2600 (2AM) el día termina, por lo que no podrías ver el texto (a lo más cargarían, y luego los descartaría el juego).

**Ejemplo**

Digamos que quieres que Willy salte y diga "¡Ey!" *sólo* entre 6:10 y 8:00, en la playa. El parche sería así:

```
"willy_ey": {

          "From": 610,
          "To": 800,
          "Location": "Beach",
          "Dialogue": "¡Ey!",
          "IsBubble": true,
          "Jump": true,
        },
```
Con esto, si entras a la playa (en el rango de tiempo), Willy hará eso.

------------


### Usar ClearOnMove
Esta opción es exclusivamente para texto "en caja" (Los que aparecen al interactuar con un PNJ).
Básicamente, remueve nuestro diálogo si el PNJ se va a otro lado. Es útil si tu diálogo es específico a un mapa (para evitar mensajes "fuera de contexto").

**Ejemplo**
Esto hace que Leah diga algo en la tienda de Pierre. Si comienza a caminar (e.j a la salida), el diálogo se remueve.
```
"pricesWentUp": {

          "Location": "SeedShop",
          "Dialogue": "Hola, @. ¿De compras?#$b#...¿Subieron los precios de nuevo?$2",
          "ClearOnMove": true,
        },
```
------------

### Usar Override
Esta opción es exclusivamente para texto "en caja" (Los que aparecen al interactuar con un PNJ).
El diálogo será forzado, sin importar el actual. Útil si necesitas que un personaje tenga un diálogo en medio de su animación de rutina.

**Importante:** Esto remueve cualquier diálogo anterior. Usar con cuidado.

**Ejemplo**
Si quieres que Emily diga algo cuando está trabajando en el salón, usa Override. (De otra forma, el diálogo quedará "enterrado" bajo el de rutina).
```
"SaloonTime": {
          "Location": "Saloon",
          "Dialogue": "¿Viniste a comer?",
          "Override": true,
        },
```
------------

### Usar Animation

"Animation" le dará una animación al personaje *una vez*.
Esto funciona sólo si el personaje *no* se está moviendo.
(e.g: Si intentas que Harvey tenga una animación mientras hace ejercicio, no funcionará- porque ya se está moviendo. Similarmente, si un personaje está caminando no se animará nada.

`*` "Enabled" debe ser `true` para activar la animación.

`**` Debe ser un frame válido- si escoges uno que no existe, causará errores.

| name     | description                                 |
|----------|---------------------------------------------|
| Enabled  | Si activar la animación.                    |
| Frames   | Cuadros de animación, separado por segundo. |
| Interval | Millisegundos para cada cuadro.             |

`Frames` inicia en 0, desde el límite izquierdo superior (continúa hacia la derecha, y luego la fila siguiente. Puedes encontrar el archivo de tu personaje en la carpeta `Content/Characters`).
Si necesitas ayuda, [ve aquí](https://stardewvalleywiki.com/Modding:NPC_data#Overworld_sprites).


**Ejemplo:**
Cuando estés en la playa, Alex jugará con la grid-ball momentáneamente. Cada cuadro se mostrará por 150 ms.

```
"gridBall": {
          "Location": "Beach",
          "Dialogue": "Qué divertido.",
          "IsBubble": true,
          "Animation": 
          {
            "Enabled": true,
            "Frames": "16 17 18 19 20 21 22 22 22 22 16 17 18 19 20 21 22 23 23 23",
            "Interval": 150,
          }
}
```
