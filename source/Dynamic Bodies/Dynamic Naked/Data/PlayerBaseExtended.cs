/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ribeena/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

using DynamicBodies.Patches;
using DynamicBodies.Framework;

namespace DynamicBodies.Data
{
    public class PlayerBaseExtended
    {
        public static Dictionary<string, PlayerBaseExtended> extendedFarmers = new Dictionary<string, PlayerBaseExtended>();

        public float initialLayerDepth = 0f;
        public bool overrideCheck = false;
        public Texture2D cacheImage;
        public Texture2D sourceImage;
        public Vector4[] paletteCache;
        public Dictionary<string, bool> dirtyLayers;
        public int shirt { get; set; }
        public int shirtOverlayIndex = -1;
        public int pants { get; set; }
        public int shoes { get; set; }
        public Color hair;
        public uint dhair;
        public uint lash;
        //Draw rendering
        public string sleeveLength { get; set; }
        public string shoeStyle { get; set; }
        public BodyPartStyle vanilla;
        public BodyPartStyle body;
        public BodyPartStyle face;
        public BodyPartStyle eyes;
        public BodyPartStyle ears;
        public BodyPartStyle nose;
        public BodyPartStyle arm;
        public BodyPartStyle beard;
        public BodyPartStyle bodyHair;
        public BodyPartStyle nakedUpper;
        public BodyPartStyle nakedLower;
        public BodyPartStyle hairStyle;
        public BodyPartStyle[] trinkets = null;

        //Overlays/Accessories rendering
        public Dictionary<int, Texture2D> hairTextures = new Dictionary<int, Texture2D>();
        public Texture2D lowerSkinOverlay = null;
        public string lowerSkinOverlayName = "Default";
        public bool dirty { get; set; }
        public int firstFrame = 1;
        public bool recalcVanillaRecolor { get; set; }
        public PlayerBaseExtended(Farmer who) : this(who, who.FarmerRenderer.textureName.Value) { }
        public PlayerBaseExtended(Farmer who, string baseTexture)
        {

            paletteCache = GetBasePalette();
            paletteCache[3] = Color.Transparent.ToVector4();
            paletteCache[16] = FarmerRendererPatched.changeBrightness(who.pantsColor.Value, new Color(50, 50, 50), false).ToVector4();

            body = new BodyPartStyle("body");

            vanilla = new BodyPartStyle("body");
            SetVanillaFile(baseTexture);

            face = new BodyPartStyle("face");
            eyes = new BodyPartStyle("eyes");
            ears = new BodyPartStyle("ears");
            nose = new BodyPartStyle("nose");
            arm = new BodyPartStyle("arm");
            beard = new BodyPartStyle("beard");
            bodyHair = new BodyPartStyle("bodyHair");
            nakedLower = new BodyPartStyle("nakedLower");
            nakedUpper = new BodyPartStyle("nakedUpper");
            hairStyle = new BodyPartStyle("hairStyle");
            trinkets = new BodyPartStyle[5] { new BodyPartStyle("trinket0"),
                new BodyPartStyle("trinket1"),
                new BodyPartStyle("trinket2"),
                new BodyPartStyle("trinket3"),
                new BodyPartStyle("trinket4")
            };

            dirtyLayers = new Dictionary<string, bool>() {
                { "sprite",true },
                { "baseTexture",true },
                { "eyes",true },
                { "skin",true },
                { "shoes",true },
                { "pants",true },
                { "shirt",true },
                { "face",true },
                { "arm",true },
                { "hair",true },
                { "beard",true },
                { "bodyHair",true },
                { "nakedLower",true },
                { "nameUpper",true },
                { "trinkets",true },
            };

            shirt = who.shirt.Value;
            pants = who.pants.Value;
            shoes = who.shoes.Value;
            hair = who.hairstyleColor.Value;
            dhair = 0;
            lash = 0;
            sleeveLength = "Normal";
            shoeStyle = "Normal";

            recalcVanillaRecolor = true;

            dirty = true;

            string whoID = GetKey(who);
            extendedFarmers[whoID] = this;

            if (!who.modData.ContainsKey("DB.trinket_owned")) who.modData["DB.trinket_owned"] = "Default";
        }

