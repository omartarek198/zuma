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
    public   class DDA
    {
        
            public float xs, ys, xe, ye, dx, dy, m, invM, currX, currY;
            public int Speed = 30;

            public void SetVals(float a, float b, float c, float d)
            {
                xs = a;
                ys = b;
                xe = c;
                ye = d;
                //////////////////
                dx = xe - xs;
                dy = ye - ys;
                m = dy / dx;
                invM = dx / dy;
                /////////////////
                currX = xs;
                currY = ys;
            }


            public void MoveStep()
            {
                if (Math.Abs(dx) > Math.Abs(dy))
                {
                    if (xs < xe)
                    {
                        currX += Speed;
                        currY += m * Speed;
                        //if (currX >= xe)
                        //{
                        //    SetVals(xe, ye, xs, ys);
                        //}
                    }
                    else
                    {
                        currX -= Speed;
                        currY -= m * Speed;
                        //if (currX <= xe)
                        //{
                        //    SetVals(xe, ye, xs, ys);
                        //}
                    }
                }
                else
                {
                    if (ys < ye)
                    {
                        currY += Speed;
                        currX += invM * Speed;
                        //if (currY >= ye)
                        //{
                        //    SetVals(xe, ye, xs, ys);
                        //}
                    }
                    else
                    {
                        currY -= Speed;
                        currX -= invM * Speed;
                        //if (currY <= ye)
                        //{
                        //    SetVals(xe, ye, xs, ys);
                        //}
                    }
                }
            }

            public void DrawYourCurrPos(Graphics g)
            {
                g.FillEllipse(Brushes.Yellow, currX - 5, currY - 5, 10, 10);
                g.DrawLine(Pens.Yellow, xs, ys, xe, ye);
            }
        }
    
}
