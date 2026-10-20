using Lab3.Core;
using Lab3.Core.Geometry;
using System;
using System.Collections.Generic;
using Figure = Lab3.Core.Geometry.Figure;

namespace Lab3.Core.Figures
{
    public class ComplexFigure
    {
        public Part Part { get; private set; }
        public Figure Figure { get; private set; }

        private List<(double raX, double raY, double rbX, double rbY, double rcX, double rcY, double rdX, double rdY, double wa, double wb, double wc, double wd)> initialStates
            = new List<(double, double, double, double, double, double, double, double, double, double, double, double)>();
        private List<(double raX, double raY, double rbX, double rbY, double rcX, double rcY, double rdX, double rdY, double wa, double wb, double wc, double wd)> circleStates
            = new List<(double, double, double, double, double, double, double, double, double, double, double, double)>();

        public ComplexFigure()
        {
            Figure = new Figure();
            Part = new Part(Figure);
            Build();
        }

        public CurveObject BuildSegment(
            string name,
            Point2D rA, Point2D rB, Point2D rC, Point2D rD,
            double wA = 1.0, double wB = 1.0, double wC = 1.0, double wD = 1.0,
            int steps = 100)
        {
            void AddUniquePoint(Point2D p)
            {
                if (p != null && !Figure.Points.Contains(p))
                {
                    Figure.Points.Add(p);
                }
            }

            var segment = new CurveObject(name, rA, rB, rC, rD, wA, wB, wC, wD, steps);

            AddUniquePoint(segment.RA);
            AddUniquePoint(segment.RB);
            AddUniquePoint(segment.RC);
            AddUniquePoint(segment.RD);

            Figure.AddObject(segment);

            initialStates.Add((rA.X, rA.Y, rB.X, rB.Y, rC.X, rC.Y, rD.X, rD.Y, wA, wB, wC, wD));

            return segment;
        }

