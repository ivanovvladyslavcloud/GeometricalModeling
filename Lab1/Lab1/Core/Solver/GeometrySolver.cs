using System.Collections.Generic;
using Lab1.Core.Constraints;

namespace Lab1.Core.Solver
{
    public class GeometrySolver
    {
        private readonly List<Constraint> constraints;

        public GeometrySolver()
        {
            constraints =
                new List<Constraint>();
        }

        public void AddConstraint(
            Constraint constraint)
        {
            constraints.Add(constraint);
        }

        public bool Solve()
        {
            foreach (
                Constraint constraint
                in constraints)
            {
                if (!constraint.Solve())
                {
                    return false;
                }
            }

            return true;
        }
    }
}