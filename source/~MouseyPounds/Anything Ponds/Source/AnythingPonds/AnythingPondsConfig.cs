namespace AnythingPonds
{
    class AnythingPondsConfig
    {
        public bool Enable_Algae_and_Seaweed_Pond_Definitions { get; set; } = true;
        public bool Allow_Empty_Ponds_to_Become_Algae_or_Seaweed { get; set; } = true;
        public int Number_of_Days_for_Empty_Pond_to_become_Algae_or_Seaweed { get; set; } = 7;

    }
}
