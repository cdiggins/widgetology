using System.Drawing;
using System.Windows.Forms;
using OriginalImplementation;

namespace Widgetology
{ 
    public class Graphics2D
    {
        private readonly Graphics _graphics;
        private Brush _brush;
        private Pen _pen;
        private Font _font = new("Arial", 12);

        public Graphics2D(Graphics g)
        {
            _graphics = g;
            setColor(Color.Gray);
        }

        public void setColor(Color color)
        {
            _brush = new SolidBrush(color);
            _pen = new Pen(_brush);
        }

        public void setPenWidth(float f)
        {
            _pen.Width = f;
        }
        
        // TODO: what is v1 etc.
        public void fillRect(int v1, int v2, int v3, int v4)        
            => _graphics.FillRectangle(_brush, v1, v2, v3, v4);

        public void drawString(string str, int v1, int v2)
            => _graphics.DrawString(str, _font, _brush, new PointF(v1, v2));

        internal void drawLine(int v1, int v2, int v3, int v4)
            => _graphics.DrawLine(_pen, v1, v2, v3, v4);

        internal void fillOval(int v1, int v2, int v3, int v4)
            => _graphics.FillEllipse(_brush, v1, v2, v3, v4);

        public int getStringWidth(string str)
            => TextRenderer.MeasureText(str, _font).Width;

        public int getStringHeight(string str)
            => TextRenderer.MeasureText(str, _font).Height;
    }

    /*
    public partial class MainForm : Form
    {
        public JPanel JPanel;
        public MasterPanel MasterPanel;

        public MainForm()
        {
            InitializeComponent();
            MouseDown += Form1_MouseDown;
            MouseMove += Form1_MouseMove;
            MouseUp += Form1_MouseUp;
            KeyPress += Form1_KeyPress;
            JPanel = new JPanel(ClientSize.Width, ClientSize.Height, () => { });
            this.DoubleBuffered = true;
            MasterPanel = new MasterPanel(JPanel);
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            MasterPanel.keyPressed(e.KeyChar);
            Invalidate();
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            MasterPanel.mouseReleased(e.X, e.Y);
            Capture = false;
            Invalidate();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (Capture)
                MasterPanel.mouseDragged(e.X, e.Y);
            else
                MasterPanel.mouseMoved(e.X, e.Y);
            Invalidate();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            MasterPanel.mousePressed(e.X, e.Y);
            Capture = true;
            Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            var g = new Graphics2D(e.Graphics);
            MasterPanel.paintComponent(g);
        }
    }

    */
    public partial class MainForm : Form
    {
        public IControl UI = MyUserInterface.Create();
        public Point MousePoint = Point.Default;

        public MainForm()
        {
            InitializeComponent();
            MouseDown += Form1_MouseDown;
            MouseMove += Form1_MouseMove;
            MouseUp += Form1_MouseUp;
            KeyPress += Form1_KeyPress;
            Resize += MainForm_Resize;
            DoubleBuffered = true;
        }

        private void MainForm_Resize(object sender, System.EventArgs e)
            => Invalidate();

        public Rect GetRect()
            => new(Point.Default, new(ClientSize.Width, ClientSize.Height));

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            UI = UI.KeyPress(UI, MousePoint, GetRect(), e.KeyChar);
            Invalidate();
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            UI = UI.Mouse(UI, MousePoint, GetRect(), MouseEvent.Up);
            Capture = false;
            Invalidate();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            MousePoint = new(e.Location.X, e.Location.Y);
            if (Capture)
                UI = UI.Mouse(UI, MousePoint, GetRect(), MouseEvent.Drag);
            else
                UI = UI.Mouse(UI, MousePoint, GetRect(), MouseEvent.Move);
            Invalidate();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            UI = UI.Mouse(UI, MousePoint, GetRect(), MouseEvent.Down);
            Capture = true;
            Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            var c = new Canvas(new Graphics2D(e.Graphics));
            UI.Paint(c, GetRect());
        }
    }
   
}
