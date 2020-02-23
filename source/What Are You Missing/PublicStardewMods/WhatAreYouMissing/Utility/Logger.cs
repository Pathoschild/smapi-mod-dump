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

        public void LogRecipeIndexParseFail(string data)
        {
            Monitor.Log($"Failed to parse recipe parentSheetIndex. Data: {data}", LogLevel.Error);
        }

        public void LogTimeParseFail(string data)
        {
            Monitor.Log($"Failed to parse 24hr time. Data: {data}", LogLevel.Error);
        }

        public void LogAreaCodeParseFail(string data)
        {
            Monitor.Log($"Failed to parse location area code. Data: {data}", LogLevel.Error);
        }

        public void LogIngredientIndexParseFail(string data)
        {
            Monitor.Log($"Failed to parse ingredient parentSheetIndex. Data: {data}", LogLevel.Error);
        }

        public void LogIngredientAmountParseFail(string data)
        {
            Monitor.Log($"Failed to parse ingredient amount. Data: {data}", LogLevel.Error);
        }

        public void LogDuplicateIngredient(int recipeIndex, int ingredientIndex, int amount, int existingAmount)
        {
            Monitor.Log($"Unexpected duplicate ingredient. Recipe Index: {recipeIndex}, Ingredient Index: {ingredientIndex}, Amount: {amount}, Existing Amount: {existingAmount}", LogLevel.Error);
        }

        public void LogFishIndexError(string lineData, string failure, int index)
        {
            Monitor.Log($"Failed to parse the parent sheet index of a fish in location data. Line Data: {lineData}, Failure on: {failure}, Index: {index}", LogLevel.Error);
        }

        public void LogRecipeBuffError(string lineData)
        {
            Monitor.Log($"There was no buff index. Data: {lineData}", LogLevel.Error);
        }
    }
}
