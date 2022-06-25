using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zuma
{
    public partial class Form1 : Form
    {


        SpriteSheetReader Sheet1;
        SpriteSheetReader Sheet2;
        Timer T = new Timer();
        int index = 0;
        Bitmap lvl1_bg;
        Bitmap off;
        PointF prevPoint = new PointF(-1, -1);
        Bitmap frog;
        Bitmap rotatedFrog;
        double oldAngle = 0;
        PointF ballPoint;
        float t = 0f;
        float inc = 0.001f;
        
        BezierCurve path = new BezierCurve();
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            this.MouseDown += Form1_MouseDown;
            this.Paint += Form1_Paint;
            T.Tick += T_Tick;
            
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
        
        }

        private void T_Tick(object sender, EventArgs e)
        {

            ballPoint = path.CalcCurvePointAtTime(t);
            t += inc;

            if (index < 45)
            {
                index++;
            }
            else
            {
                index = 0;
            }
            
            DrawDubb(CreateGraphics());
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            
        }

      
        private void Form1_Load(object sender, EventArgs e)
        {

            this.WindowState = FormWindowState.Maximized;
            off = new Bitmap(this.Width, this.Height);



            SetSheet1();
            SetSheet2();

 
              lvl1_bg = new Bitmap("assets/images/level1_bg.jpg");
            lvl1_bg = new Bitmap(lvl1_bg, new Size(Width, Height));
            this.MouseMove += Form1_MouseMove;
            frog = new Bitmap(Sheet2.atlas.Clone(Sheet2.getRectangle(0), Sheet2.atlas.PixelFormat));
            MakePath(); 
            T.Start();
          
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (prevPoint.X == -1)
            {
                prevPoint = new PointF(e.X, e.Y);
            }
            else
            {
                double diffX = e.X - prevPoint.X;
                double diffY = e.Y - prevPoint.Y;

                double slope = diffY / diffX;

                double angle = Math.Atan(slope*20);


                double x = angle +  oldAngle;

                if (x > 0)
                {
                    rotatedFrog = rotateImage(frog, (float)x);
                    oldAngle = x;
                }
          


            }
        }

        public void SetSheet1()
        {
            Sheet1 = new SpriteSheetReader(new Bitmap("assets/images/gameobjects.png"));
            int keyId = 0;
            for (int j = 0; j < 5; j++)
            {
                for (int i = 0; i < 51; i++)
                {

                    Sheet1.addSubImageToDict(keyId, new Rectangle(j * 48, i * 48, 48, 48));
                    keyId++;


                }

            }

        }

        private Bitmap rotateImage(Bitmap b, float angle)
        {
            Bitmap returnBitmap = new Bitmap(b.Width, b.Height);
            Graphics g = Graphics.FromImage(returnBitmap);
            g.TranslateTransform((float)b.Width / 2, (float)b.Height / 2);
            g.RotateTransform(angle);
            g.TranslateTransform(-(float)b.Width / 2, -(float)b.Height / 2);
            g.DrawImage(b, new Point(0, 0));
            return returnBitmap;
        }


        public void SetSheet2()
        {
            Sheet2 = new SpriteSheetReader(new Bitmap("assets/images/frog.png"));
            int keyId = 0;

            Sheet2.addSubImageToDict(keyId, new Rectangle(0, 0, 160, 160));
        }
        void DrawDubb(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2);
            g.DrawImage(off, 0, 0);
        }

        List<Point>  ReadPoints()
        {


            List<Point> points = new List<Point>();
            
            
            string[] readText = File.ReadAllLines("points.txt");
            int[] values = new int[readText.Length/2];
            int ct = 0;
            foreach (string s in readText)
            {
                if (s != ",")
                {
                    values[ct] = Int32.Parse(s);
                    ct++;
                }


            }
            int j = 1;
            for (int i=0;i<ct-1;i+=2)
            {
                Point pn = new Point(values[i], values[j]);

                j+=2;

                points.Add(pn);
                
            }
            
            return points;

        }

        void MakePath()
        {
            List<Point> points = ReadPoints();

            for (int i=0;i<points.Count;i++)
            {
                path.SetControlPoint(points[i]);


            }


        }
        public void DrawScene(Graphics g)
        {

            g.Clear(Color.White);
            g.DrawImage(lvl1_bg, 1, 1);
           // path.DrawCurve(g);

                    Rectangle pn = Sheet1.getRectangle(index);

 
            g.DrawImage(Sheet1.atlas, new RectangleF(ballPoint.X-(pn.Width/2)
                
                ,ballPoint.Y - (pn.Height / 2)
                , pn.Width, pn.Height), pn, GraphicsUnit.Pixel);
            

            if (rotatedFrog != null)
                g.DrawImage(rotatedFrog, 550, 400);

            //    keyId++;

            //}





        }
    }
}
