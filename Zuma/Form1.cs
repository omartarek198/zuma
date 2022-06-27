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

        Random R = new Random();
        SpriteSheetReader Sheet1;
        SpriteSheetReader Sheet2;
        Timer T = new Timer();
        Stack<Ball.Color> LColorsToGen = new Stack<Ball.Color>();
        int index = 0;
        Bitmap lvl1_bg;
        Bitmap off;
        PointF prevPoint = new PointF(-1, -1);
        Bitmap frog;
        Bitmap rotatedFrog;
        LinkedList<Ball> Lballs = new LinkedList<Ball>();
        double oldAngle = 0;
        PointF ballPoint;
        float t = 0f;
        float inc = 0.001f;
        int streamSize = 46;
        BezierCurve path = new BezierCurve();
        bool attach = false;
        float ref_T = 0f;
        List<Ball> LshotBalls = new List<Ball>();
        int currX, currY;
        LinkedListNode<Ball> Target;
        Ball ShotBall;
        int ct = 0;
        int portalIndex = 0;
        bool shotMove = false;
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            this.MouseDown += Form1_MouseDown;
            this.Paint += Form1_Paint;
            T.Tick += T_Tick;
            this.KeyDown += Form1_KeyDown;

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
                ShootBall();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void T_Tick(object sender, EventArgs e)
        {

            ballPoint = path.CalcCurvePointAtTime(t);
            t += inc;
            MoveBallsOnPath();
            if (index < 47)
            {
                index++;
            }
            else
            {
                index = 0;
            }
            MoveShotballs();
            DetectBallsCollision();

            OpenPortal();
            DrawDubb(CreateGraphics());

            DetectGameEnd();
        }


        public void DetectGameEnd()
        {
            if (Lballs == null)
                EndGame();
            if (Lballs.First.Value.currT >= 0.9999f)
            {
                EndGame();
            }
        }
        public void EndGame()
        {
            Dispose();
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {

        }

        public void GenerateColors(int max)
        {
            for (int i=0;i<200;i++)
            {
                int rand = R.Next(max);
                switch(rand)
                {
                    case 0:
                        LColorsToGen.Push(Ball.Color.Blue);
                        break;
                    case 1:
                        LColorsToGen.Push(Ball.Color.Red);
                        break;
                    case 2:
                        LColorsToGen.Push(Ball.Color.Green);
                        break;
                    case 3:
                        LColorsToGen.Push(Ball.Color.Yellow);
                        break;
                }
            }
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
            // MakeBall();
            GenerateColors(4);
            GenerateLevelBalls();
            MessageBox.Show("Width" + this.Width + " Height" + this.Height);
            T.Start();



        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            //if (prevPoint.X == -1)
            //{
            //    prevPoint = new PointF(e.X, e.Y);
            //}
            //else
            //{
            //    double diffX = e.X - prevPoint.X;
            //    double diffY = e.Y - prevPoint.Y;

            //    double slope = diffY / diffX;

            //    double angle = Math.Atan(slope * 20);


            //    double x = angle + oldAngle;

            //    if (x > 0)
            //    {
            //        rotatedFrog = rotateImage(frog, (float)x);
            //        oldAngle = x;
            //    }



            //}
            currX = e.X;
            currY = e.Y;
            RotateZumaWithMouse(e.X, e.Y);
        }

        public void SetSheet1()
        {
            Sheet1 = new SpriteSheetReader("assets/images/gameobjects.png");
            int keyId = 0;
            for (int j = 0; j < 5; j++)
            {
                for (int i = 0; i < 47; i++)
                {

                    Sheet1.addSubImageToDict(keyId, new Rectangle(j * 48, i * 48, 48, 48));
                    keyId++;


                }

            }

        }

        

        public void DetectMatchingBall(LinkedListNode<Ball> InsertedBall)
        {

            LinkedListNode<Ball> ptEMP;
            bool del = false;
            List<LinkedListNode<Ball>> Ldeletednodes = new List<LinkedListNode<Ball>>();
            for (LinkedListNode<Ball> pFWD = InsertedBall.Next; pFWD !=null; pFWD = pFWD.Next)
            {
                if (pFWD.Value.color == InsertedBall.Value.color)
                {

                    Ldeletednodes.Add(pFWD);
                }
                else
                {
                    break;
                }
            }
            for (LinkedListNode<Ball> pBCK = InsertedBall.Previous; pBCK != null; pBCK = pBCK.Previous)
            {
                if (pBCK.Value.color == InsertedBall.Value.color)
                {

                    Ldeletednodes.Add(pBCK);
                }
                else
                {
                    break;
                }
            }

            if (Ldeletednodes.Count>0)
                Lballs.Remove(InsertedBall);
            for (int i=0;i<Ldeletednodes.Count;i++)
            {
                Lballs.Remove(Ldeletednodes[i]);
            }
        }
        public void CollisionHandler(LinkedListNode<Ball> ball, Ball shotBall)
        {
            LinkedListNode<Ball> ptrav = ball;
            shotBall.currT = ptrav.Value.currT+ 0.007f;


            Lballs.AddBefore(ball, shotBall);
            LinkedListNode<Ball> temp = new     LinkedListNode<Ball> (shotBall); 
            LshotBalls.Remove(shotBall);
            DetectMatchingBall(ball.Previous);
            FixBallsPostion();
        }
        public void DetectBallsCollision()
        {
            for (int i=0;i<LshotBalls.Count;i++)
            {
                if (Lballs.Count > 0)
                {
                    LinkedListNode<Ball> ptrav = Lballs.First;
                    while (ptrav!=null)
                    {
                        if (LshotBalls[i].IsCollid(ptrav.Value))
                        {
                            LshotBalls[i].straightPath.Speed = 5;

                            PointF currP   = new PointF(LshotBalls[i].straightPath.currX, LshotBalls[i].straightPath.currY);
                            PointF nextP = path.CalcCurvePointAtTime(ptrav.Value.currT + 0.007f);
                            LshotBalls[i].straightPath = new DDA();
                            LshotBalls[i].straightPath.SetVals(currP.X, currP.Y, nextP.X, nextP.Y);
                            ShotBall = LshotBalls[i];


                            Target = ptrav;

                            break; 
                        }
                        else
                            ptrav = ptrav.Next;
                    }    
                }





            }
        }
        public Ball GetColoredBall()
        {
            
            Ball ball = new Ball(); 
            switch (LColorsToGen.Pop())
            {
                case Ball.Color.Blue:
                    ball = GetBlueBall();
                    break;
                case Ball.Color.Green:
                    ball = GetGreenBall();
                    break;
                case Ball.Color.Yellow:
                    ball = GetYellowBall();
                    break;
                case Ball.Color.Red:
                    ball = GetRedBall();
                    break;


            }
            return ball;
        }
        public void MakeBall()
        {

            //not proud of this
            Ball ball = GetColoredBall();

            if (Lballs.First == null)
            {
                ball.ballPosition.X = path.CalcCurvePointAtTime(ref_T).X;

                ball.ballPosition.Y = path.CalcCurvePointAtTime(ref_T).Y;
                Lballs.AddFirst(ball);
                ref_T -= 0.007f;
            }
            else
            {
                
                ball.currT = ref_T;

                ball.ballPosition.X = path.CalcCurvePointAtTime(ref_T).X;

                ball.ballPosition.Y = path.CalcCurvePointAtTime(ref_T).Y;
                Lballs.AddLast(ball);
                ref_T -= 0.007f;
            }
        }

        public Ball GetBlueBall()
        {
            Ball ball = new Ball(0, 0, 48, 48, 0, streamSize, Ball.Color.Blue);
            return ball;
        }
        public Ball GetGreenBall()
        {
            Ball ball = new Ball(0, 0, 48, 48, streamSize+1, 1+(streamSize*2), Ball.Color.Green);
            return ball;

        }
        public Ball GetYellowBall()
        {
            Ball ball = new Ball(0, 0, 48, 48, 2+ (streamSize*2), 2+ (streamSize*3), Ball.Color.Yellow);
            return ball;
        }
        public Ball GetRedBall()
        {
            Ball ball = new Ball(0, 0, 48, 48, 3 + (streamSize * 3), 3 + (streamSize * 4), Ball.Color.Red);
            return ball;
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
        void FixBallsPostion()
        {

            LinkedListNode<Ball> ptrav = Lballs.Last;
            float LastT = ptrav.Value.currT;
            ptrav = ptrav.Previous;
            while (ptrav!=null)
            {
                ptrav.Value.currT = LastT + 0.007f;
                LastT = ptrav.Value.currT;
                ptrav = ptrav.Previous;
            }
        }
        public void MoveBallsOnPath()
        {
            int flag = 0;
            for (LinkedListNode<Ball> ptrav = Lballs.First; ptrav != null; ptrav = ptrav.Next)
            {

                PointF point = path.CalcCurvePointAtTime(ptrav.Value.currT);
                ptrav.Value.ballPosition.X = point.X;
                ptrav.Value.ballPosition.Y = point.Y;
                ptrav.Value.currT += inc;
                




            }


            for (LinkedListNode<Ball> ptrav = Target; ptrav != null; ptrav = ptrav.Previous)
            {

                if (flag == 0 && Target != null && ptrav != Target)
                {
                    ptrav.Value.currT += 0.0028f;

                }
                if (ptrav.Previous == null)
                {
                    if (ct == 2)
                    {
                        flag = 1;
                        CollisionHandler(Target, ShotBall);
                        Target = null;
                        
                        ct = 0;



                    }
                    else
                    {
                        ct++;
                    }

                }
            }

           if (Lballs.Last.Value.currT > 0.007f)
            {
                //MakeBalls();
            }

          
        }

        public void MoveShotballs()
        {
            for (int i=0;i<LshotBalls.Count;i++)
            {
                LshotBalls[i].straightPath.MoveStep();
                LshotBalls[i].ballPosition.X = LshotBalls[i].straightPath.currX;

                LshotBalls[i].ballPosition.Y = LshotBalls[i].straightPath.currY;
            }
        }
        public void ShootBall()
        {

            int xCenter = 630 + (rotatedFrog.Width) / 2;
            int yCenter = 300 + (rotatedFrog.Height) / 2;
            Ball ball = GetColoredBall();
            DDA line = new DDA();
            line.SetVals(xCenter, yCenter, xCenter - (2 * (currX - (650 + (rotatedFrog.Width / 2)))), currY);
            ball.straightPath = line;
            LshotBalls.Add(ball);
        }
        public void GenerateLevelBalls()
        {
            int NofBalls = 30;
            float diff = inc * 30;
            for (int i=0;i<NofBalls;i++)
            {
                MakeBall();
            }


           
        }
        public void OpenPortal()
        {
            float  t_Portal =  0.9f ;
            float t_Curr = Lballs.First.Value.currT;
            //double distance = Math.Sqrt(Math.Pow(pPortal.Y - pCurr.Y, 2) + Math.Pow(pPortal.X - pCurr.X, 2));

            float distance = Math.Abs(t_Portal - t_Curr);
            if (distance <0.1)
            {
                portalIndex = 1;
            }
            if (distance < 0.05)
            {
                portalIndex = 3;
            }
            if (distance < 0.025)
            {
                portalIndex =   5;
            }
            


        }
        public void RotateZumaWithMouse(int x, int y)
        {
            if (rotatedFrog != null)
            {
                int xCenter = 630 + (rotatedFrog.Width) / 2;
                int yCenter = 300 + (rotatedFrog.Height) / 2;

                float diffY = (y - yCenter);
                float diffX  =(x - xCenter);

                float m = diffY / diffX;
                m = 1 / m;
                double f = Math.Atan(m);
                float rad = (float)(f * 180 / Math.PI);
                if (y < yCenter)
                    rad -= 180;
                rotatedFrog = rotateImage(frog, rad);
            }
            else
            {
                int xCenter = 630 + (frog.Width) / 2;
                int yCenter = 300 + (frog.Height) / 2;
                float m = (y - yCenter) / (x - xCenter);
                rotatedFrog = rotateImage(frog, (float)Math.Atan(1 / m));
            }
        }
        public void DrawScene(Graphics g)
        {

            g.Clear(Color.White);
            g.DrawImage(lvl1_bg, 1, 1);
            // path.DrawCurve(g);
            Bitmap atlas = new Bitmap(Sheet1.imgPath);
            //        Rectangle pn = Sheet1.getRectangle(index);


            Rectangle rect;
            //g.DrawImage(atlas, new RectangleF(ballPoint.X-(pn.Width/2)

            //    ,ballPoint.Y - (pn.Height / 2)
            //    , pn.Width, pn.Height), pn, GraphicsUnit.Pixel);


            for (LinkedListNode<Ball> ptrav = Lballs.First; ptrav != null;ptrav = ptrav.Next)
            {

              
                  rect = Sheet1.getRectangle(ptrav.Value.currIndex);


                if (Target != null)
                {
                    if (ptrav == Target)
                    {
                        g.DrawRectangle(Pens.Red, new Rectangle((int)ptrav.Value.ballPosition.X - (rect.Width / 2),
                            (int)ptrav.Value.ballPosition.Y - (rect.Height / 2),50,50

                            ));
                           
                    }
                }

                g.DrawImage(atlas, new RectangleF(ptrav.Value.ballPosition.X - (rect.Width / 2)

               , ptrav.Value.ballPosition.Y - (rect.Height / 2)
               , rect.Width, rect.Height), rect, GraphicsUnit.Pixel);


                ptrav.Value.ChangeImgFrame();
            }
            if (rotatedFrog != null)
            {
                g.DrawImage(rotatedFrog, 630, 300);

                //    keyId++;

                //}
    
             
            }

            //g.FillEllipse(Brushes.Red, (630+(rotatedFrog.Width / 2)) - (2 * (currX - (650+(rotatedFrog.Width / 2)))),
            //currY



            //  , 20, 20);
            //

            for (int i = 0; i < LshotBalls.Count; i++)
            {


                  rect = Sheet1.getRectangle(LshotBalls[i].currIndex);


                g.DrawImage(atlas, new RectangleF(LshotBalls[i].ballPosition.X - (rect.Width / 2)

               , LshotBalls[i].ballPosition.Y - (rect.Height / 2)
               , rect.Width, rect.Height), rect, GraphicsUnit.Pixel);


                LshotBalls[i].ChangeImgFrame();
            }


            Ball.Color next = LColorsToGen.Peek();
            int ind = 0;
            switch (next)
            {
                case Ball.Color.Yellow:
                    ind = 2 + (streamSize * 2);
                    break;
                case Ball.Color.Green:
                    ind = streamSize + 1;
                    break;
                case Ball.Color.Blue:
                    ind = 0;
                    break;
                case Ball.Color.Red:
                    ind = 3 + (streamSize * 3);
                    break;
            }


            rect = Sheet1.getRectangle(ind);

            g.DrawImage(atlas, new Rectangle(100,100,48,48), rect, GraphicsUnit.Pixel);

            g.DrawImage(atlas, new Rectangle(375, 300, 160, 130), new Rectangle(620,0,160,120), GraphicsUnit.Pixel);
            g.DrawImage(atlas, new Rectangle(415, 335,80,80), new Rectangle(660, 160 + (80 * portalIndex), 80, 80), GraphicsUnit.Pixel);

            atlas.Dispose();


        }
    }
}
