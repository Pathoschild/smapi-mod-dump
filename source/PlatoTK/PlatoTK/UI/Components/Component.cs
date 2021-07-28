/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using PlatoTK.UI.Styles;
using System.Collections;

namespace PlatoTK.UI.Components
{
    public class Component : IComponent
    {
        protected IPlatoHelper Helper { get; set; }
        protected RenderTarget2D ComponentRenderTarget { get; set; }

        public virtual int Priority { get; } = -1;

        public IComponent Parent { get; set; }

        public virtual bool IsSelected { get; protected set; } = false;

        public virtual Rectangle Bounds { get; set; } = new Rectangle();

        protected virtual Rectangle DefaultBounds { get; set; } = new Rectangle();


        public virtual Rectangle AbsoluteBounds { get; protected set; } = new Rectangle();

        public virtual string Id { get; protected set; } = "";

        protected virtual HashSet<string> Tags { get; } = new HashSet<string>();

        protected virtual List<IStyle> Styles { get; } = new List<IStyle>();

        protected virtual List<ParsedData> Parsed { get; } = new List<ParsedData>();

        protected virtual List<IComponent> Children { get; } = new List<IComponent>();

        protected virtual List<IDrawInstruction> DrawInstructions { get; } = new List<IDrawInstruction>();

        public virtual event EventHandler<IDrawInstructionsHandle> OnDrawInstructionsCompose;

        protected virtual bool ShouldRecompose { get; set; } = true;

        public virtual bool ShouldRedraw { get; set; } = true;

        protected virtual bool ShouldRepopulate { get; set; } = true;

        public virtual string ComponentName { get; } = "Box";
        public virtual bool CacheRender { get; set; } = true;
        public virtual bool PreRender { get; set; } = true;

        public virtual IWrapper Wrapper { get; set; }

        protected virtual IComponent Template { get; set; }

        protected virtual string TemplateId { get; set; }

        public virtual string[] Params { get; set; }

        public Component(IPlatoHelper helper)
        {
            Helper = helper;
        }

        public void Select()
        {
            IsSelected = true;
        }

        public void DeSelect()
        {
            IsSelected = false;
        }

        public void DeSelectAll(Predicate<IComponent> match)
        {
            if (match(this))
                DeSelect();

            foreach (IComponent child in Children)
                child.DeSelectAll(match);
        }

        public void SelectAll(Predicate<IComponent> match)
        {
            if (match(this))
                Select();

            foreach (IComponent child in Children)
                child.SelectAll(match);
        }

        public void Recompose()
        {
            ShouldRecompose = true;
        }

        public void Repopulate()
        {
            ShouldRepopulate = true;

            foreach (var child in Children)
                child.Repopulate();
        }

        public virtual IComponent New(IPlatoHelper helper)
        {
            return (IComponent) Activator.CreateInstance(this.GetType(),helper);
        }

        public virtual IComponent Clone(IPlatoHelper helper, string id = "", IComponent parent = null)
        {
            IComponent clone = New(helper);

            clone.Parent = parent ?? Parent;

            foreach (var parsed in Parsed.Where(p=> p.Attribute.ToLower() != "id"))
                clone.ParseAttribute(parsed.Attribute, parsed.Value);

            clone.ParseAttribute("Id", id);

            foreach (var child in Children)
            {
                var cClone = child.Clone(helper, "", clone);
                clone.AddChild(cClone);
            }

            return clone;
        }


        public virtual void AddChild(IComponent child)
        {
            if (!Children.Contains(child))
                Children.Add(child);
            child.Parent = this;
            child.Wrapper = this.GetWrapper();
            child.UpdateAbsoluteBounds();
        }

        public virtual void RemoveChild(IComponent child)
        {
            if (Children.Contains(child))
                Children.Remove(child);
            child.Parent = null;
        }

        public virtual void AddTag(string tag)
        {
            if (!HasTag(tag))
                Tags.Add(tag);
        }

        public virtual void RemoveTag(string tag)
        {
            if (HasTag(tag))
                Tags.Remove(tag);
        }

