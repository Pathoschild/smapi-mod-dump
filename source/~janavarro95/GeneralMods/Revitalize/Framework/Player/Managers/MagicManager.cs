using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revitalize.Framework.Player.Managers
{
    public class MagicManager
    {
        public int maxMagic;
        public int currentMagic;

        public MagicManager()
        {
            this.currentMagic = 100;
            this.maxMagic = 100;
        }

    }
}
