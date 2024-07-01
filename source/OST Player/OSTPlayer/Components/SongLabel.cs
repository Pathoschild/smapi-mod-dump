/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ProfeJavix/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace OSTPlayer
{
    public class SongLabel
    {
        private static Texture2D playIcon = null, stopIcon = null;
        private int scrollGlobalPos;
        private Rectangle playArea, nameArea, scrollBoxArea = Rectangle.Empty;
        private Song song;
        private bool highlighted = false;
        private bool clicked = false;

        public Rectangle ScrollBoxArea{
            get{return scrollBoxArea;}
            set{scrollBoxArea = value;}
        }
        public int Pos{
            get{return scrollGlobalPos;}
            set{scrollGlobalPos = value;}
        }
        public Song Song{
            get{return song;}
        }
        public bool IsPlaying{
            get{return Song.isPlaying;}
        }
        public bool Highlighted{
            get{return highlighted;}
            set{highlighted = value;}
        }

        public bool Clicked{
            get{return clicked;}
            set{clicked = value;}
        }

        public SongLabel(int scrollGlobalPos, Song song)
        {

            this.scrollGlobalPos = scrollGlobalPos;
            this.song = song;
            if (playIcon == null || stopIcon == null)
                loadIcons();
        }

        private void loadIcons()
        {
            if (playIcon == null)
            {
                playIcon = ModEntry.context.Helper.ModContent.Load<Texture2D>("assets/playIcon.png");
            }

            if (stopIcon == null)
            {
                stopIcon = ModEntry.context.Helper.ModContent.Load<Texture2D>("assets/stopIcon.png");
            }
        }

        private Texture2D getPlayBoxIcon(bool isPlaying)
        {
            return isPlaying ? stopIcon : playIcon;
        }

        public void togglePlaying()
        {
            song.isPlaying = !song.isPlaying;
        }

        private void setAreas(Rectangle scrollBoxArea)
        {
            ScrollBoxArea = scrollBoxArea;
            playArea = new Rectangle(
                scrollBoxArea.X,
                scrollBoxArea.Y,
                scrollBoxArea.Height,
                scrollBoxArea.Height
            );

            nameArea = new Rectangle(
                scrollBoxArea.X + 16 + scrollBoxArea.Height,
                scrollBoxArea.Y,
                scrollBoxArea.Width - scrollBoxArea.Height - 16,
                scrollBoxArea.Height
            );
        }

        private Color getIconColor(bool isPlaying){
            return isPlaying? Color.Red : Color.MediumSeaGreen;
        }

        private Color getHighlightColor(Color color){
            if(Highlighted){
                if(Clicked)
                    return UIUtils.getHighLightColor(color, 1.6f);
                return UIUtils.getHighLightColor(color);
            }

            return color;
        }

        public void draw(SpriteBatch b, Rectangle scrollBoxArea)
        {
            setAreas(scrollBoxArea);
            UIUtils.DrawBox(b, playArea, getHighlightColor(Color.SaddleBrown));
            UIUtils.DrawBox(b, nameArea, getHighlightColor(Color.SaddleBrown));
            b.Draw(getPlayBoxIcon(song.isPlaying), playArea, getHighlightColor(getIconColor(song.isPlaying)));
            b.DrawString(Game1.dialogueFont, song.Name, UIUtils.getCenteredText(nameArea, song.Name), getHighlightColor(Color.Black));
        }
    }
}
