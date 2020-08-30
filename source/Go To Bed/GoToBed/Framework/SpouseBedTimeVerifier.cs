using System;

using StardewModdingAPI;


namespace GoToBed.Framework {

    /// <summary>
    /// Verifies spouse's time to get up and to go to bed.
    /// </summary>
    internal class SpouseBedTimeVerifier {
        /// <summary>
        /// Default time for spouse to get up: Spouse gets up before farmer.
        /// </summary>
        public const int DefaultGetUpTime = 600;

        /// <summary>
        /// Default time for spouse to go to bed.
        /// </summary>
        public const int DefaultGoToBedTime = 2200;

        /// <summary>
        /// Returns time to get up.
        /// </summary>
        public int GetUpTime { get; }

        /// <summary>
        /// Returns time to go to bed.
        /// </summary>
        public int GoToBedTime { get; }

        /// <summary>
        /// Returns true if both times are default, false otherwise.
        /// </summary>
        public bool IsDefault {
            get => (GetUpTime == DefaultGetUpTime && GoToBedTime == DefaultGoToBedTime);
        }

        public SpouseBedTimeVerifier(ModConfig config, IMonitor monitor) {
            GetUpTime = NormalizeTime(config.SpouseGetUpTime);
            if (GetUpTime < DefaultGetUpTime || GetUpTime > 1200) {
                monitor.Log($"Invalid time {GetUpTime} to get up, using default (spouse gets up before 600)");
                GetUpTime = DefaultGetUpTime;
            }
            else {
                monitor.Log($"Spouse gets up at {GetUpTime}");                
            }

            GoToBedTime = NormalizeTime(config.SpouseGoToBedTime);
            // Spouse finally goes to bed at 2500 (1AM).
            if (GoToBedTime < 1800 || GoToBedTime > 2500) {
                monitor.Log($"Invalid time {GoToBedTime} to go to bed, using default ({DefaultGoToBedTime})");
                GoToBedTime = DefaultGoToBedTime;
            }
            else {
                monitor.Log($"Spouse goes to bed at {GoToBedTime}");                
            }
        }

        /// <summary>
        /// Normalizes time and rounds it to a multiple of 10 minutes.
        /// </summary>
        private static int NormalizeTime(int timeOfDay) {
            int hours   = timeOfDay / 100;
            int minutes = timeOfDay % 100;

            // Normalize time.
            hours += minutes / 60;
            minutes = minutes % 60;

            // Round minutes to multiple of 10.
            minutes = (int) Math.Round(minutes / 10.0) * 10;

            // Greatest valid number of minutes is 50 .
            minutes = Math.Min(minutes, 50);

            return hours * 100 + minutes;
        }
    }
}