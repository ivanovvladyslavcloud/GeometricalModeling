using Lab2.Core.Geometry;
using Lab2.Core;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Lab2.Rendering
{
    public class StrophoidRenderer
    {

        private readonly Pen asymptotePen;

        public StrophoidRenderer()
        {
            asymptotePen = new Pen(Brushes.Gray, 1.5)
            {
                DashStyle = DashStyles.Dash
            };
            asymptotePen.Freeze();
        }
        public void Draw(
            DrawingContext dc,
            Strophoid2D strophoid,
            Transform2D transform,
            bool drawAsymptote,
            Pen pen = null
            )
        {
            if (pen == null)
            {
                pen = new Pen(Brushes.Blue, 2);
                pen.Freeze();
            }

            List<Point> localPoints = strophoid.GetPoints();

            if (localPoints.Count >= 2)
            {
                StreamGeometry streamGeometry = new StreamGeometry();

                using (StreamGeometryContext ctx = streamGeometry.Open())
                {
                    Point startPoint = transform.ToScreen(localPoints[0].X, localPoints[0].Y);
                    ctx.BeginFigure(startPoint, isFilled: false, isClosed: false);

                    for (int i = 1; i < localPoints.Count; i++)
                    {
                        Point screenPoint = transform.ToScreen(localPoints[i].X, localPoints[i].Y);
                        ctx.LineTo(screenPoint, isStroked: true, isSmoothJoin: true);
                    }
                }

                streamGeometry.Freeze();
                dc.DrawGeometry(null, pen, streamGeometry);
            }

            if (drawAsymptote)
            {
                var (p1Local, p2Local) = strophoid.GetAsymptotePoints(height: 2000.0);
                Point p1Screen = transform.ToScreen(p1Local.X, p1Local.Y);
                Point p2Screen = transform.ToScreen(p2Local.X, p2Local.Y);

                dc.DrawLine(asymptotePen, p1Screen, p2Screen);
            }
        }

        public void DrawInteractiveElements(
            DrawingContext dc,
            Strophoid2D strophoid,
            Transform2D transform,
            double tParam,
            double lineLength = 30.0) 
        {

            Point pWorld = strophoid.GetPointAt(tParam);
            Point pScreen = transform.ToScreen(pWorld.X, pWorld.Y);

            Vector tanDir = strophoid.GetTangentAt(tParam);
            Vector normDir = strophoid.GetNormalAt(tParam);


            Vector tanScreenDir = new Vector(tanDir.X, -tanDir.Y);
            Vector normScreenDir = new Vector(normDir.X, -normDir.Y);

            Point tan1 = pScreen - tanScreenDir * lineLength;
            Point tan2 = pScreen + tanScreenDir * lineLength;

            Pen tanPen = new Pen(Brushes.Green, 2);
            dc.DrawLine(tanPen, tan1, tan2);

            Point norm1 = pScreen - normScreenDir * lineLength;
            Point norm2 = pScreen + normScreenDir * lineLength;

            Pen normPen = new Pen(Brushes.Red, 2);
            dc.DrawLine(normPen, norm1, norm2);

            dc.DrawEllipse(Brushes.Orange, new Pen(Brushes.DarkOrange, 1.5), pScreen, 4, 4);
        }
    }
}