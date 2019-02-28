using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildBedConfig
{
    class ModConfig
    {
        /*****************************/
        /**      Properties         **/
        /*****************************/
        public List<Farmer> Farmers { get; set; }

        /*****************************/
        /**      Public methods     **/
        /*****************************/
        ///<summary>Constructor, set up the example of the config for players to follow</summary>
        public ModConfig()
        {
            Farmer NoName = new Farmer();
            Farmers = new List<Farmer>();
            Farmers.Add(NoName);
        }
    }

    /// <summary>
    /// Class for the farmer -- we have a list of them in the config
    /// </summary>
    class Farmer
    {
        /*****************************/
        /**      Properties         **/
        /*****************************/
        ///<summary>Name of the farmer</summary>
        public string CharacterName { get; set; }

        ///<summary>Determines whether or not to show the crib</summary>
        public bool ShowCrib { get; set; }

        ///<summary>Determines whether or not to show the bed closest to the crib</summary>
        public bool ShowBed1 { get; set; }

        ///<summary>Determines whether or not to show the bed furthest from the crib</summary>
        public bool ShowBed2 { get; set; }

        /*****************************/
        /**      Public methods     **/
        /*****************************/
        public Farmer()
        {
            CharacterName = "NoName";
            ShowCrib = true;
            ShowBed1 = true;
            ShowBed2 = true;
        }

        public Farmer(string name, bool showcrib, bool showbed1, bool showbed2)
        {
            CharacterName = name;
            ShowCrib = showcrib;
            ShowBed1 = showbed1;
            ShowBed2 = showbed2;
        }
    }
}
