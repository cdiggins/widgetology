using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Widgetology
{
    public class MyUserInterface
    {
        public static readonly Style ButtonStyle = Style.Default;
        public static readonly Style ButtonPressedStyle = ButtonStyle.WithBackgroundColor(Color.Green);
        public static readonly Style TextEditStyle = Style.Default;
        public static readonly LayoutProperties LayoutProperties = LayoutProperties.Default;
        public static readonly Style SliderStyle = Style.Default;
        public static readonly Style SliderDraggingStyle = SliderStyle.WithPrimaryColor(Color.Red);

        public static Button Button(string s = "Button")
            => new(false, s, ButtonStyle, ButtonPressedStyle);

        public static TextEdit TextEdit(string s = "Button")
            => new(TextEditStyle, "Enter text here: ");

        public static Slider Slider()
            => new(SliderStyle, SliderDraggingStyle, 0, false);

        public static ILayout Horizontal(params IControl[] controls)
            => CommonLayouts.CreateHorizontal(LayoutProperties, controls);

        public static ILayout Vertical(params IControl[] controls)
            => CommonLayouts.CreateVertical(LayoutProperties, controls);

        public static ILayout Create()
            =>
            Vertical(
                Horizontal(Button(), TextEdit(), Button()),
                Horizontal(
                    Vertical(TextEdit(), Slider(), Button(), Button()),
                    Vertical(Button()),
                    Vertical(TextEdit(), Slider(), Button(), Button())),
                Horizontal(Button()));        
    }
}
