/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ofts-cqm/SDV_JojaExpress
**
*************************************************/

using StardewValley;

namespace JojaExpress
{
    /// <summary>
    /// <c>IJojaExpress</c> provides an API that allows other mods to access online shopping services.<br/>
    /// Each methods (except for <see cref="getCurrentCarriageFee"/> and <see cref="openJojaExpressShoppingMenu"/> since they don't invlove any items)<br/>
    /// will have three versions (2 overloads):<br/> one of them accept a <c>IDictionary&lt;string, int&gt;</c>
    /// as a param, <br/>one of them accept a <c>IDictionary&lt;ISalable, int&gt;</c> as a param, and <br/>one of them
    /// accept a <c>IEnumerable&lt;Item&gt;</c> as a param<br/>
    /// All three of them are identical, you can use the most convenient one. <br/>
    /// Sometimes there may be some performance differences due to implementation, but the difference isn't significant. <br/>
    /// If you want to know which version is fastest, you can reference the example implementation, 
    /// but the implementation may change without warning. 
    /// </summary>
    public interface IJojaExpress
    {
        /// <summary>
        /// Send a global package through mail to <c>who</c>
        /// </summary>
        /// <param name="products">the products to be mailed, each <c>Key</c> represent a 
        /// <c>QualifiedItemId</c>, and each <c>Value</c> represent the stack count</param>
        /// <param name="who">the farmer to receive this mail. </param>
        void sendGlobalPackage(IDictionary<string, int> products, Farmer who);

        /// <summary>
        /// Send a global package through mail to <c>who</c>
        /// </summary>
        /// <param name="products">the products to be mailed, each <c>Key</c> represent a 
        /// <c>ISalable</c>, and each <c>Value</c> represent the stack count</param>
        /// <param name="who">the farmer to receive this mail. </param>
        void sendGlobalPackage(IDictionary<ISalable, int> products, Farmer who);

        /// <summary>
        /// Send a global package through mail to <c>who</c>
        /// </summary>
        /// <param name="products">a list contains the items to be mailed</param>
        /// <param name="who">the farmer to receive this mail. </param>
        void sendGlobalPackage(IEnumerable<Item> products, Farmer who);

        /// <summary>
        /// Send a local package through Joja Parrot (provided by Moris) to <c>who</c>
        /// </summary>
        /// <param name="products">the products to be delivered, each <c>Key</c> represent a 
        /// <c>QualifiedItemId</c>, and each <c>Value</c> represent the stack count</param>
        /// <param name="who">the farmer to receive this package. </param>
        void sendLocalPackage(IDictionary<string, int> products, Farmer who);

        /// <summary>
        /// Send a local package through Joja Parrot (provided by Moris) to <c>who</c>
        /// </summary>
        /// <param name="products">the products to be delivered, each <c>Key</c> represent a 
        /// <c>ISalable</c>, and each <c>Value</c> represent the stack count</param>
        /// <param name="who">the farmer to receive this package. </param>
        void sendLocalPackage(IDictionary<ISalable, int> products, Farmer who);

        /// <summary>
        /// Send a local package through Joja Parrot (provided by Moris) to <c>who</c>
        /// </summary>
        /// <param name="products">a list contains the items to be delivered</param>
        /// <param name="who">the farmer to receive this package. </param>
        void sendLocalPackage(IEnumerable<Item> products, Farmer who);

        /// <summary>
        /// get the current carriage fee (percentage, usually &gt; 1.)<br/>
        /// e.g, 1.3f
        /// </summary>
        /// <returns></returns>
        float getCurrentCarriageFee();

        /// <summary>
        /// Try to buy products through JojaGlobal™<br/>
        /// </summary>
        /// <param name="products">the products to be bought, each <c>Key</c> represent a 
        /// <c>QualifiedItemId</c>, and each <c>Value</c> represent the stack count</param>
        /// <param name="who">the farmer who bought the products</param>
        /// <returns>wether the transaction succeeded or not (can Farmer <c>who</c> afford it)</returns>
        /// <remarks>Note that this method will automatically perchase the products and mail them to <c>who</c><br/>
        /// If you just want to check if the player can afford it, 
        /// use <see cref="canBuyProductFromJojaGlobal(IDictionary{string, int}, Farmer)"/><br/>
        /// If you just want to mail the product, 
        /// use <see cref="sendGlobalPackage(IDictionary{string, int}, Farmer)"/></remarks>
        bool tryBuyGlobalProducts(IDictionary<string, int> products, Farmer who);

