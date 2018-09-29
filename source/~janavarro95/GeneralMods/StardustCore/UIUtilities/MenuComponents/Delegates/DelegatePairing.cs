using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardustCore.UIUtilities.MenuComponents.Delegates
{
    public class DelegatePairing
    {
        public Delegates.paramaterDelegate click;
        public List<object> paramaters;

        public DelegatePairing(Delegates.paramaterDelegate buttonDelegate,List<object> Paramaters)
        {
            this.click = buttonDelegate;
            if (Paramaters == null)
            {
                this.paramaters = new List<object>();
            }
            else
            {
                this.paramaters = Paramaters;
            }
        }

        public void run()
        {
            if (this.click != null)
            {
                this.click.Invoke(this.paramaters);
            }
            else return;
        }
    }
}
