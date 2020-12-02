/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JacquePott/StardewValleyMods
**
*************************************************/

using ProducerFrameworkMod.ContentPack;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace MassProduction
{
    /// <summary>
    /// Data structure containing data about an input object.
    /// </summary>
    public class InputInfo
    {
        public int ID;
        public string Name;
        public int Quality;
        public int BaseQuantity;
        public bool IsFuel;
        public string[] ContextTags;

        /// <summary>
        /// Gets the info on an input.
        /// </summary>
        /// <param name="inputObject"></param>
        /// <param name="baseQuantityRequired"></param>
        /// <returns></returns>
        public static InputInfo ConvertInput(SObject inputObject, int baseQuantityRequired)
        {
            if (inputObject == null) { return null; }

            return new InputInfo()
            {
                ID = inputObject.ParentSheetIndex,
                Name = inputObject.name,
                Quality = inputObject.Quality,
                BaseQuantity = baseQuantityRequired,
                IsFuel = false,
                ContextTags = inputObject.GetContextTagList().ToArray()
            };
        }

        /// <summary>
        /// Converts a PFM producer rule's input and fuels into a list of input info.
        /// </summary>
        /// <param name="producerRule"></param>
        /// <returns></returns>
        public static List<InputInfo> ConvertPFMInputs(ProducerRule producerRule, SObject inputObject)
        {
            List<InputInfo> inputs = new List<InputInfo>();

            if (inputObject != null)
            {
                inputs.Add(ConvertInput(inputObject, producerRule.InputStack));
                inputs.AddRange(GetFromFuelList(producerRule.FuelList));
            }

            return inputs;
        }

        /// <summary>
        /// Converts the fuels required by an output config.
        /// </summary>
        /// <param name="outputConfig"></param>
        /// <returns></returns>
        public static List<InputInfo> ConvertPFMInputs(OutputConfig outputConfig)
        {
            List<InputInfo> inputs = new List<InputInfo>();
            inputs.AddRange(GetFromFuelList(outputConfig.FuelList));
            return inputs;
        }

        /// <summary>
        /// Gets all fuels required as InputInfo objects.
        /// </summary>
        /// <param name="fuelList"></param>
        /// <returns></returns>
        private static List<InputInfo> GetFromFuelList(IEnumerable<Tuple<int, int>> fuelList)
        {
            List<InputInfo> fuels = new List<InputInfo>();

            Dictionary<int, string> objects = ModEntry.Instance.Helper.Content.Load<Dictionary<int, string>>("Data\\ObjectInformation", ContentSource.GameContent);

            foreach (Tuple<int, int> fuel in fuelList)
            {
                SObject sample = new SObject(fuel.Item1, 1);

                fuels.Add(new InputInfo()
                {
                    ID = fuel.Item1,
                    Name = sample.name,
                    Quality = 0, //TOREVIEW: should we poll for fuel quality here?
                    BaseQuantity = fuel.Item2,
                    IsFuel = true,
                    ContextTags = sample.GetContextTagList().ToArray()
                });
            }

            return fuels;
        }
    }
}
