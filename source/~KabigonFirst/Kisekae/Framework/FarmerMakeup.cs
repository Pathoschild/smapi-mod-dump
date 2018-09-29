using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using SFarmer = StardewValley.Farmer;
using StardewModdingAPI;
using Kisekae.Config;
using System.IO;

namespace Kisekae.Framework {
    class FarmerMakeup {
        /// <summary>The target farmer.</summary>
        public SFarmer m_farmer { get; set; } = null;
        /// <summary>The farmer config.</summary>
        public LocalConfig m_config { get; set; } = null;
        
        #region Texture limiter
        /// <summary>The number of male faces available.</summary>
        public static int MaleFaceTypes { get; set; } = 2;
        /// <summary>The number of male noses available.</summary>
        public static int MaleNoseTypes { get; set; } = 3;
        /// <summary>The number of male bottoms available.</summary>
        public static int MaleBottomsTypes { get; set; } = 6;
        /// <summary>The number of male shoes available.</summary>
        public static int MaleShoeTypes { get; set; } = 2;
        /// <summary>The number of female faces available.</summary>
        public static int FemaleFaceTypes { get; set; } = 2;
        /// <summary>The number of female noses available.</summary>
        public static int FemaleNoseTypes { get; set; } = 3;
        /// <summary>The number of female bottoms available.</summary>
        public static int FemaleBottomsTypes { get; set; } = 12;
        /// <summary>The number of female shoes available.</summary>
        public static int FemaleShoeTypes { get; set; } = 4;
        /// <summary>Whether to hide the skirt options for male characters.</summary>
        public static bool HideMaleSkirts { get; set; } = false;
        #endregion

        private IMod m_env;
        private ContentHelper m_contentHelper;
        private const int s_defaultShoeColor = 2;

        /// <summary>Construct a FamerMakeup object.</summary>
        public FarmerMakeup(IMod env, ContentHelper contentHelper) {
            m_env = env;
            m_contentHelper = contentHelper;
        }

        /// <summary>Update config for first run and old version config.</summary>
        /// <returns>Returns whether the config was just initilized(i.e. first run).</returns>
        public bool UpdateCurConfig() {
            if (m_config.FirstRun) {
                m_env.Monitor.Log("First run:" + m_config.SaveName, LogLevel.Info);
                m_config.ChosenSkin[0] = m_farmer.skin;
                m_config.ChosenHairstyle[0] = m_farmer.hair;
                m_config.ChosenShirt[0] = m_farmer.shirt;
                m_config.ChosenAccessory[0] = m_farmer.accessory;
                m_config.ChosenHairColor[0] = m_farmer.hairstyleColor.Value.PackedValue;
                m_config.ChosenEyeColor[0] = m_farmer.newEyeColor.Value.PackedValue;
                m_config.ChosenBottomsColor[0] = m_farmer.pantsColor.Value.PackedValue;
                m_config.ChosenShoeColor[0] = -1;

                m_config.FirstRun = false;
                return true;
            }
            if (m_config.IsCurConfigOldVersion()) {
                m_env.Monitor.Log("Update old config:"+m_config.SaveName, LogLevel.Info);
                m_config.ChosenSkin[0] = m_farmer.skin;
                m_config.ChosenHairstyle[0] = m_farmer.hair;
                m_config.ChosenShirt[0] = m_farmer.shirt;
                m_config.ChosenHairColor[0] = m_farmer.hairstyleColor.Value.PackedValue;
                m_config.ChosenEyeColor[0] = m_farmer.newEyeColor.Value.PackedValue;
                m_config.ChosenBottomsColor[0] = m_farmer.pantsColor.Value.PackedValue;
                m_config.ChosenShoeColor[0] = -1;
            }
            return false;
        }

        /// <summary>Patch base texture for a player to reflect their custom settings.</summary>
        /// <param name="which">Which configure to use.</param>
        public void ApplyConfig(int which = 0) {
            if (UpdateCurConfig()) {
                // not necessary to apply config on first run
                return;
            }

            FixBase(false, which);
            if (m_config.ChosenHairstyle[which] != m_farmer.hair) {
                m_farmer.changeHairStyle(m_config.ChosenHairstyle[which]);
            }
            if (m_config.ChosenHairColor[which] != m_farmer.hairstyleColor.Value.PackedValue) {
                Color c = new Color(0, 0, 0) { PackedValue = m_config.ChosenHairColor[which] };
                m_farmer.changeHairColor(c);
            }
            if (m_config.ChosenBottomsColor[which] != m_farmer.pantsColor.Value.PackedValue) {
                Color c = new Color(0, 0, 0) { PackedValue = m_config.ChosenBottomsColor[which] };
                m_farmer.changePants(c);
            }
        }