        /// <summary>
        /// Try to buy products through JojaGlobal™<br/>
        /// </summary>
        /// <param name="products">the products to be bought, each <c>Key</c> represent a 
        /// <c>ISalable</c>, and each <c>Value</c> represent the stack count</param>
        /// <param name="who">the farmer who bought the products</param>
        /// <returns>wether the transaction succeeded or not (can Farmer <c>who</c> afford it)</returns>
        /// <remarks>Note that this method will automatically perchase the products and mail them to <c>who</c><br/>
        /// If you just want to check if the player can afford it, 
        /// use <see cref="canBuyProductFromJojaGlobal(IDictionary{ISalable, int}, Farmer)"/><br/>
        /// If you just want to mail the product, 
        /// use <see cref="sendGlobalPackage(IDictionary{ISalable, int}, Farmer)"/></remarks>
        bool tryBuyGlobalProducts(IDictionary<ISalable, int> products, Farmer who);

        /// <summary>
        /// Try to buy products through JojaGlobal™<br/>
        /// </summary>
        /// <param name="products">a list of Items to be bought</param>
        /// <param name="who">the farmer who bought the products</param>
        /// <returns>wether the transaction succeeded or not (can Farmer <c>who</c> afford it)</returns>
        /// <remarks>Note that this method will automatically perchase the products and mail them to <c>who</c><br/>
        /// If you just want to check if the player can afford it, 
        /// use <see cref="canBuyProductFromJojaGlobal(IEnumerable{Item}, Farmer)"/><br/>
        /// If you just want to mail the product, 
        /// use <see cref="sendGlobalPackage(IEnumerable{Item}, Farmer)"/></remarks>
        bool tryBuyGlobalProducts(IEnumerable<Item> products, Farmer who);

        /// <summary>
        /// Try to buy products through JojaLocal™<br/>
        /// </summary>
        /// <param name="products">the products to be bought, each <c>Key</c> represent a 
        /// <c>QualifiedItemId</c>, and each <c>Value</c> represent the stack count</param>
        /// <param name="who">the farmer who bought the products</param>
        /// <returns>wether the transaction succeeded or not (can Farmer <c>who</c> afford it)</returns>
        /// <remarks>Note that this method will automatically perchase the products and deliver them to <c>who</c><br/>
        /// If you just want to check if the player can afford it, 
        /// use <see cref="canBuyProductFromJojaLocal(IDictionary{string, int}, Farmer)"/><br/>
        /// If you just want to mail the product, 
        /// use <see cref="sendLocalPackage(IDictionary{string, int}, Farmer)"/></remarks>
        bool tryBuyLocalProducts(IDictionary<string, int> products, Farmer who);

        /// <summary>
        /// Try to buy products through JojaLocal™<br/>
        /// </summary>
        /// <param name="products">the products to be bought, each <c>Key</c> represent a 
        /// <c>ISalable</c>, and each <c>Value</c> represent the stack count</param>
        /// <param name="who">the farmer who bought the products</param>
        /// <returns>wether the transaction succeeded or not (can Farmer <c>who</c> afford it)</returns>
        /// <remarks>Note that this method will automatically perchase the products and deliver them to <c>who</c><br/>
        /// If you just want to check if the player can afford it, 
        /// use <see cref="canBuyProductFromJojaLocal(IDictionary{ISalable, int}, Farmer)"/><br/>
        /// If you just want to mail the product, 
        /// use <see cref="sendLocalPackage(IDictionary{ISalable, int}, Farmer)"/></remarks>
        bool tryBuyLocalProducts(IDictionary<ISalable, int> products, Farmer who);

