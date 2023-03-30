/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using PlatoUI.UI.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatoUI.UI.Styles
{
    public class OnClickStyle : Style
    {
        public string[] LCalls { get; set; } = new string[0];

        public string[] RCalls { get; set; } = new string[0];

        public OnClickStyle(IPlatoUIHelper helper, string option ="")
            : base(helper,option)
        {

        }

        public override IStyle New(IPlatoUIHelper helper, string option = "")
        {
            return new OnClickStyle(helper, option);
        }

        public override string[] PropertyNames => new string[] { "OnClick", "OnRightClick" };

        public override bool ShouldApply(IComponent component)
        {
            return true;
        }

        public override void Apply(IComponent component)
        {
            
        }

        public override void Update(IComponent component)
        {
            if (component.WasLeftClicked())
                foreach(string call in LCalls)
                    component.GetWrapper().TryCall(call, component);

            if (component.WasRightClicked())
                foreach (string call in RCalls)
                    component.GetWrapper().TryCall(call, component);
        }

        public override void Parse(string property, string value, IComponent component)
        {
            if(property.ToLower() == "onclick")
                LCalls = value.Trim().Split(' ').Select(s => s.Trim()).ToArray();
            else if (property.ToLower() == "onrightclick")
                RCalls = value.Trim().Split(' ').Select(s => s.Trim()).ToArray();
            else
                base.Parse(property, value, component);
        }
    }
}
