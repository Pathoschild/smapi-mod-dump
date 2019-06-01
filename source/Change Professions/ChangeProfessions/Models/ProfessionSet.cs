namespace ChangeProfessions
{
    public class ProfessionSet
    {
        public int[] Ids { get; set; }
        public int? ParentId { get; set; }
        public bool IsPrimaryProfession => ParentId == null;
    }
}