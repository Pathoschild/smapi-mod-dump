/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using FastExpressionCompiler.LightExpression;
using System.Reflection;

using BenchmarkDotNet.Attributes;

namespace Benchmarking.ReflectionBenchmarks;

public class FieldAccess
{
    const int interations = 1000;
    [Benchmark]
    public void Native()
    {
        Person person = new("John");

        FieldInfo? field = typeof(Person).GetField("name", BindingFlags.NonPublic | BindingFlags.Instance)!;

        for (int i = 0; i < interations; i++)
        {
            field.SetValue(person, "Sam");
        }
    }

    [Benchmark]
    public void Expressions()
    {
        Person person = new("John");

        var field = typeof(Person).GetField("name", BindingFlags.NonPublic | BindingFlags.Instance)!;

        var param = Expression.Parameter(typeof(Person), "person");
        var val = Expression.Parameter(typeof(string), "val");

        MemberExpression? fieldsetter = Expression.Field(param, field);
        BinaryExpression? assignexpress = Expression.Assign(fieldsetter, val);

        var del =  Expression.Lambda<Action<Person, string>>(assignexpress, param, val).CompileFast();
        for (int i = 0; i < interations; i++)
        {
            del(person, "Sam");
        }
    }
}

internal class Person
{
    private string name = string.Empty;

    internal Person(string name) => this.name = name;
}