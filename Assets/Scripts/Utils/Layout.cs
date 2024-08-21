using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = System.Random;

namespace Utils
{
    
    public class Node
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Index { get; set; }
        public float AspectRatio { get; set; }
        public bool Dummy { get; set; } 
        
        public Texture2D Texture { get; set; }
        
        public void Trace()
        {
            Debug.Log($"Node Details: X={X}, Y={Y}, Width={Width}, Height={Height}, Index={Index}, AspectRatio={AspectRatio}, Dummy={Dummy}");
        }
    }

    public class MinMax
    {
        public float Min { get; set; }
        public float Max { get; set; }
    }



    public static class Layout
    {
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



        public static List<T> Shuffle<T>(List<T> array)
        {
            Random rnd = new Random();
            var currentIndex = array.Count;
            while (currentIndex != 0)
            {
                var randomIndex = rnd.Next(currentIndex);
                currentIndex--;
                (array[currentIndex], array[randomIndex]) = (array[randomIndex], array[currentIndex]);
            }

            return array;
        }

        private  static void ArrayReplaceIn<T>(List<T> src, List<T> target)
        {
            for (var i = 0; i < src.Count; i++)
            {
                target[i] = src[i];
            }
        }

        private static void ShuffleSlice<T>(List<T> arr, int start, int end)
        {
            var shuffled = Shuffle(arr.GetRange(start, end - start));
            ArrayReplaceIn(shuffled, arr);
        }

        private static void CalculateSizes(List<Node> nodes, int splits, int nextPower, MinMax minmax)
        {
            for (var i = nextPower - 2; i >= 0; i--)
            {
                var d = i << 1;
                Node n1 = nodes[d];
                Node n2 = nodes[d + 1];
                Node parent = nodes[nextPower + i];
                n1.X = parent.X;
                n1.Y = parent.Y;
                if ((splits & (1 << i)) != 0)
                {
                    n1.Width = parent.Width;
                    n1.Height = n1.AspectRatio > 0 ? (float)(parent.Width / n1.AspectRatio) : 0;
                    n2.X = parent.X;
                    n2.Y = parent.Y + n1.Height;
                    n2.Width = parent.Width;
                    n2.Height = parent.Height - n1.Height;
                }
                else
                {
                    n1.Width = (float)(parent.Height * n1.AspectRatio);
                    n1.Height = parent.Height;
                    n2.X = parent.X + n1.Width;
                    n2.Y = parent.Y;
                    n2.Width = parent.Width - n1.Width;
                    n2.Height = parent.Height;
                }

                if (d < nextPower)
                {
                    var d1 = n1.Width + n1.Height;
                    var d2 = n2.Width + n2.Height;
                    Debug.Log(d1 + " " + d2);
                    if (!n1.Dummy)
                    {
                        minmax.Min = Math.Min(minmax.Min, d1);
                        Debug.Log("Min: " + minmax.Min);
                    }

                    if (!n2.Dummy)
                    {
                        minmax.Min = Math.Min(minmax.Min, d2);
                        Debug.Log("Min: " + minmax.Min);
                    }

                    minmax.Max = Math.Max(minmax.Max, Math.Max(d1, d2));
                }
            }
        }

        private static List<Node> DummyArray(int len)
        {
            return Enumerable
                .Repeat(new Node { X = 0, Y = 0, Width = 0, Height = 0, AspectRatio = 0, Dummy = true }, len).ToList();
        }

        private static void TraceArray(List<Node> arr)
        {
            var stringBuilder = new StringBuilder();
            foreach (var node in arr)
            {
                stringBuilder.Append(node.AspectRatio + " ");
            }
            Debug.Log(stringBuilder.ToString().Trim());
            
        }


        public static List<Node> SearchSolution(int width, int height, List<Texture2D> textures, int searchTime)
        {
            const int BIG = 999999999;

            double targetRatio = (double)width / height;
            // Debug.Log("Target ratio: " + targetRatio);
            int nextPower = NextPowerOfTwo(textures.Count);
            Debug.Log("Next power of two: " + nextPower);
            List<Node> fullArray = DummyArray((nextPower << 1) - 1);
            // Debug.Log(fullArray.Count);
            // fullArray[0].Trace();
            Dictionary<int, bool> splitCache = new Dictionary<int, bool>();
            DateTime endTime = DateTime.Now.AddMilliseconds(searchTime);
            double previousMinScore = BIG;
            int splits;
            double currentDifference;
            List<Node> winner = null;
            int arrayHalf;
            bool wider;
            MinMax minmax;
            float a1, a2, a;
            int i, l, d;
            double minAspectRatioThreshold = 0.05;

            a = 0;

            var nodes = textures.Select((r, idx) => new Node
            {
                X = 0,
                Y = 0,
                Width = 0,
                Height = 0,
                Index = idx,
                AspectRatio = (float) r.width / r.height,
                Dummy = false
            }).ToList();

            /*foreach (var node in nodes)
            {
                Debug.Log(node.AspectRatio);
            }*/
            

            if (nextPower <= 5)
            {
                minAspectRatioThreshold = BIG;
            }

            const bool allowCropping = true;

            //TraceArray(nodes);
            //TraceArray(fullArray);
            ArrayReplaceIn(nodes, fullArray);
            //TraceArray(fullArray);
            
            // Calculates 2 ^ nextPower, ex 65536 for 10 items (2^16)
            arrayHalf = 1 << nextPower;

            while (DateTime.Now < endTime)
            {
                splits = GetRandomInt(0, arrayHalf);
                if (!splitCache.TryAdd(splits, true))
                {
                    continue;
                }
                // TraceArray(fullArray);
                ShuffleSlice(fullArray, 0, nextPower);
                // TraceArray(fullArray);


                for (i = 0, l = nextPower - 1; i < l; i++)
                {
                    d = i << 1;
                    a1 = fullArray[d].AspectRatio;
                    a2 = fullArray[d + 1].AspectRatio;

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

                    fullArray[i + nextPower].AspectRatio = a;
                }

                wider = targetRatio > a;
                currentDifference = wider ? -1 + targetRatio / a : 1 - targetRatio / a;
                
                
                if (currentDifference < minAspectRatioThreshold)
                {
                    Debug.Log("Wider? (ratio vs a) " + wider + ": " + targetRatio + ">" + a);
                    Debug.Log("currentDiff < min: " + currentDifference + " : " + minAspectRatioThreshold);
                    
                    Node lastNode = fullArray[^1];
                    Debug.Log("BEFORE: " + lastNode.Width + " : " + lastNode.Height);

                    lastNode.Width = wider ? (float)(height * a) : width;
                    lastNode.Height = wider ? height : (float)(width / a);
                    
                    Debug.Log("AFTER: " + lastNode.Width + " : " + lastNode.Height);

                    minmax = new MinMax { Min = BIG, Max = -BIG };
                    Debug.Log(minmax.Min + " : " + minmax.Max);

                    CalculateSizes(fullArray, splits, nextPower, minmax);
                    Debug.Log(minmax.Min + " : " + minmax.Max);
                    var score = currentDifference + (1 - minmax.Min / minmax.Max);
                    // Debug.Log("SCORE < PREVIOUS: " + score + " : " + previousMinScore);
                    // Debug.Log(minmax.Min + " : " + minmax.Max);
                    // Debug.Log(score);

                    if (double.IsNaN(score) || score < previousMinScore)
                    {
                        Debug.Log( "SCORE < PREVIOUS: " + score + " : " + previousMinScore);
                        previousMinScore = score;
                        List<Node> arr = fullArray.Take(nextPower).Where(n => !n.Dummy).ToList();
                        // Debug.Log("Array Length: " + arr.Count);
                        winner = arr.Select(n => new Node
                        {
                            X = n.X,
                            Y = n.Y,
                            Index = n.Index,
                            Width = n.Width,
                            Height = n.Height
                        }).ToList();
                        // Debug.Log(winner.Count);

                        /*if (allowCropping)
                        {
                            
                            double scaleX = (double)width / lastNode.Width;
                            double scaleY = (double)height / lastNode.Height;
                            winner = winner.Select(n => new Node
                            {
                                X = (float)(n.X * scaleX),
                                Y = (float)(n.Y * scaleY),
                                Index = n.Index,
                                Width = (float)(n.Width * scaleX),
                                Height = (float)(n.Height * scaleY)
                            }).ToList();
                            Debug.Log("Allow Cropping. winner Set");
                            Debug.Log(n.Trace());

                        }*/
                    }
                }
            }

            return winner;
        }

    }
}
