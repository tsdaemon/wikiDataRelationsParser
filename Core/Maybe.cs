using System;

namespace Core
{
    public static class Maybe
    {
        public static TResult With<TInput, TResult>(this TInput o,
       Func<TInput, TResult> evaluator)
            where TResult : class
            where TInput : class
        {
            if (o == null) return null;
            return evaluator(o);
        }
    }
}
