namespace Ludos.Engine.Utilities
{
    using System;
    using System.Drawing;
    using Microsoft.Xna.Framework;
    using Rectangle = Microsoft.Xna.Framework.Rectangle;

    internal static class Extensions
    {
        public static PointF CenterP(this RectangleF rec)
        {
            return new PointF(
                rec.X + (rec.Width / 2),
                rec.Y + (rec.Height / 2));
        }

        public static Vector2 Center(this RectangleF rec)
        {
            return new Vector2(
                rec.X + (rec.Width / 2),
                rec.Y + (rec.Height / 2));
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

        public static Rectangle Round(this RectangleF recF)
        {
            var sysRec = System.Drawing.Rectangle.Round(recF);
            return new Rectangle(sysRec.X, sysRec.Y, sysRec.Width, sysRec.Height);
        }
    }
}
