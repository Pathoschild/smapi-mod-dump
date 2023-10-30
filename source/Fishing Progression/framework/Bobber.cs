/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chudders1231/SDV-FishingProgression
**
*************************************************/

using StardewModdingAPI;
using StardewValley.Menus;

namespace FishingProgression.reflection
{
    internal class Bobber
    {
        /*********
        ** Fields
        *********/

        /// <summary>The underlying field for <see cref="DifficultyField"/>.</summary>
        private readonly IReflectedField<float> DifficultyField;

        /// <summary>The underlying field for <see cref="MotionTypeField"/>.</summary>
        private readonly IReflectedField<int> MotionTypeField;

        /// <summary>The underlying field for <see cref="TreasureCaughtField"/>.</summary>
        private readonly IReflectedField<bool> TreasureCaughtField;

        /// <summary>The underlying field for <see cref="BobberInBar"/>.</summary>
        private readonly IReflectedField<bool> BobberInBarField;

        /// <summary>The underlying field for <see cref="Treasure"/>.</summary>
        private readonly IReflectedField<bool> TreasureField;

        /// <summary>The underlying field for <see cref="Perfect"/>.</summary>
        private readonly IReflectedField<bool> PerfectField;


        /*********
        ** Accessors
        *********/
        /// <summary>The underlying bobber bar.</summary>
        public BobberBar Instance { get; set; }

        public float Difficulty
        {
            get => this.DifficultyField.GetValue();
            set => this.DifficultyField.SetValue(value);
        }

        public int MotionType
        {
            get => this.MotionTypeField.GetValue();
            set => this.MotionTypeField.SetValue(value);
        }

        public bool BobberInBar
        {
            get => this.BobberInBarField.GetValue();
        }

        /// <summary>
        ///     Whether or not a treasure chest appears
        /// </summary>
        public bool Treasure
        {
            get => this.TreasureField.GetValue();
            set => this.TreasureField.SetValue(value);
        }

        public bool TreasureCaught
        {
            get => this.TreasureCaughtField.GetValue();
            set => this.TreasureCaughtField.SetValue(value);
        }

        public bool Perfect
        {
            get => this.PerfectField.GetValue();
            set => this.PerfectField.SetValue(value);
        }

        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="instance">The underlying bobber bar.</param>
        /// <param name="reflection">Simplifies access to private code.</param>
        public Bobber(BobberBar instance, IReflectionHelper reflection)
        {
            this.Instance = instance;

            this.DifficultyField = reflection.GetField<float>(instance, "difficulty");
            this.MotionTypeField = reflection.GetField<int>(instance, "motionType");
            this.BobberInBarField = reflection.GetField<bool>(instance, "bobberInBar");
            this.TreasureField = reflection.GetField<bool>(instance, "treasure");
            this.TreasureCaughtField = reflection.GetField<bool>(instance, "treasureCaught");
            this.PerfectField = reflection.GetField<bool>(instance, "perfect");
        }
    }
}