        /// <summary>Load favorite config to current config.</summary>
        /// <param name="which">Which configure to load.</param>
        /// <returns>Returns whether the favorite exists and was loaded.</returns>
        public bool LoadFavorite(int which) {
            if (!m_config.HasFavSlot(which))
                return false;

            /*
            ChangeFace(m_config.ChosenFace[which]);
            ChangeNose(m_config.ChosenNose[which]);
            ChangeBottoms(m_config.ChosenBottoms[which]);
            ChangeShoes(m_config.ChosenShoes[which]);
            */
            m_config.ChosenFace[0] = m_config.ChosenFace[which];
            m_config.ChosenNose[0] = m_config.ChosenNose[which];
            m_config.ChosenBottoms[0] = m_config.ChosenBottoms[which];
            m_config.ChosenShoes[0] = m_config.ChosenShoes[which];
            FixBase(true);

            ChangeShoeColor(m_config.ChosenShoeColor[which]);
            ChangeSkinColor(m_config.ChosenSkin[which]);
            ChangeHairStyle(m_config.ChosenHairstyle[which]);
            ChangeShirt(m_config.ChosenShirt[which]);
            ChangeAccessory(m_config.ChosenAccessory[which], isLogic: false);
            ChangeHairColor(m_config.ChosenHairColor[which]);
            ChangeEyeColor(m_config.ChosenEyeColor[which]);
            ChangeBottomsColor(m_config.ChosenBottomsColor[which]);
            return true;
        }

        /// <summary>Save the current values to the given favorite.</summary>
        /// <param name="index">The favorite to save.</param>
        public void SaveFavorite(int index) {
            m_config.ChosenFace[index] = m_config.ChosenFace[0];
            m_config.ChosenNose[index] = m_config.ChosenNose[0];
            m_config.ChosenBottoms[index] = m_config.ChosenBottoms[0];
            m_config.ChosenShoes[index] = m_config.ChosenShoes[0];
            m_config.ChosenShoeColor[index] = m_config.ChosenShoeColor[0];
            m_config.ChosenSkin[index] = m_config.ChosenSkin[0];
            m_config.ChosenShirt[index] = m_config.ChosenShirt[0];
            m_config.ChosenHairstyle[index] = m_config.ChosenHairstyle[0];
            m_config.ChosenAccessory[index] = m_config.ChosenAccessory[0];
            m_config.ChosenHairColor[index] = m_config.ChosenHairColor[0];
            m_config.ChosenEyeColor[index] = m_config.ChosenEyeColor[0];
            m_config.ChosenBottomsColor[index] = m_config.ChosenBottomsColor[0];
        }

        /// <summary>Switch the player to the given gender.</summary>
        /// <param name="male">Male if <c>true</c>, else female.</param>
        /// <param name="which">Which configure to change.</param>
        public void ChangeGender(bool male, int which = 0) {
            m_farmer.changeGender(male);
            m_config.ChosenFace[which] = m_config.ChosenNose[which] = m_config.ChosenBottoms[which] = m_config.ChosenShoes[which] = 0;
            ChangeAccessory(1);

            FixBase();
        }

        #region Original single change
        /// <summary>Switch the player to the given skin color.</summary>
        /// <param name="index">The skin color index.</param>
        /// <param name="noPatch">Whether to skip patching the farmer textures.</param>
        /// <param name="which">Which configure to change.</param>
        public void ChangeSkinColor(int index, bool noPatch = false, int which = 0) {
            int n = m_contentHelper.GetNumberOfTexture(LocalConfig.Attribute.Skin);
            if (index < 0) {
                index = n - 1;
            } else if (index >= n) {
                index = 0;
            }
            m_config.ChosenSkin[which] = index;

            if (!noPatch) {
                m_farmer.changeSkinColor(index);
            }
        }

        /// <summary>Switch the player to the given hair style.</summary>
        /// <param name="index">The skin hair style.</param>
        /// <param name="noPatch">Whether to skip patching the farmer textures.</param>
        /// <param name="which">Which configure to change.</param>
        public void ChangeHairStyle(int index, bool noPatch = false, int which = 0) {
            int n = m_contentHelper.GetNumberOfTexture(LocalConfig.Attribute.Hair);
            if (index < 0) {
                index = n - 1;
            } else if (index >= n) {
                index = 0;
            }
            m_config.ChosenHairstyle[which] = index;

            if (!noPatch) {
                m_farmer.changeHairStyle(index);
            }
        }

