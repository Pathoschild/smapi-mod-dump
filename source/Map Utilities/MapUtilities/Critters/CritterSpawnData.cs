using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.BellsAndWhistles;
using Microsoft.Xna.Framework;

namespace MapUtilities.Critters
{
    public class CritterSpawnData
    {
        internal static Dictionary<string, critterConstructorData> types;
        internal static List<System.Reflection.Assembly> assemblies;

        public double spawnChance;
        public Dictionary<string, double> critterWeights;
        public int startTime;
        public int endTime;
        public bool spring;
        public bool summer;
        public bool fall;
        public bool winter;
        public Vector2 tile;

        public Dictionary<string, object> customArgs;

        public CritterSpawnData(string[] critterData, Vector2 tile)
        {
            this.tile = tile;
            startTime = 600;
            endTime = 2600;
            critterWeights = new Dictionary<string, double>();
            customArgs = new Dictionary<string, object>();
            for (int i = 0; i < critterData.Length;)
            {
                if (critterData[i].EndsWith("%"))
                {
                    spawnChance = Convert.ToDouble(critterData[i].TrimEnd('%'));
                    i++;
                    continue;
                }
                if (critterData[i].StartsWith("@"))
                {
                    startTime = Convert.ToInt32(critterData[i].TrimStart('@'));
                    i++;
                    endTime = Convert.ToInt32(critterData[i]);
                    i++;
                    while (i < critterData.Length && seasonal(critterData[i]))
                    {
                        switch (critterData[i].ToLower())
                        {
                            case "spring":
                                spring = true;
                                break;
                            case "summer":
                                summer = true;
                                break;
                            case "fall":
                            case "autumn":
                                fall = true;
                                break;
                            case "winter":
                                winter = true;
                                break;
                        }
                        i++;
                    }
                    continue;
                }
                //If it made it this far, this should be a critter name!
                string critterName = critterData[i];
                double weight = 5;
                i++;
                //This value modifies the weight for this critter
                if (i < critterData.Length)
                {
                    if (critterData[i].StartsWith("$"))
                    {
                        weight = Convert.ToDouble(critterData[i].TrimStart('$'));
                        i++;
                    }
                }
                while(i < critterData.Length && critterData[i].StartsWith("&"))
                {
                    string[] argData = critterData[i].TrimStart('&').Split('=');
                    string argName = argData[0];
                    object arg = parseArgument(argData[1]);
                    //Logger.log("Parsed argument \"" + argName + "\" (" + arg.GetType().ToString() + "): " + arg.ToString());
                    customArgs[argName] = arg;
                    i++;
                }
                critterWeights[critterName] = weight;
            }
            if(!spring && !summer && !fall && !winter)
            {
                spring = true;
                summer = true;
                fall = true;
                winter = true;
            }
            string logString = "Critters to spawn: ";
            foreach(string critter in critterWeights.Keys)
            {
                logString += critter + ": " + critterWeights[critter] + ", ";
            }
            logString += "from " + startTime + " to " + endTime + ", ";
            logString += "in the " + (spring ? "spring, " : "") + (summer ? "summer, " : "") + (fall ? "fall, " : "") + (winter ? "winter, " : "");
            logString = logString.TrimEnd(' ').TrimEnd(',');
            Logger.log(logString);
        }

        internal object parseArgument(string arg)
        {
            //Logger.log("Parsing argument \"" + arg + "\"");
            if(arg.StartsWith("(") && arg.EndsWith(")"))
            {
                try
                {
                    string[] coords = arg.TrimStart('(').TrimEnd(')').Split(',');
                    return new Vector2(Convert.ToSingle(parseArgument(coords[0])), Convert.ToSingle(parseArgument(coords[1])));
                }
                catch (IndexOutOfRangeException) { }
                catch (FormatException) { }
            }
            switch (arg.ToLower())
            {
                case "x":
                    return (int)tile.X;
                case "y":
                    return (int)tile.Y;
                case "xw":
                    return (int)tile.X * 64;
                case "yw":
                    return (int)tile.Y * 64;
            }
            try
            {
                return Convert.ToInt32(arg);
            }
            catch (FormatException) { }
            try
            {
                return Convert.ToDouble(arg);
            }
            catch (FormatException) { }
            try
            {
                return Convert.ToBoolean(arg);
            }
            catch (FormatException) { }
            return arg;
        }

        public static void init()
        {
            assemblies = new List<System.Reflection.Assembly>();
        }

