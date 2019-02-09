namespace TehPers.Core {
    public static class Constants {
        ///<summary>For text inputs, number of seconds between when an input key is pressed and when it starts repeating.</summary>
        public static float KeyRepeatDelay { get; set; } = 1F;

        ///<summary>For text inputs, The minimum number of times a key is repeated each second it's held.</summary>
        public static float KeyRepeatMinFrequency { get; set; } = 15F;

        ///<summary>For text inputs, the maximum number of times a key is repeated each second it's held.</summary>
        public static float KeyRepeatMaxFrequency { get; set; } = 30F;

        ///<summary>For text inputs, the number of seconds it takes for the frequency to go from min to max.</summary>
        public static float KeyRepeatRampTime { get; set; } = 0.5F;
    }
}
