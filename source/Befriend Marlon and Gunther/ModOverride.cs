using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.Quests;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using xTile.Dimensions;
using xTile.ObjectModel;
using xTile.Tiles;


namespace StardewValley
{
    public class SocialNPC : NPC, IComparable
    {
        public SocialNPC()
        {
        }

        public SocialNPC(AnimatedSprite sprite, Vector2 position, int facingDir, string name, LocalizedContentManager content = null)
            : base(sprite, position, 2, name)
        {
            this.faceDirection(facingDir);
            this.DefaultPosition = position;
            this.defaultFacingDirection = facingDir;
            this.lastCrossroad = new Microsoft.Xna.Framework.Rectangle((int)position.X, (int)position.Y + 64, 64, 64);
            if (content == null)
                return;
            try
            {
                this.Portrait = content.Load<Texture2D>("Portraits\\" + name);
            }
            catch (Exception ex)
            {
            }
        }

        public SocialNPC(AnimatedSprite sprite, Vector2 position, string defaultMap, int facingDirection, string name, bool datable, Dictionary<int, int[]> schedule, Texture2D portrait)
            : this(sprite, position, defaultMap, facingDirection, name, schedule, portrait, false)
        {
            this.datable.Value = datable;
        }

        public SocialNPC(AnimatedSprite sprite, Vector2 position, string defaultMap, int facingDir, string name, Dictionary<int, int[]> schedule, Texture2D portrait, bool eventActor)
            : base(sprite, position, 2, name)
        {
            this.Portrait = portrait;
            this.faceDirection(facingDir);
            this.DefaultPosition = position;
            this.defaultMap.Value = defaultMap;
            this.currentLocation = Game1.getLocationFromName(defaultMap);
            this.defaultFacingDirection = facingDir;
            if (!eventActor)
                this.lastCrossroad = new Microsoft.Xna.Framework.Rectangle((int)position.X, (int)position.Y + 64, 64, 64);
            try
            {
                Dictionary<string, string> source = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
                Console.Write("Starting out good");
                if (!source.ContainsKey(name))
                    return;
                string[] strArray = source[name].Split('/');
                string str1 = strArray[0];
                if (!(str1 == nameof(teen)))
                {
                    if (str1 == nameof(child))
                        this.Age = 2;
                }
                else
                    this.Age = 1;
                string str2 = strArray[1];
           
                if (!(str2 == nameof(rude)))
                {
                    if (str2 == nameof(polite))
                        this.Manners = 1;
                }
                else
                    this.Manners = 2;
                string str3 = strArray[2];
                if (!(str3 == nameof(shy)))
                {
                    if (str3 == nameof(outgoing))
                        this.SocialAnxiety = 0;
                }
                else
                    this.SocialAnxiety = 1;
                string str4 = strArray[3];
                if (!(str4 == nameof(positive)))
                {
                    if (str4 == nameof(negative))
                        this.Optimism = 1;
                }
                else
                    this.Optimism = 0;
                string str5 = strArray[4];
                if (!(str5 == nameof(female)))
                {
                    if (str5 == nameof(undefined))
                        this.Gender = 2;
                }
                else
                    this.Gender = 1;
                string str6 = strArray[5];
  
                if (!(str6 == nameof(datable)))
                {
                    if (str6 == "not-datable")
                        this.datable.Value = false;
          
                }
                else
                    this.datable.Value = true;
                this.loveInterest = strArray[6];
                string str7 = strArray[7];
         
                if (!(str7 == "Desert"))
                {
                    if (!(str7 == "Other"))
                    {
                        if (str7 == "Town")
                            this.homeRegion = 2;
                    }
                    else
                        this.homeRegion = 0;
                }
                else
                    this.homeRegion = 1;
        
                if (strArray.Length > 8)
                {
                    this.Birthday_Season = strArray[8].Split(' ')[0];

                    this.Birthday_Day = 2;
                }
          
                for (int index = 0; index < source.Count; ++index)
                {
                    if (source.ElementAt<KeyValuePair<string, string>>(index).Key.Equals(name))
                    {
                        this.id = index;
                        break;
                    }
                }
        
                this.displayName = strArray[11];         
            }
            catch (FormatException ex)
            {
            }
        }

        public override bool CanSocialize
        {
            get
            {
                if (this.Name.Equals("Sandy") && !Game1.player.mailReceived.Contains("ccVault") || (this.Name.Equals("???")) || (this.Name.Equals("Bouncer")) || (this.Name.Equals("Gil")) || (this.Name.Equals("Henchman"))  ||(this.IsMonster) || (this is Horse) || (this is Pet) || (this is JunimoHarvester) || (this.Name.Equals("Dwarf")) || this.Name.Contains("Qi") || (this is Pet) || (this is Horse) || (this is Junimo))
                    return false;
                else if (this.Name.Equals("Krobus"))
                    return Game1.player.friendshipData.ContainsKey("Krobus");
                else
                    return true;
            }

        } 
        
    }
}