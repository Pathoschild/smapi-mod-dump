using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using StardewModdingAPI;

namespace StardewHack.Trace
{

    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper) {
            return;
            string[] corelibs={"mscorlib", "System", "Microsoft", "Windows", "Newtonsoft", "Mono." };

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies()) {
                Monitor.Log("== "+a.FullName+" ==");
                if (a.IsDynamic) {
                    Monitor.Log(" (Dynamic module)");
                } else {
                    Monitor.Log("DLL: "+a.Location);
                }
                if (corelibs.Any(a.FullName.StartsWith)) {
                    Monitor.Log(" (skipping system library)");
                } else try {
                    foreach (var cls in a.GetTypes()) {
                        Monitor.Log("  " + cls.FullName);
                    }
                } catch (Exception err) {
                    Monitor.Log(err.ToString(), LogLevel.Error);
                }
                Monitor.Log("");
            }
        }
    }
}

