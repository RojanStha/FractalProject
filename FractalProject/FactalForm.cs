using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Xml;
using System.IO;


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
        private int colorCycle;
        private Graphics g1;
        private Cursor c1, c2;
        private HSB HSBcol;
        private Pen pen;
        private Rectangle rect;
        private int colourCode = 0;
        private bool cycleForward = true;
        private char ColorPalate;
        private char newcolor;


        private void ColourCycle_Tick(object sender, EventArgs e)
        {
            mandelbrot();
            Refresh();
            if (colourCode == 0)
            {
                cycleForward = false;
            }
            if (colourCode == -250)
            {
                cycleForward = true;
            }
            if (cycleForward)
            {
                colourCode++;
            }
            else
            {
                colourCode--;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }


        //private Image picture;
        private Bitmap picture;
        private Graphics g;



        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            //e.consume();
            if (action)
            {
                mouseDown = true;
                xs = e.X;
                ys = e.Y;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (action && mouseDown)
            {
                xe = e.X;
                ye = e.Y;
                rectangle = true;
                pictureBox1.Refresh();
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            int z, w;

            //e.consume();
            if (action)
            {
                xe = e.X;
                ye = e.Y;
                if (xs > xe)
                {
                    z = xs;
                    xs = xe;
                    xe = z;
                }
                if (ys > ye)
                {
                    z = ys;
                    ys = ye;
                    ye = z;
                }
                w = (xe - xs);
                z = (ye - ys);
                if ((w < 2) && (z < 2)) initvalues();
                else
                {
                    if (((float)w > (float)z * xy)) ye = (int)((float)ys + (float)w / xy);
                    else xe = (int)((float)xs + (float)z * xy);
                    xende = xstart + xzoom * (double)xe;
                    yende = ystart + yzoom * (double)ye;
                    xstart += xzoom * (double)xs;
                    ystart += yzoom * (double)ys;
                }
                xzoom = (xende - xstart) / (double)x1;
                yzoom = (yende - ystart) / (double)y1;
                mandelbrot();
                rectangle = false;
                pictureBox1.Refresh();
                mouseDown = false;
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }
        public void Stop()
        {
            pictureBox1.Image = null;
            pictureBox1.Invalidate();
        }
        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stop();
            start();
        }
        
            private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Interval = 1;
            timer1.Start();
        }

        private void offToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (colorCycle == 4)
            {

                colorCycle = 0;
                HSBcol.colorCycling(colorCycle);
                mandelbrot();
                //start();
            }

            else
            {
                colorCycle = colorCycle + 1;
                HSBcol.colorCycling(colorCycle);
                mandelbrot();
                //start();

            }

            action = false; mouseDown = false; rectangle = false;
        }

        private void colorPalletToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void redToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorPalate = 'R';
            HSBcol.Color('R');
            mandelbrot();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            update();
        }
        public void update()
        {
            Image tempPic = Image.FromHbitmap(picture.GetHbitmap());
            Graphics g = Graphics.FromImage(tempPic);

            if (rectangle)
            {
                Pen pen = new Pen(Color.White);

                Rectangle rect;

                if (xs < xe)
                {
                    if (ys < ye)
                    {
                        rect = new Rectangle(xs, ys, (xe - xs), (ye - ys));
                    }
                    else
                    {
                        rect = new Rectangle(xs, ye, (xe - xs), (ys - ye));
                    }
                }
                else
                {
                    if (ys < ye)
                    {
                        rect = new Rectangle(xe, ys, (xs - xe), (ye - ys));
                    }
                    else
                    {
                        rect = new Rectangle(xe, ye, (xs - xe), (ys - ye));
                    }
                }

                g.DrawRectangle(pen, rect);
                pictureBox1.Image = tempPic;

            }
        }
        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawImage(pictureBox1.Image, 0, 0);
        }
        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintDocument p = new PrintDocument();
            p.PrintPage += new PrintPageEventHandler(printDocument1_PrintPage);
            p.Print();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                XmlWriter writer = XmlWriter.Create("state.xml");
                writer.WriteStartDocument();
                writer.WriteStartElement("states");
                writer.WriteElementString("xstart", xstart.ToString());
                writer.WriteElementString("ystart", ystart.ToString());
                writer.WriteElementString("xzoom", xzoom.ToString());
                writer.WriteElementString("yzoom", yzoom.ToString());
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
                MessageBox.Show("Your State Has been Saved Successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String exists = "state.xml";
            if (File.Exists(exists))
            {
                File.Delete("state.xml");
            }
            pictureBox1.Image = null;
            start();
        }

      


        public FactalForm()
        {
            InitializeComponent(); HSBcol = new HSB();
        
            this.pictureBox1.Size = new System.Drawing.Size(640, 480); // equivalent of setSize in java code
            finished = false;
            c1 = Cursors.WaitCursor;
            c2 = Cursors.Cross;
            x1 = pictureBox1.Width;
            y1 = pictureBox1.Height;
            xy = (float)x1 / (float)y1;
            picture = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g1 = Graphics.FromImage(picture);
            finished = true;
            
            start();
        }
        private void initvalues() 
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
            String exists = "state.xml";
            if (File.Exists(exists))
            {
                try
                {
                    XmlDocument state = new XmlDocument();
                    state.Load("state.xml");
                    foreach (XmlNode node in state)
                    {
                        xstart = Convert.ToDouble(node["xstart"]?.InnerText);
                        ystart = Convert.ToDouble(node["ystart"]?.InnerText);
                        xzoom = Convert.ToDouble(node["xzoom"]?.InnerText);
                        yzoom = Convert.ToDouble(node["yzoom"]?.InnerText);
                    }
                    mandelbrot();
                    Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                initvalues();
                xzoom = (xende - xstart) / (double)x1;
                yzoom = (yende - ystart) / (double)y1;
                mandelbrot();
            }
        }
       
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog f = new SaveFileDialog();
            f.Filter = "JPG(*.JPG) | *.JPG";
           
            if (f.ShowDialog() == DialogResult.OK)
            {
                picture.Save(f.FileName);
            }
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
        public bool color = false;
        public Char colorValue;
        public float newValue;
        public double cvalue = 255.0f;
        public float rChan, gChan, bChan;
        public HSB()
        {
            rChan = gChan = bChan = 0;
        }
        public void Color(Char colorValue)
        {
            this.colorValue = colorValue;
            color = true;

        }
        public void colorCycling(int i)
        {
            char[] colors = { 'R', 'B', 'G', 'P', 'D' };


            this.colorValue = colors[i];

        }
        public void cycle(double i)
        {

            if (i < 256.0f)
            {
                this.cvalue = i;
            }
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
                        if (colorValue.Equals('B'))
                        {
                            rChan = (int)(p * 255.0f + 0.5f);
                            gChan = (int)(t * 255.0f + 0.5f);
                            bChan = (int)(b * 255.0f + 0.5f);
                        }
                        else if (colorValue.Equals('G'))
                        {
                            rChan = (int)(p * 255.0f + 0.5f);
                            gChan = (int)(b * 255.0f + 0.5f);
                            bChan = (int)(t * 255.0f + 0.5f);
                        }
                        else if (colorValue.Equals('P'))
                        {
                            rChan = (int)(b * 255.0f + 0.5f);
                            gChan = (int)(t * 255.0f + 0.5f);
                            bChan = (int)(b * 255.0f + 0.5f);
                        }
                        else if (colorValue.Equals('D'))
                        {
                            rChan = (int)(b * 0.0f + 0.5f);
                            gChan = (int)(t * 0.0f + 0.5f);
                            bChan = (int)(p * 0.0f + 0.5f);
                        }
                        else
                        {
                            rChan = (int)(b * 255.0f + 0.5f);
                            gChan = (int)(t * 255.0f + 0.5f);
                            bChan = (int)(p * 255.0f + 0.5f);
                        }

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
