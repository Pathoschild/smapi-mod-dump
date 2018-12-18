using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Monsters;
using StardewValley.Quests;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnifiedSaveCore.Framework;

namespace UnifiedSaveCore
{
    /// <summary>
    /// Bare bones mod that interfaces events for saving.
    /// 
    /// Must go from really big to really small.
    /// I.E locations->characters->objects->items....
    /// TODO:
    /// Animals
    /// Trees?
    /// Chests
    /// Items
    /// 
    /// This mod will rewrite the network (server, client) code and have events that fire when a player connects, disconnects, messages are sent, and messages are received.
    /// 
    /// </summary>
    public class UnifiedSaveCore:Mod
    {
        //https://stackoverflow.com/questions/14663763/how-to-add-an-attribute-to-a-property-at-runtime

        public static List<Type> modTypes;

        public static List<IInformationHandler> dataHandlers;


        public static SaveCoreAPI saveCoreAPI;

        public static IMonitor monitor;
        public static IModHelper helper;

        public override void Entry(IModHelper helper)
        {
            
            monitor = this.Monitor;
            helper = this.Helper;
            StackFrame[] frames = new StackTrace().GetFrames();
            Assembly initialAssembly = (from f in frames
                                        select f.GetMethod().ReflectedType.Assembly
                          ).Distinct().ElementAt(1);
            Monitor.Log(initialAssembly.FullName);


            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            modTypes = new List<Type>();

            List<string> namespacesToIgnore = new List<string>()
            {
                "System",
                "Mono",
                "StardewModdingAPI",
                "StardewValley",
                "Lidgren",
                "Microsoft",
                "Monogames",
                "Monogame",
                "MonoGame",
                "Netcode",
                "Steamworks",
                "GalaxyCSharp",
                "xTile",
            };


            //taken from attribute example.
            foreach (Assembly asm in assemblies)
            {

                bool ignoreNamespace = false;
                AssemblyName name2= asm.GetName();
                
                foreach (var t in asm.GetTypes())
                {
                    string[] nameSpace = t.ToString().Split('.');

                    foreach (var name in namespacesToIgnore)
                    {
                        if (name.ToString() == nameSpace.ElementAt(0).ToString() || name.ToString()==name2.Name.ToString())
                        {
                            ignoreNamespace = true;
                            break;
                        }
                    }
                    if (ignoreNamespace) break;

                    var type = t;
                    if(t.IsInterface)
                    {
                        Monitor.Log(t.ToString()+" is an interface. Skipping XML Serialization ignore.");
                        continue;
                    }
                    var aName = name2;
                    var ab = AppDomain.CurrentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);
                    var mb = ab.DefineDynamicModule(aName.Name);
                    var tb = mb.DefineType(type.Name + "Proxy", System.Reflection.TypeAttributes.Public| TypeAttributes.NotPublic, type);


                    var attrCtorParams = new Type[] {};
                    var attrCtorInfo = typeof(System.Xml.Serialization.XmlIgnoreAttribute).GetConstructor(attrCtorParams);
                    var attrBuilder = new CustomAttributeBuilder(attrCtorInfo, new object[] { });
                    tb.SetCustomAttribute(attrBuilder);
                    Monitor.Log("XML Serialization Ignore: "+type.ToString());
                    modTypes.Add(t);

                }
                if (ignoreNamespace) continue;
                Monitor.Log(asm.FullName);
                //Monitor.Log(name2.Name.ToString());
                //FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(asm.Location);
                //AssemblyName asmName = asm.GetName();
                //string name = asmName.Name;
                //Version asmV = asmName.Version;
                //string fileV = fvi.FileVersion;
                //string prodV = fvi.ProductVersion;
                //Console.WriteLine("{0} VERSIONS: (A){1}  (F){2}  (P){3}", name, asmV, fileV, prodV);
            }

            //StardewModdingAPI.Events.SaveEvents.BeforeSave += SaveEvents_BeforeSave;
            //StardewModdingAPI.Events.SaveEvents.AfterSave += SaveEvents_AfterSave;
            //StardewModdingAPI.Events.SaveEvents.AfterLoad += SaveEvents_AfterLoad;
            saveCoreAPI = new SaveCoreAPI();

            dataHandlers = new List<IInformationHandler>();
            dataHandlers.Add(new LocationHandler());
            dataHandlers.Add(new NPCHandler());
            dataHandlers.Add(new ObjectHandler());

        }


        public static void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            saveCoreAPI.invoke_AfterLoad(sender, e); //give priority to mod authors first then use brute force methods.
            foreach (IInformationHandler handler in dataHandlers)
            {
                handler.afterLoad();
            }
        }

        public static void SaveEvents_AfterSave(object sender, EventArgs e)
        {
            saveCoreAPI.invoke_AfterSave(sender, e);//give priority to mod authors first then use brute force methods.
            foreach (IInformationHandler handler in dataHandlers)
            {
                handler.afterSave();
            }
        }

        public static void SaveEvents_BeforeSave(object sender, EventArgs e)
        {
            saveCoreAPI.invoke_BeforeSave(sender, e);//give priority to mod authors first then use brute force methods.
            foreach (IInformationHandler handler in dataHandlers)
            {
                handler.beforeSave();
            }
        }

        public override object GetApi()
        {
            return new SaveCoreAPI();
        }
    }
}
