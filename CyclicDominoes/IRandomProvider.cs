namespace CyclicDominoes
{
    public interface IRandomProvider
    {
        int GetNextRandom(int minValue, int maxValue);
    }
}