        /// <summary>Switch the player to the given shirt.</summary>
        /// <param name="index">The skin shirt.</param>
        /// <param name="noPatch">Whether to skip patching the farmer textures.</param>
        /// <param name="which">Which configure to change.</param>
        public void ChangeShirt(int index, bool noPatch = false, int which = 0) {
            int n = m_contentHelper.GetNumberOfTexture(LocalConfig.Attribute.Shirt);
            if (index < 0) {
                index = n - 1;
            } else if (index >= n) {
                index = 0;
            }
            m_config.ChosenShirt[which] = index;

            if (!noPatch) {
                m_farmer.changeShirt(index);
            }
        }

        /// <summary>change acc logic index to texture index.</summary>
        public int AccLogicToIndex(int logic, int which = 0) {
            if (m_config.ChosenFace[which] == 0) {
                return logic - 2;
            }
            if (m_config.ChosenFace[which] == 1) {
                if (logic <= 20) {
                    return logic - 2;
                }
                return logic + 110;
            }
            return -1;
        }

        /// <summary>change acc texture index to logic index.</summary>
        public int AccIndexToLogic(int index, int which = 0) {
            if (m_config.ChosenFace[which] == 0) {
                return index + 2;
            }
            if (m_config.ChosenFace[which] == 1) {
                if (index <= 18) {
                    return index + 2;
                }
                return index - 110;
            }
            return 0;
        }

        /// <summary>Switch the player to the given accessory.</summary>
        /// <param name="index">The accessory index.</param>
        /// <param name="noPatch">Whether to skip patching the farmer textures.</param>
        /// <param name="which">Which configure to change.</param>
        public void ChangeAccessory(int index, bool noPatch = false, int which = 0, bool isLogic = true) {
            /*
            int[] LastAccessoryIndex = { 127, 239 };
            int LastCommonAccessoryIndex = 18;
            int[] FirstSpecialAccessoryIndex = { LastCommonAccessoryIndex + 1, 131 };
            const int FirstIndex = -1;

            if (logicIndex < FirstIndex) {
                logicIndex = LastAccessoryIndex[m_config.ChosenFace[which]];
            } else if (logicIndex > LastAccessoryIndex[m_config.ChosenFace[which]]) {
                logicIndex = FirstIndex;
            }
            */

            // switch between common and special area
            /*
            if (logicIndex == LastCommonAccessoryIndex + 1) {
                logicIndex = FirstSpecialAccessoryIndex[m_config.ChosenFace[which]];
            } else if (logicIndex == FirstSpecialAccessoryIndex[m_config.ChosenFace[which]] - 1) {
                logicIndex = LastCommonAccessoryIndex;
            }
            */

            int phyIndex = index;
            if (isLogic) {
                // round selection
                if (index < 1) {
                    index = 129;
                } else if (index > 129) {
                    index = 1;
                }
                phyIndex = AccLogicToIndex(index);
            }
            m_config.ChosenAccessory[which] = phyIndex;
            if (!noPatch) {
                m_farmer.accessory.Set(phyIndex);
            }
        }

        /// <summary>Switch the player to the hair color.</summary>
        /// <param name="color">The hair color.</param>
        /// <param name="noPatch">Whether to skip patching the farmer textures.</param>
        /// <param name="which">Which configure to change.</param>
        public void ChangeHairColor(uint color, bool noPatch = false, int which = 0) {
            m_config.ChosenHairColor[which] = color;

            if (!noPatch) {
                Color c = new Color(0, 0, 0) { PackedValue = color };
                m_farmer.changeHairColor(c);
            }
        }

        /// <summary>Switch the player to the hair color.</summary>
        /// <param name="color">The hair color.</param>
        /// <param name="noPatch">Whether to skip patching the farmer textures.</param>
        /// <param name="which">Which configure to change.</param>
        public void ChangeHairColor(Color color, bool noPatch = false, int which = 0) {
            m_config.ChosenHairColor[which] = color.PackedValue;

            if (!noPatch) {
                m_farmer.changeHairColor(color);
            }
        }

