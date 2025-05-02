using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.Helpers
{
    public static class ExtentionMethods
    {
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> Source, bool Condition, Expression<Func<T, bool>> predicate)
            => Condition ? Source.Where(predicate) : Source;
    }
}
