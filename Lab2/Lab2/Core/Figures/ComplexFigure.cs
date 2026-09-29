using Lab2.Core;
using Lab2.Core.Geometry;

namespace Lab2.Figures
{
    public class ComplexFigure
    {
        public Part Part { get; private set; }
        public Figure Build(double a = 50.0, double tMax = 5.0, int steps = 300)
        {
            Figure figure = new Figure();

            Strophoid2D strophoid = new Strophoid2D(a: a, tMax: tMax, steps: steps);

            figure.AddObject(strophoid);

            Part = new Part(figure);

            return figure;
        }
    }
}