        public virtual bool WasMouseOver()
        {
            IWrapper wrapper = GetWrapper();
            return AbsoluteBounds.Contains(wrapper.LastMousePosition);
        }

        public virtual bool IsMouseOver()
        {
            IWrapper wrapper = GetWrapper();
            if(wrapper is IWrapper)
                return AbsoluteBounds.Contains(wrapper.CurrentMousePosition);

            return false;
        }

        public virtual bool WasDragged(out Point from, out Point to)
        {
            IWrapper wrapper = GetWrapper();

            if (wrapper.WasLeftMouseDown && !WasLeftClicked())
            {
                if(IsMouseOver() && wrapper.LastMousePosition != wrapper.CurrentMousePosition)
                {
                    from = wrapper.LastMousePosition;
                    to = wrapper.CurrentMousePosition;
                    return true;
                }
            }

            from = Point.Zero;
            to = Point.Zero;
            return false;
        }

        public virtual bool WasLeftClicked()
        {
            IWrapper wrapper = GetWrapper();
            return wrapper.WasLeftMouseDown && !wrapper.IsLeftMouseDown && IsMouseOver();
        }

        public virtual bool WasRightClicked()
        {
            IWrapper wrapper = GetWrapper();
            return wrapper.WasRightMouseDown && !wrapper.IsRightMouseDown && IsMouseOver();
        }

        public virtual int WasMouseWheelScrolled()
        {
            IWrapper wrapper = GetWrapper();
            return wrapper.CurrentMouseWheelState - wrapper.LastMouseWheelState;
        }

        public virtual void UpdateAbsoluteBounds()
        {
            Rectangle parentBounds = Parent?.AbsoluteBounds ?? Rectangle.Empty;
            AbsoluteBounds = new Rectangle(Bounds.X + parentBounds.X, Bounds.Y + parentBounds.Y, Bounds.Width, Bounds.Height);
        }

        public virtual bool HasTag(string tag)
        {
            return Tags.Contains(tag.Split(':')[0]);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
                foreach (IDrawInstruction instruction in DrawInstructions) {
                    var destination = instruction.DestinationRectangle;
                    destination = new Rectangle(destination.X + AbsoluteBounds.X, destination.Y + AbsoluteBounds.Y, destination.Width, destination.Height);
                    spriteBatch.Draw(instruction.Texture, destination, instruction.SourceRectangle, instruction.Color, instruction.Rotation, instruction.Origin, instruction.Effects, instruction.LayerDepth);
                }

                foreach (IComponent child in Children)
                    child.Draw(spriteBatch);
        }

        public virtual IComponent GetComponentById(string id)
        {
            if (Id == id)
                return this;

            return Children.FirstOrDefault(c => c.GetComponentById(id) is IComponent);
        }

        public virtual IEnumerable<IComponent> GetComponentsByTag(string tag)
        {
            if (HasTag(tag))
                yield return this;

            foreach (IComponent child in Children)
                foreach (IComponent tagged in child.GetComponentsByTag(tag))
                    yield return tagged;
        }


        public virtual void AddStyle(IStyle style)
        {
            if (!Styles.Contains(style))
                Styles.Add(style);
        }
        public virtual void RemoveStyle(IStyle style)
        {
            if (Styles.Contains(style))
                Styles.Remove(style);
        }

        public virtual IWrapper GetWrapper()
        {
            if (Wrapper == null)
            {
                if (this is IWrapper w)
                    Wrapper = w;
                else
                    Wrapper = Parent?.GetWrapper();
            }
            
            return Wrapper;
        }

        public virtual void Update(IComponent parent)
        {
            Parent = parent;

            bool recompose = ShouldRecompose;
            ShouldRecompose = false;

            if (recompose)
                Compose(Parent);

            foreach (IComponent child in Children)
            {
                if (recompose)
                    child.Recompose();

                child.Update(this);
            }

            foreach (IStyle style in Styles)
                style.Update(this);

            if (ShouldRedraw || recompose)
            {
                if(Parent != null)
                    Parent.ShouldRedraw = true;

                ComponentRenderTarget = null;
                ShouldRedraw = false;
            }
        }

