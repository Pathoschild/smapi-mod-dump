**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/SpousesIsland**

----

### Cómo agregar una pareja (mod)

Primero, debes saber [cómo funcionan los horarios.](https://es.stardewvalleywiki.com/Modding:Datos_de_horarios)


1. En `manifest.json`, agrega `mistyspring.spousesisland` a "Dependencies". 
2. Agrega la clave "IslandVisit" al horario del PNJ.

Desde las 10pm, el mod hará que las parejas vayan a la cama. Sólo asegúrate de que el __último punto en el horario__ sea `IslandWest 77 41 0` (El mod hará el resto).

El horario puede incluir *cualquier* mapa de la isla (excepto el volcán).


Ejemplo:

```
{
  "Action": "EditData",
  "Target": "Characters/schedules/Krobus",
  "Entries": {
     "IslandVisit": "700 IslandFarmHouse 16 9 0/900 IslandFarmHouse 20 15 0/1200 IslandWest 39 41 0/1400 IslandWest 39 45 3/1500 IslandWest 85 39 2/1700 IslandSouth 12 27 2/a21500 IslandWest 77 41 0"
  },
```

### Lugares al azar

Si quieres que tu pareja vaya a un lugar aleatorio, usa `Custom_Random 0 0 0` como mapa.

Ejemplo:
```
"IslandVisit": "630 IslandFarmHouse 16 9 0/900 IslandFarmHouse 20 15 0/1200 Custom_Random 0 0 0/1600 Custom_Random 0 0 0/2000 Custom_Random 0 0 0/a21500 IslandWest 77 41 0/2200 IslandFarmHouse 19 6 0"
```

Aquí, el PNJ irá a la casa en la isla, y luego a tres lugares aleatorios (a las 12pm, 4pm y 8pm). Luego regresará a casa.

### Para más información

Si todavía necesitas ayuda, puedes descargar la [plantilla que el mod ofrece](https://www.nexusmods.com/stardewvalley/mods/11037?tab=files) y ver cómo funciona.
