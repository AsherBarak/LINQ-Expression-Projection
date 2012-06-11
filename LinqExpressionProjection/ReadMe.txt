This assembly enables ruse of LINQ logic in projections.

To use, call extension method AsExpressionProjectable() on the collection queried, 
and when projecting call Project<TResult>() on a filed, method or any other code element
returning a selector of type Expression<Func<TParameter, TResult>>. TParameter must be
the type of the parameter in the projection lambda.

Exapmle:
         var projects = (from p in ctx.Projects.AsExpressionProjectable()
                                select new
                                {
                                    Project = p,
                                    AEA = GetProjectAverageEffectiveAreaSelector().Project<double>()
                                }).ToArray();