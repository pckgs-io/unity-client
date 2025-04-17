using System;

namespace Pckgs
{
    public class HttpException : ApiException
    {
        public ProblemDefinition Problem { get; private set; }
        public HttpException(ProblemDefinition problem) : base(problem.Detail)
        {
            Problem = problem;
        }
    }
}