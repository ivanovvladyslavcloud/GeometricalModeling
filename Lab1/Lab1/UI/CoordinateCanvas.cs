using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Lab1.Core;
using Lab1.Core.Geometry;
using Lab1.Rendering;
using Lab1.Figures;

namespace Lab1.UI
{
    public class CoordinateCanvas : Canvas
    {
        public Transform2D Transform { get; }
        public Figure Figure { get; private set; }
        public Part Part { get; private set; }

        public bool IsProjectiveMode { get; set; } = false;

        public double M00 { get; set; } = 1; public double M01 { get; set; } = 0; public double M02 { get; set; } = 0;
        public double M10 { get; set; } = 0; public double M11 { get; set; } = 1; public double M12 { get; set; } = 0;
        public double M20 { get; set; } = 0; public double M21 { get; set; } = 0; public double M22 { get; set; } = 1;

        private readonly CoordinateSystemRenderer coordinateRenderer;
        private readonly LineRenderer lineRenderer;
        private readonly ArcRenderer arcRenderer;
        private Point lastMousePosition;
        private bool isDragging = false;

        public CoordinateCanvas()
        {
            Transform = new Transform2D();

            coordinateRenderer = new CoordinateSystemRenderer(Transform);
            lineRenderer = new LineRenderer();
            arcRenderer = new ArcRenderer();

            ComplexFigure complexFigure = new ComplexFigure();
            Figure = complexFigure.Build();
            Part = complexFigure.Part;

            Background = Brushes.White;
            ClipToBounds = true;
        }

        #region Mouse Handling (Interactive Offset and Scale)

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.ChangedButton == MouseButton.Middle || e.ChangedButton == MouseButton.Left)
            {
                isDragging = true;
                lastMousePosition = e.GetPosition(this);
                CaptureMouse();
                Cursor = Cursors.SizeAll;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (isDragging)
            {
                Point currentMousePosition = e.GetPosition(this);
                Vector delta = currentMousePosition - lastMousePosition;

                Transform.OffsetX += delta.X;
                Transform.OffsetY += delta.Y;

                lastMousePosition = currentMousePosition;

                InvalidateVisual();
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (isDragging && (e.ChangedButton == MouseButton.Middle || e.ChangedButton == MouseButton.Left))
            {
                isDragging = false;
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

        public void SetAffineMatrix(
            double a00, double a01, double a02,
            double a10, double a11, double a12)
        {
            IsProjectiveMode = false;
            M00 = a00; M01 = a01; M02 = a02;
            M10 = a10; M11 = a11; M12 = a12;
            M20 = 0; M21 = 0; M22 = 1;

            InvalidateVisual();
        }

        public void SetProjectiveMatrix(
            double h00, double h01, double h02,
            double h10, double h11, double h12,
            double h20, double h21, double h22)
        {
            IsProjectiveMode = true;
            M00 = h00; M01 = h01; M02 = h02;
            M10 = h10; M11 = h11; M12 = h12;
            M20 = h20; M21 = h21; M22 = h22;

            InvalidateVisual();
        }

        public void ResetTransform()
        {
            IsProjectiveMode = false;
            M00 = 1; M01 = 0; M02 = 0;
            M10 = 0; M11 = 1; M12 = 0;
            M20 = 0; M21 = 0; M22 = 1;

            Transform.OffsetX = ActualWidth / 2;
            Transform.OffsetY = ActualHeight / 2;
            Transform.Scale = 20.0;

            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            dc.PushClip(new RectangleGeometry(new Rect(0, 0, ActualWidth, ActualHeight)));

            try
            {
                if (IsProjectiveMode)
                {
                    coordinateRenderer.DrawProjective(
                        dc, ActualWidth, ActualHeight,
                        M00, M01, M02,
                        M10, M11, M12,
                        M20, M21, M22
                    );
                }
                else
                {
                    coordinateRenderer.DrawAffine(
                        dc, ActualWidth, ActualHeight,
                        M00, M01, M02,
                        M10, M11, M12
                    );
                }

                foreach (GeometryObject geometryObject in Figure.GeometryObjects)
                {
                    if (geometryObject is Line2D line)
                    {
                        if (IsProjectiveMode)
                        {
                            lineRenderer.DrawProjective(
                                dc, line, Transform,
                                M00, M01, M02,
                                M10, M11, M12,
                                M20, M21, M22
                            );
                        }
                        else
                        {
                            lineRenderer.DrawAffine(
                                dc, line, Transform,
                                M00, M01, M02,
                                M10, M11, M12
                            );
                        }
                        continue;
                    }

                    if (geometryObject is Arc2D arc)
                    {
                        if (IsProjectiveMode)
                        {
                            arcRenderer.DrawProjective(
                                dc, arc, Transform,
                                M00, M01, M02,
                                M10, M11, M12,
                                M20, M21, M22
                            );
                        }
                        else
                        {
                            arcRenderer.DrawAffine(
                                dc, arc, Transform,
                                M00, M01, M02,
                                M10, M11, M12
                            );
                        }
                    }
                }
            }
            finally
            {
                dc.Pop();
            }
        }
    }
}