        /// <summary>
        /// Try to buy products through JojaLocal™<br/>
        /// </summary>
        /// <param name="products">a list of Items to be bought</param>
        /// <param name="who">the farmer who bought the products</param>
        /// <returns>wether the transaction succeeded or not (can Farmer <c>who</c> afford it)</returns>
        /// <remarks>Note that this method will automatically perchase the products and deliver them to <c>who</c><br/>
        /// If you just want to check if the player can afford it, 
        /// use <see cref="canBuyProductFromJojaLocal(IEnumerable{Item}, Farmer)"/><br/>
        /// If you just want to mail the product, 
        /// use <see cref="sendLocalPackage(IEnumerable{Item}, Farmer)"/></remarks>
        bool tryBuyLocalProducts(IEnumerable<Item> products, Farmer who);

        /// <summary>
        /// calculate the cost to buy these items<br/>
        /// Has infinate stock so you don't need to worry about stocks
        /// </summary>
        /// <param name="products">the products to be bought, each <c>Key</c> represent a 
        /// <c>QualifiedItemId</c>, and each <c>Value</c> represent the stack count</param>
        /// <returns>The amount of money required</returns>
        int calculateGlobalProductCost(IDictionary<string, int> products);

        /// <summary>
        /// calculate the cost to buy these items<br/>
        /// Has infinate stock so you don't need to worry about stocks
        /// </summary>
        /// <param name="products">the products to be bought, each <c>Key</c> represent a 
        /// <c>ISalable</c>, and each <c>Value</c> represent the stack count</param>
        /// <returns>The amount of money required</returns>
        int calculateGlobalProductCost(IDictionary<ISalable, int> products);

        /// <summary>
        /// calculate the cost to buy these items<br/>
        /// Has infinate stock so you don't need to worry about stocks
        /// </summary>
        /// <param name="products">a list of Items to be bought</param>
        /// <returns>The amount of money required</returns>
        int calculateGlobalProductCost(IEnumerable<Item> products);

        /// <summary>
        /// calculate the cost to buy these items<br/>
        /// throw a <see cref="KeyNotFoundException"/> when the shop doesn't provide one of the specified item. 
        /// </summary>
        /// <param name="products">the products to be bought, each <c>Key</c> represent a 
        /// <c>QualifiedItemId</c>, and each <c>Value</c> represent the stack count</param>
        /// <param name="trades">The trade items required</param>
        /// <param name="stockEnough">whether the stocks are enough</param>
        /// <returns>The amount of money required</returns>
        /// <remarks>Note that if stock size isn't enough (out of stock), you cannot actually buy it</remarks>
        /// <exception cref="KeyNotFoundException"/>
        int calculateLocalProductCost(IDictionary<string, int> products, out IDictionary<string, int> trades, out bool stockEnough);

        /// <summary>
        /// calculate the cost to buy these items<br/>
        /// throw a <see cref="KeyNotFoundException"/> when the shop doesn't provide one of the specified item. 
        /// </summary>
        /// <param name="products">the products to be bought, each <c>Key</c> represent a 
        /// <c>ISalable</c>, and each <c>Value</c> represent the stack count</param>
        /// <param name="trades">The trade items required</param>
        /// <param name="stockEnough">whether the stocks are enough</param>
        /// <returns>The amount of money required</returns>
        /// <remarks>Note that if stock size isn't enough (out of stock), you cannot actually buy it</remarks>
        /// /// <exception cref="KeyNotFoundException"/>
        int calculateLocalProductCost(IDictionary<ISalable, int> products, out IDictionary<ISalable, int> trades, out bool stockEnough);

        /// <summary>
        /// calculate the cost to buy these items<br/>
        /// throw a <see cref="KeyNotFoundException"/> when the shop doesn't provide one of the specified item. 
        /// </summary>
        /// <param name="products">a list contains the itmes to be bought</param>
        /// <param name="trades">The trade items required</param>
        /// <param name="stockEnough">whether the stocks are enough</param>
        /// <returns>The amount of money required</returns>
        /// <remarks>Note that if stock size isn't enough (out of stock), you cannot actually buy it</remarks>
        /// /// <exception cref="KeyNotFoundException"/>
        int calculateLocalProductCost(IEnumerable<Item> products, out IEnumerable<Item> trades, out bool stockEnough);

