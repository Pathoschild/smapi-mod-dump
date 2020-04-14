using StardewValley;

namespace WorkbenchAnywhere.Framework
{
    public class AnywhereBluePrint : BluePrint
    {
        private readonly MaterialStorage _materialStorage;

        internal AnywhereBluePrint(string name, MaterialStorage materialStorage) : base(name)
        {
            _materialStorage = materialStorage;
        }

        public new virtual bool doesFarmerHaveEnoughResourcesToBuild()
        {
            return true;
        }

        public new virtual void consumeResources()
        {
        }
    }
}