        public Figure Build()
        {

            Point2D p0 = new Point2D(8, 0);
            Point2D p1 = new Point2D(0, 0);
            Point2D p2 = new Point2D(0, 0.1);
            Point2D p3 = new Point2D(8, 0.1);
            BuildSegment("Seg1", p0, p1, p2, p3, 1.0, 1.2, 1.2, 1.0);

            Point2D p4 = new Point2D(8, -1);
            Point2D p5 = new Point2D(8, -3);
            Point2D p6 = new Point2D(10, -7);
            BuildSegment("Seg2", p0, p4, p5, p6, 1.0, 0.8, 0.8, 1.0);

            Point2D p7 = new Point2D(8, 10);
            Point2D p8 = new Point2D(15, 13);
            Point2D p9 = new Point2D(21, 6);
            BuildSegment("Seg3", p3, p7, p8, p9, 1.0, 1.0, 0.4, 1.0);

            Point2D p10 = new Point2D(22, 4);
            Point2D p11 = new Point2D(22, 5);
            Point2D p12 = new Point2D(23, 4);
            BuildSegment("Seg4", p9, p10, p11, p12, 1.0, 0.2, 0.2, 1.0);

            Point2D p13 = new Point2D(24, 6);
            Point2D p14 = new Point2D(28, 4);
            Point2D p15 = new Point2D(29, 3);
            BuildSegment("Seg5", p12, p13, p14, p15, 1.0, 0.25, 0.25, 1.0);

            Point2D p16 = new Point2D(26, 1);
            Point2D p17 = new Point2D(22, 1);
            Point2D p18 = new Point2D(20, 2);
            BuildSegment("Seg6", p15, p16, p17, p18, 1.0, 0.5, 0.25, 1.0);

            Point2D p19 = new Point2D(15, 2);
            Point2D p20 = new Point2D(15, -4);
            Point2D p21 = new Point2D(22, -8);
            BuildSegment("Seg7", p18, p19, p20, p21, 1.0, 0.5, 0.5, 1.0);

            Point2D p22 = new Point2D(23, -10);
            Point2D p23 = new Point2D(25, -12);
            Point2D p24 = new Point2D(24, -13);
            BuildSegment("Seg8", p21, p22, p23, p24, 1.0, 0.2, 0.8, 1.0);

            Point2D p25 = new Point2D(23, -9);
            Point2D p26 = new Point2D(21, -7);
            Point2D p27 = new Point2D(20, -7);
            BuildSegment("Seg9", p24, p25, p26, p27, 1.0, 0.7, 0.2, 1.0);

            Point2D p28 = new Point2D(17, -5);
            Point2D p29 = new Point2D(16, 1);
            Point2D p30 = new Point2D(18, 2);
            BuildSegment("Seg10", p27, p28, p29, p30, 1.0, 0.7, 0.7, 1.0);

            Point2D p31 = new Point2D(18, 3);
            Point2D p32 = new Point2D(19, 2);
            Point2D p33 = new Point2D(20, 3);
            BuildSegment("Seg11", p30, p31, p32, p33, 1.0, 0.2, 0.7, 1.0);

            Point2D p34 = new Point2D(17, 3);
            Point2D p35 = new Point2D(15, 5);
            Point2D p36 = new Point2D(15, 6);
            BuildSegment("Seg12", p33, p34, p35, p36, 1.0, 0.2, 0.7, 1.0);

            Point2D p37 = new Point2D(14, 6);
            Point2D p38 = new Point2D(14, 5.5);
            Point2D p39 = new Point2D(14.7, 5.1);
            BuildSegment("Seg13", p36, p37, p38, p39, 1.0, 0.2, 0.7, 1.0);

            Point2D p40 = new Point2D(13, 6);
            Point2D p41 = new Point2D(12, 6);
            Point2D p42 = new Point2D(10, 5);
            BuildSegment("Seg14", p39, p40, p41, p42, 1.0, 0.4, 0.5, 1.0);

            Point2D p43 = new Point2D(10, 6);
            Point2D p44 = new Point2D(11, 6);
            Point2D p45 = new Point2D(13, 5.7);
            BuildSegment("Seg15", p42, p43, p44, p45, 1.0, 0.2, 0.2, 1.0);

            Point2D p46 = new Point2D(14.1, 5.7);
            Point2D p47 = new Point2D(14.3, 6.2);
            Point2D p48 = new Point2D(13.3, 6.5);
            BuildSegment("Seg16", p45, p46, p47, p48, 1.0, 0.2, 0.2, 1.0);

            Point2D p49 = new Point2D(14, 7);
            Point2D p50 = new Point2D(15, 6.8);
            Point2D p51 = new Point2D(15.1, 6.1);
            BuildSegment("Seg17", p48, p49, p50, p51, 1.0, 0.2, 0.2, 1.0);

            Point2D p52 = new Point2D(15, 4);
            Point2D p53 = new Point2D(18, 4);
            Point2D p54 = new Point2D(20, 3.2);
            BuildSegment("Seg18", p51, p52, p53, p54, 1.0, 0.2, 0.2, 1.0);

            Point2D p55 = new Point2D(21, 4);
            Point2D p56 = new Point2D(22, 4);
            Point2D p57 = new Point2D(20.8, 6);
            BuildSegment("Seg19", p54, p55, p56, p57, 1.0, 0.2, 0.6, 1.0);

            Point2D p58 = new Point2D(11, 13);
            Point2D p59 = new Point2D(4, 9);
            BuildSegment("Seg20", p57, p58, p59, p6, 1.0, 0.6, 0.6, 1.0);

            Part = new Part(Figure);

            int totalSegments = 20;
            double radius = 20.0;
            double theta = 2.0 * Math.PI / totalSegments;
            double controlDist = (4.0 / 3.0) * radius * Math.Tan(theta / 4.0);
            double circleWeight = Math.Cos(theta / 2.0);

            for (int i = 0; i < totalSegments; i++)
            {
                double angle1 = i * theta;
                double angle2 = (i + 1) * theta;

                double cAx = radius * Math.Cos(angle1);
                double cAy = radius * Math.Sin(angle1);
                double cDx = radius * Math.Cos(angle2);
                double cDy = radius * Math.Sin(angle2);

                double cBx = cAx - controlDist * Math.Sin(angle1);
                double cBy = cAy + controlDist * Math.Cos(angle1);
                double cCx = cDx + controlDist * Math.Sin(angle2);
                double cCy = cDy - controlDist * Math.Cos(angle2);

                circleStates.Add((cAx, cAy, cBx, cBy, cCx, cCy, cDx, cDy, 1.0, circleWeight, circleWeight, 1.0));
            }

            return Figure;
        }

        public void UpdateMorph(double progress)
        {
            for (int i = 0; i < Figure.GeometryObjects.Count; i++)
            {
                if (Figure.GeometryObjects[i] is CurveObject curve && i < circleStates.Count)
                {
                    var init = initialStates[i];
                    var target = circleStates[i];

                    curve.RA.X = init.raX + (target.raX - init.raX) * progress;
                    curve.RA.Y = init.raY + (target.raY - init.raY) * progress;

                    curve.RB.X = init.rbX + (target.rbX - init.rbX) * progress;
                    curve.RB.Y = init.rbY + (target.rbY - init.rbY) * progress;

                    curve.RC.X = init.rcX + (target.rcX - init.rcX) * progress;
                    curve.RC.Y = init.rcY + (target.rcY - init.rcY) * progress;

                    curve.RD.X = init.rdX + (target.rdX - init.rdX) * progress;
                    curve.RD.Y = init.rdY + (target.rdY - init.rdY) * progress;

                    curve.WA = init.wa + (target.wa - init.wa) * progress;
                    curve.WB = init.wb + (target.wb - init.wb) * progress;
                    curve.WC = init.wc + (target.wc - init.wc) * progress;
                    curve.WD = init.wd + (target.wd - init.wd) * progress;
                }
            }
        }
    }
}