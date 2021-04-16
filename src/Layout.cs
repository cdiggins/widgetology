using System;
using System.Collections.Generic;
using System.Linq;

namespace Widgetology
{
    public class LayoutProperties
    {
        // TODO: there could also be a spacing propertie
        // TODO: really these should be rects not just simple floats
        // TODO: eventually we may want units of measure like in CSS/HTML
        public float Padding { get; } = 2;
        public float Margin { get; } = 5;
        public float Border { get; } = 1;

        public LayoutProperties(float padding = default, float margin = default, float border = default)
            => (Padding, Margin, Border) = (padding, margin, border);

        public readonly static LayoutProperties Default = new();

        public LayoutProperties WithPadding(float x) => new(x, Margin, Border);
        public LayoutProperties WithMargin(float x) => new(Padding, x, Border);
        public LayoutProperties WithBorder(float x) => new(Padding, Margin, x);
    }

    /// <summary>
    /// A Layout may contain child layouts, which decide how to position things
    /// and a control, which it renders. 
    /// </summary>
    public class Layout : Control<Layout>, ILayout
    {
        // Member fields 
        public IReadOnlyList<IControl> Controls { get; }
        public Func<Rect, int, int, Rect> Strategy { get; }
        public LayoutProperties Props { get; }
        public Style Style { get; }

        // Constructor 
        public Layout(
            Func<Rect, int, int, Rect> strategy,
            LayoutProperties props = null,
            Style style = null,
            IReadOnlyList<IControl> controls = null) :
            base(OnKeyPress,
                OnMouse,
                OnPaint)
        {
            Props = props ?? LayoutProperties.Default;
            Strategy = strategy;
            Style = style ?? new Style();
            Controls = controls ?? Array.Empty<ILayout>();
        }

        public Rect GetSubRect(Rect r, int i)
            => Strategy(r, i, Controls.Count);

        public IReadOnlyList<Rect> GetLayoutRects(Rect rect)
            => Enumerable.Range(0, Controls.Count).Select(i => GetSubRect(rect, i)).ToArray();

        public new Size MinSize
            =>  Controls.Aggregate(Size.Default, (a, b) => a.Max(b.MinSize));

        public Rect GetInnerRect(Rect r)
            => r.Shrink(Props.Border + Props.Margin);

        // Helper functions to allow smart instantiation and immutability return this if no changes

        public static Layout ApplyChanges(Layout self, IReadOnlyList<IControl> controls)
            => self.Controls.SequenceEqual(controls) ? self : new Layout(self.Strategy, self.Props, self.Style, controls);

        // Input handlers

        public static Layout OnMouse(Layout self, Point point, Rect rect, MouseEvent mouse)
            => ApplyChanges(self, self.Controls.Select((x, i) => x?.Mouse(x, point, self.GetSubRect(rect, i), mouse)).ToArray());
        
        public static Layout OnKeyPress(Layout self, Point point, Rect rect, char key)
            => ApplyChanges(self, self.Controls.Select((x, i) => x?.KeyPress(x, point, self.GetSubRect(rect, i), key)).ToArray());

        public static ICanvas OnPaint(Layout self, ICanvas canvas, Rect rect)
        {
            if (self.Style.BackgroundFilled)
                canvas.FillRect(rect, self.Style.BackgroundColor);
            if (self.Props.Border > 0)
                canvas = canvas.DrawRect(rect, self.Style.BorderColor, self.Props.Border);
            for (var i = 0; i < self.Controls.Count; ++i)
                canvas = self.Controls[i].Paint(canvas, self.GetSubRect(rect, i));
            return canvas;
        }
    }

    public static class CommonLayouts
    {
        public static Layout CreateHorizontal(LayoutProperties props, params IControl[] children)
            => new(LayoutHelpers.SplitHorizontally, props, null, children);

        public static Layout CreateVertical(LayoutProperties props, params IControl[] children)
            => new(LayoutHelpers.SplitVertically, props, null, children);
    }
}
