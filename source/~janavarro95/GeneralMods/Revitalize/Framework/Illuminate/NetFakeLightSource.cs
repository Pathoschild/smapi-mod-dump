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

namespace Omegasis.Revitalize.Framework.Illuminate
{
    public class NetFakeLightSource : NetField<FakeLightSource, NetFakeLightSource>
    {

        public NetFakeLightSource()
        {

        }

        public NetFakeLightSource(FakeLightSource value) : base(value)
        {

        }

        public override void Set(FakeLightSource newValue)
        {
            if (this.canShortcutSet())
                this.value = newValue;
            else if (newValue != this.value)
            {
                this.cleanSet(newValue);
                this.MarkDirty();
            }
        }

        protected override void ReadDelta(BinaryReader reader, NetVersion version)
        {

            if (this.value == null)
                this.value = new FakeLightSource();

            if (version.IsPriorityOver(this.ChangeVersion))
            {
                this.value.readFakeLightSource(reader);
                this.setInterpolationTarget(this.value);
            }
        }

        protected override void WriteDelta(BinaryWriter writer)
        {

            if (this.value == null)
                this.value = new FakeLightSource();
            this.value.writeFakeLightSource(writer);
        }
    }
}
