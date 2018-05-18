using System;
using ZedGraph;

namespace AiCableForce.graphic
{
    public class Range : IComparable
    {
        public int Left;
        public int Right;
        public LineItem Line;
        public LineItem Line_low;

        public Range()
        {
        }

        public Range(int left, int right)
        {
            Left = left;
            Right = right;
        }

        public bool Contains(int pt)
        {
            return pt >= Left && pt <= Right;
        }

        public bool isEmpty()
        {
            return Left == Right;
        }

        public void Sort()
        {
            if (Left > Right)
            {
                var x = Left;
                Left = Right;
                Right = x;
            }
        }

        public void Clear()
        {
            if (Line != null) Line.Clear();
            if (Line_low != null) Line_low.Clear();
        }

        public int CompareTo(object obj)
        {
            var range = obj as Range;
            if (range != null) return Left.CompareTo(range.Left);
            return -1;
        }
    }
}
