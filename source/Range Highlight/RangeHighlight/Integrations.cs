// Copyright 2020 Jamie Taylor
using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace RangeHighlight {
    internal class Integrations {
        private ModEntry theMod;
        public Integrations(ModEntry theMod) {
            this.theMod = theMod;
            IntegratePrismaticTools();
            IntegrateBetterJunimos();
        }

        private void IntegratePrismaticTools() {
            IPrismaticToolsAPI api = theMod.helper.ModRegistry.GetApi<IPrismaticToolsAPI>("stokastic.PrismaticTools");
            if (api == null) return;
            theMod.defaultShapes.prismaticSprinkler = theMod.api.GetSquareCircle((uint)api.SprinklerRange);
        }
        private void IntegrateBetterJunimos() {
            IBetterJunimosAPI api = theMod.helper.ModRegistry.GetApi<IBetterJunimosAPI>("hawkfalcon.BetterJunimos");
            if (api == null) return;
            int r = api.GetJunimoHutMaxRadius();
            if (r > 1) {
                theMod.defaultShapes.SetJunimoRange((uint)r);
            } else {
                theMod.Monitor.Log($"ignoring nonsense value {r} from Better Junimos for Junimo Hut radius", LogLevel.Info);
            }
        }
    }

    public interface IPrismaticToolsAPI {
        int SprinklerRange { get; }
        int SprinklerIndex { get; }
    }

    public interface IBetterJunimosAPI {
        int GetJunimoHutMaxRadius();
    }
}
