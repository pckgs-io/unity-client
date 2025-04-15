using System;

namespace Pckgs
{
    public class ApiException : Exception
    {
        public ProblemDefinition Problem { get; private set; }
        public ApiException(ProblemDefinition problem) : base(problem.Detail)
        {
            Problem = problem;
        }
    }
}