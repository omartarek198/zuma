using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Zuma
{
    class SpriteSheetReader
    {
        // x,y,w,h for sub images in atlas
        public IDictionary<int, Rectangle> subImagesPosition  = new Dictionary<int, Rectangle>();

        public Bitmap atlas;

        public SpriteSheetReader(Bitmap img)
        {
            atlas = img;

        }

        public void addSubImageToDict(int id, Rectangle rect)
        {

           
            subImagesPosition.Add(id, rect);

        }

        public Rectangle getRectangle(int id)
        {
            Rectangle pn = subImagesPosition[id];
            return pn;
        }




    }
}
