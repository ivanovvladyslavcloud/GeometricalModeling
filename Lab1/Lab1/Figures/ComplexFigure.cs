using Lab1.Core;
using Lab1.Core.Constraints;
using Lab1.Core.Geometry;
using Lab1.Core.Instrsections;
using System;
using System.Windows.Shapes;

namespace Lab1.Figures
{
    public class ComplexFigure
    {

        private T SetName<T>(
        T geometryObject,
        string name)
        where T : GeometryObject
                {
                    geometryObject.Name = name;

                    return geometryObject;
                }

        public Point2D A = new Point2D(4, 7.5);

        public Point2D B = new Point2D(-1.82, 7.5);

        public Point2D C = new Point2D(9, 1);

        public Point2D D = new Point2D(9, 14);

        public Point2D E = new Point2D(12, 1);

        public Point2D F = new Point2D(12, 14);

        public Point2D G = new Point2D(9, 11.5);

        public Point2D H = new Point2D(9, 3.5);

        public Point2D I = new Point2D(9, 9.5);

        public Point2D J = new Point2D(12, 9.5);

        public Point2D K = new Point2D(9, 5.5);

        public Point2D L = new Point2D(12, 5.5);

        public Point2D M = new Point2D(7, 11.5);

        public Point2D N = new Point2D(7, 13);

        public Point2D O = new Point2D(7, 3.5);

        public Point2D P = new Point2D(7, 2);

        public Part Part { get; private set; }

        public Figure Build()
        {
            Figure figure =
                new Figure();

           

            figure.AddPoint(A);
            figure.AddPoint(B);
            figure.AddPoint(C);
            figure.AddPoint(D);
            figure.AddPoint(E);
            figure.AddPoint(F);
            figure.AddPoint(G);
            figure.AddPoint(H);
            figure.AddPoint(I);
            figure.AddPoint(J);
            figure.AddPoint(K);
            figure.AddPoint(L);
            figure.AddPoint(M);
            figure.AddPoint(N);
            figure.AddPoint(O);
            figure.AddPoint(P);

            Arc2D A1 = SetName(new Arc2D(A, 3), "A1");

            Arc2D A2 = SetName(new Arc2D(A, 2), "A2");

            Line2D BC = new Line2D(B, C);

            Line2D BD = new Line2D(B, D);

            Line2D CE = SetName(new Line2D(C, E), "CE");

            Line2D DF = SetName(new Line2D(D, F), "DF");

            Line2D EL = SetName(new Line2D(E, L), "EL");

            Line2D JL = SetName(new Line2D(J, L), "JL");

            Line2D JF = SetName(new Line2D(J, F), "JF");

            Line2D GI = SetName(new Line2D(G, I), "GI");

            Line2D IK = SetName(new Line2D(I, K), "IK");

            Line2D KH = SetName(new Line2D(K, H), "KH");

            Line2D IJ = SetName(new Line2D(I, J), "IJ");

            Line2D KL = SetName(new Line2D(K, L), "KL");

            Line2D MN = SetName(new Line2D(M, N), "MN");

            Line2D OP = SetName(new Line2D(O, P), "OP");

            Line2D MG = SetName(new Line2D(M, G), "MG");

            Line2D OH = SetName(new Line2D(O, H), "OH");

            Point2D S = Intersection2D.TrimLineByLine(MN, BD, TrimSide.End);

            Point2D T = Intersection2D.TrimLineByLine(OP, BC, TrimSide.End);

            Point2D Q = Intersection2D.TrimLineByCircle(BC, A1, TrimSide.End);

            Point2D R = Intersection2D.TrimLineByCircle(BD, A1, TrimSide.End);

            Line2D RS = SetName(new Line2D(R, S), "RT");

            Line2D SD = SetName(new Line2D(S, D), "SD");

            Line2D QT = SetName(new Line2D(Q, T), "QT");

            Line2D TC = SetName(new Line2D(T, C), "TC");

            TangentConstraint A1_RS = new TangentConstraint(A1, RS, R); 
            TangentConstraint A1_QT = new TangentConstraint(A1, QT, Q);

            figure.AddPoint(Q);
            figure.AddPoint(R);
            figure.AddPoint(S);
            figure.AddPoint(T);
            figure.AddObject(A1);
            figure.AddObject(A2);
            figure.AddObject(CE);
            figure.AddObject(DF);
            figure.AddObject(JF);
            figure.AddObject(EL);
            figure.AddObject(JL);
            figure.AddObject(GI);
            figure.AddObject(IK);
            figure.AddObject(KH);
            figure.AddObject(IJ);
            figure.AddObject(KL);
            figure.AddObject(MN);
            figure.AddObject(OP);
            figure.AddObject(MG);
            figure.AddObject(OH);
            figure.AddObject(RS);
            figure.AddObject(SD);
            figure.AddObject(QT);
            figure.AddObject(TC);
            figure.AddConstraint(A1_RS);
            figure.AddConstraint(A1_QT);

            Part = new Part(figure);

            return figure;
        }
    }
}