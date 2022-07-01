namespace CyclicDominoes.Core
{
    public interface IRandomProvider
    {
        int GetNextRandom(int minValue, int maxValue);
    }
}