using System.Collections.Generic;
using CyclicDominoes.Core.Models;

namespace CyclicDominoes.Core
{
    public interface IDominoesCyclicPermutationFinder
    {
        public List<DominoTile> TryGetCyclicPermutationForSet(List<DominoTile> set, out bool isCyclicPermutationFound);
    }
}