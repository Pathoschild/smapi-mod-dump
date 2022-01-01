/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/WarpNetwork
**
*************************************************/

using WarpNetwork.api;

namespace WarpNetwork.models
{
    class CustomWarpLocation : WarpLocation
    {
        public readonly IWarpNetHandler handler;
        public CustomWarpLocation(IWarpNetHandler handler) : base()
        {
            this.handler = handler;
        }
        public override string Label
        {
            get { return handler.GetLabel(); }
        }
        public override string Icon
        {
            get { return handler.GetIconName(); }
        }
        public override bool Enabled
        {
            get { return handler.GetEnabled(); }
        }
    }
}
