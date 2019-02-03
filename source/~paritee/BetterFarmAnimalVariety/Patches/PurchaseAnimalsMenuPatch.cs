namespace BetterFarmAnimalVariety.Patches
{
    class PurchaseAnimalsMenuPatch
    {
        public static bool getAnimalTitlePrefix(ref string name, ref string __result)
        {
            string[] parts = name.Split('_');

            __result = parts[0];

            if (parts.Length < 2)
            {
                return true;
            }

            return false;
        }

        public static bool getAnimalDescriptionPrefix(ref string name, ref string __result)
        {
            string[] parts = name.Split('_');

            if (parts.Length < 2)
            {
                return true;
            }

            __result = parts[1];

            return false;
        }
    }
}
