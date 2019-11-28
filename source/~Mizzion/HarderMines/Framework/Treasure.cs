namespace HarderMines.Framework
{
    internal class Treasure
    {
        public int Id;
        public int Count;

        /// <summary>
        /// Iniate the Treasure
        /// </summary>
        /// <param name="id">Treasure ID</param>
        /// <param name="count">The Number of id to give.</param>
        public Treasure(int id, int count)
        {
            Id = id;
            Count = count;
        }
    }
}
