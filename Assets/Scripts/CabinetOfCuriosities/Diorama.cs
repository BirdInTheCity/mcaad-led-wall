using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;


namespace CabinetOfCuriosities
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

        public Curiosity Curiosity { get; set; }

        public void Trace()
        {
            Debug.Log(
                $"Node Details: X={X}, Y={Y}, Width={Width}, Height={Height}, Index={Index}, AspectRatio={AspectRatio}, Dummy={Dummy}");
        }
    }

    public class MinMax
    {
        public float Min { get; set; }
        public float Max { get; set; }
    }

    public class Diorama : MonoBehaviour
    {
        private const int BIG = 999999999;


        private static int NextPowerOfTwo(int v)
        {
            v--;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            v++;
            return v;
        }
        
        private static void TraceArray(Node[] arr)
        {
            var stringBuilder = new StringBuilder();
            foreach (var node in arr)
            {
                stringBuilder.Append(node.AspectRatio + " ");
            }
            Debug.Log(stringBuilder.ToString().Trim());
            
        }

        private static Node[] DummyArray(int length)
        {
            var array = new Node[length];
            for (var i = 0; i < length; i++)
            {
                array[i] = new Node { X = 0, Y = 0, Width = 0, Height = 0, AspectRatio = 0, Dummy = true };
            }

            return array;
        }

        private static T[] Shuffle<T>(T[] array)
        {
            var random = new System.Random();
            for (var i = array.Length - 1; i > 0; i--)
            {
                var j = random.Next(i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }

            return array;
        }

        private static int GetRandomInt(int min, int max)
        {
            var random = new System.Random();
            return random.Next(min, max);
        }
        
        private static void ArrayReplaceIn<T>(T[] src, T[] target)
        {
            for (int i = 0; i < src.Length; i++)
            {
                target[i] = src[i];
            }
        }
        
        private static T[] GetRange<T>(T[] array, int index, int count)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (index < 0 || count < 0 || index + count > array.Length)
                throw new ArgumentOutOfRangeException();

            T[] result = new T[count];
            Array.Copy(array, index, result, 0, count);
            return result;
        }
        
        private static void ShuffleSlice<T>(T[] arr, int start, int end)
        {
            var shuffled = Shuffle(GetRange(arr, start, end - start));
            ArrayReplaceIn(shuffled, arr);
        }


        public static Node[] SearchSolution(float width, float height, Curiosity[] pics, int searchTime)
        {
            var targetRatio = (float)width / height;
            var nextPower = NextPowerOfTwo(pics.Length);
            var fullArray = DummyArray((nextPower << 1) - 1);
            Dictionary<int, bool> splitCache = new Dictionary<int, bool>();
            var endTime = DateTime.Now.AddMilliseconds(searchTime);
            float previousMinScore = BIG;
            var splits = 0;
            float currentDifference = 0;
            Node[] winner = null;
            var arrayHalf = 0;
            var wider = false;
            var minmax = new MinMax();
            Node last;

            float a1, a2, a = 0f;
            int d;
            float minAspectRatioThreshold = 0.05f;
            
            var nodes = new Node[pics.Length];
            for (var i = 0; i < pics.Length; i++)
            {
                nodes[i] = new Node
                    { 
                        X = 0, 
                        Y = 0, 
                        Width = 0, 
                        Height = 0, 
                        AspectRatio = (float) pics[i].AspectRatio, 
                        Dummy = false,
                        Curiosity = pics[i]
                    };
            }
            
            if (nextPower <= 5)
            {
                minAspectRatioThreshold = BIG;
            }
            var allowCropping = true;
            
            ArrayReplaceIn(nodes, fullArray);
            arrayHalf = 1 << nextPower;
            
            

            while (DateTime.Now < endTime)
            {
                splits = GetRandomInt(0, arrayHalf);
                if (!splitCache.TryAdd(splits, true))
                {
                    continue;
                }

                ShuffleSlice(fullArray, 0, nextPower);
                
                for (int i = 0, l = nextPower - 1; i < l; i++)
                {
                    d = i << 1;
                    a1 = fullArray[d].AspectRatio;
                    a2 = fullArray[d + 1].AspectRatio;
                    
                    if (a1 != 0 && a2 != 0)
                    {
                        a = a1 + a2;
                        if ((splits & (1 << i)) != 0) {
                            a = (a1 * a2) / a;
                        }
                    } else {
                        a = a1 != 0 ? a1 : a2;
                    }

                    fullArray[i + nextPower].AspectRatio = a;
                }

                wider = targetRatio > a;

                currentDifference = wider ? -1 + targetRatio / a : 1 - targetRatio / a;
                
                // Debug.Log(currentDifference + " " + targetRatio + " " + a);
                if (currentDifference < minAspectRatioThreshold)
                {
                    last = fullArray[^1];
                    last.Width = wider ? height * a : width;
                    last.Height = wider ? height : width / a;
                    minmax = new MinMax
                    {
                        Min = BIG,
                        Max = -BIG
                    };
                    CalculateSizes(fullArray, splits, nextPower, ref minmax);
                    var score = currentDifference + (1 - minmax.Min / minmax.Max);
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
                            Height = n.Height,
                            Curiosity = n.Curiosity
                        }).ToArray();
                        
                        if (allowCropping)
                        {
                            double scaleX = (double)width / last.Width;
                            double scaleY = (double)height / last.Height;
                            winner = winner.Select(n => new Node
                            {
                                X = (float)(n.X * scaleX),
                                Y = (float)(n.Y * scaleY),
                                Index = n.Index,
                                Width = (float)(n.Width * scaleX),
                                Height = (float)(n.Height * scaleY),
                                Curiosity = n.Curiosity
                            }).ToArray();
                        }
                    }
                }
            }


            return winner;
        }

        private static void CalculateSizes(Node[] nodes, int splits, int nextPower, ref MinMax minmax)
        {
            int n1, n2, parent;
            float d1, d2;

            for (int i = nextPower - 2; i >= 0; i--)
            {
                n1 = i << 1;
                n2 = n1 + 1;
                parent = nextPower + i;

                nodes[n1].X = nodes[parent].X;
                nodes[n1].Y = nodes[parent].Y;

                if ((splits & (1 << i)) != 0)
                {
                    nodes[n1].Width = nodes[parent].Width;
                    nodes[n1].Height = nodes[n1].AspectRatio != 0 ? nodes[parent].Width / nodes[n1].AspectRatio : 0;
                    nodes[n2].X = nodes[parent].X;
                    nodes[n2].Y = nodes[parent].Y + nodes[n1].Height;
                    nodes[n2].Width = nodes[parent].Width;
                    nodes[n2].Height = nodes[parent].Height - nodes[n1].Height;
                }
                else
                {
                    nodes[n1].Width = nodes[parent].Height * nodes[n1].AspectRatio;
                    nodes[n1].Height = nodes[parent].Height;
                    nodes[n2].X = nodes[parent].X + nodes[n1].Width;
                    nodes[n2].Y = nodes[parent].Y;
                    nodes[n2].Width = nodes[parent].Width - nodes[n1].Width;
                    nodes[n2].Height = nodes[parent].Height;
                }

                if (n1 < nextPower)
                {
                    d1 = nodes[n1].Width + nodes[n1].Height;
                    d2 = nodes[n2].Width + nodes[n2].Height;

                    if (!nodes[n1].Dummy)
                    {
                        minmax.Min = Math.Min(minmax.Min, d1);
                    }

                    if (!nodes[n2].Dummy)
                    {
                        minmax.Min = Math.Min(minmax.Min, d2);
                    }

                    minmax.Max = Math.Max(minmax.Max, Math.Max(d1, d2));
                }
            }
        }
    }
}