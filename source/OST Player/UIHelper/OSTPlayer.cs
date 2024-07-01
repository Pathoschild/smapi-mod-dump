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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace OSTPlayer
{
    internal class OSTPlayer : IClickableMenu
    {

        private const int border = 60;
        private int UIWidth;
        private int UIHeight;
        private int bgX;
        private int bgY;

        private int songXPos;
        private int songYPos;
        private int startSongYPos;
        private Rectangle scrollArea;
        private Rectangle scrollBarInitArea;
        private int scrollPos;
        private const int scrollPwr = 40;
        private int scrollLimit = 0;

        private int scrollBarStart, scrollBarEnd;
        private Rectangle scrollBarRangeArea;
        private const int songSpacing = 90;
        private ClickableComponent lblTitle;
        private ScrollBar scrollBar;
        private List<SongLabel> songlabels = new List<SongLabel>();
        public static int PlayingIndex = -1;

        public static event Action<int>? PlayingIndexChanged;
        private void NotifyPlayingIndexChanged(){
            PlayingIndexChanged?.Invoke(PlayingIndex);
        }

        public OSTPlayer(): base()
        {
            UIWidth = Game1.viewport.Width - 2 * border;
            UIHeight = Game1.viewport.Height - 2 * border;
            Vector2 topLeftPos = Utility.getTopLeftPositionForCenteringOnScreen(UIWidth, UIHeight);
            bgX = (int)topLeftPos.X;
            bgY = (int)topLeftPos.Y;
            initialize(bgX, bgY, UIWidth, UIHeight);

            setScrollArea();
            

            songXPos = scrollArea.Left + 30;
            startSongYPos = scrollArea.Top + 30;

            initSongLabels();

            scrollPos = 0;

            lblTitle = new ClickableComponent(getTitleBounds(), "OST Player");
            
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            
            if (!isWithinBounds(x, y) && !lblTitle.containsPoint(x,y)){
                Game1.playSound(closeSound);
                exitThisMenu();
            }
            else{
                if(scrollBarRangeArea.Contains(x,y))
                    scrollBar.IsScrolling = true;

                SongLabel? sl = getPointedSonglabel(x,y);

                if(sl != null && scrollArea.Contains(x,y)){

                    int index = songlabels.IndexOf(sl);
                    toggleSong(sl);
                    Game1.playSound("select");
                }

            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            scrollBar.IsScrolling = false;
        }

        public override void leftClickHeld(int x, int y)
        {
            if(!scrollBar.IsScrolling)
                return;

            scrollBar.CurrentPos = Math.Clamp(y-scrollBar.bounds.Height/2, scrollBarStart, scrollBarEnd);
            scrollBar.tryHover(scrollBar.bounds.Center.X, scrollBar.bounds.Center.Y);

            scrollPos = getScrollPosProp();

        }

        private int getScrollPosProp(){

            double scrollVal = 0 + ((double)(scrollBar.CurrentPos - scrollBarStart)) / (scrollBarEnd - scrollBarStart) * (scrollLimit - 0);
            return Math.Clamp( (int) Math.Round(scrollVal), 0, scrollLimit);

        }

        private void toggleSong(SongLabel sl)
        {
            int idx = songlabels.IndexOf(sl);
            if(PlayingIndex != -1 && idx != PlayingIndex){
                SongLabel playingSL = songlabels.ElementAt(PlayingIndex);
                playingSL.togglePlaying();
                PlayingIndex = -1;
            }
            sl.togglePlaying();
            PlayingIndex = sl.IsPlaying? idx : -1;
            NotifyPlayingIndexChanged();

        }

        public override void draw(SpriteBatch b)
        {

            drawBG(b);
            drawSongs(b);
            drawMouse(b);
            
        }

        private void drawBG(SpriteBatch spriteBatch)
        {
            using(SpriteBatch b = new SpriteBatch(Game1.graphics.GraphicsDevice))
            {
                b.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp);
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
                UIUtils.DrawBox(b, new Rectangle(bgX, bgY, UIWidth, UIHeight), Color.SaddleBrown);
                drawTitle(b);
                drawScroll(b);
                b.End();
            }

        }

        private void drawTitle(SpriteBatch b){
            Rectangle titleBounds = lblTitle.bounds;
            Vector2 textPosition = UIUtils.getCenteredText(titleBounds, lblTitle.name);

            UIUtils.DrawBox(b, titleBounds, Color.Wheat, 255, 0);
            UIUtils.DrawBox(b, titleBounds, Color.SaddleBrown, 0);
            b.DrawString(Game1.dialogueFont, lblTitle.name,
                textPosition, Color.Black
            );

        }

        private void drawScroll(SpriteBatch b){

            UIUtils.DrawBox(b, scrollArea, Color.Wheat, 255, 0);
            UIUtils.DrawBox(b, scrollArea, Color.SaddleBrown, 0);

            UIUtils.DrawBox(b, scrollBarRangeArea, Color.Wheat, 255, 0);
            UIUtils.DrawBox(b, scrollBarRangeArea, Color.SaddleBrown, 0);

            if(scrollLimit > 0)
                scrollBar.draw(b);
        }

        private void drawSongs(SpriteBatch b){

            using(SpriteBatch scrollBatch = new SpriteBatch(Game1.graphics.GraphicsDevice)){

                Rectangle defaultScissor = Game1.graphics.GraphicsDevice.ScissorRectangle;
                Game1.graphics.GraphicsDevice.ScissorRectangle = scrollArea;
                
                scrollBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, new RasterizerState{ScissorTestEnable = true });

                songYPos = startSongYPos;
                deleteBoxAreas();
                foreach (SongLabel songlbl in songlabels)
                {
                    int startPos = songlbl.Pos;
                    int endPos = startPos + 100;
                    if(endPos < scrollPos){
                        songlbl.ScrollBoxArea = Rectangle.Empty;
                        continue;
                    }
                    if(songYPos > scrollArea.Bottom)
                        break;

                    int diff = scrollPos - startPos;
                    songYPos-= (diff >= 0)? diff : 0;

                    Rectangle boxArea = getSongRectangle(songYPos);

                    highlightSonglabel(songlbl, boxArea);

                    songlbl.draw(scrollBatch, boxArea);

                    songYPos += 100 + songSpacing;
                }
                
                scrollBatch.End();
                Game1.graphics.GraphicsDevice.ScissorRectangle = defaultScissor;
            }
        }

        private void highlightSonglabel(SongLabel sl, Rectangle boxArea){

            if(scrollBar.IsScrolling)
                return;

            Vector2 mousePos = ModEntry.context.Helper.Input.GetCursorPosition().GetScaledScreenPixels();
            if(boxArea.Contains(mousePos) && scrollArea.Contains(mousePos)){
                sl.Highlighted = true;
                if(ModEntry.context.Helper.Input.IsDown(SButton.MouseLeft))
                    sl.Clicked = true;
                else
                    sl.Clicked = false;
            }else
                sl.Highlighted = false;
        }

        private void deleteBoxAreas(){
            songlabels.ForEach(sl => sl.ScrollBoxArea = Rectangle.Empty);
        }

        private void setScrollArea(){
            int x = bgX + 30;
            int y = bgY + 80;
            int wdth = UIWidth - 116;
            int hght = UIHeight - 120;
            scrollArea = new Rectangle(x,y,wdth,hght);

            scrollBarInitArea = new Rectangle(scrollArea.Right+21, y, 30, 50);
            scrollBar = new ScrollBar(scrollBarInitArea);
            scrollBarStart = scrollBarInitArea.Top;
            scrollBarEnd = scrollArea.Bottom - 50;
            scrollBarRangeArea = new Rectangle(scrollBarInitArea.X-5, scrollBarInitArea.Y, 40, scrollArea.Height);
        }

        private Rectangle getSongRectangle(int y)
        {
            int wdth = scrollArea.Width - 60;
            int hght = 100;
            return new Rectangle(
                songXPos,
                y,
                wdth,
                hght
            );
        }

        private Rectangle getTitleBounds()
        {
            int wdth = 400;
            int hght = 50;
            int x = (int)Utility.getTopLeftPositionForCenteringOnScreen(wdth, hght).X;
            int y = bgY-41;

            return new Rectangle(x, y, wdth, hght);
        }
        public override void receiveScrollWheelAction(int direction)
        {
            if(scrollBar.IsScrolling)
                return;
                
            if(direction > 0)
                ScrollUp();
            else
                ScrollDown();
            
            scrollBar.CurrentPos = getScrollBarPosProp();
        }

        private int getScrollBarPosProp()
        {
            double scrollBarVal = scrollBarStart + (double)scrollPos / scrollLimit * (scrollBarEnd - scrollBarStart);
            return Math.Clamp((int)Math.Round(scrollBarVal), scrollBarStart, scrollBarEnd);
        }

        private void ScrollUp(){
            scrollPos -= (scrollPos - scrollPwr >= 0)? scrollPwr : scrollPos;

        }
        private void ScrollDown(){
            scrollPos += (scrollLimit - scrollPos >= scrollPwr)? scrollPwr : scrollLimit - scrollPos;
        }

        private void initSongLabels(){

            int i = 0;
            PlayingIndex = -1;
            foreach (Song song in ModEntry.songs)
            {
                SongLabel songlbl = new SongLabel(i, song);
                songlabels.Add(songlbl);

                if(song.isPlaying)
                    PlayingIndex = songlabels.Count-1;

                i += 100 + songSpacing;

            }
            int diff = i - scrollArea.Height;
            scrollLimit = diff > 0? diff: 0;

        }

        private SongLabel? getPointedSonglabel(int x, int y){


            foreach (SongLabel sl in songlabels)
            {
                if(sl.ScrollBoxArea.Contains(x,y))
                    return sl;
            }

            return null;

        }
    }
}
