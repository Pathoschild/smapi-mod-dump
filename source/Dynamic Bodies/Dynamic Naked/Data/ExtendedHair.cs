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
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;


namespace DynamicBodies.Data
{
    public class ExtendedHair
    {

        
        public struct hairSettings
        {
            public Dictionary<string, Dictionary<int, int>> anim_frames;
            public bool usesUniqueLeftSprite;
            public bool isBaldStyle;
            public int yOffset;
            public int extraWidth;


            /*	The standard hair data is...
             	public Texture2D texture;
	            public int tileX;
	            public int tileY;
	            public bool usesUniqueLeftSprite;
	            public int coveredIndex = -1;
	            public bool isBaldStyle;
            "-111": "hairstyles2/6/16/true/-1/false"*/
        }

        public Dictionary<string, hairSettings> hairStyles = new Dictionary<string, hairSettings>();

        public class ContentPackHairOption : ContentPackOption
        {

            public hairSettings settings;
            public bool hasHatTexture = false;
            public bool hasBackTexture = false;
            public ContentPackHairOption(string name, string file, string author, IContentPack contentPack, hairSettings settings)
                : base(name, file, author, contentPack)
            {
                this.settings = settings;
            }
        }

        public static List<ContentPackOption> GetDefaultHairStyles()
        {
            List<ContentPackOption> options = new List<ContentPackOption>();

            //get the list of exisiting hairstyles
            List<int> defaultHairStyles = Farmer.GetAllHairstyleIndices();
            foreach (int i in defaultHairStyles)
            {
                ContentPackHairOption hairOption;

                hairSettings settings = new hairSettings();
                settings.anim_frames = null;
                settings.isBaldStyle = ((uint)(i - 49) <= 6u); //Vanilla basic hair
                settings.usesUniqueLeftSprite = false;
                HairStyleMetadata metadata = Farmer.GetHairStyleMetadata(i);
                if (metadata != null)
                {
                    settings.usesUniqueLeftSprite = metadata.usesUniqueLeftSprite;
                    settings.isBaldStyle = metadata.isBaldStyle;
                }
                hairOption = new ContentPackHairOption(i.ToString(), i.ToString(), "Vanilla", null, settings);

                options.Add(hairOption);
            }

            return options;
        }

        public void OverrideDefaultHairStyles(ref List<ContentPackOption> options, IContentPack contentPack)
        {
            //get the list of exisiting hairstyles
            List<int> defaultHairStyles = Farmer.GetAllHairstyleIndices();
            foreach (int i in defaultHairStyles)
            {
                string id = i.ToString();
                if (hairStyles.ContainsKey(id))
                {
                    //Find the element to replace in the current options list
                    int optionId = options.FindIndex(x => x.name.Equals(id));
                    if (optionId >= 0)
                    {
                        if (contentPack.HasFile($"Hair\\{id}.png"))
                        {
                            //Replace the element with a new content pack option
                            ContentPackHairOption option = new ContentPackHairOption(id, id, contentPack.Manifest.Author, contentPack, hairStyles[id]);
                            if (contentPack.HasFile($"Hair\\{id}_back.png")) option.hasBackTexture = true;
                            if (contentPack.HasFile($"Hair\\{id}_hat.png")) option.hasHatTexture = true;
                            if (option.hasHatTexture && option.hasBackTexture && !contentPack.HasFile($"Hair\\{id}_back_hat.png"))
                            {
                                ModEntry.monitor.Log($"{contentPack.Manifest.Name} is missing a hair file for '{id}_back_hat.png'", LogLevel.Debug);
                            }
                            options[optionId] = option;
                        } else
                        {
                            ModEntry.monitor.Log($"{contentPack.Manifest.Name} is missing a override hair file for '{id}.png'", LogLevel.Debug);
                        }
                    }
                }
            }
        }

        public List<ContentPackOption> GetNewHairStyles(IContentPack contentPack)
        {
            List<ContentPackOption> options = new List<ContentPackOption>();

            //get the list of exisiting hairstyles
            List<int> defaultHairStyles = Farmer.GetAllHairstyleIndices();
            foreach (KeyValuePair<string, hairSettings> kvp in hairStyles)
            {
                int id;
                bool isInt = int.TryParse(kvp.Key, out id);
                if (isInt && defaultHairStyles.Contains(id))
                {
                    //Don't add vanilla replacement files
                }
                else
                {
                    if (contentPack.HasFile($"Hair\\{kvp.Key}.png")) {
                        ContentPackHairOption option = new ContentPackHairOption(kvp.Key, kvp.Key, contentPack.Manifest.Author, contentPack, kvp.Value);
                        if (contentPack.HasFile($"Hair\\{kvp.Key}_back.png")) option.hasBackTexture = true;
                        if (contentPack.HasFile($"Hair\\{kvp.Key}_hat.png")) option.hasHatTexture = true;
                        if(option.hasHatTexture && option.hasBackTexture && !contentPack.HasFile($"Hair\\{kvp.Key}_back_hat.png"))
                        {
                            ModEntry.monitor.Log($"{contentPack.Manifest.Name} is missing a hair file for '{kvp.Key}_back_hat.png'", LogLevel.Debug);
                        }
                        options.Add(option);
                    }
                    else
                    {
                        ModEntry.monitor.Log($"{contentPack.Manifest.Name} is missing a hair file for '{kvp.Key}.png'", LogLevel.Debug);
                    }
                    
                }
            }
            return options;
        }

    }
}