        /// <summary>Switch the player to the eye color.</summary>
        /// <param name="color">The eye color.</param>
        /// <param name="noPatch">Whether to skip patching the farmer textures.</param>
        /// <param name="which">Which configure to change.</param>
        public void ChangeEyeColor(uint color, bool noPatch = false, int which = 0) {
            m_config.ChosenEyeColor[which] = color;

            if (!noPatch) {
                Color c = new Color(0, 0, 0) { PackedValue = color };
                m_farmer.changeEyeColor(c);
            }
        }

        /// <summary>Switch the player to the eye color.</summary>
        /// <param name="color">The eye color.</param>
        /// <param name="noPatch">Whether to skip patching the farmer textures.</param>
        /// <param name="which">Which configure to change.</param>
        public void ChangeEyeColor(Color color, bool noPatch = false, int which = 0) {
            m_config.ChosenEyeColor[which] = color.PackedValue;

            if (!noPatch) {
                m_farmer.changeEyeColor(color);
            }
        }

        /// <summary>Switch the player to the bottom color.</summary>
        /// <param name="color">The bottom color.</param>
        /// <param name="noPatch">Whether to skip patching the farmer textures.</param>
        /// <param name="which">Which configure to change.</param>
        public void ChangeBottomsColor(uint color, bool noPatch = false, int which = 0) {
            m_config.ChosenBottomsColor[which] = color;

            if (!noPatch) {
                Color c = new Color(0, 0, 0) { PackedValue = color };
                m_farmer.changePants(c);
            }
        }

        /// <summary>Switch the player to the bottom color.</summary>
        /// <param name="color">The bottom color.</param>
        /// <param name="noPatch">Whether to skip patching the farmer textures.</param>
        /// <param name="which">Which configure to change.</param>
        public void ChangeBottomsColor(Color color, bool noPatch = false, int which = 0) {
            m_config.ChosenBottomsColor[which] = color.PackedValue;

            if (!noPatch) {
                m_farmer.changePants(color);
            }
        }
        #endregion

        #region Extended single change
        /// <summary>Switch the player to the given face.</summary>
        /// <param name="index">The face index.</param>
        /// <param name="noPatch">Whether to skip patching the farmer textures.</param>
        /// <param name="which">Which configure to change.</param>
        public void ChangeFace(int index, bool noPatch = false, int which = 0) {
            if (index < 0)
                index = (m_farmer.isMale ? MaleFaceTypes : FemaleFaceTypes) - 1;
            if (index >= (m_farmer.isMale ? MaleFaceTypes : FemaleFaceTypes))
                index = 0;

            int logic = AccIndexToLogic(m_config.ChosenAccessory[which]);
            m_config.ChosenFace[which] = index;
            ChangeAccessory(logic, noPatch, which);

            if (!noPatch) {
                FixBase();
            }
        }

        /// <summary>Switch the player to the given nose.</summary>
        /// <param name="index">The nose index.</param>
        /// <param name="noPatch">Whether to skip patching the farmer textures.</param>
        /// <param name="which">Which configure to change.</param>
        public void ChangeNose(int index, bool noPatch = false, int which = 0) {
            if (index < 0)
                index = (m_farmer.isMale ? MaleNoseTypes : FemaleNoseTypes) - 1;
            if (index >= (m_farmer.isMale ? MaleNoseTypes : FemaleNoseTypes))
                index = 0;

            m_config.ChosenNose[which] = index;
            if (!noPatch) {
                FixBase();
            }
        }

        /// <summary>Switch the player to the given bottom.</summary>
        /// <param name="index">The bottom index.</param>
        /// <param name="noPatch">Whether to skip patching the farmer textures.</param>
        /// <param name="which">Which configure to change.</param>
        public void ChangeBottoms(int index, bool noPatch = false, int which = 0) {
            if (index < 0)
                index = (m_farmer.isMale ? (HideMaleSkirts ? 2 : MaleBottomsTypes) : FemaleBottomsTypes) - 1;
            if (index >= (m_farmer.isMale ? (HideMaleSkirts ? 2 : MaleBottomsTypes) : FemaleBottomsTypes))
                index = 0;

            m_config.ChosenBottoms[which] = index;
            if (!noPatch) {
                FixBase();
            }
        }

        /// <summary>Switch the player to the given shoes.</summary>
        /// <param name="index">The shoes index.</param>
        /// <param name="noPatch">Whether to skip patching the farmer textures.</param>
        /// <param name="which">Which configure to change.</param>
        public void ChangeShoes(int index, bool noPatch = false, int which = 0) {
            if (index < 0)
                index = (m_farmer.isMale ? MaleShoeTypes : FemaleShoeTypes) - 1;
            if (index >= (m_farmer.isMale ? MaleShoeTypes : FemaleShoeTypes))
                index = 0;

            m_config.ChosenShoes[which] = index;
            if (!noPatch) {
                FixBase();
            }
        }

