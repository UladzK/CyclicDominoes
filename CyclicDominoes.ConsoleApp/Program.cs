using System;
using System.Collections.Generic;
using System.IO;
using CyclicDominoes.Core;
using CyclicDominoes.Core.Models;
using Newtonsoft.Json;

namespace CyclicDominoes.ConsoleApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            var filePath = args != null && args.Length > 0 ? args[0] : string.Empty;

            var dominoesSet = GetArrayFromInput(filePath);
            var dominoTilesSet = MapArrayToDominoesSet(dominoesSet);

            var cycleFinder = new GreedyCyclicPermutationFinder(new RandomProvider());
            var cyclicChain = cycleFinder.TryGetCyclicPermutationForSet(dominoTilesSet, out var isSolutionFound);
            
            if (!isSolutionFound)
            {
                Console.WriteLine("Cyclic permutation not found or does not exist. Maximum chain: ");
            }
            else
            {
                Console.WriteLine("Cyclic permutation: ");
            }
            
            foreach (var tile in cyclicChain)
            {
                Console.WriteLine($"[{tile.LeftValue}|{tile.RightValue}]");
            }

            Console.WriteLine("Press a key to exit.");
            Console.ReadKey();
        }

        private static List<DominoTile> MapArrayToDominoesSet(int[,] set)
        {
            var dominoTilesSet = new List<DominoTile>();
            
            for (var i = 0; i < set.GetLength(0); i++)
            {
                dominoTilesSet.Add(new DominoTile{ LeftValue = set[i,0], RightValue = set[i,1]});                
            }

            return dominoTilesSet;
        }

        private static int[,] GetArrayFromInput(string userInput)
        {
            if (userInput == null || !File.Exists(userInput))
            {
                throw new FileNotFoundException($"Incorrect path to file: {userInput ?? string.Empty}. Check README for usage example");
            }

            var fileContent = JsonConvert.DeserializeObject<int[,]>(File.ReadAllText(userInput));

            if (fileContent == null)
            {
                throw new FormatException("Data has incorrect format");
            }
            
            if (fileContent.GetLength(1) != 2)
            {
                throw new FormatException("Input should be a two-dimensional array");
            }

            return fileContent;
        }
    }
}
