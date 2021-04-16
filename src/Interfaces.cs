using System.Collections.Generic;

namespace Widgetology
{
    public interface ICanvas
    {
        Graphics2D Graphics { get; }
    }

    public enum MouseEvent
    {
        Move, Up, Down, Drag
    }

    // TODO: I need to introduce the concept of "capture"
    // TODO: I need to handle mouse in and mouse out. 
    public interface IInputHandler<T>
    {
        T Mouse(T self, Point point, Rect rect, MouseEvent mouse);
        T KeyPress(T self, Point point, Rect rect, char key);
        bool CaptureInput { get; }
    }

    public interface IPaintable
    {
        ICanvas Paint(ICanvas c, Rect r);
    }

    public interface IUIElement : IPaintable
    {
        Size MinSize { get; }

    }

    public interface IControl : IInputHandler<IControl>, IUIElement
    {
    }

    public interface ILayout : IControl
    {
        LayoutProperties Props { get; }
        Rect GetInnerRect(Rect r);
        IReadOnlyList<Rect> GetLayoutRects(Rect rect);
        IReadOnlyList<IControl> Controls { get; }
    }
}