        public virtual void ParseStyle(string property, string value, string option = "")
        {
            if (TryGetStyle(property, out IStyle style, option))
            {
                style.Parse(property, value, this);
            }
            else if (Helper.UI.TryGetStyle(property, out IStyle newStyle, option))
            {
                AddStyle(newStyle);
                newStyle.Parse(property, value, this);
            }

            ShouldRecompose = true;
        }

        public virtual void ParseAttribute(string attribute, string value)
        {
            ParsedData p = new ParsedData(attribute, value);

            if (Parsed.Contains(p))
                return;

            Parsed.Add(p);
           
            bool parsed = true;

            switch (attribute.ToLower())
            {
                case "id": Id = value;break;
                case "tags":
                    {
                        Tags.Clear();
                        foreach (var tag in value.Split(' '))
                            Tags.Add(tag);
                        break;
                    }
                case "params": Params = value.Trim().Split(' ').Select(s => s.Trim()).ToArray();break;
                default: parsed = false;break;
            }

            if (!parsed)
            {
                string option = "";

                if (value.Contains(":"))
                {
                    var v = value.Split(':');
                    
                    value = v[1];
                    option = v[0];
                }
                ParseStyle(attribute, value, option);
            }
        }

        protected virtual bool TryGetStyle(string propertyName, out IStyle style, string option = "")
        {
            if (Styles.FirstOrDefault(s => s.PropertyNames.Any(p => p.ToLower() == propertyName.ToLower()) && s.Option.ToLower() == option.ToLower()) is IStyle loaded)
            {
                style = loaded;
                return true;
            }

            style = null;
            return false;
        }


        public virtual void AddDrawInstructions(IDrawInstruction drawInstructions)
        {
            if (!DrawInstructions.Contains(drawInstructions))
                DrawInstructions.Add(drawInstructions);
        }

        public virtual void RemoveDrawInstructions(IDrawInstruction drawInstructions)
        {
            if (DrawInstructions.Contains(drawInstructions))
                DrawInstructions.Remove(drawInstructions);
        }

        public virtual void RemoveDrawInstructions(Predicate<IDrawInstruction> match)
        {
                DrawInstructions.RemoveAll(match);
        }

        public virtual void Setup()
        {
           
        }

        protected void PopulateComponents()
        {
            if (ShouldRepopulate)
            {
                Children.Clear();
                if (GetWrapper().TryCall(TemplateId, out IEnumerable<IComponent> components, Template))
                    foreach (IComponent element in components)
                        Children.Add(element);
                ShouldRepopulate = false;
            }
        }

        public virtual void Compose(IComponent parent)
        {
            Bounds = DefaultBounds;
            DrawInstructions.Clear();

            Setup();

            foreach (IStyle style in Styles.Where(s => s.ShouldApply(this)).OrderBy(s => s.Priority))
                style.Apply(this);

            UpdateAbsoluteBounds();

            foreach (IDrawInstruction instruction in DrawInstructions)
                OnDrawInstructionsCompose?.Invoke(this, new DrawInstructionHandle(instruction, this, parent));

            OnDrawInstructionsCompose = null;

            if (Template == null && Children.FirstOrDefault(c => c.Id.ToLower().StartsWith("template")) is IComponent template)
            {
                Template = template.Clone(Helper);
                Template.AddTag(template.Id + ".element");
                TemplateId = template.Id;
                template.Compose(this);
                Children.Remove(template);
                template.Parent = this;
            }

            if (Template != null)
                PopulateComponents();

            foreach (IComponent child in Children)
                child.Compose(this);
        }

        public virtual void Dispose()
        {
            OnDrawInstructionsCompose = null;
            foreach (IComponent child in Children)
                child.Dispose();

            foreach (IStyle style in Styles)
                style.Dispose();

            foreach (IDrawInstruction instruction in DrawInstructions)
                instruction.Dispose();
        }
    }
}
