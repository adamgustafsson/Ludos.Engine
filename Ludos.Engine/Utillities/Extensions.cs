using Microsoft.Xna.Framework;
using System;
using SD = System.Drawing;

namespace Ludos.Engine.Utilities
{
    static class Extensions
    {
        public static SD.PointF CenterP(this SD.RectangleF rec)
        {
            return new System.Drawing.PointF(rec.X + rec.Width / 2,
                              rec.Y + rec.Height / 2);
        }
        public static Vector2 Center(this SD.RectangleF rec)
        {
            return new Vector2(rec.X + rec.Width / 2,
                              rec.Y + rec.Height / 2);
        }
        public static int ToInt32(this float value)
        {
            return Convert.ToInt32(value);
        }
        public static Rectangle AdjustLocation(this Rectangle rec, int x, int y)
        {
            rec.Offset(x, y);
            return rec;
        }
        public static Rectangle Round(this SD.RectangleF recF)
        {
            var sysRec = SD.Rectangle.Round(recF);
            return new Rectangle(sysRec.X, sysRec.Y, sysRec.Width, sysRec.Height);
        }
    }
}