        /// <summary>Switch the player to the given shoe color.</summary>
        /// <param name="index">The shoe color index.</param>
        /// <param name="noPatch">Whether to skip patching the farmer textures.</param>
        /// <param name="which">Which configure to change.</param>
        public void ChangeShoeColor(int index, bool noPatch = false, int which = 0) {
            int n = m_contentHelper.GetNumberOfTexture(LocalConfig.Attribute.ShoeColor);
            if (index < -1) {
                index = n;
            } else if (index > n) {
                index = -1;
            }
            m_config.ChosenShoeColor[which] = index;

            if (index == -1) {
                if (!noPatch) {
                    if (m_farmer.boots != null && m_farmer.boots.Value != null) {
                        m_farmer.changeShoeColor(m_farmer.boots.Value.indexInColorSheet);
                    } else {
                        m_farmer.changeShoeColor(s_defaultShoeColor);
                    }
                }
            } else {
                m_farmer.changeShoeColor(index);
            }
        }
        #endregion

        /// <summary>Randomize character apperance.</summary>
        public void Randomize() {
            // randomise base
            if (m_farmer.isMale) {
                //m_farmer.changeHairStyle(Game1.random.Next(16));
                ChangeFace(Game1.random.Next(MaleFaceTypes), true);
                ChangeNose(Game1.random.Next(MaleNoseTypes), true);
                ChangeBottoms(Game1.random.Next(MaleBottomsTypes), true);
                ChangeShoes(Game1.random.Next(MaleShoeTypes), true);
            } else {
                //m_farmer.changeHairStyle(Game1.random.Next(16, 32));
                ChangeFace(Game1.random.Next(FemaleFaceTypes), true);
                ChangeNose(Game1.random.Next(FemaleNoseTypes), true);
                ChangeBottoms(Game1.random.Next(FemaleBottomsTypes), true);
                ChangeShoes(Game1.random.Next(FemaleShoeTypes), true);
            }
            FixBase(true);

            // randomise accessories
            int acc = 1;
            if (Game1.random.NextDouble() < 0.88) {
                acc = Game1.random.Next(1, 129);
            }
            ChangeAccessory(acc);

            // randomise hair style
            ChangeHairStyle(m_farmer.isMale ? Game1.random.Next(16) : Game1.random.Next(16, 32));

            // randomise hair color
            Color c = new Color(Game1.random.Next(25, 254), Game1.random.Next(25, 254), Game1.random.Next(25, 254));
            if (Game1.random.NextDouble() < 0.5) {
                c.R /= 2;
                c.G /= 2;
                c.B /= 2;
            }
            if (Game1.random.NextDouble() < 0.5)
                c.R = (byte)Game1.random.Next(15, 50);
            if (Game1.random.NextDouble() < 0.5)
                c.G = (byte)Game1.random.Next(15, 50);
            if (Game1.random.NextDouble() < 0.5)
                c.B = (byte)Game1.random.Next(15, 50);
            ChangeHairColor(c);

            // randomise shirt
            ChangeShirt(Game1.random.Next(112));

            // randomise skin color
            if (Game1.random.NextDouble() < 0.25) {
                ChangeSkinColor(Game1.random.Next(24));
            } else {
                ChangeSkinColor(Game1.random.Next(6));
            }

            // randomise pants color
            Color color = new Color(Game1.random.Next(25, 254), Game1.random.Next(25, 254), Game1.random.Next(25, 254));
            if (Game1.random.NextDouble() < 0.5) {
                color.R /= 2;
                color.G /= 2;
                color.B /= 2;
            }
            if (Game1.random.NextDouble() < 0.5)
                color.R = (byte)Game1.random.Next(15, 50);
            if (Game1.random.NextDouble() < 0.5)
                color.G = (byte)Game1.random.Next(15, 50);
            if (Game1.random.NextDouble() < 0.5)
                color.B = (byte)Game1.random.Next(15, 50);
            ChangeBottomsColor(color);

            // randomize eye color
            Color c2 = new Color(Game1.random.Next(25, 254), Game1.random.Next(25, 254), Game1.random.Next(25, 254));
            c2.R /= 2;
            c2.G /= 2;
            c2.B /= 2;
            if (Game1.random.NextDouble() < 0.5)
                c2.R = (byte)Game1.random.Next(15, 50);
            if (Game1.random.NextDouble() < 0.5)
                c2.G = (byte)Game1.random.Next(15, 50);
            if (Game1.random.NextDouble() < 0.5)
                c2.B = (byte)Game1.random.Next(15, 50);
            ChangeEyeColor(c2);
        }

