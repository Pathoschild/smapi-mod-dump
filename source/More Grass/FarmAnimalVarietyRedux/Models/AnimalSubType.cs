using StardewModdingAPI;

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
            // check it's not JA item (just a string with no ':')
            if (int.TryParse(token, out var id))
                return id.ToString();

            // ensure there are enough sections of the token to be valid
            var splitToken = token.Split(':');
            if (splitToken.Length != 3 && splitToken.Length != 1)
            {
                ModEntry.ModMonitor.Log("Invalid number of arguments passed. Correct layout is: '[uniqueId]:[apiMethodName]:[valueToPass]'. Or just the [itemName] for Json Assets items.", LogLevel.Error);
                return "-1";
            }

            // resolve JA item name
            if (splitToken.Length == 1)
            {
                var api = GetApi("SpaceChase0.JsonAssets");
                if (api == null)
                    return "-1";

                var apiMethodInfo = GetApiMethodInfo(api, "GetObjectId", "SpaceChase.JsonAssets");
                if (apiMethodInfo == null)
                    return "-1";

                return GetApiResult(apiMethodInfo, splitToken[0], "SpaceChase.JsonAssets").ToString();
            }

            // resold full api token
            else
            {
                string uniqueId = splitToken[0];
                string apiMethodName = splitToken[1];
                string valueToPass = splitToken[2];

                var api = GetApi(uniqueId);
                if (api == null)
                    return "-1";

                var apiMethodInfo = GetApiMethodInfo(api, apiMethodName, uniqueId);
                if (apiMethodInfo == null)
                    return "-1";

                return GetApiResult(apiMethodInfo, valueToPass, uniqueId).ToString();
            }
        }

        /// <summary>Gets an API provided from a mod with the given unique id.</summary>
        /// <param name="uniqueId">The unique id of the mod whose api to get.</param>
        /// <returns>The mod api if one exists; otherwise, null.</returns>
        private object GetApi(string uniqueId)
        {
            // ensure an api could be found with the unique id
            object api = ModEntry.ModHelper.ModRegistry.GetApi(uniqueId);
            if (api == null)
                ModEntry.ModMonitor.Log($"No api could be found provided by: {uniqueId}", LogLevel.Error);

            return api;
        }

        /// <summary>Gets a method from the given api.</summary>
        /// <param name="api">The api to get the method from.</param>
        /// <param name="methodName">The name of the method to get.</param>
        /// <param name="uniqueId">The unique of the api (Used only for logging).</param>
        /// <returns>The mod method if one exists; otherwise, null.</returns>
        private IReflectedMethod GetApiMethodInfo(object api, string methodName, string uniqueId)
        {
            // ensure the api has the correct method
            var apiMethodInfo = ModEntry.ModHelper.Reflection.GetMethod(api, methodName);
            if (apiMethodInfo == null)
                ModEntry.ModMonitor.Log($"No api method with the name: {methodName} could be found for api provided by: {uniqueId}", LogLevel.Error);

            return apiMethodInfo;
        }

        /// <summary>Gets a result from a given <see cref="IReflectedMethod"/>.</summary>
        /// <param name="methodInfo">The method to invoke with the value.</param>
        /// <param name="value">The value to pass to the method.</param>
        /// <param name="uniqueId">The unique of the api (Used only for logging).</param>
        /// <returns>The api result from the method with the passed value.</returns>
        private int GetApiResult(IReflectedMethod methodInfo, string value, string uniqueId)
        {
            // ensure the api returned a value
            int apiResult = methodInfo.Invoke<int>(value);
            if (apiResult == -1)
                ModEntry.ModMonitor.Log($"No value was returned from method: {methodInfo} in api provided by: {uniqueId} with a passed value of: {value}", LogLevel.Error);

            return apiResult;
        }
    }
}
