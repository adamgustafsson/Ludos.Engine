namespace Ludos.Engine.Utilities
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using PointF = System.Drawing.PointF;
    using RectangleF = System.Drawing.RectangleF;

    public static class Extensions
    {
        public static PointF CenterP(this RectangleF rec)
        {
            return new PointF(
                rec.X + (rec.Width / 2f),
                rec.Y + (rec.Height / 2f));
        }

        public static Vector2 Center(this RectangleF rec)
        {
            return new Vector2(
                rec.X + (rec.Width / 2f),
                rec.Y + (rec.Height / 2f));
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

        public static RectangleF ToRectangleF(this Rectangle rec)
        {
            return new RectangleF(rec.X, rec.Y, rec.Width, rec.Height);
        }

        public static Rectangle ToRectangle(this RectangleF rec)
        {
            return new Rectangle(rec.X.ToInt32(), rec.Y.ToInt32(), rec.Width.ToInt32(), rec.Height.ToInt32());
        }

        public static GraphicsDevice GetGraphicsDevice(this ContentManager content)
        {
            return ((IGraphicsDeviceService)content.ServiceProvider.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;
        }
    }
}
