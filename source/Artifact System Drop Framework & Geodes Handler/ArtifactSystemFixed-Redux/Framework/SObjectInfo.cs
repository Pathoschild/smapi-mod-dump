/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://sourceforge.net/p/sdvmods-artifact-fix-redux/
**
*************************************************/

/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;

using StardewValley;

using SObject = StardewValley.Object;

namespace ArtifactSystemFixed_Redux.Framework
    {
    class SObjectInfo
        {
        public int ID { get; private set; }
        public bool IsArchaeology => this.AcquisitionMethod == "Arch";
        public bool IsMineral => this.AcquisitionMethod.Contains("Mineral");
        public bool IsGeode => _IsGeode.Value;
        private readonly Lazy<bool> _IsGeode;
        public string Name { get; private set; }
        public int SellPrice { get; private set; }
        public int Edibility { get; private set; }
        public string AcquisitionMethod { get; private set; }
        public string LocalizedName { get; private set; }
        public string Description { get; private set; }

        public string WhenWhereWhat { get; private set; } = null;
        public string[] Treasures => _Treasures.Value;
        private readonly Lazy<string[]> _Treasures = new(() => null);
        public string Action { get; private set; } = null;

        public string[] Elements;

        private bool IsGeode_Init() {
            if (this.ID == 275 || this.ID == 791) return true;
            if (Elements.Length <= 6) return false;
            if (this.Treasures.Length == 0) return false;
            if (!int.TryParse(this.Treasures[0], out var _)) return false;
            return true;
            }

        public SObjectInfo(KeyValuePair<int, string> objectInfoKVP) : this(objectInfoKVP.Key, objectInfoKVP.Value) { }

        public SObjectInfo(string ID, string objectInfo) : this(Convert.ToInt32(ID), objectInfo) { }

        public SObjectInfo(int ID, string objectInfo) {
            this.ID = ID;
            Elements = objectInfo.Split('/');
            this.Name = Elements[0];
            this.SellPrice = Convert.ToInt32(Elements[1]);
            this.Edibility = Convert.ToInt32(Elements[2]);
            this.AcquisitionMethod = Elements[3];
            this.LocalizedName = Elements[4];
            this.Description = Elements[5];
            this._IsGeode = new(IsGeode_Init);
            if (Elements.Length <= 6) return;
            this.WhenWhereWhat = Elements[6];
            this._Treasures = new(() => this.WhenWhereWhat.Split(' '));
            if (Elements.Length <= 7) return;
            this.Action = Elements[7];
            }

        public static SObjectInfo FromObject(SObject obj) {
            int index = obj.ParentSheetIndex;
            if(Game1.objectInformation.TryGetValue(index, out string objInfoStr)) {
                return new SObjectInfo(index, objInfoStr);
                }
            return null;
            }

        public static SObjectInfo FromItem(Item item) => FromObject((SObject)item);

        public static SObjectInfo FromIndex(int index) => new(index, Game1.objectInformation[index]);
        }
    }
