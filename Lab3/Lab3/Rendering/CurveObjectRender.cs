using Lab3.Core;
using Lab3.Core.Geometry;
using System.Windows;
using System.Windows.Media;

namespace Lab3.Rendering
{
    public class CurveObjectRenderer
    {
        public void Draw(
            DrawingContext dc,
            CurveObject curve,
            Transform2D transform,
            bool showControlPolygon = true,
            bool isSelected = false)
        {
            if (curve == null) return;

            if (showControlPolygon)
            {
                Pen polygonPen = new Pen(Brushes.Gray, 1) { DashStyle = DashStyles.Dash };

                Point screenRA = transform.ToScreen(curve.RA.X, curve.RA.Y);
                Point screenRB = transform.ToScreen(curve.RB.X, curve.RB.Y);
                Point screenRC = transform.ToScreen(curve.RC.X, curve.RC.Y);
                Point screenRD = transform.ToScreen(curve.RD.X, curve.RD.Y);

                dc.DrawLine(polygonPen, screenRA, screenRB);
                dc.DrawLine(polygonPen, screenRB, screenRC);
                dc.DrawLine(polygonPen, screenRC, screenRD);

                double rSize = 3;
                dc.DrawEllipse(Brushes.Blue, null, screenRA, rSize, rSize);
                dc.DrawEllipse(Brushes.Blue, null, screenRB, rSize, rSize);
                dc.DrawEllipse(Brushes.Blue, null, screenRC, rSize, rSize);
                dc.DrawEllipse(Brushes.Blue, null, screenRD, rSize, rSize);
            }

            var renderPoints = curve.GenerateRenderPoints();
            if (renderPoints.Count < 2) return;

            StreamGeometry geometry = new StreamGeometry();
            using (StreamGeometryContext context = geometry.Open())
            {
                Point firstScreenPoint = transform.ToScreen(renderPoints[0].X, renderPoints[0].Y);
                context.BeginFigure(firstScreenPoint, false, false);

                for (int i = 1; i < renderPoints.Count; i++)
                {
                    Point nextScreenPoint = transform.ToScreen(renderPoints[i].X, renderPoints[i].Y);
                    context.LineTo(nextScreenPoint, true, false);
                }
            }
            geometry.Freeze();

            Pen curvePen = new Pen(isSelected ? Brushes.Blue : Brushes.Black, isSelected ? 3 : 2);
            dc.DrawGeometry(null, curvePen, geometry);
        }
    }
}