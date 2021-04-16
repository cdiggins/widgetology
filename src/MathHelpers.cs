using System;

namespace Widgetology
{
    public class Point
    {
        public Point(float x, float y) => (X, Y) = (x, y);
        public float X { get; }
        public float Y { get; }
        public static readonly Point Default = new(0, 0);
    }

    public class Size
    {
        public Size(float w, float h) => (Width, Height) = (w, h);
        public float Width { get; }
        public float Height { get; }
        public static readonly Size Default = new(0, 0);
    }

    public class Rect
    {
        public Rect(Point pt, Size sz) => (TopLeft, Size) = (pt, sz);
        public Point TopLeft { get; }
        public Size Size { get; }
        public static readonly Rect Default = new(Point.Default, Size.Default);
    }

    public static class MathHelpers
    {
        public static Rect Shrink(this Rect r, float x)
            => new(r.TopLeft.Add(x), r.Size.Sub(x));

        public static Rect Grow(this Rect r, float x)
            => new(r.TopLeft.Sub(x), r.Size.Add(x));

        public static Point Add(this Point p, float x)
            => new(p.X + x, p.Y + x);

        public static Point Add(this Point self, Point p)
            => new(self.X + p.X, self.Y + p.Y);

        public static Point Sub(this Point p, float x)
            => new(p.X - x, p.Y - x);

        public static Point Sub(this Point self, Point p)
            => new(self.X - p.X, self.Y - p.Y);

        public static Size Add(this Size p, float x)
            => new(p.Width + x, p.Height + x);

        public static Size Sub(this Size p, float x)
            => new(p.Width - x, p.Height - x);

        public static Point BottomRight(this Rect r)
            => r.TopLeft.Add(r.Size);

        public static float Top(this Rect r)
            => r.TopLeft.Y;

        public static float Left(this Rect r)
            => r.TopLeft.X;

        public static Point Center(this Rect r)
            => new(r.MidX(), r.MidY());

        public static float MidX(this Rect r)
            => r.Left() + r.Width() / 2f;
        
        public static float MidY(this Rect r)
            => r.Top() + r.Height() / 2f;

        public static float Height(this Rect r)
            => r.Size.Height;

        public static float Width(this Rect r)
            => r.Size.Width;

        public static float Right(this Rect r)
            => r.Left() + r.Width();

        public static float Bottom(this Rect r)
            => r.Top() + r.Height();

        public static Point TopRight(this Rect r)
            => new(r.Right(), r.Top());

        public static Point BottomLeft(this Rect r)
            => new(r.Left(), r.Bottom());

        public static Rect ToRect(this Point a, Point b)
            => a.ToRect(b.Sub(a).ToSize());

        public static Rect ToRect(this Point self, Size sz)
            => new(self, sz);

        public static Point Add(this Point p, Size sz)
            => p.Add(sz.ToPoint());

        static void Test()
        {
            var p = new Point(1, 2);
            p.ToSize();
        }

        public static Size ToSize(this Point p)
            => new(p.X, p.Y);

        public static Point ToPoint(this Size sz)
            => new(sz.Width, sz.Height);

        public static Size ComputeSize(this Point a, Point b)
            => b.Sub(a).ToSize();

        public static Point Max(this Point a, Point b)
            => new(Math.Max(a.X, b.X), Math.Max(a.X, b.X));

        public static Point Min(this Point a, Point b)
            => new(Math.Min(a.X, b.X), Math.Min(a.X, b.X));

        public static Size Min(this Size a, Size b)
            => a.ToPoint().Min(b.ToPoint()).ToSize();

        public static Size Max(this Size a, Size b)
            => a.ToPoint().Max(b.ToPoint()).ToSize();

        public static Rect ToRect(this Size size)
            => Point.Default.ToRect(size);

        public static Rect Merge(this Rect r1, Rect r2)
            => r1.TopLeft.Min(r2.TopLeft).ToRect(r1.BottomRight().Max(r2.BottomRight()));

        public static bool Contains(this Rect r, Point p)
            => p.X >= r.Left() && p.X < r.Right() && p.Y >= r.Top() && p.Y < r.Bottom();
    }
}
