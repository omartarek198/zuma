 
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


    public class Ball
    {
       public enum Color
        {
           Red,Blue,Yellow,Green
        }
        public RectangleF ballPosition;
        public int lowImgIndex;
        public int highImgIndex;
        public int currIndex = 0;
        public float currT = 0.0f;
        public Color color;
        public DDA straightPath = null;

        public Ball(int x, int y, int w, int h, int low, int high, Color c)
        {
            currIndex = lowImgIndex;
            highImgIndex = high;
            lowImgIndex = low;

            currIndex = lowImgIndex;
            ballPosition = new Rectangle(x, y, w, h);
            color = c;
        }
        public Ball()
        {
            // default constructor
        }
        public void ChangeImgFrame()
        {
            if (currIndex < highImgIndex)
                currIndex++;
            else
                currIndex = lowImgIndex;

        }

        public bool IsCollid(Ball obj)
        {

            if (this.ballPosition.X < obj.ballPosition.X)
            {
                if (this.ballPosition.X + this.ballPosition.Width > obj.ballPosition.X )
                {

                    if (this.ballPosition.Y < obj.ballPosition.Y)
                    {
                        if (this.ballPosition.Y + this.ballPosition.Height > obj.ballPosition.Y)
                        {
                            return true;
                        }
                    }
                }
            }


            if (this.ballPosition.X+ this.ballPosition.Width > obj.ballPosition.X)
            {
                if (this.ballPosition.X + this.ballPosition.Width < obj.ballPosition.X+ obj.ballPosition.Width)
                {

                    if (this.ballPosition.Y < obj.ballPosition.Y)
                    {
                        if (this.ballPosition.Y + this.ballPosition.Height > obj.ballPosition.Y)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }


    }
}
