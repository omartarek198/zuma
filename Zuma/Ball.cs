 
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

        public Ball(int x, int y, int w, int h, int low, int high, Color c)
        {
            currIndex = lowImgIndex;
            highImgIndex = high;
            lowImgIndex = low;

            currIndex = lowImgIndex;
            ballPosition = new Rectangle(x, y, w, h);
            color = c;
        }

        public void ChangeImgFrame()
        {
            if (currIndex < highImgIndex)
                currIndex++;
            else
                currIndex = lowImgIndex;

        }


    }
}
