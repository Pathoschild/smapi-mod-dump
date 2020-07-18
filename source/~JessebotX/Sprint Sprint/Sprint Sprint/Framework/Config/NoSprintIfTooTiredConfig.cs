namespace Sprint_Sprint.Framework.Config
{
    class NoSprintIfTooTiredConfig
    {
        /// <summary> If sprinting is disabled when you are below the <see cref="TiredStamina"/> value </summary>
        public bool Enabled { get; set; } = true;
        /// <summary> Going below this value will prevent you from sprinting if <see cref="Enabled"/> is true </summary>
        public float TiredStamina { get; set; } = 20f;
    }
}
