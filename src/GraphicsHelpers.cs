using System.Drawing;

namespace Widgetology
{
    public class Canvas : ICanvas
    {
        public Graphics2D Graphics { get; }
        public Canvas(Graphics2D g)
            => Graphics = g;
    }

    public static class GraphicsHelpers
    {
        public static ICanvas DrawCenteredText(this ICanvas canvas, Color color, Rect r, string s)
        {
            var g2 = canvas.Graphics;
            var width = g2.getStringWidth(s);
            var height = g2.getStringHeight(s);
            g2.setColor(color);
            g2.drawString(s, (int)r.MidX() - width / 2, (int)r.MidY() - height / 2);
            return canvas;
        }

        public static ICanvas FillRect(this ICanvas canvas, Rect r, Color color)
        {
            var g2 = canvas.Graphics;
            g2.setColor(color);
            g2.fillRect((int)r.Left(), (int)r.Top(), (int)r.Width(), (int)r.Height());
            return canvas;
        }

        public static ICanvas DrawRect(this ICanvas canvas, Rect r, Color color, float width)
        {
            var g2 = canvas.Graphics;
            g2.setPenWidth(width);
            g2.setColor(color);
            g2.fillRect((int)r.Left(), (int)r.Top(), (int)r.Width(), (int)r.Height());
            return canvas;
        }
    }
}
