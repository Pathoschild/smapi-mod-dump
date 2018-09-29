using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquivalentExchange.Models
{
    public class AlchemyTransmutationRecipe
    {
        public int InputId { get; set; }

        public int OutputId { get; set; }        

        /// <summary>
        ///     The cost of performing this recipe, typically 1 unless a profession enables a special input/output.
        /// </summary>
        public double CostCoefficient { get; set; }
        
        public AlchemyTransmutationRecipe(int input, int output, double costCoefficient = 1D)
        {
            InputId = input;
            OutputId = output;
            CostCoefficient = costCoefficient;
        }

        public double GetEnergyCost()
        {
            return this.CostCoefficient * (this.GetInputCost() * Util.GetItemValue(this.InputId)) / 10D;
        }

        public int GetInputCost()
        {
            var inputValue = Util.GetItemValue(this.InputId);
            var outputValue = Util.GetItemValue(this.OutputId);
            var lcd = Util.LowestCommonDenominator(inputValue, outputValue);
            //Log.debug($"Input of {Util.GetItemName(this.InputId)} ({Util.GetItemValue(this.InputId)})");
            //Log.debug($"Output of {Util.GetItemName(this.OutputId)} ({Util.GetItemValue(this.OutputId)})");
            //Log.debug($"Lcd of {lcd}");
            return lcd / inputValue;
        }

        public int GetOutputQuantity()
        {
            var inputValue = Util.GetItemValue(this.InputId);
            var outputValue = Util.GetItemValue(this.OutputId);
            var lcd = Util.LowestCommonDenominator(outputValue, inputValue);
            //Log.debug($"Input of {Util.GetItemName(this.InputId)} ({Util.GetItemValue(this.InputId)})");
            //Log.debug($"Output of {Util.GetItemName(this.OutputId)} ({Util.GetItemValue(this.OutputId)})");
            //Log.debug($"Lcd of {lcd}");
            return lcd / outputValue;
        }
    }
}
