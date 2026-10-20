using Lab3.Core;
using Lab3.Core.Figures;
using Lab3.Core.Geometry;
using Lab3.Rendering;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Figure = Lab3.Core.Geometry.Figure;

namespace Lab3.UI
{
    public class CoordinateCanvas : Canvas
    {
        public Transform2D Transform { get; }
        public Figure Figure { get; set; }
        public Part Part { get; private set; }

        private readonly CoordinateSystemRenderer coordinateRenderer;
        private readonly PointRenderer pointRenderer;
        private readonly CurveObjectRenderer curveObjectRenderer;

        public bool ShowControlPolygon { get; set; } = true;
        public double CurrentT { get; set; } = 0.5;
        public bool ShowTangents { get; set; } = true;
        public bool ShowAsymptote { get; set; } = true;
        public Point PivotPoint { get; set; } = new Point(0, 0);
        public bool ShowPivotPoint { get; set; } = true;

        private Point lastMousePosition;
        private bool isPanning = false;
        private bool isDraggingPoint = false;

        private Point2D selectedPoint = null;
        private readonly double hitRadiusPixels = 10.0;

        public CurveObject SelectedCurve { get; private set; } = null;
        private readonly double curveHitTolerancePixels = 8.0;

        public event Action<CurveObject> OnCurveSelected;

        public ComplexFigure ComplexFigureRef { get; private set; }

        public CoordinateCanvas()
        {
            Transform = new Transform2D();

            coordinateRenderer = new CoordinateSystemRenderer(Transform);
            pointRenderer = new PointRenderer();
            curveObjectRenderer = new CurveObjectRenderer();

            ComplexFigureRef = new ComplexFigure();
            Figure = ComplexFigureRef.Figure;
            Part = ComplexFigureRef.Part;

            Background = Brushes.White;
            ClipToBounds = true;
        }

        public void UpdateMorph(double progress)
        {
            ComplexFigureRef?.UpdateMorph(progress);
        }

        #region Mouse Handling

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            Point mousePos = e.GetPosition(this);

            if (Figure != null)
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    selectedPoint = FindPointNearMouse(mousePos);
                    if (selectedPoint != null)
                    {
                        isDraggingPoint = true;
                        CaptureMouse();
                        Cursor = Cursors.Cross;
                        e.Handled = true;
                        return;
                    }
                }

                if (e.ChangedButton == MouseButton.Left)
                {
                    var clickedCurve = FindCurveNearMouse(mousePos);
                    if (clickedCurve != SelectedCurve)
                    {
                        SelectedCurve = clickedCurve;
                        InvalidateVisual();
                        OnCurveSelected?.Invoke(SelectedCurve);
                    }

                    if (SelectedCurve != null)
                    {
                        e.Handled = true;
                        return;
                    }
                }
            }

            if (e.ChangedButton == MouseButton.Right)
            {
                double worldX = (mousePos.X - Transform.OffsetX) / Transform.Scale;
                double worldY = (Transform.OffsetY - mousePos.Y) / Transform.Scale;

                PivotPoint = new Point(worldX, worldY);
                InvalidateVisual();
                e.Handled = true;
                return;
            }

            if (e.ChangedButton == MouseButton.Middle || e.ChangedButton == MouseButton.Left)
            {
                if (SelectedCurve != null)
                {
                    SelectedCurve = null;
                    InvalidateVisual();
                    OnCurveSelected?.Invoke(null);
                }

                isPanning = true;
                lastMousePosition = mousePos;
                CaptureMouse();
                Cursor = Cursors.SizeAll;
                e.Handled = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Point currentMousePosition = e.GetPosition(this);

            if (isDraggingPoint && selectedPoint != null)
            {
                double worldX = (currentMousePosition.X - Transform.OffsetX) / Transform.Scale;
                double worldY = (Transform.OffsetY - currentMousePosition.Y) / Transform.Scale;

                selectedPoint.X = worldX;
                selectedPoint.Y = worldY;

                InvalidateVisual();
                e.Handled = true;
                return;
            }

            if (isPanning)
            {
                Vector delta = currentMousePosition - lastMousePosition;

                Transform.OffsetX += delta.X;
                Transform.OffsetY += delta.Y;

                lastMousePosition = currentMousePosition;
                InvalidateVisual();
                e.Handled = true;
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

        #region Hit Testing Helpers

        private Point2D FindPointNearMouse(Point mousePos)
        {
            if (Figure?.Points == null) return null;

            foreach (var point in Figure.Points)
            {
                Point screenPt = Transform.ToScreen(point.X, point.Y);
                double dx = screenPt.X - mousePos.X;
                double dy = screenPt.Y - mousePos.Y;
                double distance = Math.Sqrt(dx * dx + dy * dy);

                if (distance <= hitRadiusPixels)
                {
                    return point;
                }
            }
            return null;
        }

        private CurveObject FindCurveNearMouse(Point mousePos)
        {
            if (Figure?.GeometryObjects == null) return null;

            foreach (var obj in Figure.GeometryObjects)
            {
                if (obj is CurveObject curve)
                {
                    var renderPoints = curve.GenerateRenderPoints();
                    foreach (var pt in renderPoints)
                    {
                        Point screenPt = Transform.ToScreen(pt.X, pt.Y);
                        double dx = screenPt.X - mousePos.X;
                        double dy = screenPt.Y - mousePos.Y;
                        if (Math.Sqrt(dx * dx + dy * dy) <= curveHitTolerancePixels)
                        {
                            return curve;
                        }
                    }
                }
            }
            return null;
        }

        #endregion

        #region Rendering

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
                        if (geometryObject is CurveObject curve)
                        {
                            bool isSelected = (curve == SelectedCurve);
                            curveObjectRenderer.Draw(dc, curve, Transform, ShowControlPolygon, isSelected);
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

        #endregion
    }
}