/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneSprinklerOneScarecrow
{
    class OldCode
    {
        // Harmony Original Code credit goes to Cat from the SDV Modding Discord, I modified his Harmony code.
            try
            {
                HarmonyInstance Harmony = HarmonyInstance.Create("mizzion.onesprinkleronescarecrow");

        //Now we set up the patches, will use a dictionary, just in case I need to expand later. Idea of using Harmony this way came from Cat#2506's mod  from the SDV discord
        IDictionary<string, Type> replacements = new Dictionary<string, Type>
        {
            [nameof(Farm.addCrows)] = typeof(AddCrowsPatch),
            [nameof(Object.GetBaseRadiusForSprinkler)] = typeof(GetBaseRadiusForSprinklerPatch),
            [nameof(Object.IsSprinkler)] = typeof(IsSprinklerPatch)
        };

        IList<Type> typesToPatch = new List<Type>();
        typesToPatch.Add(typeof(Farm));
                typesToPatch.Add(typeof(Object));;
                

                //Go through and set up the patching
                foreach (Type t in typesToPatch)
                    foreach (KeyValuePair<string, Type> replacement in replacements)
                    {
                        MethodInfo original = t.GetMethods(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(m => m.Name == replacement.Key);

        MethodInfo prefix = replacement.Value
            .GetMethods(BindingFlags.Static | BindingFlags.Public).FirstOrDefault(item => item.Name == "Prefix");
        MethodInfo postfix = replacement.Value
            .GetMethods(BindingFlags.Static | BindingFlags.Public).FirstOrDefault(item => item.Name == "Postfix");

                        //this.Monitor.Log($"Patching {original} with {prefix} {postfix}", LogLevel.Trace);
                        this.Monitor.Log($"Patching {original} with {prefix} {postfix}", LogLevel.Trace);

                        Harmony.Patch(original, prefix == null ? null : new HarmonyMethod(prefix),
                            postfix == null ? null : new HarmonyMethod(postfix));
                    }

}
            catch (Exception ex)
{
    Monitor.Log($"There was an error setting up harmony.\n {ex}", LogLevel.Trace);
}
addcrows = new AddCrowsPatch(Monitor);
    }
}
*/