using System.Drawing;

namespace Widgetology
{
    public class EmptyControl : Control<EmptyControl>
    {
        public EmptyControl()
            : base(default)
        { }

        public static readonly EmptyControl Default = new EmptyControl();
    }

    public class Button : Control<Button>
    {
        public bool Pressed { get; }
        public string Label { get; }
        public Style Style => Pressed ? PressedStyle : DefaultStyle;
        public Style DefaultStyle { get; }
        public Style PressedStyle { get; }

        public Button(bool pressed, string label, Style defaultStyle, Style pressedStyle)
            : base(null, OnMouse, OnPaint)
            => (Pressed, Label, DefaultStyle, PressedStyle) = (pressed, label, defaultStyle, pressedStyle);

        public static readonly Button Default
            = new(false, "Button", Style.Default, Style.Default.WithBackgroundColor(Color.Red));

        public static ICanvas OnPaint(Button self, ICanvas canvas, Rect rect)
        {
            if (self.Style.BackgroundFilled)
                canvas = canvas.FillRect(rect, self.Style.BackgroundColor);
            return canvas.DrawCenteredText(self.Style.PrimaryColor, rect, self.Label);
        }

        public static Button OnMouse(Button self, Point point, Rect rect, MouseEvent mouse)
            => mouse switch
            {
                MouseEvent.Down => new(true, self.Label, self.DefaultStyle, self.PressedStyle),
                MouseEvent.Up   => new(false, self.Label, self.DefaultStyle, self.PressedStyle),
                _ => self,
            };
    }

    public class TextEdit : Control<TextEdit>
    {
        public Style Style { get; }
        public string Text { get; }

        public TextEdit(Style style, string text)
            : base(OnKeyPress, null, OnPaint)
            => (Style, Text) = (style, text);

        public static readonly EmptyControl Default = new();

        public static TextEdit OnKeyPress(TextEdit self, Point point, Rect rect, char key)
            => new(self.Style, self.Text + key);

        public static ICanvas OnPaint(TextEdit self, ICanvas canvas, Rect rect)
        {
            if (self.Style.BackgroundFilled)
                canvas = canvas.FillRect(rect, self.Style.BackgroundColor);
            return canvas.DrawCenteredText(self.Style.PrimaryColor, rect, self.Text);
        }
    }

    public class Slider : Control<Slider>
    {
        public float Position { get; }
        public bool Dragging { get; }
        public Style Style => Dragging ? DraggingStyle : DefaultStyle;
        public Style DefaultStyle { get; }
        public Style DraggingStyle { get; }

        public Slider(Style defaultStyle, Style draggingStyle, float pos, bool dragging)
            : base(null, OnMouse, OnPaint)
            => (Position, DefaultStyle, DraggingStyle, Dragging) = (pos, defaultStyle, draggingStyle, dragging);

        public static Slider OnMouse(Slider self, Point pt, Rect rect, MouseEvent mouse) => mouse switch
        {
            MouseEvent.Drag => new(self.DefaultStyle, self.DraggingStyle, (pt.X - rect.Left()) / rect.Width(), true),
            MouseEvent.Up => new(self.DefaultStyle, self.DraggingStyle, self.Position, false),
            _ => self,
        };

        public static ICanvas OnPaint(Slider self, ICanvas canvas, Rect rect)
        {
            if (self.Style.BackgroundFilled)
                canvas = canvas.FillRect(rect, self.Style.BackgroundColor);

            var g2 = canvas.Graphics;
            
            //draw the background
            var top = (int)rect.Top();
            var left = (int)rect.Left();
            var width = (int)rect.Width();
            var height = (int)rect.Height();

            g2.fillRect(top, left, width, height);

            //draw the line
            g2.setColor(self.Style.SecondaryColor);
            var midY = top + height / 2;
            g2.drawLine(left, midY, left + width, midY);

            //draw the draggable circle 
            g2.setColor(self.Style.PrimaryColor);
            var xPos = (int)(left + width * self.Position);
            g2.fillOval(xPos - 10, midY - 10, 20, 20);

            return canvas;
        }        
    }
}
