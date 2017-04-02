# LINQ-Expression-Projection

## Project Description
LINQ Expression Projection library provides tool that enable using lambda expressions in LINQ projections (to anonymous and predefined types) even if the lambda expression is not defined in the query but rather is stored in a variable or retrieved via a function or property.

## Goal
This library is a tool for development of applications following the DRY principle. It is used to facilitate reuse LINQ expressions.
LINQ Expression Projection library provides tool that enable using lambda expressions in LINQ projections (to anonymous and predefined types) even if the lambda expression is not defined in the query but rather is stored in a variable or retrieved via a function or property.

## Usage
Refrence LinqExpressionProjection.dll and add using statement:
```
using LinqExpressionProjection.Test.Model;
```
Define your projection expression of type ``Expression<Func<TIn, TOut>>``:
```
private Expression<Func<Project, double>> averageAreaSelector = p => p.Subprojects.Average(p => p.Area);
```
In query, call ``AsExpressionProjectable()`` on collection and ``Project<TOut>()`` on the expression:
```
var projects = (from p in ctx.Projects.AsExpressionProjectable()
select new
{
  Project = p,
  AverageArea = averageAreaSelector.Project<double>()
}).ToArray();
```

Make sure the selection expression ``<TIn>`` is of the same type as the projection (``Select()`` call) lambda parameter
