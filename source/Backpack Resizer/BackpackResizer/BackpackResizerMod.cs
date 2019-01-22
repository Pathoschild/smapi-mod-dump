using StardewModdingAPI;
using StardewValley;

namespace BackpackResizer
{
    public class BackpackResizerMod : Mod
    {
        private readonly string setBackpackSizeCommandName;
        private readonly string setBackpackSizeForceName;

        private readonly string getBackpackSizeCommandName;

        // Set defaults
        public BackpackResizerMod()
        {
            setBackpackSizeCommandName = "set_backpack_size";
            setBackpackSizeForceName = "force";

            getBackpackSizeCommandName = "get_backpack_size";
        }

        // Entry code - executed on mod load
        public override void Entry(IModHelper helper)
        {
            string helpText = "Sets size of the player's inventory.\n\nUsage: " + setBackpackSizeCommandName + " size [force]" +
                "\n- size: the number of inventory slots in inventory screen (a number that is a multiple of 12) " +
                "\n- force: NOT RECOMMENDED - deletes items out of backpack that cannot be dropped and allows for sizes that are not a multiple of 12";

            helper.ConsoleCommands.Add(setBackpackSizeCommandName, helpText, (string command, string[] args) =>
            {
                if (!Context.IsWorldReady)
                {
                    Monitor.Log("Warning: Please wait for the world to load and try again.", LogLevel.Warn);
                    return;
                }

                int backpackSize;
                if (args.Length < 1)
                {
                    Monitor.Log("Usage: " + setBackpackSizeCommandName + " 36 ", LogLevel.Error);
                    return;
                }
                else if (!int.TryParse(args[0], out backpackSize))
                {
                    Monitor.Log("Error: '" + args[0] + "' is not a valid integer. Usage: " + setBackpackSizeCommandName + " 36 ", LogLevel.Error);
                    return;
                }
                else if (backpackSize % 12 != 0 && (args.Length < 2 || args[1] != setBackpackSizeForceName))
                {
                    Monitor.Log("Warning: " + args[0] + " is not divisible by 12 and will cause weird results.\n" +
                        "To update regardless, use: " + setBackpackSizeCommandName + " " + backpackSize + " " + setBackpackSizeForceName, LogLevel.Warn);
                    return;
                }
                else if (HasItemsThatWillBeDeleted(Game1.player, backpackSize) && (args.Length < 2 || args[1] != setBackpackSizeForceName))
                {
                    Monitor.Log("Warning: Making backpack smaller WILL DELETE ITEMS PERMANENTLY from your inventory. " +
                        "Put your items into a chest and try again.\n" +
                        "To update the size AND delete items, use: " + setBackpackSizeCommandName + " " + backpackSize + " " + setBackpackSizeForceName, LogLevel.Warn);

                    return;
                }

                SetBackpackSizeForce(Game1.player, backpackSize);

                Monitor.Log("Set backpack size to " + Game1.player.MaxItems, LogLevel.Info);
            });

            string getHelpText = "Gets the size of the player's inventory.\n\nUsage: " + getBackpackSizeCommandName;
            helper.ConsoleCommands.Add(getBackpackSizeCommandName, getHelpText, (string command, string[] args) =>
            {
                Monitor.Log(Game1.player.Name + "'s backpack size: " + Game1.player.MaxItems, LogLevel.Info);
            });
        }

        /// <summary>
        ///     Resizes the backpack if no items are getting deleted in the process
        /// </summary>
        /// <param name="farmer"></param>
        /// <param name="size"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public bool SetBackpackSize(Farmer farmer, int size)
        {
            if (HasItemsThatWillBeDeleted(farmer, size)) return false;

            SetBackpackSizeForce(farmer, size);
            return true;
        }

        /// <summary>
        ///     Resizes the backpack if no items are getting deleted in the process or if the force parameter is set to true.
        /// </summary>
        /// <param name="farmer">Player affected by resize</param>
        /// <param name="size">Individual inventory slot count</param>
        /// <param name="force">Force operation, even if it deletes player's items permanently</param>
        /// <returns>True if backpack resized, false if backpack was not resized</returns>
        public bool SetBackpackSize(Farmer farmer, int size, bool force)
        {
            if (!force && HasItemsThatWillBeDeleted(farmer, size)) return false;

            SetBackpackSizeForce(farmer, size);
            return true;
        }

        /// <summary>
        ///     Checks to see if the proposed resize will delete items in the inventory
        /// </summary>
        /// <param name="farmer"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool HasItemsThatWillBeDeleted(Farmer farmer, int size)
        {
            if (size >= farmer.MaxItems) return false;

            for (int i = size; i < farmer.Items.Count; i++)
            {
                
                if (farmer.Items[i] != null && !farmer.Items[i].canBeDropped()) return true;
            }

            return false;
        }

        private void SetBackpackSizeForce(Farmer farmer, int size)
        {
            if (farmer.MaxItems == size) return;
            else if (farmer.MaxItems < size) IncreaseBackpackSize(farmer, size);
            else DecreaseBackpackSize(farmer, size);

            farmer.MaxItems = size;
        }

        // Drop item if it is droppable. If not, it will be deleted.
        private void DecreaseBackpackSize(Farmer farmer, int size)
        {
            for (int i = farmer.Items.Count - 1; i >= size; i--)
            {
                farmer.dropItem(farmer.Items[i]);
                farmer.Items.RemoveAt(i);
            }
        }

        // Add backpack slots. Items collection must be updated as well.
        private void IncreaseBackpackSize(Farmer farmer, int size)
        {
            int additional = size - farmer.MaxItems;
            for (int i = 0; i < additional; i++) farmer.Items.Add(null);
        }
    }
}