        public static PlayerBaseExtended Get(Farmer who)
        {
            if (extendedFarmers.ContainsKey(GetKey(who)))
            {
                return extendedFarmers[GetKey(who)];
            }
            return null;
        }

        public static PlayerBaseExtended Get(string whoID)
        {
            if (extendedFarmers.ContainsKey(whoID))
            {
                return extendedFarmers[whoID];
            }
            return null;
        }

        public static string GetKey(Farmer who)
        {
            if (who.isFakeEventActor)
            {
                return who.Name + "_fake";
            }
            return who.Name;
        }

        //Handles up to 24 colours currently
        public static Vector4[] GetBasePalette()
        {
            IRawTextureData defaultColors = ModEntry.context.Helper.ModContent.Load<IRawTextureData>($"assets\\Character\\palette_skin.png");
            Vector4[] basePalette = new Vector4[25];
            for(int i = 0; i < 25; i++)
            {
                basePalette[i] = defaultColors.Data[i].ToVector4();
                //ModEntry.debugmsg($"Added palette colour {basePalette[i].ToString()}", LogLevel.Debug);
            }
            return basePalette;
        }

        public void DefaultOptions(Farmer who)
        {
            body.SetDefault(who);
            SetVanillaFile(who.FarmerRenderer.textureName.Value);
            body.file = vanilla.file;

            face.SetDefault(who);
            eyes.SetDefault(who);
            ears.SetDefault(who);
            nose.SetDefault(who);
            arm.SetDefault(who);
            beard.SetDefault(who);
            bodyHair.SetDefault(who);
            nakedLower.SetDefault(who);
            nakedUpper.SetDefault(who);
            hairStyle.SetDefault(who);
            foreach (BodyPartStyle trinket in trinkets) { trinket.SetDefault(who); }

            who.modData["DB.lash"] = new Color(15, 10, 8).PackedValue.ToString();
        }

        public void SetModData(Farmer who, string key, string value)
        {
            bool change = false;
            if (who.modData.ContainsKey(key))
            {
                if(who.modData[key] != value)
                {
                    change = true;
                }
            } else
            {
                change = true;
            }
            who.modData[key] = value;
            if (change)
            {
                if (dirtyLayers.ContainsKey(key.Substring(3)))
                {
                    ModEntry.debugmsg($"Change happened - {key.Substring(3)}", LogLevel.Debug);
                    dirtyLayers[key.Substring(3)] = true;
                }
                switch (key)
                {
                    case "DB.eyes":
                    case "DB.eyeColorS":
                    case "DB.eyeColorR":
                    case "DB.nose":
                    case "DB.ears":
                        dirtyLayers["face"] = true;
                        dirtyLayers["baseTexture"] = true;
                        break;
                    case "DB.body":
                        dirtyLayers["baseTexture"] = true;
                        break;
                    case "DB.hairStyle":
                        dirtyLayers["hair"] = true;
                        break;
                }
                
                UpdateTextures(who);
            }
        }

        public void SetVanillaFile(string file)
        {
            vanilla.file = file.Split("\\").Last();
            if (vanilla.file == "farmer_girl_base") { vanilla.file = "farmer_base"; }
            if (vanilla.file == "farmer_girl_base_bald") { vanilla.file = "farmer_base_bald"; }
        }

        public static Color GetColorSetting(Farmer who, string name)
        {
            Color toReturn = Color.Transparent;
            if (who.modData.ContainsKey("DB."+name))
            {
                toReturn = new Color(uint.Parse(who.modData["DB."+name]));
            }
            return toReturn;
        }

