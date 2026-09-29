using Lab2.Figures;
using Lab2.UI;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace Lab2
{
    public partial class MainWindow : Window
    {

        private bool isAnimating = false;
        private double animationTime = 0;
        private readonly double baseA = 50.0;
        public MainWindow()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                UpdateViewControls();
                UpdateCharacteristics();

            };
        }

        private void UpdateViewControls()
        {
            if (DrawingCanvas?.Transform == null) return;

            ScaleValueTextBox.Text = DrawingCanvas.Transform.Scale.ToString("0.##");
            OffsetXTextBox.Text = DrawingCanvas.Transform.OffsetX.ToString("0.##");
            OffsetYTextBox.Text = DrawingCanvas.Transform.OffsetY.ToString("0.##");

            if (ScaleStatusTextBlock != null)
                ScaleStatusTextBlock.Text = $"Scale: {DrawingCanvas.Transform.Scale:0.#} px/unit";
        }

        #region View and Coordinate System Handlers (View Controls)

        private void SetScaleButton_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(ScaleValueTextBox.Text, out double scale) && scale > 0)
            {
                DrawingCanvas.Transform.Scale = scale;
                DrawingCanvas.InvalidateVisual();
                UpdateViewControls();
            }
            else
            {
                MessageBox.Show("Scale must be positive number.");
            }
        }

        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            DrawingCanvas.Transform.Zoom(1.2, DrawingCanvas.ActualWidth, DrawingCanvas.ActualHeight);
            DrawingCanvas.InvalidateVisual();
            UpdateViewControls();
        }

        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            DrawingCanvas.Transform.Zoom(1.0 / 1.2, DrawingCanvas.ActualWidth, DrawingCanvas.ActualHeight);
            DrawingCanvas.InvalidateVisual();
            UpdateViewControls();
        }

        private void ApplyOffsetButton_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(OffsetXTextBox.Text, out double ox) &&
                double.TryParse(OffsetYTextBox.Text, out double oy))
            {
                DrawingCanvas.Transform.OffsetX = ox;
                DrawingCanvas.Transform.OffsetY = oy;
                DrawingCanvas.InvalidateVisual();
                UpdateViewControls();
            }
            else
            {
                MessageBox.Show("Invalid offset values.");
            }
        }

        private void ResetViewButton_Click(object sender, RoutedEventArgs e)
        {
            DrawingCanvas.Transform.CenterOrigin(DrawingCanvas.ActualWidth, DrawingCanvas.ActualHeight);
            DrawingCanvas.Transform.Scale = 20;
            DrawingCanvas.InvalidateVisual();
            UpdateViewControls();
        }

        #endregion

        private void OnStrophoidParameterChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtA == null || TxtTMax == null || TxtSteps == null) return;
            if (DrawingCanvas?.Figure == null) return;

            bool isAParsed = double.TryParse(TxtA.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double a);
            bool isTMaxParsed = double.TryParse(TxtTMax.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double tMax);
            bool isStepsParsed = int.TryParse(TxtSteps.Text, out int steps);

            if (isAParsed && isTMaxParsed && isStepsParsed && steps > 0)
            {
                var oldStrophoid = DrawingCanvas.Figure.GeometryObjects
                    .OfType<Core.Geometry.Strophoid2D>()
                    .FirstOrDefault();

                if (oldStrophoid != null)
                {
                    oldStrophoid.A = a;
                    oldStrophoid.TMax = tMax;
                    oldStrophoid.Steps = steps;
                }
                else
                {
                    ComplexFigure complexFigure = new ComplexFigure();
                    DrawingCanvas.Figure = complexFigure.Build(a, tMax, steps);
                }

                DrawingCanvas.InvalidateVisual();
                UpdateCharacteristics();
            }
        }

        private void OnAnimationTick(object sender, EventArgs e)
        {
            if (!isAnimating) return;

            animationTime += 0.03;
            double newA = baseA + Math.Sin(animationTime) * 30.0;

            var strophoid = DrawingCanvas?.Figure?.GeometryObjects
                .OfType<Core.Geometry.Strophoid2D>()
                .FirstOrDefault();

            if (strophoid != null)
            {
                strophoid.A = newA;

                if (TxtA != null)
                {
                    TxtA.TextChanged -= OnStrophoidParameterChanged;
                    TxtA.Text = newA.ToString("F1", CultureInfo.InvariantCulture);
                    TxtA.TextChanged += OnStrophoidParameterChanged;
                }

                DrawingCanvas.InvalidateVisual();
                UpdateCharacteristics();
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            CompositionTarget.Rendering -= OnAnimationTick;
            base.OnClosed(e);
        }

        private void BtnToggleAnimation_Click(object sender, RoutedEventArgs e)
        {
            isAnimating = !isAnimating;

            if (isAnimating)
            {
                BtnToggleAnimation.Content = "Stop Animation";
                
                CompositionTarget.Rendering += OnAnimationTick;
            }
            else
            {
                BtnToggleAnimation.Content = "Start Animation";

                CompositionTarget.Rendering -= OnAnimationTick;
            }
        }

        private void SliderT_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (DrawingCanvas != null)
            {
                DrawingCanvas.CurrentT = e.NewValue;
                DrawingCanvas.InvalidateVisual();
                UpdateCharacteristics();
            }
        }

        private void ChkShowAsymptote_Click(object sender, RoutedEventArgs e)
        {
            if (DrawingCanvas != null)
            {
                DrawingCanvas.ShowAsymptote = ChkShowAsymptote.IsChecked ?? true;
                DrawingCanvas.InvalidateVisual();
            }
        }

        private void UpdateCharacteristics()
        {

            if (LblRadius == null || LblLoopLength == null || LblArcLength == null || LblLoopArea == null || LblAsymptoteArea == null || LblInflections == null)
                return;

            if (DrawingCanvas?.Figure == null) return;

            var strophoid = DrawingCanvas.Figure.GeometryObjects
                .OfType<Core.Geometry.Strophoid2D>()
                .FirstOrDefault();

            if (strophoid == null) return;

            double currentT = DrawingCanvas.CurrentT;

            double R = strophoid.GetRadiusOfCurvatureAt(currentT);
            LblRadius.Text = double.IsInfinity(R) ? "∞" : R.ToString("F2", CultureInfo.InvariantCulture);

            double loopLength = strophoid.GetLoopLength();
            LblLoopLength.Text = loopLength.ToString("F2", CultureInfo.InvariantCulture);

            double arcLength = strophoid.GetArcLength(-strophoid.TMax, strophoid.TMax);
            LblArcLength.Text = arcLength.ToString("F2", CultureInfo.InvariantCulture);

            double loopArea = strophoid.GetLoopArea();
            LblLoopArea.Text = loopArea.ToString("F2", CultureInfo.InvariantCulture);

            double asympArea = strophoid.GetAsymptoteArea();
            LblAsymptoteArea.Text = asympArea.ToString("F2", CultureInfo.InvariantCulture);

            LblInflections.Text = "None";
        }

        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (DrawingCanvas?.Part == null) return;

            double step = 5.0;       
            double angleStep = 5.0;  

            bool needRedraw = false;

            Point pivot = DrawingCanvas.PivotPoint;

            switch (e.Key)
            {

                case System.Windows.Input.Key.A:
                    DrawingCanvas.Part.Translate(-step, 0);
                    needRedraw = true;
                    break;
                case System.Windows.Input.Key.D:
                    DrawingCanvas.Part.Translate(step, 0);
                    needRedraw = true;
                    break;
                case System.Windows.Input.Key.W:
                    DrawingCanvas.Part.Translate(0, step);
                    needRedraw = true;
                    break;
                case System.Windows.Input.Key.S:
                    DrawingCanvas.Part.Translate(0, -step);
                    needRedraw = true;
                    break;

                case System.Windows.Input.Key.Q:
                    DrawingCanvas.Part.Rotate(pivot.X, pivot.Y, angleStep);
                    needRedraw = true;
                    break;
                case System.Windows.Input.Key.E:
                    DrawingCanvas.Part.Rotate(pivot.X, pivot.Y, -angleStep);
                    needRedraw = true;
                    break;
            }

            if (needRedraw)
            {
                DrawingCanvas.InvalidateVisual();
            }
        }

    }
}