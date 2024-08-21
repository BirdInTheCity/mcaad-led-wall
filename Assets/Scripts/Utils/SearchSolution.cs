/*
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Utils
{


    /*public class Node
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Index { get; set; }
        public float AspectRatio { get; set; }
        public bool Dummy { get; set; }
        
        public Texture2D Texture { get; set; }
    }

    public class MinMax
    {
        public float Min { get; set; }
        public float Max { get; set; }
    }
    #1#




    public static class SolutionSearcher
    {
        private const float BIG = 999999999;

        private static void CalculateSizes(Node[] fullArray, int splits, int nextPower, MinMax minmax)
        {
            var wider = fullArray[^1].AspectRatio >= 1;
            fullArray[nextPower - 1].Width = wider ? 1 : fullArray[nextPower - 1].AspectRatio;
            fullArray[nextPower - 1].Height = wider ? 1 / fullArray[nextPower - 1].AspectRatio : 1;

            for (int i = nextPower - 2; i >= 0; i--)
            {
                var d = i << 1;
                var a1 = fullArray[d].AspectRatio;
                var a2 = fullArray[d + 1].AspectRatio;
                if ((splits & (1 << i)) != 0)
                {
                    fullArray[i].Width = fullArray[d].Width;
                    fullArray[d + 1].X = fullArray[d].Width;
                    fullArray[d + 1].Width = fullArray[d].Width * a2 / a1;
                    fullArray[i].Height = fullArray[d + 1].Height;
                    fullArray[d].Y = fullArray[d + 1].Height;
                    fullArray[d].Height = fullArray[d + 1].Height * a1 / a2;
                }
                else
                {
                    fullArray[i].Width = fullArray[d].Width + fullArray[d + 1].Width;
                    fullArray[i].Height = fullArray[d].Height;
                    fullArray[d + 1].Y = fullArray[d].Height;
                    fullArray[d + 1].Height = fullArray[d].Height * a2 / a1;
                }

                if (fullArray[i].Width < minmax.Min)
                {
                    minmax.Min = fullArray[i].Width;
                }

                if (fullArray[i].Width > minmax.Max)
                {
                    minmax.Max = fullArray[i].Width;
                }
            }
        }
        
        public static Node[] FindSolution(int width, int height, float[] aspectRatios, int searchTime)
        {
            const int BIG = 999999999;

            int targetRatio = width / height;
            int nextPower = NextPowerOfTwo(aspectRatios.Length);
            Node[] fullArray = new Node[(nextPower << 1) - 1];
            bool[] splitCache = new bool[nextPower];
            DateTime endTime = DateTime.Now.AddMilliseconds(searchTime);
            float previousMinScore = BIG;
            Node[] winner = null;

            for (int i = 0; i < fullArray.Length; i++)
            {
                fullArray[i] = new Node();
            }

            while (DateTime.Now < endTime)
            {
                int splits = GetRandomInt(0, nextPower);
                if (splitCache[splits])
                {
                    continue;
                }

                splitCache[splits] = true;

                NewShuffle(fullArray, 0, nextPower);

                for (int i = 0; i < nextPower - 1; i++)
                {
                    int d = i << 1;
                    float a1 = fullArray[d].AspectRatio;
                    float a2 = fullArray[d + 1].AspectRatio;
                    if (a1 != 0 && a2 != 0)
                    {
                        float a = a1 + a2;
                        if ((splits & (1 << i)) != 0)
                        {
                            a = (a1 * a2) / a;
                        }
                    }
                    else
                    {
                        a = a1 || a2;
                    }

                    fullArray[i + nextPower].AspectRatio = a;
                }

                bool wider = targetRatio > fullArray[nextPower - 1].AspectRatio;
                float currentDifference = wider ? -1 + targetRatio / fullArray[nextPower - 1].AspectRatio : 1 - targetRatio / fullArray[nextPower - 1].AspectRatio;

                if (currentDifference < 0.05f)
                {
                    Node last = fullArray[fullArray.Length - 1];
                    last.Width = wider ? height * fullArray[nextPower - 1].AspectRatio : width;
                    last.Height = wider ? height : width / fullArray[nextPower - 1].AspectRatio;
                    MinMax minmax = new MinMax { Min = BIG, Max = -BIG };
                    CalculateSizes(fullArray, splits, nextPower, minmax);

                    float score = currentDifference + (1 - minmax.Min / minmax.Max);
                    if (score < previousMinScore)
                    {
                        previousMinScore = score;
                        Node[] arr = fullArray.Take(nextPower).Where(n => !n.Dummy).ToArray();
                        winner = arr.Select(n => new Node
                        {
                            X = n.X,
                            Y = n.Y,
                            Index = n.Index,
                            Width = n.Width,
                            Height = n.Height
                        }).ToArray();
                    }
                }
            }

            return winner;
        }

        public static Node[] SearchSolution(int width, int height, Texture2D[] pics, int searchTime)
        {
            var nodes = pics.Select((r, i) => new Node
            {
                X = 0,
                Y = 0,
                Width = 0,
                Height = 0,
                Index = i,
                AspectRatio = (float) r.width / r.height,
                Dummy = false
            }).ToArray();

            var targetRatio = (float)width / height;
            var nextPower = NextPowerOfTwo(pics.Length);
            var fullArray = new Node[nextPower << 1];
            
            for (var i = 0; i < fullArray.Length; i++)
            {
                fullArray[i] = new Node();
            }
            var splitCache = new Dictionary<int, bool>();
            var endTime = DateTime.Now.AddMilliseconds(searchTime);
            var previousMinScore = BIG;
            Node[] winner = null;

            while (DateTime.Now < endTime)
            {
                var splits = GetRandomInt(0, nextPower);
                if (splitCache.ContainsKey(splits))
                {
                    continue;
                }

                splitCache[splits] = true;

                var shuffledNodes = Shuffle(nodes);
                var a = 0f;
                for (int i = 0; i < nextPower - 1; i++)
                {
                    var d = i << 1;
                    var a1 = shuffledNodes[d].AspectRatio;
                    var a2 = shuffledNodes[d + 1].AspectRatio;
                    if (a1 != 0 && a2 != 0)
                    {
                        a = a1 + a2;
                        if ((splits & (1 << i)) != 0)
                        {
                            a = (a1 * a2) / a;
                        }
                    }
                    else
                    {
                        a = a1 != 0 ? a1 : a2;
                    }
                    Debug.Log(fullArray.Length + " : " + (i + nextPower));
                    fullArray[i + nextPower].AspectRatio = a;
                }

                var wider = targetRatio > fullArray[^1].AspectRatio;
                var currentDifference =
                    wider
                        ? -1 + targetRatio / fullArray[^1].AspectRatio
                        : 1 - targetRatio / fullArray[^1].AspectRatio;

                if (currentDifference < previousMinScore)
                {
                    previousMinScore = currentDifference;
                    var minmax = new MinMax { Min = BIG, Max = -BIG };
                    CalculateSizes(fullArray, splits, nextPower, minmax);
                    var score = currentDifference + (1 - minmax.Min / minmax.Max);
                    if (score < previousMinScore)
                    {
                        winner = fullArray.Take(nextPower).Where(n => !n.Dummy).ToArray();
                        if (winner.Length > 0)
                        {
                            var last = winner[^1];
                            last.Width = wider ? height * last.AspectRatio : width;
                            last.Height = wider ? height : width / last.AspectRatio;
                            winner = winner.Select(n => new Node
                            {
                                X = n.X,
                                Y = n.Y,
                                Index = n.Index,
                                Width = n.Width,
                                Height = n.Height
                            }).ToArray();
                        }
                    }
                }
            }

            return winner;
        }

        /*
        private static int NextPowerOfTwo(int v)
        {
            v--;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            v |= v >> 32;
            v++;
            return v;
        }

        private static int GetRandomInt(int min, int max)
        {
            return new Random().Next(min, max);
        }
        
        private static void NewShuffle<T>(T[] array, int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                int j = UnityEngine.Random.Range(start, end);
                (array[i], array[j]) = (array[j], array[i]);
            }
        }
        
        

        private static T[] Shuffle<T>(T[] array)
        {
            int currentIndex = array.Length;
            T temporaryValue;
            int randomIndex;
            var rnd = new Random();

            while (0 != currentIndex)
            {
                randomIndex = (int)Math.Floor(rnd.NextDouble() * currentIndex);
                currentIndex -= 1;
                temporaryValue = array[currentIndex];
                array[currentIndex] = array[randomIndex];
                array[randomIndex] = temporaryValue;
            }

            return array;
        }#1#
    }
}
*/