        public void UpdateTextures(Farmer who)
        {
            PlayerBaseExtended pbe = PlayerBaseExtended.Get(who);

            //Set sprite dirty base 
            pbe.dirtyLayers["sprite"] = pbe.dirtyLayers["baseTexture"] || pbe.dirtyLayers["face"];

            //Check for custom body
            if (!pbe.body.OptionMatchesModData(who))
            {
                pbe.body.SetOptionFromModData(who, ModEntry.bodyOptions);
                pbe.dirtyLayers["sprite"] = true;
                pbe.dirty = true;
            }

            //Check for custom face
            if (!pbe.face.OptionMatchesModData(who))
            {
                pbe.face.SetOptionFromModData(who, ModEntry.faceOptions);
                pbe.dirtyLayers["sprite"] = true;
                pbe.dirty = true;
            }

            //Check for custom eyes
            if (!pbe.eyes.OptionMatchesModData(who))
            {
                pbe.eyes.SetOptionFromModData(who, ModEntry.eyesOptions);
                pbe.dirtyLayers["sprite"] = true;
                pbe.dirty = true;
            }

            //Check for custom ears
            if (!pbe.ears.OptionMatchesModData(who))
            {
                pbe.ears.SetOptionFromModData(who, ModEntry.earsOptions);
                pbe.dirtyLayers["sprite"] = true;
                pbe.dirty = true;
            }

            //Check for custom nose
            if (!pbe.nose.OptionMatchesModData(who))
            {
                pbe.nose.SetOptionFromModData(who, ModEntry.noseOptions);
                pbe.dirtyLayers["sprite"] = true;
                pbe.dirty = true;
            }

            //Check for custom arms
            if (!pbe.arm.OptionMatchesModData(who))
            {
                pbe.arm.SetOptionFromModData(who, ModEntry.armOptions);
                pbe.dirtyLayers["arm"] = true;
                pbe.dirty = true;
            }

            //Check for beard
            if (!pbe.beard.OptionMatchesModData(who))
            {
                pbe.beard.SetOptionFromModData(who, ModEntry.beardOptions);
                pbe.dirtyLayers["beard"] = true;
                pbe.dirty = true;
            }

            //Check for bodyhair
            if (!pbe.bodyHair.OptionMatchesModData(who))
            {
                pbe.bodyHair.SetOptionFromModData(who, ModEntry.bodyHairOptions);
                pbe.dirtyLayers["bodyHair"] = true;
            }

            //Check for hairStyle
            if (!pbe.hairStyle.OptionMatchesModData(who))
            {
                pbe.hairStyle.SetOptionFromModData(who, ModEntry.hairOptions);
                pbe.dirtyLayers["hair"] = true;
            }
            if (pbe.hairStyle.option == "Default")
            {
                //Copy the value from farmer sprite 
                pbe.hairStyle.option = "Vanilla's " + who.hair.Value;
                pbe.hairStyle.file = who.hair.Value.ToString();
                who.modData["DB." + pbe.hairStyle.name] = pbe.hairStyle.option;
            }

            if (pbe.dirtyLayers["hair"])
            {
                pbe.dirtyLayers["beard"] = true;
                pbe.dirtyLayers["bodyHair"] = true;
            }

            if (who.modData.ContainsKey("DB.lash")){
                if (who.modData["DB.lash"] != new Color(pbe.paletteCache[17]).PackedValue.ToString())
                {
                    pbe.dirtyLayers["eyes"] = true;
                }
            }

            //Check for trinkets
            int i = 0;
            foreach (BodyPartStyle trinket in pbe.trinkets) {
                if (!trinket.OptionMatchesModData(who))
                {
                    trinket.SetOptionFromModData(who, ModEntry.trinketOptions[i]);
                    pbe.dirtyLayers["trinkets"] = true;
                }
                i++;
            }

            //Check for naked overlay
            if (!pbe.nakedLower.OptionMatchesModData(who))
            {
                pbe.nakedLower.SetOptionFromModData(who, ModEntry.nudeLOptions);
                pbe.dirty = true;
            }

            if (!pbe.nakedUpper.OptionMatchesModData(who))
            {
                pbe.nakedUpper.SetOptionFromModData(who, ModEntry.nudeUOptions);
                pbe.dirty = true;
            }
        }

        public Texture2D GetHairTexture(Farmer who, int hair_style, Texture2D source_texture, Rectangle rect)
        {

            CheckHairTextures(who); //Redraw if needed

            //there's a cached image
            if (hairTextures.ContainsKey(hair_style) && hairTextures[hair_style] != null)
            {
                return hairTextures[hair_style];
            }

            hairTextures[hair_style] = RenderHair(who, source_texture, rect);
            return hairTextures[hair_style];
        }

