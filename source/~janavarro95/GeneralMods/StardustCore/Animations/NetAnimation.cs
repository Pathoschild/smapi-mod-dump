/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Netcode;

namespace Omegasis.StardustCore.Animations
{
    public class NetAnimation : NetField<Animation, NetAnimation>
    {

        public NetAnimation()
        {

        }

        public NetAnimation(Animation value) : base(value)
        {

        }

        public override void Set(Animation newValue)
        {
            if (base.canShortcutSet())
            {
                base.value = newValue;
            }
            else if (newValue != base.value)
            {
                base.cleanSet(newValue);
                base.MarkDirty();
            }
        }

        protected override void ReadDelta(BinaryReader reader, NetVersion version)
        {

            if (this.value == null)
            {
                this.value = new Animation();
            }

            if (version.IsPriorityOver(base.ChangeVersion))
            {
                this.value.readAnimation(reader);
                base.setInterpolationTarget(this.value);
            }
        }

        protected override void WriteDelta(BinaryWriter writer)
        {

            if (this.value == null)
            {
                this.value = new Animation();
            }
            this.value.writeAnimation(writer);
        }

    }
}
