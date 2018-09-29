using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CraftAnything
{
    public enum ReplacementFlag
    {
        Normal, CreateFurniture, CreateGardenPot, CustomObject, CustomFurniture
    };

    public class ItemReplacer
    {
        private Dictionary<string, Replacement> replacements = new Dictionary<string, Replacement>();

        public class Replacement
        {
            private static Regex parseRegex = new Regex(@"(?i)(.+) creates (?:(\d+) of )?item ((\d+)(?: as (Furniture))?|Garden Pot)");

            public string ItemToReplace { get; }
            public int ItemToCreate { get; }
            public int QuantityToCreate { get; }
            public Type CustomType { get; }
            public ReplacementFlag Flag { get; }

            public Replacement(string replace, int create, int quantity, ReplacementFlag flag, Type customType = null)
            {
                this.ItemToReplace = replace;
                this.ItemToCreate = create;
                this.QuantityToCreate = quantity;
                this.Flag = flag;
                this.CustomType = customType;
            }
        }

        public void AddReplacement(Replacement r)
        {
            replacements[r.ItemToReplace] = r;
        }

        public Item ReplaceItem(Item i)
        {
            if (i == null)
                return null;

            if (replacements.TryGetValue(i.Name, out Replacement r))
            {
                switch (r.Flag)
                {
                    case ReplacementFlag.CustomObject:
                        try
                        {
                            return Activator.CreateInstance(r.CustomType, new object[] { Vector2.Zero, r.QuantityToCreate }) as StardewValley.Object;
                        } catch (System.MissingMethodException)
                        {
                            return Activator.CreateInstance(r.CustomType, new object[] { Vector2.Zero }) as StardewValley.Object;
                        }
                    case ReplacementFlag.CreateFurniture:
                        return new Furniture(r.ItemToCreate, Vector2.Zero);
                    case ReplacementFlag.CreateGardenPot:
                        return new IndoorPot(Vector2.Zero);
                    default:
                        return new StardewValley.Object(Vector2.Zero, r.ItemToCreate, r.QuantityToCreate);
                }
            }

            return i;
        }

        // File Parsing
        private static Regex parseRegex = new Regex(@"(?i)(.+) creates (?:(\d+) of )?item ((\d+)(?: as (Furniture))?|Garden Pot)");
        private static Regex parseCustomType = new Regex(@"(?i)(.+) creates (?:(\d+) of )?item (\w[a-zA-Z0-9_]*)");

        public void ParseCustomType(string line, string packName)
        {
            var parseResult = parseCustomType.Match(line);
            if (!parseResult.Success)
            {
                System.Console.Write($"Failed to parse input line {line}");
                throw new Exception();
            }

            

            string name = parseResult.Groups[1].ToString();
            string itemToCreate = $"{packName}.{parseResult.Groups[3].ToString()}";
            System.Console.Write($"Loading transform to custom type ${itemToCreate}");
            if (!customTypes.TryGetValue(itemToCreate, out Type t)) {
                System.Console.Write($"Unable to add replacer for unregistered custom type {itemToCreate}");
                throw new Exception();
            }

            if (Int32.TryParse(parseResult.Groups[2].ToString(), out int quantity))
                this.AddReplacement(new Replacement(
                    name,
                    0,
                    quantity,
                    ReplacementFlag.CustomObject,
                    t
                    ));
            else
                this.AddReplacement(new Replacement(
                    name,
                    0,
                    1,
                    ReplacementFlag.CustomObject,
                    t
                    ));
        }

        public void ParseLine(string line, string packName)
        {
            var parseResult = parseRegex.Match(line);
            if (!parseResult.Success)
            {
                this.ParseCustomType(line, packName);
                return;
            }

            string name = parseResult.Groups[1].ToString();
            string itemToCreate = parseResult.Groups[3].ToString();
            switch (itemToCreate)
            {
                case "Garden Pot":
                    this.AddReplacement(new Replacement(name, 0, 1, ReplacementFlag.CreateGardenPot));
                    return;

                default:
                    ReplacementFlag flag = ReplacementFlag.Normal;
                    if (parseResult.Groups[5].Length > 0)
                        flag = ReplacementFlag.CreateFurniture;

                    if (Int32.TryParse(parseResult.Groups[2].ToString(), out int quantity))
                        this.AddReplacement(new Replacement(
                            name,
                            Int32.Parse(parseResult.Groups[4].ToString()),
                            quantity,
                            flag));
                    else
                        this.AddReplacement(new Replacement(
                            name,
                            Int32.Parse(parseResult.Groups[4].ToString()),
                            1,
                            flag));                        

                    break;
            }
        }

        public void LoadReplacements(string file, string packName)
        {
            string line;
            try
            {
                System.IO.StreamReader f = new System.IO.StreamReader(@file);
                while ((line = f.ReadLine()) != null)
                {
                    this.ParseLine(line, packName);
                }
            }
            catch (Exception)
            {
                System.Console.Write($"Unable to read configuration file {file}");
            }
        }

        private static Dictionary<string, Type> customTypes = new Dictionary<string, Type>();

        private bool derivesFromSDVFurniture(Type t)
        {
            if (t.BaseType == null)
                return false;

            if (t.BaseType.FullName == "StardewValley.Objects.Furniture")
                return true;

            return this.derivesFromSDVFurniture(t);
        }

        private bool derivesFromSDVObject(Type t)
        {
            if (t.BaseType == null)
                return false;

            if (t.BaseType.FullName == "StardewValley.Object")
                return true;

            return this.derivesFromSDVObject(t.BaseType);
        }

        public bool RegisterCustomType(Type t, string packName)
        {
            if (!this.derivesFromSDVObject(t))
                return false;

            string customTypeName = $"{packName}.{t.Name}";
            customTypes[customTypeName] = t;

            return true;
        }
    }
}
