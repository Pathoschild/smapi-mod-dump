**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

## Randomized text

Los PNJs dirán texto random cuando no tienen nada más que decir. Este texto es escogido de una lista, y ocurre cada 30 minutos en el juego.

**Cómo usar:** Sólo agrega una entrada que comience con "random" (en el archivo vainilla del personaje).

Ejemplo:

```
{
  "Action":"EditData",
  "Target":"Characters/Dialogue/Krobus",
  "Entries":{
    "Random.001": "@, ¿qué significa tu nombre?",
    "Random.002": "Nadie viene aquí...#$b#Es perfecto para esconderse.",
    "Random.003": "Mi cuerpo es sensible a la luz."
  }
}
```

El mod toma todos los diálogos que comiencen con "Random", los agrega a una lista, y escoge uno aleatoriamente.
En este caso, "Random.001" tiene un 30% de ser escogido.

Luego de ver un diálogo, no aparecerá más durante el día.