        public static void getAllTypes()
        {
            types = new Dictionary<string, critterConstructorData>();
            List<Critter> objects = new List<Critter>();
            foreach (System.Reflection.Assembly assembly in assemblies)
            {
                foreach (Type type in assembly.GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Critter))))
                {
                    Logger.log("Registered Critter \"" + type.Namespace + "." + type.Name + "\"");
                    types[type.Name] = new critterConstructorData(type);
                }
            }
        }

        public bool canSpawn()
        {
            Logger.log("Time of Day: " + Game1.timeOfDay + ", Season: " + Game1.currentSeason);
            return Game1.timeOfDay >= startTime && Game1.timeOfDay <= endTime && 
                ((Game1.currentSeason == "spring" && spring) || (Game1.currentSeason == "summer" && summer) || (Game1.currentSeason == "fall" && fall) || (Game1.currentSeason == "winter" && winter));
        }

        public void spawnCritter(GameLocation location)
        {
            List<string> critterNames = critterWeights.Keys.ToList();
            double total = 0f;
            foreach(string critter in critterNames)
            {
                if (types.ContainsKey(critter))
                {
                    double weight = critterWeights[critter];
                    //Logger.log(critter + " weight = " + weight + ", total = " + total + weight);
                    total += weight;
                }
            }
            if (total <= 0)
                return;

            double randomWeight = Game1.random.NextDouble() * total;

            //Logger.log("Random value: " + randomWeight);

            foreach(string critter in critterNames)
            {
                if (!types.ContainsKey(critter) || !critterWeights.ContainsKey(critter))
                    continue;
                if(randomWeight <= critterWeights[critter])
                {
                    //Logger.log(critter + " was chosen.");
                    Critter newCritter = types[critter].construct(tile, customArgs);
                    if (newCritter != null)
                        location.addCritter(newCritter);
                    return;
                }
                randomWeight -= critterWeights[critter];
            }
        }

        internal static bool seasonal(string compare)
        {
            return (new string[] { "spring", "summer", "fall", "autumn", "winter" }).Contains(compare.ToLower());
        }
    }

    internal class critterConstructorData
    {
        Type type;

        internal critterConstructorData(Type type)
        {
            this.type = type;
        }

        internal Critter construct(Vector2 v, Dictionary<string,object> customArgs = null)
        {
            return construct((int)(v.X), (int)(v.Y), customArgs);
        }

        internal Critter construct(int x, int y, Dictionary<string, object> customArgs = null)
        {
            if (customArgs == null)
                customArgs = new Dictionary<string, object>();

            //System.Reflection.ConstructorInfo ctor = null;

            //string customArgString = "Custom args for " + type.Name.ToString() + ": ";

            //foreach(string key in customArgs.Keys)
            //{
            //    customArgString += "[\"" + key + "\": " + customArgs[key].ToString() + "] ";
            //}

            //Logger.log(customArgString);

            foreach(System.Reflection.ConstructorInfo ctor in type.GetConstructors())
            {
                if (ctor.GetParameters().Length < 1)
                    continue;
                //List<Type> paramTypes = new List<Type>();
                List<object> paramValues = new List<object>();
                System.Reflection.ParameterInfo[] parameters = ctor.GetParameters();
                bool foundCoords = false;
                for (int i = 0; i < parameters.Length; i++)
                {
                    System.Reflection.ParameterInfo param = parameters[i];
                    //Logger.log("Examining parameter " + param.Name + " (" + param.ParameterType.ToString() + ")");
                    if (customArgs.ContainsKey(param.Name))
                    {
                        //Logger.log("Parameter \"" + param.Name + "\" (" + param.ParameterType.ToString() + ") defined in critter data: \"" + customArgs[param.Name].ToString() + "(" + customArgs[param.Name].GetType().ToString() + ")");
                        if(customArgs[param.Name].GetType() != param.ParameterType)
                        {
                            Logger.log("Invalid value for the parameter " + param.Name + " (" + param.ParameterType.Name + "): " + customArgs[param.Name].ToString() + " (" + (customArgs[param.Name].GetType()) + ")!", StardewModdingAPI.LogLevel.Error);
                            paramValues.Add(null);
                        }
                        else
                        {
                            paramValues.Add(customArgs[param.Name]);
                        }
                    }
                    else if(!foundCoords && param.ParameterType == typeof(Vector2))
                    {
                        paramValues.Add(new Vector2(x, y));
                        foundCoords = true;
                    }
                    else if(!foundCoords && param.ParameterType == typeof(int) && i+1 < parameters.Length && parameters[i+1].ParameterType == typeof(int))
                    {
                        paramValues.Add(x);
                        paramValues.Add(y);
                        i++;
                        foundCoords = true;
                    }
                    else if (param.IsOptional)
                    {
                        paramValues.Add(param.DefaultValue);
                    }
                    else
                    {
                        Logger.log("Unhandled parameter " + param.Name + " (" + param.ParameterType.Name + ")!", StardewModdingAPI.LogLevel.Warn);
                    }
                }
                try
                {
                    return (Critter)ctor.Invoke(paramValues.ToArray());
                }
                catch (Exception e)
                {
                    Logger.log(e.Message + e.StackTrace, StardewModdingAPI.LogLevel.Error);
                }
            }


            //try
            //{
            //    ctor = Harmony.AccessTools.Constructor(type, new Type[] { typeof(Vector2) });
            //    return (Critter)ctor.Invoke(new object[] { new Vector2(x, y) });
            //}
            //catch (NullReferenceException){}
            //try
            //{
            //    ctor = Harmony.AccessTools.Constructor(type, new Type[] { typeof(Vector2), typeof(bool) });
            //    return (Critter)ctor.Invoke(new object[] { new Vector2(x, y), false });
            //}
            //catch (NullReferenceException) { }
            //try
            //{
            //    ctor = Harmony.AccessTools.Constructor(type, new Type[] { typeof(int), typeof(int) });
            //    return (Critter)ctor.Invoke(new object[] { x, y });
            //}
            //catch (NullReferenceException) { }

            Logger.log("Could not find a standard constructor for the type " + type.Namespace + "." + type.Name + "!", StardewModdingAPI.LogLevel.Error);
            return null;
        }
    }
}
