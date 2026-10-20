using Lab3.Core.Figures;
using Lab3.Core.Geometry;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;



namespace Lab3
{
    public partial class MainWindow : Window
    {
        private bool isAnimating = false;
        private double morphProgress = 0.0; 
        private bool isMorphingToCircle = true;
        private System.Windows.Threading.DispatcherTimer morphTimer;
        private ComplexFigure complexFigureInstance;
        public MainWindow()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                UpdateViewControls();
                DrawingCanvas.OnCurveSelected += Canvas_OnCurveSelected;

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

        private void ShowPolygonCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (DrawingCanvas != null && ShowPolygonCheckBox != null)
            {
                DrawingCanvas.ShowControlPolygon = ShowPolygonCheckBox.IsChecked ?? true;
                DrawingCanvas.InvalidateVisual();
            }
        }

        private bool isUpdatingUI = false;

        private void Canvas_OnCurveSelected(CurveObject curve)
        {
            isUpdatingUI = true;
            if (curve != null)
            {
                WeightsPanel.IsEnabled = true;
                WaTextBox.Text = curve.WA.ToString("0.##", CultureInfo.InvariantCulture);
                WbTextBox.Text = curve.WB.ToString("0.##", CultureInfo.InvariantCulture);
                WcTextBox.Text = curve.WC.ToString("0.##", CultureInfo.InvariantCulture);
                WdTextBox.Text = curve.WD.ToString("0.##", CultureInfo.InvariantCulture);
            }
            else
            {
                WeightsPanel.IsEnabled = false;
            }
            isUpdatingUI = false;
        }

        private void WeightTextBox_Changed(object sender, TextChangedEventArgs e)
        {
            if (isUpdatingUI || DrawingCanvas?.SelectedCurve == null) return;

            var curve = DrawingCanvas.SelectedCurve;

            if (double.TryParse(WaTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double wa)) curve.WA = wa;
            if (double.TryParse(WbTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double wb)) curve.WB = wb;
            if (double.TryParse(WcTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double wc)) curve.WC = wc;
            if (double.TryParse(WdTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double wd)) curve.WD = wd;

            DrawingCanvas.InvalidateVisual();
        }

        private void MorphButton_Click(object sender, RoutedEventArgs e)
        {
            if (isAnimating) return;
            isAnimating = true;

            double startProgress = isMorphingToCircle ? 0.0 : 1.0;
            double targetProgress = isMorphingToCircle ? 1.0 : 0.0;

            double durationSec = 1.0;
            DateTime startTime = DateTime.Now;

            CompositionTarget.Rendering += OnRenderingFrame;

            void OnRenderingFrame(object senderObj, EventArgs args)
            {
                double elapsed = (DateTime.Now - startTime).TotalSeconds;
                double t = elapsed / durationSec;

                if (t >= 1.0)
                {
                    t = 1.0;
                    CompositionTarget.Rendering -= OnRenderingFrame;
                    isAnimating = false;

                    isMorphingToCircle = !isMorphingToCircle;
                    MorphButton.Content = isMorphingToCircle ? "Morph to Circle" : "Morph to Contour";
                }

                double smoothT = t * t * (3.0 - 2.0 * t);

                morphProgress = startProgress + (targetProgress - startProgress) * smoothT;

                if (DrawingCanvas != null)
                {
                    UpdateCanvasMorph(morphProgress);
                }
            }
        }

        private void UpdateCanvasMorph(double progress)
        {
            DrawingCanvas?.UpdateMorph(progress);
            DrawingCanvas?.InvalidateVisual();
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