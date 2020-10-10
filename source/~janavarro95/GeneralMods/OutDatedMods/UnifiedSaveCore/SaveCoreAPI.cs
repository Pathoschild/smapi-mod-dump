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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnifiedSaveCore
{
    public class SaveCoreAPI
    {
        public event EventHandler BeforeSave;
        public event EventHandler AfterSave;
        public event EventHandler AfterLoad;


        public SaveCoreAPI()
        {
            BeforeSave = new EventHandler(empty);
            AfterSave = new EventHandler(empty);
            AfterLoad = new EventHandler(empty);
        }

        /// <summary>
        /// Used to initialize empty event handlers.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="args"></param>
        private void empty(object o, EventArgs args)
        {

        }

        public void invoke_BeforeSave(object sender, EventArgs args)
        {
            if(BeforeSave!=null)
            BeforeSave.Invoke(sender, args);
        }

        public void invoke_AfterSave(object sender, EventArgs args)
        {
            if(AfterSave!=null)
            AfterSave.Invoke(sender, args);
        }

        public void invoke_AfterLoad(object sender, EventArgs args)
        {
            if(AfterLoad!=null)
            AfterLoad.Invoke(sender, args);
        }
    }
}
