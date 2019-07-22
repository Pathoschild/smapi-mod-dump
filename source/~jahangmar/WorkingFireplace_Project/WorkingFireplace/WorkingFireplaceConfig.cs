class WorkingFireplaceConfig
{
    public int wood_pieces { get; set; } = 5;
    public bool penalty { get; set; } = true;
    public bool need_fire_in_winter { get; set; } = true;
    public bool need_fire_in_winter_rain { get; set; } = true;
    public bool need_fire_in_spring { get; set; } = false;
    public bool need_fire_in_spring_rain { get; set; } = true;
    public bool need_fire_in_summer { get; set; } = false;
    public bool need_fire_in_summer_rain { get; set; } = false;
    public bool need_fire_in_fall { get; set; } = false;
    public bool need_fire_in_fall_rain { get; set; } = true;
    public double reduce_health { get; set; } = 0.5;
    public double reduce_stamina { get; set; } = 0.5;
    public bool showMessageOnStartOfDay { get; set; } = true;
    public int reduce_friendship_spouse { set; get; } = 40;
    public int reduce_friendship_children { set; get; } = 20;
    public bool COFIntegration { set; get; } = true;
    public double COFMinTemp { set; get; } = 15;
    public double COFRainImpact { set; get; } = 5;
    public bool show_temperature_in_bed { set; get; } = true;
}