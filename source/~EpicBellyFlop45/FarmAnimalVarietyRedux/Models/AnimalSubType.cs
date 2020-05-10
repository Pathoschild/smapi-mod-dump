using StardewModdingAPI;
using System.Reflection;

namespace FarmAnimalVarietyRedux.Models
{
    /// <summary>Metadata about an animal sub type.</summary>
    public class AnimalSubType
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The name of the sub type.</summary>
        public string Name { get; set; }

        /// <summary>The produce for the sub type.</summary>
        public AnimalProduce Produce { get; set; }

        /// <summary>The sprite sheets for the sub type.</summary>
        public AnimalSprites Sprites { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="name">The name of the sub type.</param>
        /// <param name="produce">The produce for the sub type.</param>
        /// <param name="sprites">The sprite sheets for the subtype.</param>
        public AnimalSubType(string name, AnimalProduce produce, AnimalSprites sprites)
        {
            Name = name;
            Produce = produce;
            Sprites = sprites;
        }

        /// <summary>Get whether the sub type is valid.</summary>
        /// <returns>Whether the sub type is valid.</returns>
        public bool IsValid()
        {
            if (Produce == null)
                return false;

            var isValid = true;

            var seasonsInfo = typeof(AnimalProduce).GetProperties();
            foreach (var seasonInfo in seasonsInfo)
            {
                var season = (AnimalProduceSeason)seasonInfo.GetValue(Produce);

                // regular products
                if (season?.Products != null)
                {
                    foreach (var productId in season.Products)
                    {
                        if (!int.TryParse(productId.Id, out _))
                        {
                            ModEntry.ModMonitor.Log($"Animal Sub Type Data Validation failed, ProductId was not valid. Sub Type: {Name} >> Season: {seasonInfo.Name}", LogLevel.Error);
                            isValid = false;
                        }
                    }
                }
                
                // deluxe products
                if (season?.DeluxeProducts != null)
                {
                    foreach (var deluxeProductId in season.DeluxeProducts)
                    {
                        if (!int.TryParse(deluxeProductId.Id, out _))
                        {
                            ModEntry.ModMonitor.Log($"Animal Sub Type Data Validation failed, DeluxeProductId was not valid. Sub Type: {Name} >> Season: {seasonInfo.Name}", LogLevel.Error);
                            isValid = false;
                        }
                    }
                }
            }

            return isValid;
        }

        /// <summary>Convert all the external mod API tokens into numerical ids.</summary>
        public void ResolveTokens()
        {
            var seasonsInfo = typeof(AnimalProduce).GetProperties();
            foreach (var seasonInfo in seasonsInfo)
            {
                var season = (AnimalProduceSeason)seasonInfo.GetValue(Produce);

                // regular products
                if (season?.Products != null)
                    for (var i = 0; i < season.Products.Count; i++)
                        season.Products[i].Id = ResolveToken(season.Products[i].Id);

                // deluxe products
                if (season?.DeluxeProducts != null)
                    for (var i = 0; i < season.DeluxeProducts.Count; i++)
                        season.DeluxeProducts[i].Id = ResolveToken(season.DeluxeProducts[i].Id);
            }
        }


        /*********
        ** Private Methods
        *********/
        /// <summary>Convert a potential token into a numerical id.</summary>
        /// <param name="token">The potential token string.</param>
        /// <returns>A string containing an id (This is so the same string 'Id' properties can be used).</returns>
        private string ResolveToken(string token)
        {
            // ensure it's actually a token
            if (!token.Contains(":"))
                return token;

            // ensure there are enough sections of the token to be valid
            var splitToken = token.Split(':');
            if (splitToken.Length != 3)
            {
                ModEntry.ModMonitor.Log("Invalid number of arguments passed. Correct layout is: '[uniqueId]:[apiMethodName]:[valueToPass]'", LogLevel.Error);
                return "-1";
            }

            string uniqueId = splitToken[0];
            string apiMethodName = splitToken[1];
            string valueToPass = splitToken[2];

            // ensure an api could be found with the unique id
            object api = ModEntry.ModHelper.ModRegistry.GetApi(uniqueId);
            if (api == null)
            {
                ModEntry.ModMonitor.Log($"No api could be found provided by: {uniqueId}", LogLevel.Error);
                return "-1";
            }

            // ensure the api has the correct method
            var apiMethodInfo = ModEntry.ModHelper.Reflection.GetMethod(api, apiMethodName);
            if (apiMethodInfo == null)
            {
                ModEntry.ModMonitor.Log($"No api method with the name: {apiMethodName} could be found for api provided by: {uniqueId}", LogLevel.Error);
                return "-1";
            }

            // ensure the api returned a value
            int apiResult = apiMethodInfo.Invoke<int>(valueToPass);
            if (apiResult == -1)
            {
                ModEntry.ModMonitor.Log($"No value was returned from method: {apiMethodName} in api provided by: {uniqueId} with a passed value of: {valueToPass}", LogLevel.Error);
                return "-1";
            }

            return apiResult.ToString();
        }
    }
}
