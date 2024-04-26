/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Framework.Borders;
using Microsoft.Xna.Framework;
using Netcode;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishCatalogue.Parsing.Conditions;
internal abstract class BaseCondition
{
    protected static Color UnMetColor = new Color(72, 72, 72, 230);
    public abstract string Description();
    protected abstract string ItemID();
    public ItemLabel Label()
    {
        string id = ItemID();
        string desc = Description();
        if (id is null || desc is null)
                return null;

        ItemLabel lab = new(id, desc);
        if (!IsTrue())
            lab.SetColor(UnMetColor);
        return lab;
    }
    public abstract bool IsTrue();

}
