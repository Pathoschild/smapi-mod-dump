using StardewModdingAPI;

namespace WhatAreYouMissing
{
    public class Logger
    {
        IMonitor Monitor;
        public Logger(Mod mod)
        {
            Monitor = mod.Monitor;
        }

        public void LogWarning(string warning)
        {
            if (ModEntry.modConfig.LogWarnings)
            {
                Monitor.Log(warning, LogLevel.Warn);
            }
        }

        public void LogRecipeIndexParseFail(string data)
        {
            LogWarning($"Failed to parse recipe parentSheetIndex. Data: {data}");
        }

        public void LogTimeParseFail(string data)
        {
            LogWarning($"Failed to parse 24hr time. Data: {data}");
        }

        public void LogAreaCodeParseFail(string data)
        {
            LogWarning($"Failed to parse location area code. Data: {data}");
        }

        public void LogIngredientIndexParseFail(string data)
        {
            LogWarning($"Failed to parse ingredient parentSheetIndex. Data: {data}");
        }

        public void LogIngredientAmountParseFail(string data)
        {
            LogWarning($"Failed to parse ingredient amount. Data: {data}");
        }

        public void LogDuplicateIngredient(int recipeIndex, int ingredientIndex, int amount, int existingAmount)
        {
            LogWarning($"Unexpected duplicate ingredient. Recipe Index: {recipeIndex}, Ingredient Index: {ingredientIndex}, Amount: {amount}, Existing Amount: {existingAmount}");
        }

        public void LogFishIndexError(string lineData, string failure, int index)
        {
            LogWarning($"Failed to parse the parent sheet index of a fish in location data. Line Data: {lineData}, Failure on: {failure}, Index: {index}");
        }

        public void LogRecipeBuffError(string lineData)
        {
            LogWarning($"There was no buff index. Data: {lineData}");
        }
    }
}
