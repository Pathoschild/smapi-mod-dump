/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/elfuun1/FlowerDanceFix
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;

namespace FlowerDanceFix
{
    public class AnimationModel
    {
        public int Duration { get; set; }
        public string Frames { get; set; }
        public bool Loops { get; set; }
    }

    public class SpectatorAnimationModel
    {
        public string Log { get; set; }
        public string Character { get; set; }
        public string Position { get; set; }
        public string StartPose { get; set; }
        public AnimationModel Animation1 { get; set; }
        public AnimationModel Animation2 { get; set; }
        public AnimationModel Animation3 { get; set; }
        public bool EndAnimation { get; set; }

        public string Warp()
        {
            return $"/warp {Character} {Position}";
        }
        public string ShowFrame()
        {
            return $"/showFrame {Character} {StartPose}";
        }
        public string Animate1()
        {
            return $"/animate {Character} false {Animation1.Loops} {Animation1.Duration} {Animation1.Frames}";
        }
        public string Animate2()
        {
            return $"/animate {Character} false {Animation2.Loops} {Animation2.Duration} {Animation2.Frames}";
        }
        public string Animate3()
        {
            return $"/animate {Character} false {Animation3.Loops} {Animation3.Duration} {Animation3.Frames}";
        }
        public string StopAnimation()
        {
            if (EndAnimation == true)
            { return $"/stopAnimation {Character}"; }
            else 
            { return ""; }
        }
    }

    public class ContentPackModel
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string UniqueID { get; set; }
        public string[] updateKeys { get; set; }
    }

    public class DancePairModel
    {
        public string UpperLine { get; set; }
        public string BottomLine { get; set; }
    }
}
