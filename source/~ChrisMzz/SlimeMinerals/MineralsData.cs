/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChrisMzz/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SlimeMinerals.MineralsData
{

    public class Mineral
    {
        public string name = "";
        public string iD = "";
        public Color targetColor;

        public Mineral(string name, string iD, Color targetColor)
        {
            this.name = name; // this is mostly for debugging purposes
            this.iD = iD;
            this.targetColor = targetColor;
        }
    } 
    internal class Minerals : IEnumerable<Mineral>
    {
        List<Mineral> minerals = new List<Mineral>();
        public Minerals()
        {
            minerals.Add(new Mineral("Opal", "564", new Color(185, 218, 225)));
            minerals.Add(new Mineral("Fire Opal", "565", new Color(57, 47, 105)));
            minerals.Add(new Mineral("Bixite", "539", new Color(32, 41, 37)));
            minerals.Add(new Mineral("Baryte", "540", new Color(200, 79, 66)));
            minerals.Add(new Mineral("Dolomite", "543", new Color(224, 152, 199)));
            minerals.Add(new Mineral("Calcite", "542", new Color(243, 214, 102)));
            minerals.Add(new Mineral("Aerinite", "541", new Color(98, 169, 190)));
            minerals.Add(new Mineral("Esperite", "544", new Color(163, 166, 193)));
            minerals.Add(new Mineral("Fluoropatite", "545", new Color(147, 142, 198)));
            minerals.Add(new Mineral("Geminite", "546", new Color(158, 212, 145)));
            minerals.Add(new Mineral("Helvite", "547", new Color(215, 50, 50)));
            minerals.Add(new Mineral("Jamborite", "548", new Color(151, 217, 88)));
            minerals.Add(new Mineral("Jagoite", "549", new Color(195, 211, 30)));
            minerals.Add(new Mineral("Limestone", "571", new Color(210, 220, 214)));
            minerals.Add(new Mineral("Soapstone", "572", new Color(194, 187, 168)));
            minerals.Add(new Mineral("Mudstone", "574", new Color(72, 32, 9)));
            minerals.Add(new Mineral("Obsidian", "575", new Color(62, 22, 81)));
            minerals.Add(new Mineral("Slate", "576", new Color(119, 209, 175)));
            minerals.Add(new Mineral("Fairy Stone", "577", new Color(59, 48, 140)));
            minerals.Add(new Mineral("Star Shard", "578", new Color(211, 55, 205)));
            minerals.Add(new Mineral("Alamite", "538", new Color(105, 200, 120)));
        }

        // https://stackoverflow.com/a/13135465/17091581
        #region Implementation of IEnumerable
        public IEnumerator<Mineral> GetEnumerator()
        {
            return minerals.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }



}
