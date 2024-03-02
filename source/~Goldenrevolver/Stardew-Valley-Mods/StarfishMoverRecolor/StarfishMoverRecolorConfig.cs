/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace StarfishMoverRecolor
{
    using Microsoft.Xna.Framework;

    internal class StarfishMoverRecolorConfig
    {
        public bool DebugAllowClamClickDuringSong = false;
        public bool ExactClamClickLocationCheck = true;
        public bool HideLastBigMermaid = true;
        public Vector2 Clam1Position = new(1, 8);
        public Vector2 Clam2Position = new(2, 9);
        public Vector2 Clam3Position = new(5, 7);
        public Vector2 Clam4Position = new(6, 9);
        public Vector2 Clam5Position = new(7, 8);
        public int Clam1Pitch = 300;
        public int Clam2Pitch = 600;
        public int Clam3Pitch = 800;
        public int Clam4Pitch = 1000;
        public int Clam5Pitch = 1200;
        public Color Clam1Color = new(244, 177, 245);
        public Color Clam2Color = new(215, 177, 245);
        public Color Clam3Color = new(245, 177, 207);
        public Color Clam4Color = new(177, 180, 245);
        public Color Clam5Color = new(245, 177, 240);
        public Color BackgroundSwirl1Color = new(244, 177, 245);
        public Color BackgroundSwirl2Color = new(215, 177, 245);
        public Color BackgroundSwirl3Color = new(245, 177, 207);
        public Color BackgroundSwirl4Color = new(177, 180, 245);

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