namespace AfkTimer.Models
{
    class TimeConfig
    {
        public bool IsActive { get; set; } = true;
        public int IdleTime { get; set; } = 30;
    }
}