        public Texture2D GetBodyHairTexture(Farmer who)
        {
            CheckHairTextures(who); //Flag dirty because hair colour changed

            if (bodyHair.texture == null || dirtyLayers["bodyHair"])
            {
                Texture2D bodyHairText2D = bodyHair.provider.ModContent.Load<Texture2D>($"assets\\bodyhair\\{bodyHair.file}.png");
                bodyHair.texture = RenderHair(who, bodyHairText2D, new Rectangle(0,0,bodyHairText2D.Width, bodyHairText2D.Height));
            }
            return bodyHair.texture;
        }

        //Used for vanilla scenarios
        public Texture2D GetBeardTexture(Farmer who, int beard_style, Texture2D source_texture, Rectangle rect)
        {
            CheckHairTextures(who); //Redraw if needed

            //there's a cached image
            if (beard.textures.ContainsKey(beard_style.ToString()))
            {
                return beard.textures[beard_style.ToString()];
            }

            beard.textures[beard_style.ToString()] = RenderHair(who, source_texture, rect);
            return beard.textures[beard_style.ToString()];
        }

        //Content pack scenarios
        public Texture2D GetBeardTexture(Farmer who)
        {
            CheckHairTextures(who); //Redraw if needed

            if (!beard.textures.ContainsKey(beard.option) && beard.option != "Default")
            {
                //Build a new one
                Texture2D beardText2D = beard.provider.ModContent.Load<Texture2D>($"assets\\beards\\{beard.file}.png");
                Rectangle rect = new Rectangle(0, 0, beardText2D.Width, beardText2D.Height);
                beard.textures[beard.option] = new Texture2D(Game1.graphics.GraphicsDevice, beardText2D.Width, beardText2D.Height);
                Color[] data = new Color[beardText2D.Width * beardText2D.Height];
                beardText2D.GetData(data, 0, data.Length);
                beard.textures[beard.option].SetData(data);
                beard.textures[beard.option] = RenderHair(who, beard.textures[beard.option], rect);
            }
            return beard.textures[beard.option];
        }

        public Texture2D GetHairStyleTexture(Farmer who, string type = "")
        {
            CheckHairTextures(who); //Redraw if needed

            if (type == "")
            {
                if (hairStyle.texture == null || dirtyLayers["hair"])
                {
                    Texture2D hairText2D = hairStyle.provider.ModContent.Load<Texture2D>($"Hair\\{hairStyle.file}.png");
                    hairStyle.texture = RenderHair(who, hairText2D, new Rectangle(0, 0, hairText2D.Width, hairText2D.Height));
                }
                return hairStyle.texture;
            } else
            {
                if (!hairStyle.textures.ContainsKey(type) && hairStyle.option != "Default" || dirtyLayers["hair"])
                {
                    //Build a new one
                    Texture2D hairText2D = hairStyle.provider.ModContent.Load<Texture2D>($"Hair\\{hairStyle.file}_{type}.png");
                    hairStyle.textures[type] = RenderHair(who, hairText2D, new Rectangle(0, 0, hairText2D.Width, hairText2D.Height));
                }
                return hairStyle.textures[type];
            }
        }

