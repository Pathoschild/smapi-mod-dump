using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardustCore.UIUtilities.SpriteFonts.Components;

namespace StardustCore.UIUtilities.SpriteFonts.CharacterSheets
{
    public class VanillaCharacterSheet :GenericCharacterSheets
    {

        public VanillaCharacterSheet(string directoryToFonts)
        {
            Color fontColor = StardustCore.IlluminateFramework.Colors.getColorFromList("Black");
            this.CharacterAtlus = new Dictionary<char, TexturedCharacter>();
            this.CharacterAtlus.Add('0', new TexturedCharacter('0', Path.Combine(Utilities.getRelativePath(directoryToFonts), "0"), fontColor));
            this.CharacterAtlus.Add('1', new TexturedCharacter('1', Path.Combine(Utilities.getRelativePath(directoryToFonts), "1"), fontColor));
            this.CharacterAtlus.Add('2', new TexturedCharacter('2', Path.Combine(Utilities.getRelativePath(directoryToFonts), "2"), fontColor));
            this.CharacterAtlus.Add('3', new TexturedCharacter('3', Path.Combine(Utilities.getRelativePath(directoryToFonts), "3"), fontColor));
            this.CharacterAtlus.Add('4', new TexturedCharacter('4', Path.Combine(Utilities.getRelativePath(directoryToFonts), "4"), fontColor));
            this.CharacterAtlus.Add('5', new TexturedCharacter('5', Path.Combine(Utilities.getRelativePath(directoryToFonts), "5"), fontColor));
            this.CharacterAtlus.Add('6', new TexturedCharacter('6', Path.Combine(Utilities.getRelativePath(directoryToFonts), "6"), fontColor));
            this.CharacterAtlus.Add('7', new TexturedCharacter('7', Path.Combine(Utilities.getRelativePath(directoryToFonts), "7"), fontColor));
            this.CharacterAtlus.Add('8', new TexturedCharacter('8', Path.Combine(Utilities.getRelativePath(directoryToFonts), "8"), fontColor));
            this.CharacterAtlus.Add('9', new TexturedCharacter('9', Path.Combine(Utilities.getRelativePath(directoryToFonts), "9"), fontColor));

            this.CharacterAtlus.Add('&', new TexturedCharacter('&', Path.Combine(Utilities.getRelativePath(directoryToFonts), "ampersand"), fontColor));
            this.CharacterAtlus.Add('*', new TexturedCharacter('*', Path.Combine(Utilities.getRelativePath(directoryToFonts), "asterisk"), fontColor));
            this.CharacterAtlus.Add('\\', new TexturedCharacter('\\', Path.Combine(Utilities.getRelativePath(directoryToFonts), "backSlash"), fontColor));

            this.CharacterAtlus.Add('A', new TexturedCharacter('A', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalA"), fontColor));
            this.CharacterAtlus.Add('B', new TexturedCharacter('B', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalB"), fontColor));
            this.CharacterAtlus.Add('C', new TexturedCharacter('C', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalC"), fontColor));
            this.CharacterAtlus.Add('D', new TexturedCharacter('D', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalD"), fontColor));
            this.CharacterAtlus.Add('E', new TexturedCharacter('E', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalE"), fontColor));
            this.CharacterAtlus.Add('F', new TexturedCharacter('F', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalF"), fontColor));
            this.CharacterAtlus.Add('G', new TexturedCharacter('G', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalG"), fontColor));
            this.CharacterAtlus.Add('H', new TexturedCharacter('H', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalH"), fontColor));
            this.CharacterAtlus.Add('I', new TexturedCharacter('I', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalI"), fontColor));
            this.CharacterAtlus.Add('J', new TexturedCharacter('J', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalJ"), fontColor));
            this.CharacterAtlus.Add('K', new TexturedCharacter('K', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalK"), fontColor));
            this.CharacterAtlus.Add('L', new TexturedCharacter('L', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalL"), fontColor));
            this.CharacterAtlus.Add('M', new TexturedCharacter('M', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalM"), fontColor));
            this.CharacterAtlus.Add('N', new TexturedCharacter('N', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalN"), fontColor));
            this.CharacterAtlus.Add('O', new TexturedCharacter('O', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalO"), fontColor));
            this.CharacterAtlus.Add('P', new TexturedCharacter('P', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalP"), fontColor));
            this.CharacterAtlus.Add('Q', new TexturedCharacter('Q', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalQ"), fontColor));
            this.CharacterAtlus.Add('R', new TexturedCharacter('R', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalR"), fontColor));
            this.CharacterAtlus.Add('S', new TexturedCharacter('S', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalS"), fontColor));
            this.CharacterAtlus.Add('T', new TexturedCharacter('T', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalT"), fontColor));
            this.CharacterAtlus.Add('U', new TexturedCharacter('U', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalU"), fontColor));
            this.CharacterAtlus.Add('V', new TexturedCharacter('V', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalV"), fontColor));
            this.CharacterAtlus.Add('W', new TexturedCharacter('W', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalW"), fontColor));
            this.CharacterAtlus.Add('X', new TexturedCharacter('X', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalX"), fontColor));
            this.CharacterAtlus.Add('Y', new TexturedCharacter('Y', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalY"), fontColor));
            this.CharacterAtlus.Add('Z', new TexturedCharacter('Z', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalZ"), fontColor));

            this.CharacterAtlus.Add('^', new TexturedCharacter('^', Path.Combine(Utilities.getRelativePath(directoryToFonts), "caret"), fontColor));
            this.CharacterAtlus.Add(':', new TexturedCharacter(':', Path.Combine(Utilities.getRelativePath(directoryToFonts), "colon"), fontColor));
            this.CharacterAtlus.Add(',', new TexturedCharacter(',', Path.Combine(Utilities.getRelativePath(directoryToFonts), "comma"), fontColor));
            this.CharacterAtlus.Add('\"', new TexturedCharacter('\"', Path.Combine(Utilities.getRelativePath(directoryToFonts), "doubleQuotes"), fontColor));
            this.CharacterAtlus.Add('!', new TexturedCharacter('!', Path.Combine(Utilities.getRelativePath(directoryToFonts), "exclamationMark"), fontColor));
            this.CharacterAtlus.Add('/', new TexturedCharacter('/', Path.Combine(Utilities.getRelativePath(directoryToFonts), "forwardSlash"), fontColor));
            this.CharacterAtlus.Add('`', new TexturedCharacter('`', Path.Combine(Utilities.getRelativePath(directoryToFonts), "grave"), fontColor));
            this.CharacterAtlus.Add('[', new TexturedCharacter('[', Path.Combine(Utilities.getRelativePath(directoryToFonts), "leftBracket"), fontColor));
            this.CharacterAtlus.Add('{', new TexturedCharacter('{', Path.Combine(Utilities.getRelativePath(directoryToFonts), "leftCurlyBracket"), fontColor));
            this.CharacterAtlus.Add('(', new TexturedCharacter('(', Path.Combine(Utilities.getRelativePath(directoryToFonts), "leftParenthesis"), fontColor));

            this.CharacterAtlus.Add('a', new TexturedCharacter('a', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseA"), fontColor));
            this.CharacterAtlus.Add('b', new TexturedCharacter('b', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseB"), fontColor));
            this.CharacterAtlus.Add('c', new TexturedCharacter('c', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseC"), fontColor));
            this.CharacterAtlus.Add('d', new TexturedCharacter('d', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseD"), fontColor));
            this.CharacterAtlus.Add('e', new TexturedCharacter('e', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseE"), fontColor));
            this.CharacterAtlus.Add('f', new TexturedCharacter('f', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseF"), fontColor));
            this.CharacterAtlus.Add('g', new TexturedCharacter('g', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseG"), fontColor));
            this.CharacterAtlus.Add('h', new TexturedCharacter('h', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseH"), fontColor));
            this.CharacterAtlus.Add('i', new TexturedCharacter('i', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseI"), fontColor));
            this.CharacterAtlus.Add('j', new TexturedCharacter('j', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseJ"), fontColor));
            this.CharacterAtlus.Add('k', new TexturedCharacter('k', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseK"), fontColor));
            this.CharacterAtlus.Add('l', new TexturedCharacter('l', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseL"), fontColor));
            this.CharacterAtlus.Add('m', new TexturedCharacter('m', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseM"), fontColor));
            this.CharacterAtlus.Add('n', new TexturedCharacter('n', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseN"), fontColor));
            this.CharacterAtlus.Add('o', new TexturedCharacter('o', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseO"), fontColor));
            this.CharacterAtlus.Add('p', new TexturedCharacter('p', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseP"), fontColor));
            this.CharacterAtlus.Add('q', new TexturedCharacter('q', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseQ"), fontColor));
            this.CharacterAtlus.Add('r', new TexturedCharacter('r', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseR"), fontColor));
            this.CharacterAtlus.Add('s', new TexturedCharacter('s', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseS"), fontColor));
            this.CharacterAtlus.Add('t', new TexturedCharacter('t', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseT"), fontColor));
            this.CharacterAtlus.Add('u', new TexturedCharacter('u', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseU"), fontColor));
            this.CharacterAtlus.Add('v', new TexturedCharacter('v', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseV"), fontColor));
            this.CharacterAtlus.Add('w', new TexturedCharacter('w', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseW"), fontColor));
            this.CharacterAtlus.Add('x', new TexturedCharacter('x', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseX"), fontColor));
            this.CharacterAtlus.Add('y', new TexturedCharacter('y', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseY"), fontColor));
            this.CharacterAtlus.Add('z', new TexturedCharacter('z', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseZ"), fontColor));

            this.CharacterAtlus.Add('-', new TexturedCharacter('-', Path.Combine(Utilities.getRelativePath(directoryToFonts), "minus"), fontColor));
            this.CharacterAtlus.Add('%', new TexturedCharacter('%', Path.Combine(Utilities.getRelativePath(directoryToFonts), "percent"), fontColor));
            this.CharacterAtlus.Add('.', new TexturedCharacter('.', Path.Combine(Utilities.getRelativePath(directoryToFonts), "period"), fontColor));
            this.CharacterAtlus.Add('+', new TexturedCharacter('+', Path.Combine(Utilities.getRelativePath(directoryToFonts), "plus"), fontColor));
            this.CharacterAtlus.Add('#', new TexturedCharacter('#', Path.Combine(Utilities.getRelativePath(directoryToFonts), "pound"), fontColor));

            this.CharacterAtlus.Add('?', new TexturedCharacter('?', Path.Combine(Utilities.getRelativePath(directoryToFonts), "questionMark"), fontColor));
            this.CharacterAtlus.Add(']', new TexturedCharacter(']', Path.Combine(Utilities.getRelativePath(directoryToFonts), "rightBracket"), fontColor));
            this.CharacterAtlus.Add('}', new TexturedCharacter('}', Path.Combine(Utilities.getRelativePath(directoryToFonts), "rightCurlyBracket"), fontColor));

            this.CharacterAtlus.Add(')', new TexturedCharacter(')', Path.Combine(Utilities.getRelativePath(directoryToFonts), "rightParenthesis"), fontColor));

            this.CharacterAtlus.Add(';', new TexturedCharacter(';', Path.Combine(Utilities.getRelativePath(directoryToFonts), "semicolon"), fontColor));

            this.CharacterAtlus.Add('\'', new TexturedCharacter('\'', Path.Combine(Utilities.getRelativePath(directoryToFonts), "singleQuote"), fontColor));
            this.CharacterAtlus.Add(' ', new TexturedCharacter(' ', Path.Combine(Utilities.getRelativePath(directoryToFonts), "space"), fontColor));
            this.CharacterAtlus.Add('~', new TexturedCharacter('~', Path.Combine(Utilities.getRelativePath(directoryToFonts), "tilde"), fontColor));
            this.CharacterAtlus.Add('_', new TexturedCharacter('_', Path.Combine(Utilities.getRelativePath(directoryToFonts), "underScore"), fontColor));
            this.CharacterAtlus.Add('|', new TexturedCharacter('|', Path.Combine(Utilities.getRelativePath(directoryToFonts), "verticalLine"), fontColor));

            this.CharacterAtlus.Add('$', new TexturedCharacter('$', Path.Combine(Utilities.getRelativePath(directoryToFonts), "coin"), fontColor));
            this.CharacterAtlus.Add('=', new TexturedCharacter('=', Path.Combine(Utilities.getRelativePath(directoryToFonts), "star"), fontColor));
            this.CharacterAtlus.Add('@', new TexturedCharacter('@', Path.Combine(Utilities.getRelativePath(directoryToFonts), "heart"), fontColor));
        }

        public VanillaCharacterSheet(string directoryToFonts, Color fontColor)
        {
            this.CharacterAtlus = new Dictionary<char, TexturedCharacter>();
            this.CharacterAtlus.Add('0', new TexturedCharacter('0', Path.Combine(Utilities.getRelativePath(directoryToFonts), "0"), fontColor));
            this.CharacterAtlus.Add('1', new TexturedCharacter('1', Path.Combine(Utilities.getRelativePath(directoryToFonts), "1"), fontColor));
            this.CharacterAtlus.Add('2', new TexturedCharacter('2', Path.Combine(Utilities.getRelativePath(directoryToFonts), "2"), fontColor));
            this.CharacterAtlus.Add('3', new TexturedCharacter('3', Path.Combine(Utilities.getRelativePath(directoryToFonts), "3"), fontColor));
            this.CharacterAtlus.Add('4', new TexturedCharacter('4', Path.Combine(Utilities.getRelativePath(directoryToFonts), "4"), fontColor));
            this.CharacterAtlus.Add('5', new TexturedCharacter('5', Path.Combine(Utilities.getRelativePath(directoryToFonts), "5"), fontColor));
            this.CharacterAtlus.Add('6', new TexturedCharacter('6', Path.Combine(Utilities.getRelativePath(directoryToFonts), "6"), fontColor));
            this.CharacterAtlus.Add('7', new TexturedCharacter('7', Path.Combine(Utilities.getRelativePath(directoryToFonts), "7"), fontColor));
            this.CharacterAtlus.Add('8', new TexturedCharacter('8', Path.Combine(Utilities.getRelativePath(directoryToFonts), "8"), fontColor));
            this.CharacterAtlus.Add('9', new TexturedCharacter('9', Path.Combine(Utilities.getRelativePath(directoryToFonts), "9"), fontColor));

            this.CharacterAtlus.Add('&', new TexturedCharacter('&', Path.Combine(Utilities.getRelativePath(directoryToFonts),  "ampersand"), fontColor));
            this.CharacterAtlus.Add('*', new TexturedCharacter('*', Path.Combine(Utilities.getRelativePath(directoryToFonts),  "asterisk"), fontColor));
            this.CharacterAtlus.Add('\\', new TexturedCharacter('\\', Path.Combine(Utilities.getRelativePath(directoryToFonts), "backSlash"), fontColor));

            this.CharacterAtlus.Add('A', new TexturedCharacter('A', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalA"), fontColor));
            this.CharacterAtlus.Add('B', new TexturedCharacter('B', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalB"), fontColor));
            this.CharacterAtlus.Add('C', new TexturedCharacter('C', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalC"), fontColor));
            this.CharacterAtlus.Add('D', new TexturedCharacter('D', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalD"), fontColor));
            this.CharacterAtlus.Add('E', new TexturedCharacter('E', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalE"), fontColor));
            this.CharacterAtlus.Add('F', new TexturedCharacter('F', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalF"), fontColor));
            this.CharacterAtlus.Add('G', new TexturedCharacter('G', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalG"), fontColor));
            this.CharacterAtlus.Add('H', new TexturedCharacter('H', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalH"), fontColor));
            this.CharacterAtlus.Add('I', new TexturedCharacter('I', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalI"), fontColor));
            this.CharacterAtlus.Add('J', new TexturedCharacter('J', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalJ"), fontColor));
            this.CharacterAtlus.Add('K', new TexturedCharacter('K', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalK"), fontColor));
            this.CharacterAtlus.Add('L', new TexturedCharacter('L', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalL"), fontColor));
            this.CharacterAtlus.Add('M', new TexturedCharacter('M', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalM"), fontColor));
            this.CharacterAtlus.Add('N', new TexturedCharacter('N', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalN"), fontColor));
            this.CharacterAtlus.Add('O', new TexturedCharacter('O', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalO"), fontColor));
            this.CharacterAtlus.Add('P', new TexturedCharacter('P', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalP"), fontColor));
            this.CharacterAtlus.Add('Q', new TexturedCharacter('Q', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalQ"), fontColor));
            this.CharacterAtlus.Add('R', new TexturedCharacter('R', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalR"), fontColor));
            this.CharacterAtlus.Add('S', new TexturedCharacter('S', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalS"), fontColor));
            this.CharacterAtlus.Add('T', new TexturedCharacter('T', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalT"), fontColor));
            this.CharacterAtlus.Add('U', new TexturedCharacter('U', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalU"), fontColor));
            this.CharacterAtlus.Add('V', new TexturedCharacter('V', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalV"), fontColor));
            this.CharacterAtlus.Add('W', new TexturedCharacter('W', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalW"), fontColor));
            this.CharacterAtlus.Add('X', new TexturedCharacter('X', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalX"), fontColor));
            this.CharacterAtlus.Add('Y', new TexturedCharacter('Y', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalY"), fontColor));
            this.CharacterAtlus.Add('Z', new TexturedCharacter('Z', Path.Combine(Utilities.getRelativePath(directoryToFonts), "capitalZ"), fontColor));

            this.CharacterAtlus.Add('^', new TexturedCharacter('^', Path.Combine(Utilities.getRelativePath(directoryToFonts), "caret"), fontColor));
            this.CharacterAtlus.Add(':', new TexturedCharacter(':', Path.Combine(Utilities.getRelativePath(directoryToFonts), "colon"), fontColor));
            this.CharacterAtlus.Add(',', new TexturedCharacter(',', Path.Combine(Utilities.getRelativePath(directoryToFonts), "comma"), fontColor));
            this.CharacterAtlus.Add('\"', new TexturedCharacter('\"', Path.Combine(Utilities.getRelativePath(directoryToFonts), "doubleQuotes"), fontColor));
            this.CharacterAtlus.Add('!', new TexturedCharacter('!', Path.Combine(Utilities.getRelativePath(directoryToFonts), "exclamationMark"), fontColor));
            this.CharacterAtlus.Add('/', new TexturedCharacter('/', Path.Combine(Utilities.getRelativePath(directoryToFonts), "forwardSlash"), fontColor));
            this.CharacterAtlus.Add('`', new TexturedCharacter('`', Path.Combine(Utilities.getRelativePath(directoryToFonts), "grave"), fontColor));
            this.CharacterAtlus.Add('[', new TexturedCharacter('[', Path.Combine(Utilities.getRelativePath(directoryToFonts), "leftBracket"), fontColor));
            this.CharacterAtlus.Add('{', new TexturedCharacter('{', Path.Combine(Utilities.getRelativePath(directoryToFonts), "leftCurlyBracket"), fontColor));
            this.CharacterAtlus.Add('(', new TexturedCharacter('(', Path.Combine(Utilities.getRelativePath(directoryToFonts), "leftParenthesis"), fontColor));

            this.CharacterAtlus.Add('a', new TexturedCharacter('a', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseA"), fontColor));
            this.CharacterAtlus.Add('b', new TexturedCharacter('b', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseB"), fontColor));
            this.CharacterAtlus.Add('c', new TexturedCharacter('c', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseC"), fontColor));
            this.CharacterAtlus.Add('d', new TexturedCharacter('d', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseD"), fontColor));
            this.CharacterAtlus.Add('e', new TexturedCharacter('e', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseE"), fontColor));
            this.CharacterAtlus.Add('f', new TexturedCharacter('f', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseF"), fontColor));
            this.CharacterAtlus.Add('g', new TexturedCharacter('g', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseG"), fontColor));
            this.CharacterAtlus.Add('h', new TexturedCharacter('h', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseH"), fontColor));
            this.CharacterAtlus.Add('i', new TexturedCharacter('i', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseI"), fontColor));
            this.CharacterAtlus.Add('j', new TexturedCharacter('j', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseJ"), fontColor));
            this.CharacterAtlus.Add('k', new TexturedCharacter('k', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseK"), fontColor));
            this.CharacterAtlus.Add('l', new TexturedCharacter('l', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseL"), fontColor));
            this.CharacterAtlus.Add('m', new TexturedCharacter('m', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseM"), fontColor));
            this.CharacterAtlus.Add('n', new TexturedCharacter('n', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseN"), fontColor));
            this.CharacterAtlus.Add('o', new TexturedCharacter('o', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseO"), fontColor));
            this.CharacterAtlus.Add('p', new TexturedCharacter('p', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseP"), fontColor));
            this.CharacterAtlus.Add('q', new TexturedCharacter('q', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseQ"), fontColor));
            this.CharacterAtlus.Add('r', new TexturedCharacter('r', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseR"), fontColor));
            this.CharacterAtlus.Add('s', new TexturedCharacter('s', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseS"), fontColor));
            this.CharacterAtlus.Add('t', new TexturedCharacter('t', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseT"), fontColor));
            this.CharacterAtlus.Add('u', new TexturedCharacter('u', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseU"), fontColor));
            this.CharacterAtlus.Add('v', new TexturedCharacter('v', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseV"), fontColor));
            this.CharacterAtlus.Add('w', new TexturedCharacter('w', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseW"), fontColor));
            this.CharacterAtlus.Add('x', new TexturedCharacter('x', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseX"), fontColor));
            this.CharacterAtlus.Add('y', new TexturedCharacter('y', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseY"), fontColor));
            this.CharacterAtlus.Add('z', new TexturedCharacter('z', Path.Combine(Utilities.getRelativePath(directoryToFonts), "lowercaseZ"), fontColor));

            this.CharacterAtlus.Add('-', new TexturedCharacter('-', Path.Combine(Utilities.getRelativePath(directoryToFonts), "minus"), fontColor));
            this.CharacterAtlus.Add('%', new TexturedCharacter('%', Path.Combine(Utilities.getRelativePath(directoryToFonts), "percent"), fontColor));
            this.CharacterAtlus.Add('.', new TexturedCharacter('.', Path.Combine(Utilities.getRelativePath(directoryToFonts), "period"), fontColor));
            this.CharacterAtlus.Add('+', new TexturedCharacter('+', Path.Combine(Utilities.getRelativePath(directoryToFonts), "plus"), fontColor));
            this.CharacterAtlus.Add('#', new TexturedCharacter('#', Path.Combine(Utilities.getRelativePath(directoryToFonts), "pound"), fontColor));

            this.CharacterAtlus.Add('?', new TexturedCharacter('?', Path.Combine(Utilities.getRelativePath(directoryToFonts), "questionMark"), fontColor));
            this.CharacterAtlus.Add(']', new TexturedCharacter(']', Path.Combine(Utilities.getRelativePath(directoryToFonts), "rightBracket"), fontColor));
            this.CharacterAtlus.Add('}', new TexturedCharacter('}', Path.Combine(Utilities.getRelativePath(directoryToFonts), "rightCurlyBracket"), fontColor));

            this.CharacterAtlus.Add(')', new TexturedCharacter(')', Path.Combine(Utilities.getRelativePath(directoryToFonts), "rightParenthesis"), fontColor));

            this.CharacterAtlus.Add(';', new TexturedCharacter(';', Path.Combine(Utilities.getRelativePath(directoryToFonts), "semicolon"), fontColor));

            this.CharacterAtlus.Add('\'', new TexturedCharacter('\'', Path.Combine(Utilities.getRelativePath(directoryToFonts), "singleQuote"), fontColor));
            this.CharacterAtlus.Add(' ', new TexturedCharacter(' ', Path.Combine(Utilities.getRelativePath(directoryToFonts), "space"), fontColor));
            this.CharacterAtlus.Add('~', new TexturedCharacter('~', Path.Combine(Utilities.getRelativePath(directoryToFonts), "tilde"), fontColor));
            this.CharacterAtlus.Add('_', new TexturedCharacter('_', Path.Combine(Utilities.getRelativePath(directoryToFonts), "underScore"), fontColor));
            this.CharacterAtlus.Add('|', new TexturedCharacter('|', Path.Combine(Utilities.getRelativePath(directoryToFonts), "verticalLine"), fontColor));

            this.CharacterAtlus.Add('$', new TexturedCharacter('$', Path.Combine(Utilities.getRelativePath(directoryToFonts), "coin"), fontColor));
            this.CharacterAtlus.Add('=', new TexturedCharacter('=', Path.Combine(Utilities.getRelativePath(directoryToFonts), "star"), fontColor));
            this.CharacterAtlus.Add('@', new TexturedCharacter('@', Path.Combine(Utilities.getRelativePath(directoryToFonts), "heart"), fontColor));
        }

        public override TexturedCharacter getTexturedCharacter(char c)
        {
            var original = this.CharacterAtlus[c];
            return TexturedCharacter.Copy(original);
        }
    }
}
