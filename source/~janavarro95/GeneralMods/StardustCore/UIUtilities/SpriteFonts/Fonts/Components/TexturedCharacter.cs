using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardustCore.UIUtilities.SpriteFonts.Components
{
    public class TexturedCharacter
    {
        public char character;
        public string pathToTexture;
        public Texture2D texture;
        public CharacterSpacing spacing;
        public Color drawColor;
        public Vector2 position;
        
        public TexturedCharacter()
        {

        }

        public TexturedCharacter(char Character,string PathToTexture,Color color)
        {
            this.character = Character;
            this.pathToTexture = PathToTexture;
            string text = this.pathToTexture.Remove(0, 1);
            this.texture = StardustCore.ModCore.ModHelper.Content.Load<Texture2D>(text+".png");
            this.spacing = new CharacterSpacing();
            this.drawColor = color;
            this.position = new Vector2();
        }

        public TexturedCharacter(char Character, string PathToTexture,Color color,int left, int right,int top, int bottom)
        {
            this.character = Character;
            this.pathToTexture = PathToTexture;
            string text = this.pathToTexture.Remove(0, 1);
            this.texture = StardustCore.ModCore.ModHelper.Content.Load<Texture2D>(text + ".png");
            this.spacing = new CharacterSpacing(left,right,top,bottom);
            this.drawColor = color;
            this.position = new Vector2();
        }

        public static TexturedCharacter Copy(TexturedCharacter original)
        {
            TexturedCharacter copy = new TexturedCharacter(original.character,original.pathToTexture,original.drawColor);
            copy.spacing = new CharacterSpacing(original.spacing.LeftPadding, original.spacing.RightPadding, original.spacing.TopPadding, original.spacing.BottomPadding);
            return copy;
        }


        public void draw(SpriteBatch b)
        {
            b.Draw(this.texture, this.position, this.drawColor);
        }
    }
}
