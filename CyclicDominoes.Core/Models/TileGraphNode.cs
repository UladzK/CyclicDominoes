namespace CyclicDominoes.Core.Models
{
    internal class TileGraphNode
    {
        public int LeftValue { get; init; }
        public int RightValue { get; init; }
        public bool IsNodeTraversed { get; set; }

        public bool IsDouble => LeftValue == RightValue;
    }
}