        public Texture2D RenderHair(Farmer who, Texture2D source_texture, Rectangle rect)
        {

            Texture2D hairText2D = null;
            if (source_texture.Height != rect.Height || source_texture.Width != rect.Width)
            {
                //Need to render a partial texture
                hairText2D = new Texture2D(Game1.graphics.GraphicsDevice, rect.Width, rect.Height);
                Color[] HairData = new Color[hairText2D.Width * hairText2D.Height];
                source_texture.GetData<Color>(source_texture.LevelCount - 1, rect, HairData, 0, hairText2D.Width * hairText2D.Height);
                hairText2D.SetData(HairData);
            }
            else
            {
                //just use the whole thing
                hairText2D = source_texture;
            }
            //Colours to replace
            Color hairdark = new Color(57, 57, 57);//default, generally dark
            if (who.modData.ContainsKey("DB.darkHair"))
            {
                hairdark = new Color(uint.Parse(who.modData["DB.darkHair"]));
            }

            Texture2D texture = null;
            //Need to render a new texture
            texture = new Texture2D(Game1.graphics.GraphicsDevice, hairText2D.Width, hairText2D.Height);

            //Use a pixel shader to handle the recolouring    
            ModEntry.hairRamp.Parameters["xColor"].SetValue(who.hairstyleColor.Value.ToVector4());
            ModEntry.hairRamp.Parameters["xDarkColor"].SetValue(hairdark.ToVector4());

            FarmerRendererPatched.renderTarget = new RenderTarget2D(Game1.graphics.GraphicsDevice, hairText2D.Width, hairText2D.Height, false, Game1.graphics.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.None);
            //Store current render targets
            RenderTargetBinding[] currentRenderTargets = Game1.graphics.GraphicsDevice.GetRenderTargets();
            Game1.graphics.GraphicsDevice.SetRenderTarget(FarmerRendererPatched.renderTarget);

            if (currentRenderTargets is not null && currentRenderTargets.Length > 0 && currentRenderTargets[0].RenderTarget is not null)
            {
                FarmerRendererPatched._cachedRenderer = currentRenderTargets[0].RenderTarget as RenderTarget2D;
            }

            Game1.graphics.GraphicsDevice.SetRenderTarget(FarmerRendererPatched.renderTarget);

            Game1.graphics.GraphicsDevice.Clear(Color.Transparent);

            using (SpriteBatch sb = new SpriteBatch(FarmerRendererPatched.renderTarget.GraphicsDevice))
            {
                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, effect: ModEntry.hairRamp);
                sb.Draw(hairText2D, new Rectangle(0, 0, hairText2D.Width, hairText2D.Height), Color.White);
                sb.End();
            }

            Color[] pixel_data = new Color[FarmerRendererPatched.renderTarget.Width * FarmerRendererPatched.renderTarget.Height];
            FarmerRendererPatched.renderTarget.GetData(pixel_data);
            texture.SetData(pixel_data);


            if (FarmerRendererPatched.renderTarget is not null)
            {
                FarmerRendererPatched.renderTarget.Dispose();
            }

            //return current render target
            Game1.graphics.GraphicsDevice.SetRenderTarget(FarmerRendererPatched._cachedRenderer);
            FarmerRendererPatched._cachedRenderer = null;

            return texture;
        }

        public void CheckHairTextures(Farmer who)
        {
            //Check for hair colour
            if (hair != who.hairstyleColor.Value)
            {
                hair = who.hairstyleColor.Value;
                ResetHairTextures();
            }

            //Check for dark hair colour
            if (who.modData.ContainsKey("DB.darkHair"))
            {
                if (dhair != uint.Parse(who.modData["DB.darkHair"]))
                {
                    dhair = uint.Parse(who.modData["DB.darkHair"]);
                    ResetHairTextures();
                }
            }
        }

        public void ResetHairTextures()
        {
            ModEntry.debugmsg($"Reset hair textures", LogLevel.Debug);

            dirtyLayers["bodyHair"] = true;

            hairTextures.Clear();
            beard.Clear();
        }

        public Texture2D GetNakedUpperTexture()
        {
            if (dirty)
            {
                nakedUpper.texture = null;
            }

            if (nakedUpper.texture == null && nakedUpper.option != "Default")
            {
                Texture2D texture = nakedUpper.provider.ModContent.Load<Texture2D>($"assets\\nakedUpper\\{nakedUpper.file}.png");
                if (texture != null)
                {
                    //recalculate the skin colours on the overlay
                    nakedUpper.texture = ApplyPaletteColors(ApplyTwoColors(texture, new Color(paletteCache[0]), new Color(paletteCache[16])));
                }
            }
            if (nakedUpper.option == "Default")
            {
                return null;
            }
            return nakedUpper.texture;
        }

