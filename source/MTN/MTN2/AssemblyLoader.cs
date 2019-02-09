using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MTN2 {
    internal class AssemblyLoader {
        public Assembly LoadDll(string assemblyPath) {
            Assembly logicAssembly;
            FileInfo file = new FileInfo(assemblyPath);

            if (!file.Exists) {
                //Print error
                return null;
            }

            logicAssembly = Assembly.UnsafeLoadFrom(file.FullName);

            return logicAssembly;
        }

        public MtnLogic LoadEntryPoint(Assembly dll) {
            TypeInfo[] entries = dll.DefinedTypes.Where(type => typeof(MtnLogic).IsAssignableFrom(type) && !type.IsAbstract).Take(2).ToArray();
            if (entries.Length == 0) {
                // Not loaded
                return null;
            } else if (entries.Length > 1) {
                //Multiple subclasses. Bad.
                return null;
            }

            MtnLogic newLogic = (MtnLogic)dll.CreateInstance(entries[0].ToString());

            if (newLogic == null) {
                //Error. class shouldn't be instantiated.
            }

            return newLogic;
        }
    }
}
