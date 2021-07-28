/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using PlatoTK.UI.Components;

namespace PlatoTK.UI.Styles
{
    public class Style : IStyle
    {
        protected IPlatoHelper Helper { get; set; }

        public virtual int Priority { get; set; } = -1;

        public virtual string Option { get;  } = "";

        protected bool IsActive { get; set; } = true;

        public Style(IPlatoHelper helper, string option = "")
        {
            Option = option;
            Helper = helper;
        }

        public virtual string[] PropertyNames { get; } = new string[0];

        public virtual bool ShouldApply(IComponent component)
        {
            IsActive = CheckOption(component);
            return IsActive;
        }

        public virtual void Apply(IComponent component)
        {
        }

        public virtual void Dispose()
        {

        }

        public virtual void Parse(string property, string value, IComponent component)
        {

        }

        protected virtual bool CheckOption(IComponent component)
        {
            if (string.IsNullOrEmpty(Option))
                return true;

            if (Option.ToLower() == "hover")
                return component.IsMouseOver();
            else if (Option.ToLower() == "selected")
                return component.IsSelected;

            return component.HasTag(Option);
        }

        public virtual void Update(IComponent component)
        {
            bool should = CheckOption(component);
            if (IsActive != should)
                component.Recompose();
        }

        public virtual IStyle New(IPlatoHelper helper, string option = "")
        {
            return new Style(helper, option);
        }
    }
}
