/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using System.IO;
using System.Text;
using Namotion.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TehPers.Core.Api.Items;
using TehPers.FishingOverhaul.Config;
using TehPers.FishingOverhaul.Config.ContentPacks;

namespace TehPers.FishingOverhaul.SchemaGen
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Check args
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: <outDir>");
                return;
            }

            // Get output directory
            var outDir = Path.GetFullPath(args[0]);
            Console.WriteLine($"Output directory: {outDir}");

            // Generate the schemas
            Program.WriteFishingOverhaulSchemas(
                Path.Join(outDir, "TehPers.FishingOverhaul", "schemas")
            );
        }

        private static void WriteFishingOverhaulSchemas(string outDir)
        {
            // Known definitions
            var knownDefinitions = new DefinitionMap();
            knownDefinitions.Assign(
                typeof(NamespacedKey),
                new()
                {
                    ["type"] = "string",
                    ["pattern"] = @"(?<namespace>[^:].+):(?<key>.*)"
                }
            );

            // Content pack
            Program.WriteSchema<FishingContentPack>(
                knownDefinitions,
                Path.Join(outDir, "contentPacks/content.schema.json")
            );
            Program.WriteSchema<FishTraitsPack>(
                knownDefinitions,
                Path.Join(outDir, "contentPacks/fishTraits.schema.json")
            );
            Program.WriteSchema<FishPack>(
                knownDefinitions,
                Path.Join(outDir, "contentPacks/fish.schema.json")
            );
            Program.WriteSchema<TrashPack>(
                knownDefinitions,
                Path.Join(outDir, "contentPacks/trash.schema.json")
            );
            Program.WriteSchema<TreasurePack>(
                knownDefinitions,
                Path.Join(outDir, "contentPacks/treasure.schema.json")
            );

            // Configs
            Program.WriteSchema<FishConfig>(
                knownDefinitions,
                Path.Join(outDir, "configs/fish.schema.json")
            );
            Program.WriteSchema<TreasureConfig>(
                knownDefinitions,
                Path.Join(outDir, "configs/treasure.schema.json")
            );
            Program.WriteSchema<HudConfig>(
                knownDefinitions,
                Path.Join(outDir, "configs/hud.schema.json")
            );
        }

        private static void WriteSchema<T>(DefinitionMap knownDefinitions, string path)
        {
            Console.WriteLine($"Generating {path} from {typeof(T).FullName}");
            Program.EnsurePath(path);
            using var outFile = File.Open(path, FileMode.Create);
            using var writer = new StreamWriter(outFile, Encoding.UTF8);

            // Generate schema
            var definitionMap = new DefinitionMap(knownDefinitions);
            var schema = definitionMap.Register(typeof(T).ToContextualType(), true);

            // Add standard properties
            schema["$schema"] = "http://json-schema.org/draft-04/schema#";
            schema["title"] = typeof(T).Name;

            // Add definitions
            var definitions = new JObject();
            schema["definitions"] = definitions;
            foreach (var (name, defSchema) in definitionMap.Definitions)
            {
                definitions[name] = defSchema;
            }

            // Write schema
            writer.Write(schema.ToString(Formatting.Indented));
        }

        private static void EnsurePath(string path)
        {
            if (Path.GetDirectoryName(path) is { } dir)
            {
                Directory.CreateDirectory(dir);
            }
        }
    }
}