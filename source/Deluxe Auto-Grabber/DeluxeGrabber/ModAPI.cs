/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stokastic/CoopGrabber
**
*************************************************/

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DeluxeGrabber {
    public class PrismaticAPI {
        private ModConfig config = new ModConfig();
        public int GrabberRange { get { return config.GrabberRange; } }

        public IEnumerable<Vector2> GetGrabberCoverage(Vector2 origin) {
            for (int x = -GrabberRange; x <= GrabberRange; x++) {
                for (int y = -GrabberRange; y <= GrabberRange; y++) {
                    yield return new Vector2(x, y) + origin;
                }
            }
        }
    }
}
