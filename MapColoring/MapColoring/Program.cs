using System;
using System.Collections.Generic;
using System.Linq;

/// Sharon Aspir - 7/18/2013
/// Writen for Socialcam
namespace MapColoring
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                // Read path from user
                Console.WriteLine("Hello, Please enter full path to the map file.");
                string path = Console.ReadLine();

                // Read map from path
                char[,] map = ReadMapFromPath(path);

                // Check if map is valid
                if (map != null && map.GetLongLength(0) > 0 && map.GetLongLength(1) > 0)
                {
                    int[,] coloredMap = new int[map.GetLongLength(0), map.GetLongLength(1)];

                    // Color the map
                    ColorAllRegions(map, coloredMap);

                    Console.WriteLine("Original map");
                    PrintMapToScreen(map);

                    Console.WriteLine("Colored map");
                    PrintMapToScreen(IntToColorChar(coloredMap));
                }
                else
                {
                    Console.WriteLine("Error Reading the txt file from path. Please try again.");
                }
            }
        }

        /// <summary>
        ///     Go over map and color all region accordinly
        /// </summary>
        /// <param name="map"></param>
        /// <param name="coloredMap"></param>
        private static void ColorAllRegions(char[,] map, int[,] coloredMap)
        {
            // Go over the map starting the top left corner
            for (int i = 0; i < map.GetLongLength(0); i++)
            {
                for (int j = 0; j < map.GetLongLength(1); j++)
                {
                    // if This is a new region to color   
                    if (coloredMap[i, j] == 0)
                    {
                        // Get The minimal color that is legal to use
                        int color = GetMinimalColorForRegion(map, coloredMap, i, j);

                        // Color the region with this color
                        ColorRegion(map, coloredMap, i, j, color);
                    }
                }
            }
        }

        /// <summary>
        ///     Color a region, that starts at (x,y), with the given color
        /// </summary>
        /// <param name="map"></param>
        /// <param name="colorMap"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        private static void ColorRegion(char[,] map, int[,] colorMap, int x, int y, int color)
        {
            char regionAlpha = map[x, y];
            // Points of the region that we didnt check yet
            List<Tuple<int, int>> unvisitedRegionPoints = new List<Tuple<int, int>>();

            // First point of region inserted to unvisitedRegionPoints
            unvisitedRegionPoints.Add(new Tuple<int, int>(x, y));

            // As long as there are region points we didnt check yet
            while (unvisitedRegionPoints.Count > 0)
            {
                // pop top point
                Tuple<int, int> point = unvisitedRegionPoints.ElementAt(0);
                unvisitedRegionPoints.RemoveAt(0);

                int i = point.Item1;
                int j = point.Item2;

                // Color the selected point
                colorMap[i, j] = color;

                // Checking near points - is them in the same region:
                // bottom point
                CheckPointAndAddToUnvisited(i, j - 1, unvisitedRegionPoints, map, colorMap, regionAlpha);
                // upper region
                CheckPointAndAddToUnvisited(i, j + 1, unvisitedRegionPoints, map, colorMap, regionAlpha);
                // left region
                CheckPointAndAddToUnvisited(i - 1, j, unvisitedRegionPoints, map, colorMap, regionAlpha);
                // right region
                CheckPointAndAddToUnvisited(i + 1, j, unvisitedRegionPoints, map, colorMap, regionAlpha);
            }
        }

        /// <summary>
        ///     Get the minimal color that is valid to color the region with.
        ///     Region marked by point (x,y) inside the region
        /// </summary>
        /// <param name="map"></param>
        /// <param name="coloredMap"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static int GetMinimalColorForRegion(char[,] map, int[,] coloredMap, int x, int y)
        {
            char regionAlpha = map[x, y];
            List<int> neighboringColors = new List<int>();

            // Points of the region that we didnt check yet
            List<Tuple<int, int>> unvisitedRegionPoints = new List<Tuple<int, int>>();

            // Points of the region that we already checked
            List<Tuple<int, int>> visitedRegionPoints = new List<Tuple<int, int>>();

            // First point of region inserted to unvisitedRegionPoints
            unvisitedRegionPoints.Add(new Tuple<int, int>(x, y));

            while (unvisitedRegionPoints.Count > 0)
            {
                // pop top point
                Tuple<int, int> point = unvisitedRegionPoints.ElementAt(0);
                unvisitedRegionPoints.RemoveAt(0);

                // Mark it as visited
                visitedRegionPoints.Add(point);

                int i = point.Item1;
                int j = point.Item2;

                CheckPointAndAddNeighboringColor(i, j - 1, visitedRegionPoints, unvisitedRegionPoints, map, coloredMap, regionAlpha, neighboringColors);
                CheckPointAndAddNeighboringColor(i, j + 1, visitedRegionPoints, unvisitedRegionPoints, map, coloredMap, regionAlpha, neighboringColors);
                CheckPointAndAddNeighboringColor(i + 1, j, visitedRegionPoints, unvisitedRegionPoints, map, coloredMap, regionAlpha, neighboringColors);
                CheckPointAndAddNeighboringColor(i - 1, j, visitedRegionPoints, unvisitedRegionPoints, map, coloredMap, regionAlpha, neighboringColors);

            }

            int colorPicked = 1;
            while (neighboringColors.Contains(colorPicked)) { colorPicked++; }

            return colorPicked;
        }

        /// <summary>
        ///     Checks if given point is valid, is not visited and in the same region.
        ///     If so - we add it to the unvisitedRegionPoints
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="visitedRegionPoints"></param>
        /// <param name="unvisitedRegionPoints"></param>
        /// <param name="map"></param>
        /// <param name="regionAlpha"></param>
        private static void CheckPointAndAddToUnvisited(int x, int y, List<Tuple<int, int>> unvisitedRegionPoints, char[,] map, int[,] coloredMap, char regionAlpha)
        {
            if (x >= 0 && y >= 0 && x < map.GetLongLength(0) && y < map.GetLongLength(1) && map[x, y] == regionAlpha)
            {
                // If coloredMap[x, y] != 0 this is a visited point
                if (coloredMap[x, y] == 0)
                {
                    unvisitedRegionPoints.Add(new Tuple<int, int>(x, y));
                }
            }
        }

        /// <summary>
        ///     Check Pgiven point (x,y), if it is unvisited and from our region - we add it to unvisitedRegionPoints list
        ///     If it is not our region, it is a neighboring region, so we add its color to the neighboringColors list
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="visitedRegionPoints"></param>
        /// <param name="unvisitedRegionPoints"></param>
        /// <param name="map"></param>
        /// <param name="coloredMap"></param>
        /// <param name="regionAlpha"></param>
        /// <param name="neighboringColors"></param>
        private static void CheckPointAndAddNeighboringColor(int x, int y, List<Tuple<int, int>> visitedRegionPoints, List<Tuple<int, int>> unvisitedRegionPoints, char[,] map, int[,] coloredMap, char regionAlpha, List<int> neighboringColors)
        {
            // Is valid point
            if (x >= 0 && y >= 0 && x < map.GetLongLength(0) && y < map.GetLongLength(1))
            {
                // This is an unvisited region point
                if (map[x, y] == regionAlpha && !visitedRegionPoints.Contains(new Tuple<int, int>(x, y)))
                {
                    unvisitedRegionPoints.Add(new Tuple<int, int>(x, y));
                }
                // This is a neighboring region
                if (map[x, y] != regionAlpha)
                {
                    int color = coloredMap[x, y];
                    // Add the color to neighboring colors list, if it is not there alredy
                    if (!neighboringColors.Contains(color)) neighboringColors.Add(color);
                }
            }
        }

        /// <summary>
        ///     Read text from the file in the input path, 
        ///     And return the text as two dimitinal array of chars 
        /// </summary>
        /// <param name="path">
        ///     File path
        /// </param>
        /// <returns>
        ///     The file text as two dimitinal array of chars . Or null if failed.
        /// </returns>
        private static char[,] ReadMapFromPath(string path)
        {
            try
            {
                // Read all lines
                string[] lines = System.IO.File.ReadAllLines(path);
                char[,] map = new char[lines.Length, lines[0].Length];

                // Break lines into char[]
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    for (int j = 0; j < line.Length; j++)
                    {
                        map[i, j] = line.ElementAt(j);
                    }
                }
                return map;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     Print the two dimincnial map to console screen
        /// </summary>
        /// <typeparam name="T">
        ///     Type of two dimincnial array
        /// </typeparam>
        /// <param name="map">
        ///     The two dimincnial map
        /// </param>
        private static void PrintMapToScreen<T>(T[,] map)
        {
            Console.WriteLine("-----");

            // Go over map and print each char
            for (int i = 0; i < map.GetLongLength(0); i++)
            {
                for (int j = 0; j < map.GetLongLength(1); j++)
                {
                    Console.Write(map[i, j]);
                }
                // End line
                Console.WriteLine();
            }
            Console.WriteLine("-----");
        }

        /// <summary>
        ///     Change map from colors as nubers to colors as chars
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        private static char[,] IntToColorChar(int[,] map)
        {
            char[,] coloredMap = new char[map.GetLongLength(0), map.GetLongLength(1)];

            // Go over map and print each char
            for (int i = 0; i < map.GetLongLength(0); i++)
            {
                for (int j = 0; j < map.GetLongLength(1); j++)
                {
                    switch (map[i, j])
                    {
                        case 1:
                            coloredMap[i, j] = 'R';
                            break;
                        case 2:
                            coloredMap[i, j] = 'G';
                            break;
                        case 3:
                            coloredMap[i, j] = 'B';
                            break;
                        case 4:
                            coloredMap[i, j] = 'Y';
                            break;
                    }
                }
            }
            return coloredMap;
        }
    }
}
