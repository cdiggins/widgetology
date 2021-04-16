using System;

namespace Widgetology
{
    // Strategy pattern on steroids. Using lambdas (which are effectively interfaces with one function).
    // This exposes an explicit implementation of an abstract class without the complications. 
    // The virtual functions are made explicit as strongly typed parameters to the constructor. 
    // Makes reasoning about the thing easier, and it can be created without declaring a class!
    // Shows that functional programming gives you polymorphism with better controls.     
    public class Control<TControl> : IControl
        where TControl : class, IControl
    {
        public Size MinSize => Size.Default;

        public Func<TControl, Point, Rect, char, TControl> _onKeyPress { get; }
        public Func<TControl, Point, Rect, MouseEvent, TControl> _onMouse { get; }
        public Func<TControl, ICanvas, Rect, ICanvas> _onPaint { get; }

        public IControl KeyPress(IControl self, Point point, Rect rect, char key)
            => rect.Contains(point) ? _onKeyPress?.Invoke(self as TControl, point, rect, key) ?? self : self;

        public IControl Mouse(IControl self, Point point, Rect rect, MouseEvent mouse)
            => rect.Contains(point) ? _onMouse?.Invoke(self as TControl, point, rect, mouse) ?? self : self;

        public ICanvas Paint(ICanvas c, Rect rect)
            => _onPaint?.Invoke(this as TControl, c, rect) ?? c;

        public Control(Func<TControl, Point, Rect, char, TControl> onKeyPress = null
            , Func<TControl, Point, Rect, MouseEvent, TControl> onMouse = null
            , Func<TControl, ICanvas, Rect, ICanvas> onPaint = null)
            =>
            (_onKeyPress, _onMouse, _onPaint) = (onKeyPress, onMouse, onPaint);

        public bool CaptureInput => false;
    }
}
