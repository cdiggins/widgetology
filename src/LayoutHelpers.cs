namespace Widgetology
{
    public static class LayoutHelpers
    {
        public static Size SplitHorizontallySize(this Rect r, int count)
            => new(r.Width() / count, r.Height());

        public static Size SplitVerticallySize(this Rect r, int count)
            => new(r.Width(), r.Height() / count);

        public static Point SplitHorizontallyTopLeft(this Rect r, int n, int count)
            => new(r.Left() + r.Width() / count * n, r.Top());

        public static Point SplitVerticallyTopLeft(this Rect r, int n, int count)
            => new(r.Left(), r.Top() + r.Height() / count * n);

        public static Rect SplitHorizontally(this Rect r, int i, int count)
            => new(SplitHorizontallyTopLeft(r, i, count), SplitHorizontallySize(r, count));

        public static Rect SplitVertically(this Rect r, int i, int count)
            => new(SplitVerticallyTopLeft(r, i, count), SplitVerticallySize(r, count));
    }
}
