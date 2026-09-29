using Lab2.Core;
using Lab2.Core.Geometry;
using Lab2.Figures;
using Lab2.Rendering;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Lab2.UI
{
    public class CoordinateCanvas : Canvas
    {
        public Transform2D Transform { get; }
        public Figure Figure { get; set; }
        public Part Part { get; private set; }

        private readonly CoordinateSystemRenderer coordinateRenderer;
        private readonly StrophoidRenderer strophoidRenderer;

        private Point lastMousePosition;
        private bool isPanning = false;
        private bool isDraggingPoint = false;

        public double CurrentT { get; set; } = 0.5;
        public bool ShowTangents { get; set; } = true;
        public bool ShowAsymptote { get; set; } = true;
        public Point PivotPoint { get; set; } = new Point(0, 0);
        public bool ShowPivotPoint { get; set; } = true;

        public CoordinateCanvas()
        {
            Transform = new Transform2D();

            coordinateRenderer = new CoordinateSystemRenderer(Transform);
            strophoidRenderer = new StrophoidRenderer();

            ComplexFigure complexFigure = new ComplexFigure();
            Figure = complexFigure.Build();
            Part = complexFigure.Part;

            Background = Brushes.White;
            ClipToBounds = true;
        }

        #region Mouse Handling

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            Point mousePos = e.GetPosition(this);

            if (e.LeftButton == MouseButtonState.Pressed && Figure != null)
            {
                Strophoid2D strophoid = Figure.GeometryObjects.OfType<Strophoid2D>().FirstOrDefault();
                if (strophoid != null)
                {
                    Point pWorld = strophoid.GetPointAt(CurrentT);
                    Point pScreen = Transform.ToScreen(pWorld.X, pWorld.Y);

                    if ((mousePos - pScreen).Length < 15)
                    {
                        isDraggingPoint = true;
                        CaptureMouse();
                        return;
                    }
                }
            }

            if (e.ChangedButton == MouseButton.Middle || e.ChangedButton == MouseButton.Left)
            {
                isPanning = true;
                lastMousePosition = mousePos;
                CaptureMouse();
                Cursor = Cursors.SizeAll;
            }

            if (e.ChangedButton == MouseButton.Right)
            {
                Point mouseScreen = e.GetPosition(this);

                double worldX = (mouseScreen.X - Transform.OffsetX) / Transform.Scale;
                double worldY = (Transform.OffsetY - mouseScreen.Y) / Transform.Scale;

                PivotPoint = new Point(worldX, worldY);
                InvalidateVisual();
                return;
            }
        }



        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Point currentMousePosition = e.GetPosition(this);

            if (isPanning)
            {
                Vector delta = currentMousePosition - lastMousePosition;

                Transform.OffsetX += delta.X;
                Transform.OffsetY += delta.Y;

                lastMousePosition = currentMousePosition;
                InvalidateVisual();
                return;
            }

            if (isDraggingPoint && Figure != null)
            {
                Strophoid2D strophoid = Figure.GeometryObjects.OfType<Strophoid2D>().FirstOrDefault();
                if (strophoid != null)
                {

                    double bestT = CurrentT;
                    double minDstSq = double.MaxValue;

                    double dt = (2 * strophoid.TMax) / strophoid.Steps;
                    for (int i = 0; i <= strophoid.Steps; i++)
                    {
                        double t = -strophoid.TMax + i * dt;
                        Point pWorld = strophoid.GetPointAt(t);
                        Point pScreen = Transform.ToScreen(pWorld.X, pWorld.Y);

                        double dstSq = (pScreen.X - currentMousePosition.X) * (pScreen.X - currentMousePosition.X) +
                                       (pScreen.Y - currentMousePosition.Y) * (pScreen.Y - currentMousePosition.Y);

                        if (dstSq < minDstSq)
                        {
                            minDstSq = dstSq;
                            bestT = t;
                        }
                    }

                    CurrentT = bestT;
                    InvalidateVisual();
                }
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (isPanning || isDraggingPoint)
            {
                isPanning = false;
                isDraggingPoint = false;
                ReleaseMouseCapture();
                Cursor = Cursors.Arrow;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            double zoomFactor = e.Delta > 0 ? 1.1 : 0.9;
            Point mousePos = e.GetPosition(this);

            Transform.Scale *= zoomFactor;
            Transform.OffsetX = mousePos.X - (mousePos.X - Transform.OffsetX) * zoomFactor;
            Transform.OffsetY = mousePos.Y - (mousePos.Y - Transform.OffsetY) * zoomFactor;

            InvalidateVisual();
        }

        #endregion

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            dc.PushClip(new RectangleGeometry(new Rect(0, 0, ActualWidth, ActualHeight)));

            try
            {
                coordinateRenderer.Draw(dc, ActualWidth, ActualHeight);

                if (Figure != null)
                {
                    foreach (GeometryObject geometryObject in Figure.GeometryObjects)
                    {
                        if (geometryObject is Strophoid2D strophoid)
                        {
                            strophoidRenderer.Draw(dc, strophoid, Transform, ShowAsymptote);

                            if (ShowTangents)
                            {
                                strophoidRenderer.DrawInteractiveElements(dc, strophoid, Transform, CurrentT);
                            }
                        }
                    }
                }

                if (ShowPivotPoint)
                {
                    Point pivotScreen = Transform.ToScreen(PivotPoint.X, PivotPoint.Y);

                    Pen pivotPen = new Pen(Brushes.Magenta, 1.5);
                    dc.DrawEllipse(null, pivotPen, pivotScreen, 6, 6);
                    dc.DrawLine(pivotPen, new Point(pivotScreen.X - 9, pivotScreen.Y), new Point(pivotScreen.X + 9, pivotScreen.Y));
                    dc.DrawLine(pivotPen, new Point(pivotScreen.X, pivotScreen.Y - 9), new Point(pivotScreen.X, pivotScreen.Y + 9));
                }
            }
            finally
            {
                dc.Pop();
            }
        }
    }
}