        public Texture2D GetNakedLowerTexture()
        {
            if (dirty)
            {
                nakedLower.texture = null;
            }

            if (nakedLower.texture == null && nakedLower.option != "Default")
            {
                Texture2D texture = nakedLower.provider.ModContent.Load<Texture2D>($"assets\\nakedLower\\{nakedLower.file}.png");

                if (texture != null)
                {
                    if (nakedLower.CheckForOption("no skin"))
                    {
                        //Don't calculate skin colours, it doesn't have them
                        nakedLower.texture = ApplyTwoColors(texture, new Color(paletteCache[0]), new Color(paletteCache[16]));
                    }
                    else
                    {
                        //recalculate the skin colours on the overlay
                        nakedLower.texture = ApplyPaletteColors(ApplyTwoColors(texture, new Color(paletteCache[0]), new Color(paletteCache[16])));
                    }
                }
            }
            if (nakedLower.option == "Default")
            {
                return null;
            }
            return nakedLower.texture;
        }

        public Texture2D GetTrinketTexture(Farmer who, int i)
        {
            if (trinkets[i].texture == null && trinkets[i].option != "Default")
            {
                Texture2D texture = trinkets[i].provider.ModContent.Load<Texture2D>($"Trinkets\\{trinkets[i].file}.png");
                bool recolour = false;
                Color c1 = Color.White;
                Color c2 = Color.White;
                if (who.modData.ContainsKey("DB.trinket" + i + "_c1"))
                {
                    c1 = new Color(uint.Parse(who.modData["DB.trinket" + i + "_c1"]));
                }
                if (who.modData.ContainsKey("DB.trinket" + i + "_c2"))
                {
                    c2 = new Color(uint.Parse(who.modData["DB.trinket" + i + "_c2"]));
                }
                if (!c1.Equals(Color.White)) { recolour = true; }
                if (!c2.Equals(Color.White)) { recolour = true; }

                if (recolour)
                {
                    trinkets[i].texture = ApplyTwoColors(texture, c1, c2);
                } else
                {
                    trinkets[i].texture = texture;
                }
            }
            if (trinkets[i].option == "Default")
            {
                return null;
            }
            return trinkets[i].texture;
        }

        public static Texture2D ApplyPaletteColors(Texture2D source_texture)
        {
            Texture2D texture = null;
            //Need to render a new texture
            texture = new Texture2D(Game1.graphics.GraphicsDevice, source_texture.Width, source_texture.Height);

            //Use a pixel shader to handle the recolouring    
            //Assumes ModEntry.paletteSwap.Parameters["xTargetPalette"].SetValue(pbe.paletteCache); is done

            RenderTarget2D renderTarget = new RenderTarget2D(Game1.graphics.GraphicsDevice, source_texture.Width, source_texture.Height, false, Game1.graphics.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.None);
            //Store current render targets
            RenderTargetBinding[] currentRenderTargets = Game1.graphics.GraphicsDevice.GetRenderTargets();

            if (currentRenderTargets is not null && currentRenderTargets.Length > 0 && currentRenderTargets[0].RenderTarget is not null)
            {
                FarmerRendererPatched._cachedRenderer = currentRenderTargets[0].RenderTarget as RenderTarget2D;
            }

            Game1.graphics.GraphicsDevice.SetRenderTarget(renderTarget);

            Game1.graphics.GraphicsDevice.Clear(Color.Transparent);

            using (SpriteBatch sb = new SpriteBatch(renderTarget.GraphicsDevice))
            {
                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, effect: ModEntry.paletteSwap);
                sb.Draw(source_texture, new Rectangle(0, 0, source_texture.Width, source_texture.Height), Color.White);
                sb.End();
            }
            Color[] pixel_data = new Color[renderTarget.Width * renderTarget.Height];
            renderTarget.GetData(pixel_data);
            texture.SetData(pixel_data);

            if (FarmerRendererPatched.renderTarget is not null)
            {
                FarmerRendererPatched.renderTarget.Dispose();
            }
            //return current render targets
            //return current render target
            Game1.graphics.GraphicsDevice.SetRenderTarget(FarmerRendererPatched._cachedRenderer);
            FarmerRendererPatched._cachedRenderer = null;

