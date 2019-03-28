class WorkingFireplaceConfig
{
    public int wood_pieces { get; set; } = 5;
    public bool penalty { get; set; } = true;
    public double reduce_health { get; set; } = 0.5;
    public double reduce_stamina { get; set; } = 0.5;
    public bool showMessageOnStartOfDay { get; set; } = true;
}