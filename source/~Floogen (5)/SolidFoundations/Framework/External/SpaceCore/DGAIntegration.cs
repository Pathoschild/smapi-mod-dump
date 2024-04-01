/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

#nullable enable

namespace SolidFoundations.Framework.External.SpaceCore;
internal static class DGAIntegration
{
    internal const string DGAModataKey = "spacechase0.DynamicGameAssets/preserved-parent-ID";

    private static Lazy<Func<object, bool>?> isDGASObject = new(() =>
    {
        Type? dgaSObject = AccessTools.TypeByName("DynamicGameAssets.Game.CustomObject, DynamicGameAssets");
        if (dgaSObject is null)
            return null;
        var obj = Expression.Parameter(typeof(object));
        var isinst = Expression.TypeIs(obj, dgaSObject);
        return Expression.Lambda<Func<object, bool>>(isinst, obj).Compile();
    });

    /// <summary>
    /// A function that returns true if an object is a DGA custom object.
    /// If null, means DGA wasn't loaded (or could not grab the custom type).
    /// </summary>
    internal static Func<object, bool>? IsDGASObject => isDGASObject.Value;

    private static Lazy<Func<object, string?>?> getDGAFullID = new(() =>
    {
        Type? dgaSObject = AccessTools.TypeByName("DynamicGameAssets.Game.CustomObject, DynamicGameAssets");
        if (dgaSObject is null)
            return null;

        var obj = Expression.Parameter(typeof(object));
        var isinst = Expression.TypeIs(obj, dgaSObject);

        var loc = Expression.Parameter(typeof(string), "ret");

        MethodInfo idGetter = dgaSObject.GetProperty("FullId")?.GetGetMethod() ?? throw new InvalidOperationException("DGA SObject's FullId not found....");
        var casted = Expression.TypeAs(obj, dgaSObject);
        var assign = Expression.Assign(loc, Expression.Call(casted, idGetter));
        var returnnull = Expression.Assign(loc, Expression.Constant(null, typeof(string)));

        var branch = Expression.IfThenElse(isinst, assign, returnnull);
        List<ParameterExpression> param = new();
        param.Add(loc);

        var block = Expression.Block(typeof(string), param, branch, loc);
        return Expression.Lambda<Func<object, string?>>(block, obj).Compile();
    });

    /// <summary>
    /// A function that returns a DGA object's full ID (if applicable).
    /// Null otherwise.
    /// </summary>
    internal static Func<object, string?>? GetDGAFullID => getDGAFullID.Value;

    private static Lazy<Type[]> getSpacecoreTypes = new(() =>
    {
        Type? spacecore = AccessTools.TypeByName("SpaceCore.SpaceCore");
        FieldInfo fieldInfo = AccessTools.Field(spacecore, "ModTypes");
        if (fieldInfo == null)
            return Type.EmptyTypes;
        return (fieldInfo.GetValue(null) as List<Type>)?.ToArray() ?? Type.EmptyTypes;
    });

    internal static Type[] SpacecoreTypes => getSpacecoreTypes.Value;
}
