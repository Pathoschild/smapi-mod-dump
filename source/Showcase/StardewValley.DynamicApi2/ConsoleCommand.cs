using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Igorious.StardewValley.DynamicApi2
{
    public abstract class ConsoleCommand
    {
        private IMonitor Monitor { get; }
        private string Name { get; }
        private string FullDescription { get; }
        private IReadOnlyDictionary<MethodInfo, ParameterInfo[]> ExecuteMethods { get; }

        protected ConsoleCommand(IMonitor monitor, string name, string description)
        {
            Monitor = monitor;
            Name = name;
            ExecuteMethods = GetExecutableMethods();
            FullDescription = GetFullDescription(description);
        }

        public void Register()
        {
            Command.RegisterCommand(Name, FullDescription).CommandFired += OnFired;
        }

        private IReadOnlyDictionary<MethodInfo, ParameterInfo[]> GetExecutableMethods()
        {
            return GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.Name == "Execute")
                .ToDictionary(m => m, m => m.GetParameters());
        }

        private string GetFullDescription(string description)
        {
            var buffer = new StringBuilder(description);
            if (ExecuteMethods.Count == 1)
            {
                buffer.Append(" | ").Append(GetSignature(ExecuteMethods.Values.First()));
            }
            else
            {
                var i = 0;
                foreach (var parametersInfo in ExecuteMethods.Values)
                {
                    buffer.AppendLine().Append("  ").Append(++i).Append(". ").Append(GetSignature(parametersInfo));
                }
            }
            return buffer.ToString();
        }

        private string GetSignature(ParameterInfo[] parametersInfo)
        {
            return parametersInfo.Any()
                ? string.Join(", ", parametersInfo.Select(p => p.Name + ":" + GetTypeName(p.ParameterType)))
                : "No arguments";
        }

        private string GetTypeName(Type parameterType)
        {
            if (!parameterType.IsEnum) return parameterType.Name;
            return $"({string.Join("|", Enum.GetNames(parameterType))})";
        }

        private void OnFired(object sender, EventArgsCommand e)
        {
            var args = e.Command.CalledArgs;
            var canditate = ExecuteMethods.FirstOrDefault(m => m.Value.Length == args.Length);
            if (canditate.Key == null)
            {
                var argsCount = ExecuteMethods.Select(m => m.Value.Length).Distinct().OrderBy(l => l);
                Error($"Command can accept only {string.Join(", ", argsCount)} arguments.");
                return;
            }

            var parsedArgs = new object[args.Length];
            for (var i = 0; i < args.Length; ++i)
            {
                var arg = args[i];
                var type = canditate.Value[i].ParameterType;
                var result = TryParseArgument(arg, type);
                if (result == null)
                {
                    Error($"Can't convert arg{i} from \"{arg}\" to {GetTypeName(type)}.");
                    return;
                }
                parsedArgs[i] = result;
            }

            canditate.Key.Invoke(this, parsedArgs);
        }   

        private object TryParseArgument(string arg, Type type)
        {
            return type.IsEnum
                ? TryParseEnumArgument(arg, type) 
                : TryParseStandartArgument(arg, type);
        }

        private object TryParseEnumArgument(string arg, Type type)
        {
            try
            {
                return Enum.Parse(type, arg);
            }
            catch
            {
                return null;
            }
        }

        private object TryParseStandartArgument(string arg, Type type)
        {
            try
            {
                return Convert.ChangeType(arg, type, CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }

        protected void Error(string message) => Monitor.Log(message, LogLevel.Error);
        protected void Info(string message) => Monitor.Log(message, LogLevel.Info);
    }
}