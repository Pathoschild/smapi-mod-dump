/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System;
using System.Security.Cryptography;
using AtraBase.Toolkit.StringHandler;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Benchmarking;

[MemoryDiagnoser]
public class SplitTest
{
    private static readonly string text = System.IO.File.ReadAllText(@"C:\Users\night\source\repos\Benchmarking\TestingStuff\text.txt");

    [Benchmark]
    public void SpanSplit()
    {
        for (int i = 0; i < 1000; i++)
        {
            foreach (var line in text.SpanSplit())
            {
                int.TryParse(line, out var result);
            }
        }
    }

    [Benchmark]
    public void Split()
    {
        for (int i = 0; i < 1000; i++)
        {
            foreach (var line in text.Split())
            {
                int.TryParse(line, out var result);
            }
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
    }
}
