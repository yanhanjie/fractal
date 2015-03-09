using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FractalAssignment
{
    public partial class Form1 : Form
    {
    	private const int MAX = 256;      // max iterations
        private const double SX = -2.025; // start value real
        private const double SY = -1.125; // start value imaginary
        private const double EX = 0.6;    // end value real
        private const double EY = 1.125;  // end value imaginary
        private static int x1, y1, xs, ys, xe, ye;
        private static double xstart, ystart, xende, yende, xzoom, yzoom;
        private static bool action, rectangle, working, cycle, maxj;
        private static float xy;
        private static int j = 0;
        private Bitmap picture;
        private Graphics g1;
        ///private Cursor c1, c2;
        private Pen p;
        private HSBColor HSBcol;
        private bool _mousePressed;
        private ToolStripMenuItem selitem; // static required to store sender object in a variable when moving between forms
        // Temporary copies of variables for load method
        private static double txstart, tystart, txzoom, tyzoom;
        private static int tJ;

        public int J
        {
            get
            {
                return j;
            }
            set
            {
                j = value;
            }
        }

        [STAThread]
        public static void Main(string[] args)
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1()); // Create a new windows form and open
        }
        
	    public void init() // all instances will be prepared
	    {
		    //HSBcol = new HSB();
		    ///setSize(640,480); // set by form properties
		    ///finished = false;
		    ///addMouseListener(this); // windows forms handles the mouse stuff
		    ///addMouseMotionListener(this);
		    ///c1 = new Cursor(Cursor.WAIT_CURSOR); // cursor objects don't need to be created
		    ///c2 = new Cursor(Cursor.CROSSHAIR_CURSOR);
            //this.Cursor = Cursors.Cross; // Set in form property
		    x1 = this.Width;
		    y1 = this.Height;
		    xy = (float)x1 / (float)y1;

            /*
            // Colour cycling by shifting colours in the palette
            picture = new Bitmap(x1, y1, PixelFormat.Format8bppIndexed);
            ColorPalette palette = picture.Palette;
            Color first = palette.Entries[0];
            for (int i = 0; i < 245; i++)
            {
                palette.Entries[i] = palette.Entries[i + 11];
                //System.Diagnostics.Debug.WriteLine(palette.Entries[i]);
            }
            palette.Entries[(palette.Entries.Length - 1)] = first;
            picture.Palette = palette;

            picture2 = new Bitmap(picture.Width, picture.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            g1 = Graphics.FromImage(picture2);
            */

            picture = new Bitmap(x1, y1); // Double buffer property enabled on form removes flickering when using rubber band box
            g1 = Graphics.FromImage(picture);
            ///finished = true;
	    }

	    ///public void destroy() // delete all instances 
	    ///{
		///    if (finished)
		///    {
		///	    removeMouseListener(this);
		///	    removeMouseMotionListener(this);
		///	    picture = null;
		///	    g1 = null;
		///	    c1 = null;
		///	    c2 = null;
		///	    System.gc(); // garbage collection
		///    }
	    ///}

	    public void start()
	    {
		    action = false;
		    rectangle = false;
		    initvalues();
		    xzoom = (xende - xstart) / (double)x1;
		    yzoom = (yende - ystart) / (double)y1;
		    mandelbrot();
	    }
	    
	    ///public void stop()
	    ///{
	    ///}
	
	    public void paint(Graphics g)
	    {
		    update(g);
	    }
	
	    public void update(Graphics g) // Update the canvas to render image
	    {
            g.DrawImage(picture, 0, 0);
		    if (rectangle)
		    {
		    	p = new Pen(Color.White);
			    if (xs < xe)
			    {
				    if (ys < ye) g.DrawRectangle(p, xs, ys, (xe - xs), (ye - ys));
				    else g.DrawRectangle(p, xs, ye, (xe - xs), (ys - ye));
			    }
			    else
			    {
				    if (ys < ye) g.DrawRectangle(p, xe, ys, (xs - xe), (ye - ys));
				    else g.DrawRectangle(p, xe, ye, (xs - xe), (ys - ye));
			    }
		    }
	    }
	
	    private void mandelbrot() // calculate all points
        {
		    int x, y;
		    float h, b, alt = 0.0f;
		    Color col;
		
		    action = false;
            if (cycle == false || working == true) this.Cursor = Cursors.WaitCursor;
		    ///showStatus("Mandelbrot-Set will be produced - please wait...");
		    for (x = 0; x < x1; x+=2)
			    for (y = 0; y < y1; y++)
			    {
				    h = pointcolour(xstart + xzoom * (double)x, ystart + yzoom * (double)y); // color value
				    if (h != alt)
				    {
					    /////djm test
				    	///Color col = Color.getHSBColor(h,0.8f,b);
				    	///int red = col.getRed();
				    	///int green = col.getGreen();
				    	///int blue = col.getBlue();
					    /////djm
					    b = 1.0f - h * h; // brightness
					    HSBcol = new HSBColor(h,0.8f,b); // values multiplied by 255 in HSBColor class
					    col = HSBcol.FromHSB(HSBcol); // Convert hsb colours to rgb
					    ///g1.setColor(Color.getHSBColor(h, 0.8f, b)); // In C# set pen colour instead
					    p = new Pen(col); // Change the pen color
                        //System.Diagnostics.Debug.WriteLine(col); // Show pen colour for each plot in rgb
					    alt = h;
				    }      
				    g1.DrawLine(p, x, y, x + 1, y); // Draw line on canvas of Bitmap
			    }
		    ///showStatus("Mandelbrot-Set ready - please select zoom area with pressed mouse.");
            if (cycle == false || working == true) this.Cursor = Cursors.Cross;
            working = false;
		    action = true;
	    }
	    
	    private float pointcolour(double xwert, double ywert) // color value from 0.0 to 1.0 by iterations
	    {
		    double r = 0.0, i = 0.0, m = 0.0;
		    int j = J; // Use for colour cycling - alternatively edit pallet the pallet dynamically, storing the colours in and array and shifting them

		    while ((j < MAX) && (m < 4.0))
		    {
			    j++;
			    m = r * r - i * i;
			    i = 2.0 * r * i + ywert;
			    r = m + xwert;
		    }
		    return (float)j / (float)MAX;
	    }
	    
	    private void initvalues() // reset start values
	    {
		    xstart = SX;
		    ystart = SY;
		    xende = EX;
		    yende = EY;
		    if ((float)((xende - xstart) / (yende - ystart)) != xy ) 
			    xstart = xende - (yende - ystart) * (double)xy;
	    }
	    
	    ///public String getAppletInfo()
	    ///{
		///    return "fractal.class - Mandelbrot Set a Java Applet by Eckhard Roessel 2000-2001";
	    ///}
	    
    	public Form1()
        {
            InitializeComponent();
            this.Text = "Fractal Draw";
            init();
            start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            mandelbrot();
            paletteShift();
            Invalidate();
        }

        public void paletteShift()
        {
            if (maxj == false && J < 200)
            {
                J = J + 2;
            }
            else if (maxj == false && J == 200)
            {
                maxj = true;
                J = J - 2;
            }
            else if (maxj == true && J > 0)
            {
                J = J - 2;
            }
            else if (maxj == true && J == 0)
            {
                maxj = false;
                J = J + 2;
            }
        }
        
        private void Form1Paint(object sender, PaintEventArgs e)
        {
        	paint(e.Graphics);
        	//MessageBox.Show("Welcome to DotNet");
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                _mousePressed = true;
                ///e.consume();
                if (action)
                {
                    xs = e.X;
                    ys = e.Y;
                }

                if (e.Button == MouseButtons.Right)
                {
                    zoomFractal(e);
                }
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                zoomFractal(e);
            }
        }

        public void zoomFractal(MouseEventArgs e)
        {
            _mousePressed = false;
            int z, w;

            ///e.consume();
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
                working = true;
                mandelbrot();
                rectangle = false;
                Invalidate(); // Recall paint method
            }
        }
        
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mousePressed)
            {
                ///e.consume();
                if (action)
                {
                    xe = e.X;
                    ye = e.Y;
                    rectangle = true;
                    Invalidate(); // Recall paint method
                }

                int TitlebarHeight = SystemInformation.CaptionHeight;
                int BorderWidth = SystemInformation.FrameBorderSize.Width;

                if (e.X >= -BorderWidth && e.X <= x1 - 3 * BorderWidth && e.Y >= 24 && e.Y <= y1 - 24 - TitlebarHeight + 3 * BorderWidth) // Correct dimensions on windows 7, may not be on other versions (not on windows XP)
                {
                    this.Cursor = Cursors.Cross;
                }
                else
                {
                    this.Cursor = Cursors.Arrow;
                }
            }
        }

        private void Form1_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Cross;
        }

        private void Form1_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //openFileDialog1.ShowDialog();
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = Convert.ToString(Environment.SpecialFolder.MyDocuments);
            openFileDialog1.Filter = "Text Documents (*.txt)|*.txt|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                String line;
                System.IO.StreamReader file = new System.IO.StreamReader(openFileDialog1.FileName);
                bool[] set = { false, false, false, false, false };
                while ((line = file.ReadLine()) != null)
                {
                    String[] splitline = line.Split('=');
                    if (splitline.Length == 2)
                    {
                        double val1;
                        int val2;
                        bool check1 = double.TryParse(splitline[1], out val1);
                        bool check3 = int.TryParse(splitline[1], out val2);
                        switch (splitline[0].ToLower())
                        {
                            case "xstart":
                                if (check1 == true)
                                {
                                    txstart = val1;
                                    set[0] = true;
                                }
                                break;
                            case "ystart":
                                if (check1 == true)
                                {
                                    tystart = val1;
                                    set[1] = true;
                                }
                                break;
                            case "xzoom":
                                if (check1 == true)
                                {
                                    txzoom = val1;
                                    set[2] = true;
                                }
                                break;
                            case "yzoom":
                                if (check1 == true)
                                {
                                    tyzoom = val1;
                                    set[3] = true;
                                }
                                break;
                            case "j":
                                if (check3 == true)
                                {
                                    tJ = val2;
                                    set[4] = true;
                                }
                                break;
                        }
                    }
                }
                file.Close();

                foreach (bool value in set)
                {
                    if (value == false)
                    {
                        MessageBox.Show("The file you have selected is not a valid save state", "Error");
                        return; // Prevents the following code from executing
                    }
                }

                // Commit changes to variables
                xstart = txstart;
                ystart = tystart;
                xzoom = txzoom;
                yzoom = tyzoom;
                J = tJ; // cycle mode is disabled on save to prevent crashes when loading a cycling fractal twice

                mandelbrot();
                Invalidate(); // Recall paint method
                checkItem(sender);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //saveFileDialog1.ShowDialog();
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = Convert.ToString(Environment.SpecialFolder.MyDocuments);
            saveFileDialog1.Filter = "Text Documents (*.txt)|*.txt";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.FileName = "SaveState_1.txt";
            
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                String extension = Path.GetExtension(saveFileDialog1.FileName);
                String filename;
                switch (extension.ToLower())
                {
                    case ".txt":
                        filename = saveFileDialog1.FileName;
                        break;
                    default:
                        filename = saveFileDialog1.FileName + ".txt";
                        break;
                }

                string lines = "xstart=" + xstart + "\r\nystart=" + ystart + "\r\nxzoom=" + xzoom + "\r\nyzoom=" + yzoom + "\r\nJ=" + J;

                // Write the string to a file
                System.IO.StreamWriter file = new System.IO.StreamWriter(filename);
                file.WriteLine(lines);
                file.Close();
            }
        }

        private void copyToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetImage(picture);
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = Convert.ToString(Environment.SpecialFolder.MyDocuments);
            //saveFileDialog1.DefaultExt = "png";
            saveFileDialog1.Filter = "JPEG (*.jpg;*.jpeg;*.jpe;*.jfif)|*.jpg|GIF (*.gif)|*.gif|PNG (*.png)|*.png";
            saveFileDialog1.FilterIndex = 3;
            saveFileDialog1.FileName = "MyFractal_1.png";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                String extension = Path.GetExtension(saveFileDialog1.FileName);

                switch (extension.ToLower())
                {
                    case ".jpg":
                        // Save as JPEG
                        picture.Save(saveFileDialog1.FileName, ImageFormat.Jpeg);
                        break;
                    case ".jpeg":
                        // Save as JPEG
                        picture.Save(saveFileDialog1.FileName, ImageFormat.Jpeg);
                        break;
                    case ".gif":
                        // Save as GIF
                        picture.Save(saveFileDialog1.FileName, ImageFormat.Gif);
                        break;
                    case ".png":
                        // Save as PNG
                        picture.Save(saveFileDialog1.FileName, ImageFormat.Png);
                        break;
                    default:
                        // Save as PNG
                        picture.Save(saveFileDialog1.FileName + ".png", ImageFormat.Png);
                        //throw new ArgumentOutOfRangeException(extension);
                        break;
                }
            }
        }

        public void checkItem(object sender)
        {
            // Checks a single item in the tool strip menu
            if (selitem == null)
            {
                redToolStripMenuItem.Checked = false;
            }
            else
            {
                selitem.Checked = false;
            }

            if (cycle == true)
            {
                cycleToolStripMenuItem.Checked = true;
                selitem = cycleToolStripMenuItem;
            }
            else if (J == 0)
            {
                redToolStripMenuItem.Checked = true;
                selitem = redToolStripMenuItem;
            }
            else if (J == 10)
            {
                orangeToolStripMenuItem.Checked = true;
                selitem = orangeToolStripMenuItem;
            }
            else if (J == 35)
            {
                yellowToolStripMenuItem.Checked = true;
                selitem = yellowToolStripMenuItem;
            }
            else if (J == 70)
            {
                greenToolStripMenuItem.Checked = true;
                selitem = greenToolStripMenuItem;
            }
            else if (J == 120)
            {
                lightBlueToolStripMenuItem.Checked = true;
                selitem = lightBlueToolStripMenuItem;
            }
            else if (J == 160)
            {
                darkBlueToolStripMenuItem.Checked = true;
                selitem = darkBlueToolStripMenuItem;
            }
            else if (J == 180)
            {
                indigoToolStripMenuItem.Checked = true;
                selitem = indigoToolStripMenuItem;
            }
            else if (J == 200)
            {
                violetToolStripMenuItem.Checked = true;
                selitem = violetToolStripMenuItem;
            }
            else
            {
                customToolStripMenuItem.Checked = true;
                selitem = customToolStripMenuItem;
            }
            //selitem = (ToolStripMenuItem)sender;
            //((ToolStripMenuItem)sender).Checked = true;
        }

        private void redToolStripMenuItem_Click(object sender, EventArgs e)
        {
            J = 0;
            cycle = false;
            timer1.Stop();
            mandelbrot();
            Invalidate(); // Recall paint method
            checkItem(sender);
        }

        private void orangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            J = 10;
            cycle = false;
            timer1.Stop();
            mandelbrot();
            Invalidate(); // Recall paint method
            checkItem(sender);
        }

        private void yellowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            J = 35;
            cycle = false;
            timer1.Stop();
            mandelbrot();
            Invalidate(); // Recall paint method
            checkItem(sender);
        }

        private void greenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            J = 70;
            cycle = false;
            timer1.Stop();
            mandelbrot();
            Invalidate(); // Recall paint method
            checkItem(sender);
        }

        private void lightBlueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            J = 120;
            cycle = false;
            timer1.Stop();
            mandelbrot();
            Invalidate(); // Recall paint method
            checkItem(sender);
        }

        private void darkBlueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            J = 160;
            cycle = false;
            timer1.Stop();
            mandelbrot();
            Invalidate(); // Recall paint method
            checkItem(sender);
        }

        private void indigoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            J = 180;
            cycle = false;
            timer1.Stop();
            mandelbrot();
            Invalidate(); // Recall paint method
            checkItem(sender);
        }

        private void violetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            J = 200;
            cycle = false;
            timer1.Stop();
            mandelbrot();
            Invalidate(); // Recall paint method
            checkItem(sender);
        }

        private void cycleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cycle = true;
            timer1.Start();
            checkItem(sender);
        }

        private void customToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 f1 = new Form2();
            f1.StartPosition = FormStartPosition.CenterParent; // Set the dialog box to appear in the center of the parent form upon opening
            if (f1.ShowDialog() == DialogResult.OK) // Execution continues here after dialog is closed
            {
                J = f1.Value;
                cycle = false;
                timer1.Stop();
                mandelbrot();
                Invalidate(); // Recall paint method
                checkItem(sender);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 f1 = new AboutBox1();
            f1.Show();
        }
    }
}
