namespace FamilyPlanning
{
    class ModConfig
    {
        public bool AdoptChildrenWithRoommate { get; set; }
        public bool BabyQuestionMessages { get; set; }

        public ModConfig()
        {
            AdoptChildrenWithRoommate = false;
            BabyQuestionMessages = false;
        }
    }
}