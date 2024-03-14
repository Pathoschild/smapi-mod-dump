/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

namespace SonoCore.Attributes;

/// <summary>Specifies information about a Harmony patch.</summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public class PatchAttribute : Attribute
{
    /*********
    ** Properties
    *********/
    /// <summary>The type of patch the method is.</summary>
    public PatchType PatchType { get; }

    /// <summary>The method to patch.</summary>
    public MethodBase OriginalMethod { get; }


    /*********
    ** Constructors
    *********/
    /// <summary>Constructs an instance.</summary>
    /// <param name="patchType">The type of patch the method is.</param>
    /// <param name="type">The type that contains the method to patch.</param>
    /// <param name="methodName">The name of the method to patch; otherwise, <see langword="null"/> to patch a constructor.</param>
    /// <param name="parameterTypes">The types of parameters of the method to patch.</param>
    public PatchAttribute(PatchType patchType, Type type, string methodName, Type[] parameterTypes = null)
    {
        PatchType = patchType;

        if (methodName == null)
            OriginalMethod = AccessTools.Constructor(type, parameterTypes) ?? throw new MissingMethodException($"Couldn't find constructor '{type.FullName}({string.Join(", ", (parameterTypes ?? new Type[0]).Select(parameterType => parameterType.Name))})'");
        else
            OriginalMethod = AccessTools.Method(type, methodName, parameterTypes) ?? throw new MissingMethodException($"Couldn't find method '{type.FullName}.{methodName}({string.Join(", ", (parameterTypes ?? new Type[0]).Select(parameterType => parameterType.Name))})'");
    }
}
