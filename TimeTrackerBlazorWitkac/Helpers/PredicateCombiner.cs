using System.Linq.Expressions;

namespace TimeTrackerBlazorWitkac.Helpers;

public static class PredicateCombiner
{
    public static Expression<Func<T, bool>> Combine<T>(params Expression<Func<T, bool>>[] predicates)
    {
        if (predicates.Length == 0)
        {
            return _ => true;
        }

        Expression<Func<T, bool>> predicateResult =  a => true;;

        foreach (var predicate in predicates)
        {
            predicateResult = CombinePredicates<T>(predicateResult, predicate);
        }

        return predicateResult;
    }

    private static Expression<Func<T, bool>> CombinePredicates<T>(
        Expression<Func<T, bool>> first,
        Expression<Func<T, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var body = Expression.AndAlso(
            Expression.Invoke(first, parameter),
            Expression.Invoke(second, parameter));

        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }
}