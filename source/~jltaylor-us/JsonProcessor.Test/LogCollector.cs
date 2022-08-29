/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewJsonProcessor
**
*************************************************/

// // Copyright 2022 Jamie Taylor
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonProcessor.Test
{
    public class LogCollector
    {
        private readonly List<Tuple<string, string>> lines = new();
        public LogCollector()
        {
        }

        public void Clear() {
            lines.Clear();
        }

        public List<Tuple<string, string>> Lines => lines;

        public void Log(string path, string message) {
            lines.Add(Tuple.Create(path, message));
        }

        public override string ToString() {
            StringBuilder sb = new();
            foreach (Tuple<string, string> t in lines) {
                sb.Append(t.Item1).Append(": ").Append(t.Item2).Append("\n");
            }
            return sb.ToString();
        }

        public bool IsEmpty => lines.Count == 0;
    }
}

