/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Utilities.Ranges;
using Omegasis.Revitalize.Framework.Utilities;
using Omegasis.Revitalize.Framework.World.Objects.Items.Utilities;

namespace Omegasis.Revitalize.Framework.World.WorldUtilities.Items
{
    /// <summary>
    /// TODO finish filling this out.
    ///
    /// All the inputs and outputcs for machines, objects and buiildings for items.
    /// </summary>
    public class ProcessingRecipe
    {

        public string id = "";
        public GameTimeStamp timeToProcess = new GameTimeStamp();
        public List<ItemReference> inputs=new List<ItemReference>();
        public List<LootTableEntry> outputs=new List<LootTableEntry>();

        public ProcessingRecipe()
        {

        }

        public ProcessingRecipe(string Id, GameTimeStamp TimeToProcess, ItemReference Input, LootTableEntry Output)
        {
            this.id= Id;
            this.timeToProcess = TimeToProcess;
            this.inputs.Add(Input);
            this.outputs.Add(Output);
        }

        public ProcessingRecipe(string Id, GameTimeStamp TimeToProcess, ItemReference Input, List<LootTableEntry> Output)
        {
            this.id = Id;
            this.timeToProcess = TimeToProcess;
            this.inputs.Add(Input);
            this.outputs.AddRange(Output);
        }

        public ProcessingRecipe(string Id, GameTimeStamp TimeToProcess, List<ItemReference> Input, LootTableEntry Output)
        {
            this.id = Id;
            this.timeToProcess = TimeToProcess;
            this.inputs.AddRange(Input);
            this.outputs.Add(Output);
        }

        public ProcessingRecipe(string id, GameTimeStamp timeToProcess, List<ItemReference> inputs, List<LootTableEntry> outputs)
        {
            this.id = id;
            this.timeToProcess = timeToProcess;
            this.inputs = inputs;
            this.outputs = outputs;
        }


        public static KeyValuePair<string, ProcessingRecipe> GenerateLootTableEntry(string baseId, Enums.SDVObject inputObject, Enums.SDVObject outputObject, int outputStackSize)
        {
            return GenerateLootTableEntry(baseId, inputObject, outputObject, new IntRange(outputStackSize, outputStackSize));
        }

        public static KeyValuePair<string, ProcessingRecipe> GenerateLootTableEntry(string baseId, Enums.SDVObject inputObject, Enums.SDVObject outputObject, IntRange outputStackSizeRange)
        {
            return GenerateLootTableEntry(baseId, inputObject, outputObject, new List<IntOutcomeChanceDeterminer>() { new IntOutcomeChanceDeterminer(new DoubleRange(0, 100), outputStackSizeRange) });
        }

        public static KeyValuePair<string, ProcessingRecipe> GenerateLootTableEntry(string baseId, Enums.SDVObject inputObject, Enums.SDVObject outputObject, List<IntOutcomeChanceDeterminer> outputStackSizeRange)
        {
            string inputName = Enum.GetName(inputObject);
            string outputName = Enum.GetName(outputObject);
            string id = baseId + inputName + "." + outputName;
            string relativePath = Path.Combine(inputName, outputName);

            return new KeyValuePair<string, ProcessingRecipe>(relativePath, new ProcessingRecipe(id, new GameTimeStamp(0, 0, 0, 1, 0), new ItemReference(inputObject), new LootTableEntry(new ItemReference(outputObject), outputStackSizeRange)));
        }

        public static KeyValuePair<string, ProcessingRecipe> GenerateLootTableEntry(string Id, string Path, Enums.SDVObject inputObject, ItemReference outputObject, List<IntOutcomeChanceDeterminer> outputStackSizeRange)
        {

            return new KeyValuePair<string, ProcessingRecipe>(Path, new ProcessingRecipe(Id, new GameTimeStamp(0, 0, 0, 1, 0), new ItemReference(inputObject), new LootTableEntry(outputObject, outputStackSizeRange)));
        }

    }
}
