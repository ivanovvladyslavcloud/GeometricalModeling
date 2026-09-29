using Lab1.Core;
using Lab1.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Lab1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                UpdateViewControls();
                BuildParameterPanel();
            };
        }

        private void TransformMode_Changed(object sender, RoutedEventArgs e)
        {
            if (DrawingCanvas == null) return;

            bool isProjective = ProjectiveModeRadioButton?.IsChecked == true;
            DrawingCanvas.IsProjectiveMode = isProjective;
            DrawingCanvas.InvalidateVisual();
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

        private void FitToViewButton_Click(object sender, RoutedEventArgs e)
        {
            DrawingCanvas.Transform.FitToView(
                DrawingCanvas.Figure.Points,
                DrawingCanvas.ActualWidth,
                DrawingCanvas.ActualHeight
            );
            DrawingCanvas.InvalidateVisual();
            UpdateViewControls();
        }

        #endregion

        private void BuildParameterPanel()
        {
            ParametersPanel.Children.Clear();

            foreach (GeometryObject geometryObject in DrawingCanvas.Figure.GeometryObjects)
            {
                AddGeometryParameterPanel(geometryObject);
            }
        }

        private void AddGeometryParameterPanel(GeometryObject geometryObject)
        {
            Border border = new Border
            {
                BorderBrush = System.Windows.Media.Brushes.LightGray,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(0, 0, 0, 10),
                Padding = new Thickness(10)
            };

            StackPanel panel = new StackPanel();

            TextBlock name = new TextBlock
            {
                Text = geometryObject.Name,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 8)
            };

            panel.Children.Add(name);

            if (geometryObject is Line2D line)
            {
                AddLineParameters(panel, line);
            }

            if (geometryObject is Arc2D arc)
            {
                AddArcParameters(panel, arc);
            }

            border.Child = panel;
            ParametersPanel.Children.Add(border);
        }

        private void AddLineParameters(StackPanel panel, Line2D line)
        {
            TextBlock label = new TextBlock { Text = "Length" };
            panel.Children.Add(label);

            StackPanel row = new StackPanel { Orientation = Orientation.Horizontal };
            TextBox textBox = new TextBox
            {
                Width = 100,
                Margin = new Thickness(0, 5, 5, 0),
                Text = line.Length.ToString("0.###")
            };

            Button button = new Button
            {
                Content = "Apply",
                Margin = new Thickness(5, 5, 0, 0),
                Height = 25
            };

            button.Click += delegate
            {
                if (!double.TryParse(textBox.Text, out double value) || value <= 0)
                {
                    MessageBox.Show("Length must be greater than zero.");
                    return;
                }

                DrawingCanvas.Figure.SetParameter(line, "Length", value);
                DrawingCanvas.Figure.Solve();
                DrawingCanvas.InvalidateVisual();
                textBox.Text = line.Length.ToString("0.###");
            };

            row.Children.Add(textBox);
            row.Children.Add(button);
            panel.Children.Add(row);
        }

        private void AddArcParameters(StackPanel panel, Arc2D arc)
        {
            AddRadiusControl(panel, arc, "RadiusX", arc.RadiusX, val => arc.RadiusX = val);
            AddRadiusControl(panel, arc, "RadiusY", arc.RadiusY, val => arc.RadiusY = val);
        }

        private void AddRadiusControl(
            StackPanel panel,
            Arc2D arc,
            string paramName,
            double currentValue,
            Action<double> updateAction)
        {
            TextBlock label = new TextBlock { Text = paramName };
            panel.Children.Add(label);

            StackPanel row = new StackPanel { Orientation = Orientation.Horizontal };
            TextBox textBox = new TextBox
            {
                Width = 100,
                Margin = new Thickness(0, 5, 5, 0),
                Text = currentValue.ToString("0.###")
            };

            Button button = new Button
            {
                Content = "Apply",
                Margin = new Thickness(5, 5, 0, 0),
                Height = 25
            };

            button.Click += delegate
            {
                if (!double.TryParse(textBox.Text, out double value) || value <= 0)
                {
                    MessageBox.Show($"{paramName} must be greater than zero.");
                    return;
                }

                DrawingCanvas.Figure.SetParameter(arc, paramName, value);
                DrawingCanvas.Figure.Solve();
                DrawingCanvas.InvalidateVisual();
            };

            row.Children.Add(textBox);
            row.Children.Add(button);
            panel.Children.Add(row);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            DrawingCanvas.Figure.Reset();
            DrawingCanvas.ResetTransform();
            DrawingCanvas.InvalidateVisual();
            BuildParameterPanel();
        }

        private void MoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(MoveXTextBox.Text, out double dx) ||
                !double.TryParse(MoveYTextBox.Text, out double dy))
            {
                MessageBox.Show("Invalid Move parameters.");
                return;
            }

            DrawingCanvas.Part.Translate(dx, dy);
            DrawingCanvas.InvalidateVisual();
        }

        private void RotateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(RotateXTextBox.Text, out double x) ||
                !double.TryParse(RotateYTextBox.Text, out double y) ||
                !double.TryParse(RotateAngleTextBox.Text, out double angle))
            {
                MessageBox.Show("Invalid Rotate parameters.");
                return;
            }

            DrawingCanvas.Part.Rotate(x, y, angle);
            DrawingCanvas.InvalidateVisual();
        }

        private void AffineButton_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(AffineA00TextBox.Text, out double a00) ||
                !double.TryParse(AffineA01TextBox.Text, out double a01) ||
                !double.TryParse(AffineA02TextBox.Text, out double a02) ||
                !double.TryParse(AffineA10TextBox.Text, out double a10) ||
                !double.TryParse(AffineA11TextBox.Text, out double a11) ||
                !double.TryParse(AffineA12TextBox.Text, out double a12))
            {
                MessageBox.Show("Invalid affine parameters.");
                return;
            }

            AffineModeRadioButton.IsChecked = true;
            DrawingCanvas.IsProjectiveMode = false;
            DrawingCanvas.SetAffineMatrix(a00, a01, a02, a10, a11, a12);
            DrawingCanvas.InvalidateVisual();
        }

        private Point2D GetPivotPoint()
        {
            double x0 = double.TryParse(PointX0TextBox.Text, out double x) ? x : 0;
            double y0 = double.TryParse(PointY0TextBox.Text, out double y) ? y : 0;
            return new Point2D(x0, y0);
        }

        private void SymmetryX_Click(object sender, RoutedEventArgs e)
        {
            Point2D p = GetPivotPoint();

            double a00 = -1, a01 = 0, a02 = 2 * p.X;
            double a10 = 0, a11 = 1, a12 = 0;

            ApplyAffineAndSyncUI(a00, a01, a02, a10, a11, a12);
        }

        private void SymmetryY_Click(object sender, RoutedEventArgs e)
        {
            Point2D p = GetPivotPoint();

            double a00 = 1, a01 = 0, a02 = 0;
            double a10 = 0, a11 = -1, a12 = 2 * p.Y;

            ApplyAffineAndSyncUI(a00, a01, a02, a10, a11, a12);
        }

        private void SymmetryXY_Click(object sender, RoutedEventArgs e)
        {
            Point2D p = GetPivotPoint();

            double a00 = -1, a01 = 0, a02 = 2 * p.X;
            double a10 = 0, a11 = -1, a12 = 2 * p.Y;

            ApplyAffineAndSyncUI(a00, a01, a02, a10, a11, a12);
        }

        private void Scale_Click(object sender, RoutedEventArgs e)
        {
            Point2D p = GetPivotPoint();
            double kx = double.TryParse(ScaleKxTextBox.Text, out double x) ? x : 1;
            double ky = double.TryParse(ScaleKyTextBox.Text, out double y) ? y : 1;

            double a00 = kx, a01 = 0, a02 = p.X * (1 - kx);
            double a10 = 0, a11 = ky, a12 = p.Y * (1 - ky);

            ApplyAffineAndSyncUI(a00, a01, a02, a10, a11, a12);
        }

        private void ApplyAffineAndSyncUI(double a00, double a01, double a02, double a10, double a11, double a12)
        {
            AffineModeRadioButton.IsChecked = true;
            DrawingCanvas.IsProjectiveMode = false;
            DrawingCanvas.SetAffineMatrix(a00, a01, a02, a10, a11, a12);

            if (AffineA00TextBox != null) AffineA00TextBox.Text = a00.ToString("G5");
            if (AffineA01TextBox != null) AffineA01TextBox.Text = a01.ToString("G5");
            if (AffineA02TextBox != null) AffineA02TextBox.Text = a02.ToString("G5");
            if (AffineA10TextBox != null) AffineA10TextBox.Text = a10.ToString("G5");
            if (AffineA11TextBox != null) AffineA11TextBox.Text = a11.ToString("G5");
            if (AffineA12TextBox != null) AffineA12TextBox.Text = a12.ToString("G5");

            DrawingCanvas.InvalidateVisual();
        }

        private void ProjectiveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(ProjH00TextBox.Text, out double h00) ||
                !double.TryParse(ProjH01TextBox.Text, out double h01) ||
                !double.TryParse(ProjH02TextBox.Text, out double h02) ||
                !double.TryParse(ProjH10TextBox.Text, out double h10) ||
                !double.TryParse(ProjH11TextBox.Text, out double h11) ||
                !double.TryParse(ProjH12TextBox.Text, out double h12) ||
                !double.TryParse(ProjH20TextBox.Text, out double h20) ||
                !double.TryParse(ProjH21TextBox.Text, out double h21) ||
                !double.TryParse(ProjH22TextBox.Text, out double h22))
            {
                MessageBox.Show("Invalid projective parameters.");
                return;
            }

            ProjectiveModeRadioButton.IsChecked = true;
            DrawingCanvas.IsProjectiveMode = true;
            DrawingCanvas.SetProjectiveMatrix(h00, h01, h02, h10, h11, h12, h20, h21, h22);

            DrawingCanvas.InvalidateVisual();
            BuildParameterPanel();
        }

    }
}