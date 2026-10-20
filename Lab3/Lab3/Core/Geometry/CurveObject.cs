using Lab3.Core.Geometry;
using System;
using System.Collections.Generic;

namespace Lab3.Core.Geometry
{
    public class CurveObject : GeometryObject
    {
      
        public Point2D RA { get; set; }
        public Point2D RB { get; set; }
        public Point2D RC { get; set; }
        public Point2D RD { get; set; }
        public double WA { get; set; }
        public double WB { get; set; }
        public double WC { get; set; }
        public double WD { get; set; }

        public int Steps { get; set; } 

        public CurveObject(
            string name,
            Point2D rA, Point2D rB, Point2D rC, Point2D rD,
            double wA, double wB, double wC, double wD,
            int steps = 100)
        {
            Name = name;
            RA = rA;
            RB = rB;
            RC = rC;
            RD = rD;
            WA = wA;
            WB = wB;
            WC = wC;
            WD = wD;
            Steps = steps;
        }

        public Point2D Evaluate(double u)
        {
            double uInv = 1.0 - u; 
            double uInv2 = uInv * uInv;
            double uInv3 = uInv2 * uInv;

            double u2 = u * u;
            double u3 = u2 * u;

            double b0 = WA * uInv3;
            double b1 = 3.0 * WB * u * uInv2;
            double b2 = 3.0 * WC * u2 * uInv; 
            double b3 = WD * u3;

            double denominator = b0 + b1 + b2 + b3;

            if (Math.Abs(denominator) < 1e-12)
            {
                return new Point2D(RA.X, RA.Y); 
            }

            double numX = (b0 * RA.X) + (b1 * RB.X) + (b2 * RC.X) + (b3 * RD.X);
            double numY = (b0 * RA.Y) + (b1 * RB.Y) + (b2 * RC.Y) + (b3 * RD.Y);

            return new Point2D(numX / denominator, numY / denominator);
        }

        public List<Point2D> GenerateRenderPoints()
        {
            List<Point2D> points = new List<Point2D>();
            for (int i = 0; i <= Steps; i++)
            {
                double u = (double)i / Steps;
                points.Add(Evaluate(u));
            }
            return points;
        }

        public void LerpTo(
    Point2D targetRA, Point2D targetRB, Point2D targetRC, Point2D targetRD,
    double targetWA, double targetWB, double targetWC, double targetWD,
    double t) 
        {

            RA.X = RA.X + (targetRA.X - RA.X) * t;
            RA.Y = RA.Y + (targetRA.Y - RA.Y) * t;

            RB.X = RB.X + (targetRB.X - RB.X) * t;
            RB.Y = RB.Y + (targetRB.Y - RB.Y) * t;

            RC.X = RC.X + (targetRC.X - RC.X) * t;
            RC.Y = RC.Y + (targetRC.Y - RC.Y) * t;

            RD.X = RD.X + (targetRD.X - RD.X) * t;
            RD.Y = RD.Y + (targetRD.Y - RD.Y) * t;

            WA = WA + (targetWA - WA) * t;
            WB = WB + (targetWB - WB) * t;
            WC = WC + (targetWC - WC) * t;
            WD = WD + (targetWD - WD) * t;
        }
    }
}