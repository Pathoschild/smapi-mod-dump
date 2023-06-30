/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Survivalistic
**
*************************************************/

namespace Survivalistic
{
    public class Config
    {
        // MULTIPLIERS
        public float thirst_multiplier { get; set; } = 0.5f;
        public float hunger_multiplier { get; set; } = 1f;

        public float hunger_action_multiplier { get; set; } = 1f;

        public float thirst_action_multiplier { get; set; } = 1f;

        // BARS PIVOT
        public string bars_position { get; set; } = "bottom-right";

        // CUSTOM BARS AXIS (use "custom" in the pivot to use this)
        public int bars_custom_x { get; set; } = 0;
        public int bars_custom_y { get; set; } = 0;

        public bool non_supported_food { get; set; }

        public bool decrease_values_after_sleep { get; set; }

        public int food_decrease_after_sleep { get; set; }

        public int thirst_decrease_after_sleep { get; set; }
    }
}