        public void TougleMultiplayerFix() {
            m_config.MutiplayerFix = !m_config.MutiplayerFix;
            if (!m_config.MutiplayerFix) {
                //m_env.Helper.Reflection.GetMethod(m_farmer.FarmerRenderer);
                //m_farmer.FarmerRenderer.textureName.fieldChangeEvent -= ;
                IReflectedField<string> tn = m_env.Helper.Reflection.GetField<string>(m_farmer.FarmerRenderer.textureName, "value");
                tn.SetValue(Path.Combine("Characters", "Farmer", m_farmer.IsMale ? "farmer_base" : "farmer_girl_base"));
                //m_farmer.FarmerRenderer.textureName.MarkClean();
            }
            //FixBase();
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Patch base texture for a player to reflect their custom settings.</summary>
        /// <param name="applyBaseOnly">Whether fix base dependent effects.</param>
        /// <param name="which">Which configure to use.</param>
        private void FixBase(bool applyBaseOnly = false, int which = 0) {
            m_contentHelper.PatchBaseTexture(m_farmer, m_config, which);
            if (!applyBaseOnly) {
                FixBaseDependentEffects(which);
            }
        }

        /// <summary>Force a player character to render the correct accessories.</summary>
        /// <param name="which">Which configure to use.</param>
        private void FixBaseDependentEffects(int which = 0) {
            if (which != 0 || !m_config.IsCurConfigOldVersion()) {    //use config setting
                //fix skincolor.  Assigning same color won't get update so we may assign a different color first
                //(another way is to use private method in Farmerender which is faster but unstalbe)
                int skin = m_config.ChosenSkin[which];
                if (skin == m_farmer.skin) {
                    m_farmer.changeSkinColor(skin + 1);
                }
                m_farmer.changeSkinColor(skin);

                int shirt = m_config.ChosenShirt[which];
                if (shirt == m_farmer.shirt) {
                    m_farmer.changeShirt(shirt + 1);
                }
                m_farmer.changeShirt(shirt);

                //same problem with skin color.
                Color c = new Color(0, 0, 0) { PackedValue = m_config.ChosenEyeColor[which] };
                if (c == m_farmer.newEyeColor) {
                    m_farmer.changeEyeColor(new Color(255 - c.R, 255 - c.G, 255 - c.B, 255 - c.A));
                }
                m_farmer.changeEyeColor(c);

                //SDV 1.3 limit the range of accessory when using changeAccessory() so set the value directly
                m_farmer.accessory.Set(m_config.ChosenAccessory[which]);

                int shoeColor = s_defaultShoeColor;
                if (m_config.ChosenShoeColor[which] < 0 ) {
                    if (m_farmer.boots != null && m_farmer.boots.Value != null) {
                        shoeColor = m_farmer.boots.Value.indexInColorSheet;
                    }
                } else {
                    shoeColor = m_config.ChosenShoeColor[which];
                }
                if (m_farmer.shoes == shoeColor) {
                    if (shoeColor == 0) {
                        m_farmer.changeShoeColor(1);
                    } else {
                        m_farmer.changeShoeColor(shoeColor - 1);
                    }
                }
                m_farmer.changeShoeColor(shoeColor);
            } else {    //use default farmer setting, compatible with Get dressed
                int skin = m_farmer.skin;
                m_farmer.changeSkinColor(skin + 1);
                m_farmer.changeSkinColor(skin);

                int shirt = m_farmer.shirt;
                m_farmer.changeShirt(shirt + 1);
                m_farmer.changeShirt(shirt);

                Color c = m_farmer.newEyeColor;
                m_farmer.changeEyeColor(new Color(255 - c.R, 255 - c.G, 255 - c.B, 255 - c.A));
                m_farmer.changeEyeColor(c);

                m_farmer.accessory.Set(m_config.ChosenAccessory[which]);

                if (m_farmer.boots != null && m_farmer.boots.Value != null) {
                    m_farmer.changeShoeColor(m_farmer.boots.Value.indexInColorSheet);
                }
            }
        }
    }
}
