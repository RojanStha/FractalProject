using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace FractalProject
{
    public partial class FactalForm : Form

    {
        private const int MAX = 256;      // max iterations
        private const double SX = -2.025; // start value real
        private const double SY = -1.125; // start value imaginary
        private const double EX = 0.6;    // end value real
        private const double EY = 1.125;  // end value imaginary
        private static int x1, y1, xs, ys, xe, ye;
        private static double xstart, ystart, xende, yende, xzoom, yzoom;
        private static bool action, rectangle, finished;
        private static float xy;
        private bool mouseDown = false;

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }


        //private Image picture;
        private Bitmap picture;
        private Graphics g;
        private Graphics g1;
        private Cursor c1, c2;
        private HSB HSBcol;
        private Pen pen;
        private Rectangle rect;


        public FactalForm()
        {
            InitializeComponent(); HSBcol = new HSB();
            //setSize(640, 480);
            this.pictureBox1.Size = new System.Drawing.Size(640, 480); // equivalent of setSize in java code
            finished = false;
            //addMouseListener(this);
            //addMouseMotionListener(this);
            //c1 = new Cursor(Cursor.WAIT_CURSOR);
            //c2 = new Cursor(Cursor.CROSSHAIR_CURSOR);
            c1 = Cursors.WaitCursor;
            c2 = Cursors.Cross;
            //x1 = getsize().width;
            //y1 = getsize().height;
            x1 = pictureBox1.Width;
            y1 = pictureBox1.Height;
            xy = (float)x1 / (float)y1;
            //picture = createImage(x1, y1);
            picture = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            //g1 = picture.getGraphics();
            g1 = Graphics.FromImage(picture);
            finished = true;
            //editToolStripMenuItem.Enabled = false;
            start();
        }
        private void initvalues() // reset start values
        {
            xstart = SX;
            ystart = SY;
            xende = EX;
            yende = EY;
            if ((float)((xende - xstart) / (yende - ystart)) != xy)
                xstart = xende - (yende - ystart) * (double)xy;
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        public void start()
        {
            action = false;
            rectangle = false;
            initvalues();
            xzoom = (xende - xstart) / (double)x1;
            yzoom = (yende - ystart) / (double)y1;
            mandelbrot();
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void mandelbrot() // calculate all points
        {
            int x, y;
            float h, b, alt = 0.0f;
            Pen pen = new Pen(Color.White);

            action = false;
            //this.Cursor = c1; // in java setCursor(c1)
            pictureBox1.Cursor = c2;

            //showStatus("Mandelbrot-Set will be produced - please wait..."); will do later
            for (x = 0; x < x1; x += 2)
            {
                for (y = 0; y < y1; y++)
                {
                    h = pointcolour(xstart + xzoom * (double)x, ystart + yzoom * (double)y); // hue value

                    if (h != alt)
                    {
                        b = 1.0f - h * h; // brightness

                        HSBcol.fromHSB(h, 0.8f, b); //convert hsb to rgb then make a Java Color
                        Color col = Color.FromArgb(Convert.ToByte(HSBcol.rChan), Convert.ToByte(HSBcol.gChan), Convert.ToByte(HSBcol.bChan));

                        pen = new Pen(col);

                        //djm end
                        //djm added to convert to RGB from HSB

                        alt = h;
                    }
                    g1.DrawLine(pen, new Point(x, y), new Point(x + 1, y)); // drawing pixel
                }
                //showStatus("Mandelbrot-Set ready - please select zoom area with pressed mouse.");
                Cursor.Current = c1;
                action = true;
            }

            pictureBox1.Image = picture;
        }
        private float pointcolour(double xwert, double ywert) // color value from 0.0 to 1.0 by iterations
        {
            double r = 0.0, i = 0.0, m = 0.0;// real, imaginary, absolute value or distance
            int j = 0;

            while ((j < MAX) && (m < 4.0))
            {
                j++;
                m = r * r - i * i; // x^2 - y^2
                i = 2.0 * r * i + ywert; // 2xy + c
                r = m + xwert;
            }
            return (float)j / (float)MAX;
        }
    }






    class HSB
    {
        public float rChan, gChan, bChan;
        public HSB()
        {
            rChan = gChan = bChan = 0;
        }
        public void fromHSB(float h, float s, float b)
        {
            if (s == 0)
            {
                rChan = gChan = bChan = (int)(b * 255.0f + 0.5f);
            }
            else
            {
                h = (h - (float)Math.Floor(h)) * 6.0f;
                float f = h - (float)Math.Floor(h);
                float p = b * (1.0f - s);
                float q = b * (1.0f - s * f);
                float t = b * (1.0f - (s * (1.0f - f)));
                switch ((int)h)
                {
                    case 0:
                        rChan = (int)(b * 255.0f + 0.5f);
                        gChan = (int)(t * 255.0f + 0.5f);
                        bChan = (int)(p * 255.0f + 0.5f);
                        break;
                    case 1:
                        rChan = (int)(q * 255.0f + 0.5f);
                        gChan = (int)(b * 255.0f + 0.5f);
                        bChan = (int)(p * 255.0f + 0.5f);
                        break;
                    case 2:
                        rChan = (int)(p * 255.0f + 0.5f);
                        gChan = (int)(b * 255.0f + 0.5f);
                        bChan = (int)(t * 255.0f + 0.5f);
                        break;
                    case 3:
                        rChan = (int)(p * 255.0f + 0.5f);
                        gChan = (int)(q * 255.0f + 0.5f);
                        bChan = (int)(b * 255.0f + 0.5f);
                        break;
                    case 4:
                        rChan = (int)(t * 255.0f + 0.5f);
                        gChan = (int)(p * 255.0f + 0.5f);
                        bChan = (int)(b * 255.0f + 0.5f);
                        break;
                    case 5:
                        rChan = (int)(b * 255.0f + 0.5f);
                        gChan = (int)(p * 255.0f + 0.5f);
                        bChan = (int)(q * 255.0f + 0.5f);
                        break;
                }
            }
        }
    }

}
