using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.MapData {
    public enum SpawningSeason {
        allYear,
        springOnly,
        summerOnly,
        fallOnly,
        winterOnly,
        notWinter,
        notFall,
        notSummer,
        notSpring,
        firstHalf,
        secondHalf,
        springFall,
        summerWinter,
        springWinter,
        summerFall
    }

    public enum SpawnType {
        noSpawn,
        pathTileBound,
        areaBound
    }
}
