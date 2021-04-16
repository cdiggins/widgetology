using System.Drawing;

namespace Widgetology
{
    /// <summary>
    /// An immutable class with a fluent interface for managing the style.
    /// </summary>
    public class Style
    {
        public Style(Color primaryColor, Color secondaryColor, Color borderColor, string fontName, int fontSize, Color fontColor, Color backgroundColor, bool backgroundFilled)
        {
            PrimaryColor = primaryColor;
            SecondaryColor = secondaryColor;
            BorderColor = borderColor;
            FontName = fontName;
            FontSize = fontSize;
            FontColor = fontColor;
            BackgroundColor = backgroundColor;
            BackgroundFilled = backgroundFilled;
        }

        public Style() { }

        public Color PrimaryColor { get; } = Color.Blue;
        public Color SecondaryColor { get; } = Color.Green;
        public Color BorderColor { get; } = Color.Black;
        public string FontName { get; } = "Arial";
        public int FontSize { get; } = 12;
        public Color FontColor { get; } = Color.Black;
        public Color BackgroundColor { get; } = Color.LightGray;
        public bool BackgroundFilled { get; } = false;

        // Example of the fluent interface pattern
        public Style WithPrimaryColor(Color color) 
            => new(color, SecondaryColor, BackgroundColor, FontName, FontSize, FontColor, BackgroundColor, BackgroundFilled);
        
        public Style WithSecondaryColor(Color color) 
            => new(PrimaryColor, color, BackgroundColor, FontName, FontSize, FontColor, BackgroundColor, BackgroundFilled);
        
        public Style WithBorderColor(Color color) 
            => new(PrimaryColor, SecondaryColor, color, FontName, FontSize, FontColor, BackgroundColor, BackgroundFilled);
        
        public Style WithFontName(string name) 
            => new(PrimaryColor, SecondaryColor, BackgroundColor, name, FontSize, FontColor, BackgroundColor, BackgroundFilled);
        
        public Style WithFontSize(int size) 
            => new(PrimaryColor, SecondaryColor, BackgroundColor, FontName, size, FontColor, BackgroundColor, BackgroundFilled);
        
        public Style WithFontColor(Color color) 
            => new(PrimaryColor, SecondaryColor, BackgroundColor, FontName, FontSize, color, BackgroundColor, BackgroundFilled);
        
        public Style WithBackgroundColor(Color color) 
            => new(PrimaryColor, SecondaryColor, BackgroundColor, FontName, FontSize, FontColor, color, true);
        
        public static readonly Style Default = new();
    }

}
