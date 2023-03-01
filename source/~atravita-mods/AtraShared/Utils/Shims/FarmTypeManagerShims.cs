/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Reflection;

using AtraBase.Toolkit.Reflection;

using FastExpressionCompiler.LightExpression;

using HarmonyLib;

using Netcode;

using StardewValley.TerrainFeatures;

namespace AtraShared.Utils.Shims;

/// <summary>
/// Shims to make working with FTM stuff easier.
/// </summary>
public static class FarmTypeManagerShims
{
    private static readonly Lazy<Func<LargeTerrainFeature, ResourceClump?>?> getEmbeddedResourceClump = new(
        () =>
        {
            Type ftmClump = AccessTools.TypeByName("FarmTypeManager.LargeResourceClump");

            if (ftmClump is null)
            {
                return null;
            }

            ParameterExpression potentialClump = Expression.ParameterOf<LargeTerrainFeature>("potential");
            TypeBinaryExpression isInst = Expression.TypeIs(potentialClump, ftmClump);
            ParameterExpression ret = Expression.ParameterOf<ResourceClump>("ret");

            BinaryExpression returnNull = Expression.Assign(ret, Expression.ConstantNull<ResourceClump>());

            FieldInfo clumpField = ftmClump.InstanceFieldNamed("Clump");
            MethodInfo valueGetter = typeof(NetRef<ResourceClump>).InstancePropertyNamed("Value").GetGetMethod()
                ?? ReflectionThrowHelper.ThrowMethodNotFoundException<MethodInfo>("ResourceClump Value");

            UnaryExpression casted = Expression.TypeAs(potentialClump, ftmClump);
            MemberExpression clump = Expression.Field(casted, clumpField);
            BinaryExpression clumpValue = Expression.Assign(ret, Expression.Call(clump, valueGetter));

            ConditionalExpression ifStatement = Expression.IfThenElse(isInst, clumpValue, returnNull);

            BlockExpression block = Expression.Block(typeof(ResourceClump), new List<ParameterExpression>() { ret }, ifStatement, ret);
            return Expression.Lambda<Func<LargeTerrainFeature, ResourceClump?>>(block, potentialClump).CompileFast();
        });

    /// <summary>
    /// Gets the resource clump wrapped by a FTM largeterrainfeature.
    /// </summary>
    public static Func<LargeTerrainFeature, ResourceClump?>? GetEmbeddedResourceClump => getEmbeddedResourceClump.Value;
}
