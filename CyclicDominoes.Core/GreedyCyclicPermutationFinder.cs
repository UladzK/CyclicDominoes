using System.Collections.Generic;
using System.Linq;
using CyclicDominoes.Core.Models;

namespace CyclicDominoes.Core
{ 
    public class GreedyCyclicPermutationFinder : IDominoesCyclicPermutationFinder
    {
        private readonly IRandomProvider _randomProvider;
        
        public GreedyCyclicPermutationFinder(IRandomProvider randomProvider)
        {
            _randomProvider = randomProvider;
        }

        public List<DominoTile> TryGetCyclicPermutationForSet(List<DominoTile> set, out bool isCyclicPermutationFound)
        {
            var composedTileChain = new List<DominoTile>();

            var tileGraphPathsMap = CreateTileGraphPathsMapFromSet(set);

            var tileSideToAppend = RandomlyPickFirstDigit(tileGraphPathsMap);
            while (TryPickNextSuitableTile(tileGraphPathsMap, tileSideToAppend, out var pickedTile))
            {
                tileGraphPathsMap[tileSideToAppend].Remove(pickedTile);

                if (!pickedTile.IsNodeTraversed)
                {
                    var tile = GetProperTileSide(tileSideToAppend, pickedTile);

                    composedTileChain.Add(new DominoTile { LeftValue = tile.leftSide, RightValue = tile.rightSide});
                    tileSideToAppend = tile.rightSide;

                    pickedTile.IsNodeTraversed = true;
                }
            }

            isCyclicPermutationFound = composedTileChain.Count == set.Count
                                       && composedTileChain[0].LeftValue == composedTileChain[^1].RightValue;

            return composedTileChain;
        }

        private int RandomlyPickFirstDigit(Dictionary<int, List<TileGraphNode>> tileGraphPathsMap)
        {
            var digitsInPool = tileGraphPathsMap.Keys.ToList();

            return digitsInPool[_randomProvider.GetNextRandom(0, digitsInPool.Count)];
        }

        private bool TryPickNextSuitableTile(Dictionary<int, List<TileGraphNode>> pathsMap, int tileSideNeeded,
            out TileGraphNode pickedTileGraph)
        {
            var tilesWithNeededDigit = pathsMap[tileSideNeeded];

            if (!tilesWithNeededDigit.Any())
            {
                pickedTileGraph = null;
                return false;
            }

            var doubleTile = tilesWithNeededDigit.FirstOrDefault(t => t.IsDouble);
            if (doubleTile != null)
            {
                pickedTileGraph = doubleTile;
                return true;
            }

            pickedTileGraph = GetTileWithMostUnusedDigit(pathsMap, tileSideNeeded);

            return true;
        }

        private static TileGraphNode GetTileWithMostUnusedDigit(Dictionary<int, List<TileGraphNode>> pathsMap, int tileSideNeeded)
        {
            var tilesWithRequiredDigit = pathsMap[tileSideNeeded];

            var otherUniqueDigitsInPool = tilesWithRequiredDigit
                .Aggregate(new List<int>(),
                    (acc, t) => acc.Concat(new List<int> {t.LeftValue, t.RightValue}).ToList())
                .Where(ud => ud != tileSideNeeded)
                .Distinct();

            var tilesLeftWithThatDigit = otherUniqueDigitsInPool
                .Select(d => new {digit = d, howManyTilesLeft = pathsMap[d].Count})
                .ToList();
            var tileWithMaxLeftDigit = tilesLeftWithThatDigit.Max(t => t.howManyTilesLeft);

            var preferredSecondDigit =
                tilesLeftWithThatDigit.First(t => t.howManyTilesLeft == tileWithMaxLeftDigit).digit;

            return tilesWithRequiredDigit.First(t =>
                t.LeftValue == preferredSecondDigit || t.RightValue == preferredSecondDigit);
        }

        private (int leftSide, int rightSide) GetProperTileSide(int previousTileNumber, TileGraphNode graphNode)
        {
            if (previousTileNumber == graphNode.LeftValue)
            {
                return (graphNode.LeftValue, graphNode.RightValue);
            }

            return (graphNode.RightValue, graphNode.LeftValue);
        }

        private Dictionary<int, List<TileGraphNode>> CreateTileGraphPathsMapFromSet(List<DominoTile> set)
        {
            var tileGraphPathsMap = new Dictionary<int, List<TileGraphNode>>();

            foreach (var tile in set)
            {
                var tileNode = new TileGraphNode
                {
                    LeftValue = tile.LeftValue,
                    RightValue = tile.RightValue,
                    IsNodeTraversed = false
                };

                if (tileNode.IsDouble)
                {
                    tileGraphPathsMap.TryGetValue(tileNode.LeftValue, out var listForLeftValue);
                    (listForLeftValue ??= new List<TileGraphNode>()).Insert(0, tileNode);
                    tileGraphPathsMap[tileNode.LeftValue] = listForLeftValue;
                }
                else
                {
                    tileGraphPathsMap.TryGetValue(tileNode.LeftValue, out var listForLeftValue);
                    (listForLeftValue ??= new List<TileGraphNode>()).Add(tileNode);
                    tileGraphPathsMap[tileNode.LeftValue] = listForLeftValue;

                    tileGraphPathsMap.TryGetValue(tileNode.RightValue, out var liftForRightValue);
                    (liftForRightValue ??= new List<TileGraphNode>()).Add(tileNode);
                    tileGraphPathsMap[tileNode.RightValue] = liftForRightValue;
                }
            }

            return tileGraphPathsMap;
        }

        private bool CheckIfAnyNumberHasSingleOccurrence(List<DominoTile> set)
        {
            var digitOccurrencesMap = new Dictionary<int, int>();

            foreach (var s in set)
            {
                digitOccurrencesMap.TryGetValue(s.LeftValue, out var numberOfOccurrencesForLeftDigit);
                digitOccurrencesMap[s.LeftValue] = numberOfOccurrencesForLeftDigit + 1;
                
                digitOccurrencesMap.TryGetValue(s.RightValue, out var numberOfOccurrencesForRightDigit);
                digitOccurrencesMap[s.RightValue] = numberOfOccurrencesForRightDigit + 1;
            }

            return digitOccurrencesMap.Values.Any(o => o == 1);
        }
    }
}