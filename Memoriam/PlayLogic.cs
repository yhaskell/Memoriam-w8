using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Memoriam
{
    public struct Circle
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Radius { get; set; }

        static Random seed = new Random();

        public static Circle[] GeneratePoints(int Count, int w, int h, double rad = 0)
        {

            var points = new Point[Count];
            var radiuses = new double[Count];

            for (int i = 0; i < Count; i++)
                radiuses[i] = rad != 0 ? rad : seed.NextDouble(250.0 / Math.Pow(Count, 0.5), 250.0 / Math.Pow(Count, 0.25));

            for (int i = 0; i < Count; i++)
                points[i] = new Point(seed.NextDouble(radiuses[i], w - radiuses[i]), seed.NextDouble(radiuses[i], h - radiuses[i]));

            int iter = 0;
            while (points.Intersections(radiuses) != 0)
            {
                if (iter++ > 100000) return GeneratePoints(Count, w, h, 100);
                for (int i = 0; i < points.Length; i++)
                    for (int j = i + 1; j < points.Length; j++)
                    {
                        var d = points[i].DistanceTo(points[j]);
                        if (d > radiuses[i] + radiuses[j] + 30) continue;

                        points[i] = points[i].Move(points[i].To(points[j]), -5 / (d * d));
                        points[j] = points[j].Move(points[j].To(points[i]), -5 / (d * d));

                        if (points[i].X < radiuses[i]) points[i].X = radiuses[i];
                        if (points[i].Y < radiuses[i]) points[i].Y = radiuses[i];
                        if (points[i].X > w - radiuses[i]) points[i].X = w - radiuses[i];
                        if (points[i].Y > h - radiuses[i]) points[i].Y = h - radiuses[i];

                        if (points[j].X < radiuses[j]) points[j].X = radiuses[j];
                        if (points[j].Y < radiuses[j]) points[j].Y = radiuses[j];
                        if (points[j].X > w - radiuses[j]) points[j].X = w - radiuses[j];
                        if (points[j].Y > h - radiuses[j]) points[j].Y = h - radiuses[j];

                    }
            }

            var crc = new Circle[Count];
            for (int i = 0; i < Count; i++)
            {
                crc[i].X = points[i].X;
                crc[i].Y = points[i].Y;
                crc[i].Radius = radiuses[i];
            }
            return crc;
        }



    }

    public static class PointExtensions
    {
        public static int Intersections(this Point[] points, double[] radiuses)
        {
            int res = 0;
            for (int i = 0; i < radiuses.Length; i++)
                for (int j = i + 1; j < radiuses.Length; j++)
                {
                    if (points[i].DistanceTo(points[j]) < radiuses[i] + radiuses[j]) res++;
                }

            return res;
        }

        public static double DistanceTo(this Point curr, Point p)
        {
            return Math.Sqrt((curr.X - p.X) * (curr.X - p.X) + (curr.Y - p.Y) * (curr.Y - p.Y));
        }

        public static double NextDouble(this Random r, double from, double to)
        {
            return from + r.NextDouble() * (to - from);
        }

        public static double Radius(this IEnumerable<Point> center)
        {
            double md = double.MaxValue;
            double cd = 0;
            foreach (var c in center) foreach (var d in center) if (c != d && (cd = c.DistanceTo(d)) < md) md = cd;
            return md;
        }

        public static Point Center(Point p1, Point p2, double w1, double w2)
        {
            return new Point((p1.X * w1 + p2.X * w2) / (w1 + w2), (p1.Y * w1 + p2.Y * w2) / (w1 + w2));
        }

        public static Point Center(this Point[] center)
        {
            Point res = center[0];
            for (int i = 1; i < center.Length; i++) res = Center(res, center[i], i, 1);
            return res;
        }

        public static Point To(this Point from, Point to) { return new Point(to.X - from.X, to.Y - from.Y); }
        public static Point Move(this Point curr, Point vec, double w = 1) { return new Point(curr.X + w * vec.X, curr.Y + w * vec.Y); }

    }

    public enum PlayStyle { GeneralRun = 0, TimedRun = 1 };
}