using Widgetology;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace OriginalImplementation
{
    /*
        UIElement interface, implemented by layout and button.
        allows layouts to treat elements (buttons, textView, etc.) and sub-layouts as the same type
        this enables functions like 'addUIElement' in 'Layout',
        which can add either a button, or another layout
        both the layouts and the buttons implement this interface, so they can adjust their dimensions and
        draw themselves

        for an interface, this actually has quite a few methods. maybe it should be broken up
     */
    // NOTE: yes I agree this should be broken up. Let's discuss that.  
    // There are arguable several different concerns here:
    // 1. handling input (mouse/keyboard)
    // 2. styling (aka presentation state management)
    // 3. position / dimension 
    // 4. application state management 
    // 5. rendering    
    public interface UIElement
    {
        void drawSelf(Graphics2D g2);
        void setUpperLeftCorner(int xPos, int yPos);
        void setWidth(int width);
        void setHeight(int height);
        // NOTE: this function is not clear from the name
        void setAction(MouseInfo e);
        void recalculateElementBounds(Layout parentLayout);
        void sendKeyPress(char key);
        void setPreferredSize(double percentageX, double percentageY);
        double getPreferredSizeX();
        double getPreferredSizeY();
        // NOTE: does this really care if it is a mouse 
        bool mouseWithin(MouseInfo e);
    }
    
    public class JPanel
    {
        int _width;
        int _height;
        Action _repaint;

        public JPanel(int w, int h, Action repaint)
            => (_width, _height, _repaint) = (w, h, repaint);

        public int getWidth()
            => _width;

        public int getHeight()
            => _height;

        public void repaint()
            => _repaint?.Invoke();
    }

    /*
        Button class. used to represent a button. user can click on the button.
        right now the button has no functionality.
        functionality could easily be added (and will be required in readonly project)
     */

    public class Button : Widget
    {
        public Button(double preferredSizeX = 0, double preferredSizeY = 0)
            : base(preferredSizeX, preferredSizeY)
        {
        }

        public override void drawSelf(Graphics2D g2)
        {
            g2.setColor(dynamicColor);

            // NOTE: we see this type of algorithm in multiple places. It is a bit hard to read, and easy to mess up. 
            g2.fillRect(xPos + horizontalPadding, yPos + verticalPadding,
                    width - horizontalPadding * 2, height - verticalPadding * 2);

            drawCenteredLabel(g2);
        }

        // NOTE: this could be a reusable static function outside of the class. 
        // In general making functions static and reusable is a good practice.
        // They are often easier to get correct, and test, and make the code more flexible  and less brittle. 
        private void drawCenteredLabel(Graphics2D g2)
        {
            string str = "button";
            int stringWidth = g2.getStringWidth(str);

            if (within == false)
                g2.setColor(Color.White);
            else g2.setColor(Color.Black);

            g2.drawString(str, xPos + width / 2 - stringWidth / 2, yPos + height / 2);
        }
    }

    /*
        center pane layout class. used to create a layout with 5 panels:
        1) top panel
        2) left panel
        3) center panel (the largest panel - used to hold main display)
        4) right panel
        5) bottom panel

        these panels are built up from horizontal and vertical layouts
        main panel is a vertical layout, which holds the following
        1) horizontal layout (top panel
        2) horizontal layout - middle panel, which holds:
            a) left panel (Vertical layout)
            b) center panel (vertical layout)
            c) right panel (vertical layout)
        3) bottom panel (horizontal layout

        each panel can contain multiple widgets, but center panel should remain clear (for display)

        // use a builder to construct this

     */

    // NOTE: This is not a reusable class: its name suggest that it is one. 
    // It is one of the 
    public class CenterPaneLayout
    {
        Layout layout;

        public CenterPaneLayout(JPanel panel)
        {
            layout = new VerticalLayout(panel);

            // three main sections: top, middle, bottom
            Layout topSection = new HorizontalLayout(panel);
            topSection.setPreferredSize(100, 15);
            Layout middleSection = new HorizontalLayout(panel);
            middleSection.setPreferredSize(100, 70);

            Layout bottomSection = new HorizontalLayout(panel);
            bottomSection.setPreferredSize(100, 15);

            // three parts of the middle layout
            Layout middleLeft = new VerticalLayout(panel);
            middleLeft.setPreferredSize(15, 100);
            Layout middleMiddle = new HorizontalLayout(panel); // can be horizontal or vertical
            middleMiddle.setPreferredSize(70, 100);
            Layout middleRight = new VerticalLayout(panel);
            middleRight.setPreferredSize(15, 100);

            // set up the buttons/textview/sliders/etc for each layout
            topSection.addUIElement(new Button(34, 100));
            topSection.addUIElement(new TextEdit(33, 100));
            topSection.addUIElement(new Button(33, 100));

            middleLeft.addUIElement(new TextEdit(100, 25));
            middleLeft.addUIElement(new Slider(100, 25));
            middleLeft.addUIElement(new Button(100, 25));
            middleLeft.addUIElement(new Button(100, 25));

            middleMiddle.addUIElement(new Button(100, 100));

            middleRight.addUIElement(new TextEdit(100, 25));
            middleRight.addUIElement(new Slider(100, 25));
            middleRight.addUIElement(new Button(100, 25));
            middleRight.addUIElement(new Button(100, 25));

            middleSection.addUIElement(middleLeft);
            middleSection.addUIElement(middleMiddle);
            middleSection.addUIElement(middleRight);

            bottomSection.addUIElement(new Button(100, 100));

            layout.addUIElement(topSection);
            layout.addUIElement(middleSection);
            layout.addUIElement(bottomSection);
        }

        // NOTE: in C# this would be a property
        public Layout getLayouts()
        {
            return layout;
        }
    }

    /*
        Horizontal, extends Layout. the only thing it changes is how to recalculate the element bounds
        horizontal layout stacks elements (including other layouts) horizontally
     */

    public class HorizontalLayout : Layout
    {
        public HorizontalLayout(JPanel panel)
            : base(panel)
        {
        }

        // this function places elements horizontally within the bounds of the layout
        public override void recalculateElementBounds(Layout parentLayout)
        {
            int currentXOffset = 0;
            for (int i = 0; i < elements.Count; i++)
            {
                int currentElementHeight = (int)((height * (elements[i].getPreferredSizeY())) / 100);
                int currentElementWidth = (int)((width * (elements[i].getPreferredSizeX())) / 100);
                elements[i].setUpperLeftCorner(
                        topLeftX + currentXOffset, topLeftY);
                currentXOffset += currentElementWidth;
                elements[i].setHeight(currentElementHeight);
                elements[i].setWidth(currentElementWidth);
                elements[i].recalculateElementBounds(this);
            }
        }
    }

    /*
        abstract Layout class. extended by both Vertical and Horizontal layouts.
        provides methods for adjusting dimensions of rectangular layout element on a JPanel
        provides methods for adding buttons/sub-layouts and automatically updating size
        updates size automatically based on size of JPanel
        allows the MasterPanel to treat all layouts in a uniform way
        (can have an array of 'Layouts' containing vertical, horizontal, and CenterPane layouts)

        using a combination of horizontal and vertical layouts, pretty much any UI can be composed
     */
    // NOTE: this implements some of the same stuff as Widget, the fact that there is repetition violates the DRY principle.     
    // This suggests that both classes share the same base. I would propose a Layout is a Widget. 
    public abstract class Layout : UIElement
    {
        protected readonly JPanel panel;
        protected readonly List<UIElement> elements;
        // NOTE: some of the same comments apply that applied previously to Widget.        
        // Refactoring code is now much harder because of the violation of DRY. 
        protected int topLeftX, topLeftY;
        protected int width, height;
        protected double preferredSizeX;
        protected double preferredSizeY;

        protected Layout(JPanel panel)
        {
            this.panel = panel;
            elements = new List<UIElement>();
            setUpperLeftCorner(0, 0);
            setWidth(panel.getWidth());
            setHeight(panel.getHeight());
        }

        // NOTE: is this checking or informing?
        public void checkMouseAction(MouseInfo e)
        {
            foreach (var uiElement in elements)
                uiElement.setAction(e);
        }

        // NOTE: is this checking or informing?
        public void checkReadyTextEdits(char key)
        {
            foreach (var uiElement in elements)
                uiElement.sendKeyPress(key);
        }

        public void addUIElement(UIElement element)
        {
            elements.Add(element);
            recalculateElementBounds(this);
            panel.repaint();
        }

        public virtual void recalculateElementBounds(Layout parentLayout) { }

        public void drawSelf(Graphics2D g2)
        {
            foreach (var element in elements)
                element.drawSelf(g2);
        }

        public void setUpperLeftCorner(int xPos, int yPos)
        {
            topLeftX = xPos;
            topLeftY = yPos;
        }

        public void setWidth(int width)
            => this.width = width;

        public void setHeight(int height)
            => this.height = height;

        public int getWidth()
            => width;

        public int getHeight()
            => height;

        public void setAction(MouseInfo e)
        {
            foreach (var element in elements)
                element.setAction(e);
        }

        public void sendKeyPress(char key)
        {
            foreach (var element in elements)
                element.sendKeyPress(key);
        }

        public bool mouseWithin(MouseInfo e)
        {
            return topLeftX <= e.getX() && topLeftX + width > e.getX()
                    && topLeftY <= e.getY() && topLeftY + height < e.getY();
        }

        public double getPreferredSizeX()
            => preferredSizeX;        

        public double getPreferredSizeY()
            => preferredSizeY;

        public void setPreferredSize(double percentX, double percentY)
        {
            preferredSizeX = percentX;
            preferredSizeY = percentY;
            height = (int)((panel.getHeight() * preferredSizeY) / 100);
            width = (int)((panel.getHeight() * preferredSizeX) / 100);
        }
    }

    /*
        MasterPanel is the entire JPanel
        used to listen for mouse and key evens
        also used to set initialize the layout (centerPaneLayout, or some other)
     */
    public class MasterPanel 
    {
        private readonly List<Layout> layouts = new();
        private readonly JPanel panel;

        public MasterPanel(JPanel panel)
        {
            this.panel = panel;
            layouts.Add(new CenterPaneLayout(panel).getLayouts());
        }

        private void checkForAction(MouseInfo e)
        {
            foreach (var layout in layouts)
                layout.checkMouseAction(e);
        }

        private void checkReadyTextEdits(char key)
        {
            foreach (var layout in layouts)
                layout.checkReadyTextEdits(key);
        }

        private void updateLayoutDimensions()
        {
            foreach (var layout in layouts)
            {
                layout.setHeight(panel.getHeight());
                layout.setWidth(panel.getWidth());
                layout.recalculateElementBounds(layout);
            }
        }

        public void paintComponent(Graphics2D g)
        {
            updateLayoutDimensions();
            g.setColor(Color.Black);
            g.fillRect(0, 0, panel.getWidth(), panel.getHeight());
            foreach (var layout in layouts)
                layout.drawSelf(g);
        }

        public void mousePressed(int x, int y)
        {
            checkForAction(new MouseInfo(x, y, MouseActionEventType.ACTION_PRESSED));
        }

        public void mouseReleased(int x, int y)
        {
            checkForAction(new MouseInfo(x, y, MouseActionEventType.ACTION_RELEASED));
        }

        public void mouseDragged(int x, int y)
        {
            checkForAction(new MouseInfo(x, y, MouseActionEventType.ACTION_DRAGGED));
        }

        public void mouseMoved(int x, int y)
        {
            checkForAction(new MouseInfo(x, y, MouseActionEventType.ACTION_MOVED));
        }

        public void keyPressed(char key)
        {
            checkReadyTextEdits(key);
        }
    }

    // enums to identify mouse actions. set in masterPanel based on which callback is triggered
    public enum MouseActionEventType
    {
        ACTION_PRESSED, ACTION_RELEASED, ACTION_MOVED, ACTION_DRAGGED
    }
    /*
    small data class for holding info about a mouse press (action and xy coordinate)
    added this class because for some reason MouseEvent from awt doesn't contain info
    about the type of mouse action

    this class is used to construct objects passed from MasterPanel to Button, to update
    the button's color based on the mouse movement/click

     */

    // NOTE: Why doesn't it contain a MouseEvent? 
    public class MouseInfo
    {
        public readonly MouseActionEventType action;

        // NOTE: should be a point. 
        private readonly int x;
        private readonly int y;

        public MouseInfo(int x, int y, MouseActionEventType actionEventType)
        {
            this.x = x;
            this.y = y;
            this.action = actionEventType;
        }
        public int getX()
            => x;
        public int getY()
            => y;
    }

    /*
        slider class. extends Widget, implements UIElement (like TextEdit and Button).
        as with button, the slider is non-functional, but this can easily be added.
        right now, it just slides.
     */
    public class Slider : Widget
    {
        private Color sliderColor = Color.Orange;
        private int sliderXOffset = 0;

        public Slider() { }

        public Slider(double preferredSizeX, double preferredSizeY)
            : base(preferredSizeX, preferredSizeY) { }

        public override void drawSelf(Graphics2D g2)
        {
            g2.setColor(dynamicColor);

            //draw the background
            g2.fillRect(xPos + horizontalPadding, yPos + verticalPadding,
                    width - horizontalPadding * 2, height - verticalPadding * 2);

            //draw the line
            g2.setColor(Color.White);
            g2.drawLine(xPos + horizontalPadding, yPos + height / 2,
                    xPos + width - horizontalPadding, yPos + height / 2);

            //draw the draggable circle
            g2.setColor(sliderColor);
            g2.fillOval(xPos - sliderXOffset - 10, yPos + height / 2 - 10, 20, 20);
        }

        public override void setAction(MouseInfo e)
        {
            if (mouseWithin(e))
            {
                within = true;
                if (e.action == MouseActionEventType.ACTION_PRESSED)
                    pressed = true;
                else if (e.action == MouseActionEventType.ACTION_RELEASED)
                    pressed = false;
                if (pressed && within || e.action == MouseActionEventType.ACTION_DRAGGED)
                {
                    sliderColor = Color.Red;
                    sliderXOffset = xPos - e.getX();
                }
                else sliderColor = Color.Orange;
            }
            else
            {
                pressed = false;
                sliderColor = Color.Orange;
            }
        }
    }

    /*
        TextEdit class - display a string within a small rectangle. when the user clicks within
        the rectangle, the string becomes editable and the user can type some text that will be
        appended to the string and displayed
     */
    public class TextEdit : Widget
    {
        private bool waitingForInput = false;
        private string currentText = "";

        public TextEdit() { }

        public TextEdit(double preferredSizeX, double preferredSizeY)
            : base(preferredSizeX, preferredSizeY) { }

        public override void drawSelf(Graphics2D g2)
        {
            if (waitingForInput)
            {
                g2.setColor(Color.LightGray);
                g2.fillRect(xPos + horizontalPadding, yPos + verticalPadding,
                        width - horizontalPadding * 2, height - verticalPadding * 2);
                g2.setColor(Color.Black);
                g2.drawString("Enter Text Here:" + currentText, xPos, yPos + height / 2);
            }
            else if (!currentText.Equals(""))
            {
                g2.setColor(dynamicColor);
                g2.fillRect(xPos + horizontalPadding, yPos + verticalPadding,
                        width - horizontalPadding * 2, height - verticalPadding * 2);
                g2.setColor(Color.Black);
                g2.drawString("Enter Text Here:" + currentText, xPos, yPos + height / 2);
            }
            else
            {
                g2.setColor(dynamicColor);
                g2.fillRect(xPos + horizontalPadding, yPos + verticalPadding,
                        width - horizontalPadding * 2, height - verticalPadding * 2);
                drawCenteredLabel(g2);
            }
        }

        private void drawCenteredLabel(Graphics2D g2)
        {
            var str = "textEdit";
            int stringWidth = g2.getStringWidth(str);

            if (within == false)
                g2.setColor(Color.White);
            else g2.setColor(Color.Black);

            g2.drawString(str, xPos + width / 2 - stringWidth / 2, yPos + height / 2);
        }

        protected override void respondPressed(bool currentWithin)
        {
            if (currentWithin && !pressed)
            {
                pressed = true;
                dynamicColor = Color.Green;
                waitingForInput = true;
            }
            else if (!currentWithin)
            {
                waitingForInput = false;
                pressed = false;
                dynamicColor = Color.DarkGray;
            }
        }

        protected override void respondReleased(bool currentWithin)
        {
            if (pressed)
            {
                pressed = false;
                dynamicColor = Color.Cyan;
            }
        }

        public override void sendKeyPress(char key)
        {
            if (waitingForInput)
                currentText += key;
        }
    }

    /*
    VerticalLayout, extends Layout. the only thing it changes is hwo to recalculate the element bounds
     */

    public class VerticalLayout : Layout
    {
        public VerticalLayout(JPanel panel)
            : base(panel)
        { }

        // this function places elements vertically within the bounds of the layout
        public override void recalculateElementBounds(Layout parentLayout)
        {
            height = parentLayout.getHeight();
            int currentYOffset = 0;

            for (int i = 0; i < elements.Count; i++)
            {
                int currentElementHeight = (int)((height * (elements[i].getPreferredSizeY())) / 100);
                int currentElementWidth = (int)((width * (elements[i].getPreferredSizeX())) / 100);
                elements[i].setUpperLeftCorner(
                        topLeftX, currentYOffset + topLeftY);
                currentYOffset += currentElementHeight;
                elements[i].setHeight(currentElementHeight);
                elements[i].setWidth(currentElementWidth);
                elements[i].recalculateElementBounds(this);
            }
        }

    }
    /*
        Widget - abstract class extended by Button, Slider, and TextEdit (and possibly others.
        implements UIElement
        provides all the functionality needed for a typical user interface element, such as
        changing color when hovered by mouse or clicked, setting size and resizing automatically, etc.

        the widget is a 'primitive' in the sense that it cannot contain other widgets or layouts
        layouts can contain other layouts and widgets

        all the fields are protected right now. some of them could probably be made private
        (whichever fields aren't used directly in Slider, Button, or TextEdit could be made private)
     */
    public abstract class Widget : UIElement
    {
        // NOTE: should use a point class 
        protected int xPos, yPos;
        
        // NOTE: should use dimension
        protected int width, height;
        
        // NOTE: Managing state like this is very hard
        protected bool within = false;
        protected bool pressed = false;
        
        // NOTE: in modern Web UI system this would be considered a style and is decoupled from the drawing
        protected readonly int verticalPadding = 5;
        protected readonly int horizontalPadding = 5;

        // NOTE: what makes this dynamic
        protected Color dynamicColor = Color.DarkGray;

        // NOTE: should use dimension. 
        // NOTE: why would this change? Should be readonly. 
        protected double preferredSizeX;
        protected double preferredSizeY;

        // NOTE: in C# we can have 
        public Widget() { }

        public Widget(double preferredSizeX, double preferredSizeY)
        {
            this.preferredSizeX = preferredSizeX;
            this.preferredSizeY = preferredSizeY;
        }

        protected virtual void respondReleased(bool currentWithin)
        {
            if (pressed)
            {
                pressed = false;
                // NOTE: hard coding color in the handler is very inflexible
                dynamicColor = Color.Gray;
            }
        }

        protected virtual void respondPressed(bool currentWithin)
        {
            if (currentWithin && !pressed)
            {
                pressed = true;
                dynamicColor = Color.LightGray;
            }
        }

        protected virtual void respondMoved(bool currentWithin)
        {
            if (!currentWithin && within)
            {
                within = false;
                dynamicColor = Color.DarkGray;
            }
            else if (currentWithin && !within)
            {
                within = true;
                dynamicColor = Color.Gray;
            }
        }

        public virtual void setAction(MouseInfo e)
        {
            bool currentWithin = mouseWithin(e);

            switch (e.action)
            {
                case MouseActionEventType.ACTION_MOVED: respondMoved(currentWithin); break;
                case MouseActionEventType.ACTION_PRESSED: respondPressed(currentWithin); break;
                case MouseActionEventType.ACTION_RELEASED: respondReleased(currentWithin); break;
            }
        }
        
        public void setUpperLeftCorner(int xPos, int yPos)
        {
            Widget widget = this;
            widget.xPos = xPos;
            this.yPos = yPos;
        }

        public void setWidth(int width)
        {
            this.width = width;
        }

        public void setHeight(int height)
        {
            this.height = height;
        }

        public virtual void recalculateElementBounds(Layout parentLayout) { }
        public virtual void sendKeyPress(char key) { }

        public void setPreferredSize(double xPercentage, double yPercentage)
        {
            preferredSizeX = xPercentage;
            preferredSizeY = yPercentage;
        }

        // NOTE: why are these two implemented as virtual? In the other 
        public virtual double getPreferredSizeX()
            => preferredSizeX;

        public virtual double getPreferredSizeY()
            => preferredSizeY;

        // NOTE: should be implemented outside of function, and not using a function
        public virtual bool mouseWithin(MouseInfo e)
        {
            if (e.getX() > xPos && e.getX() <= xPos + width && e.getY() > yPos && e.getY() <= yPos + height)
                return true;
            else return false;
        }

        public abstract void drawSelf(Graphics2D g2);
    }
}