            return texture;
        }

        private static Texture2D ApplyTwoColors(Texture2D source_texture, Color c1, Color c2)
        {
            Texture2D texture = null;
            //Need to render a new texture
            texture = new Texture2D(Game1.graphics.GraphicsDevice, source_texture.Width, source_texture.Height);

            //Use a pixel shader to handle the recolouring
            ModEntry.twoColorRamp.Parameters["xColor1"].SetValue(c1.ToVector4());
            ModEntry.twoColorRamp.Parameters["xColor2"].SetValue(c2.ToVector4());

            RenderTarget2D renderTarget = new RenderTarget2D(Game1.graphics.GraphicsDevice, source_texture.Width, source_texture.Height, false, Game1.graphics.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.None);
            //Store current render targets
            RenderTargetBinding[] currentRenderTargets = Game1.graphics.GraphicsDevice.GetRenderTargets();

            if (currentRenderTargets is not null && currentRenderTargets.Length > 0 && currentRenderTargets[0].RenderTarget is not null)
            {
                FarmerRendererPatched._cachedRenderer = currentRenderTargets[0].RenderTarget as RenderTarget2D;
            }

            Game1.graphics.GraphicsDevice.SetRenderTarget(renderTarget);

            Game1.graphics.GraphicsDevice.Clear(Color.Transparent);

            using (SpriteBatch sb = new SpriteBatch(renderTarget.GraphicsDevice))
            {
                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, effect: ModEntry.twoColorRamp);
                sb.Draw(source_texture, new Rectangle(0, 0, source_texture.Width, source_texture.Height), Color.White);
                sb.End();
            }
            Color[] pixel_data = new Color[renderTarget.Width * renderTarget.Height];
            renderTarget.GetData(pixel_data);
            texture.SetData(pixel_data);

            if (FarmerRendererPatched.renderTarget is not null)
            {
                FarmerRendererPatched.renderTarget.Dispose();
            }
            //return current render targets
            //return current render target
            Game1.graphics.GraphicsDevice.SetRenderTarget(FarmerRendererPatched._cachedRenderer);
            FarmerRendererPatched._cachedRenderer = null;

            return texture;
        }
    }

    public class BodyPartStyle
    {
        public string name;
        public string option;
        public string file;
        public IContentPack provider;
        public Texture2D texture;
        public Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        public List<string> metadata;
        public bool fullAnimation = true;
        public BodyPartStyle(string n)
        {
            name = n;
            option = "Default";
            file = "";
            textures = new Dictionary<string, Texture2D>();
        }

        public string GetOptionModData(Farmer who)
        {
            if (who == null) { return "Default"; }
            return (who.modData.ContainsKey("DB." + name) ? who.modData["DB." + name] : "Default");
        }

        public bool OptionMatchesModData(Farmer who)
        {
            return option == (who.modData.ContainsKey("DB." + name) ? who.modData["DB." + name] : "Default");
        }

        public bool CheckForOption(string option)
        {
            if (metadata == null) return false;
            return metadata.Contains(option);
        }

        public void SetOptionFromModData(Farmer who, List<ContentPackOption> options)
        {
            option = (who.modData.ContainsKey("DB." + name) ? who.modData["DB." + name] : "Default");
            //Clear the texture loaded as we are setting a new value
            if (texture != null) texture.Dispose();
            texture = null;

            if (option == "Default")
            {
                provider = null;
                file = null;
                metadata = null;
            }
            else
            {
                ContentPackOption choice = ModEntry.getContentPack(options, who.modData["DB." + name], who.IsMale);
                if (choice == null)
                {
                    //Option not installed
                    option = "Default";
                    provider = null;
                    metadata = null;
                }
                else
                {
                    provider = choice.contentPack;
                    file = choice.file;
                    metadata = choice.metadata;
                    if(metadata != null && metadata.Contains("no animation"))
                    {
                        fullAnimation = false;
                    } else
                    {
                        fullAnimation = true;
                    }
                }
            }
        }

        public void SetDefault(Farmer who)
        {
            who.modData["DB." + name] = "Default";
            option = "Default";
            provider = null;
            file = null;
        }

        public void Clear()
        {
            texture = null;
            textures.Clear();
        }
    }
}