        /// <summary>
        /// Test whether of not the Farmer <c>who</c> can afford the products<br/>
        /// Sometimes the farmer can't buy them because they are out of stock. 
        /// </summary>
        /// <param name="products">the products to be bought, each <c>Key</c> represent a 
        /// <c>QualifiedItemId</c>, and each <c>Value</c> represent the stack count</param>
        /// <param name="who">The farmer who want to buy these products</param>
        /// <returns>Whether the farmer can afford it</returns>
        /// <remarks>This method won't actually buy the products. If you want to actually buy them, 
        /// <see cref="tryBuyLocalProducts(IDictionary{string, int}, Farmer)"/></remarks>
        bool canBuyProductFromJojaLocal(IDictionary<string, int> products, Farmer who);

        /// <summary>
        /// Test whether of not the Farmer <c>who</c> can afford the products<br/>
        /// Sometimes the farmer can't buy them because they are out of stock. 
        /// </summary>
        /// <param name="products">the products to be bought, each <c>Key</c> represent a 
        /// <c>ISalable</c>, and each <c>Value</c> represent the stack count</param>
        /// <param name="who">The farmer who want to buy these products</param>
        /// <returns>Whether the farmer can afford it</returns>
        /// <remarks>This method won't actually buy the products. If you want to actually buy them, 
        /// <see cref="tryBuyLocalProducts(IDictionary{ISalable, int}, Farmer)"/></remarks>
        bool canBuyProductFromJojaLocal(IDictionary<ISalable, int> products, Farmer who);

        /// <summary>
        /// Test whether of not the Farmer <c>who</c> can afford the products<br/>
        /// Sometimes the farmer can't buy them because they are out of stock. 
        /// </summary>
        /// <param name="products">a list contains the products to be bought</param>
        /// <param name="who">The farmer who want to buy these products</param>
        /// <returns>Whether the farmer can afford it</returns>
        /// <remarks>This method won't actually buy the products. If you want to actually buy them, 
        /// <see cref="tryBuyLocalProducts(IEnumerable{Item}, Farmer)"/></remarks>
        bool canBuyProductFromJojaLocal(IEnumerable<Item> products, Farmer who);

        /// <summary>
        /// Test whether of not the Farmer <c>who</c> can afford the products
        /// </summary>
        /// <param name="products">the products to be bought, each <c>Key</c> represent a 
        /// <c>QualifiedItemId</c>, and each <c>Value</c> represent the stack count</param>
        /// <param name="who">The farmer who want to buy these products</param>
        /// <returns>Whether the farmer can afford it</returns>
        /// <remarks>This method won't actually buy the products. If you want to actually buy them, 
        /// <see cref="tryBuyGlobalProducts(IDictionary{string, int}, Farmer)"/></remarks>
        bool canBuyProductFromJojaGlobal(IDictionary<string, int> products, Farmer who);

        /// <summary>
        /// Test whether of not the Farmer <c>who</c> can afford the products
        /// </summary>
        /// <param name="products">the products to be bought, each <c>Key</c> represent a 
        /// <c>ISalable</c>, and each <c>Value</c> represent the stack count</param>
        /// <param name="who">The farmer who want to buy these products</param>
        /// <returns>Whether the farmer can afford it</returns>
        /// <remarks>This method won't actually buy the products. If you want to actually buy them, 
        /// <see cref="tryBuyGlobalProducts(IDictionary{ISalable, int}, Farmer)"/></remarks>
        bool canBuyProductFromJojaGlobal(IDictionary<ISalable, int> products, Farmer who);

        /// <summary>
        /// Test whether of not the Farmer <c>who</c> can afford the products
        /// </summary>
        /// <param name="products">a list contains the products to be bought</param>
        /// <param name="who">The farmer who want to buy these products</param>
        /// <returns>Whether the farmer can afford it</returns>
        /// <remarks>This method won't actually buy the products. If you want to actually buy them, 
        /// <see cref="tryBuyGlobalProducts(IEnumerable{Item}, Farmer)"/></remarks>
        bool canBuyProductFromJojaGlobal(IEnumerable<Item> products, Farmer who);


        /// <summary>
        /// Open the JojaExpress™ shopping menu.
        /// </summary>
        /// <remarks>Note that this method will NOT examine the conditions of oppening a new menu, <br/>it
        /// will always try to open the menu despite the current game state, <br/>
        /// so if you call this method when you actrually cannot open a menu 
        /// <br/>(e.g, when the game save isn't loaded), this method may give you an error!</remarks>
        void openJojaExpressShoppingMenu();
    }
}
