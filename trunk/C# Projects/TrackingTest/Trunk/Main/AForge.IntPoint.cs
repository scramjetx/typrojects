using System;

namespace AForge
{
    public struct IntPoint
    {
        public int X;
        public int Y;

        public IntPoint(int x, int y);

        public static IntPoint operator -(IntPoint p, int valueToSubtract);
        public static IntPoint operator -(IntPoint p1, IntPoint p2);
        public static IntPoint operator *(IntPoint p, int factor);
        public static IntPoint operator /(IntPoint p, int factor);
        public static IntPoint operator +(IntPoint p, int valueToAdd);
        public static IntPoint operator +(IntPoint p1, IntPoint p2);
        public static implicit operator DoublePoint(IntPoint p);

        public double DistanceTo(IntPoint anotherPoint);
        public double EuclideanNorm();
        public override string ToString();
    }
}
