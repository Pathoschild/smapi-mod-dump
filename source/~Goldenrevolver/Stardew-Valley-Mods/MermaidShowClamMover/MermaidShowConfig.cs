/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace MermaidShowClamMover
{
    using Microsoft.Xna.Framework;

    internal class MermaidShowConfig
    {
        public bool DebugAllowClamClickDuringSong = false;
        public bool ExactClamClickLocationCheck = true;
        public bool HideLastBigMermaid = false;
        public Vector2 Clam1Position = new Vector2(1, 8);
        public Vector2 Clam2Position = new Vector2(2, 9);
        public Vector2 Clam3Position = new Vector2(5, 7);
        public Vector2 Clam4Position = new Vector2(6, 9);
        public Vector2 Clam5Position = new Vector2(7, 8);
        public int Clam1Pitch = 300;
        public int Clam2Pitch = 600;
        public int Clam3Pitch = 800;
        public int Clam4Pitch = 1000;
        public int Clam5Pitch = 1200;
        public Color Clam1Color = Color.Aquamarine;
        public Color Clam2Color = Color.LightPink;
        public Color Clam3Color = Color.Lavender;
        public Color Clam4Color = Color.CornflowerBlue;
        public Color Clam5Color = Color.LightCoral;
        public Color BackgroundSwirl1Color = Color.PaleTurquoise;
        public Color BackgroundSwirl2Color = Color.LightPink;
        public Color BackgroundSwirl3Color = Color.Lavender;
        public Color BackgroundSwirl4Color = Color.CornflowerBlue;

        internal Vector2[] ClamPositions => new Vector2[]
        {
            Clam1Position,
            Clam2Position,
            Clam3Position,
            Clam4Position,
            Clam5Position
        };

        internal int[] ClamPitches => new int[]
        {
            Clam1Pitch,
            Clam2Pitch,
            Clam3Pitch,
            Clam4Pitch,
            Clam5Pitch
        };

        internal Color[] ClamColors => new Color[]
        {
            Clam1Color,
            Clam2Color,
            Clam3Color,
            Clam4Color,
            Clam5Color
        };

        internal Color[] BackgroundSwirlColors => new Color[]
        {
            BackgroundSwirl1Color,
            BackgroundSwirl2Color,
            BackgroundSwirl3Color,
            BackgroundSwirl4Color
        };
    }
}