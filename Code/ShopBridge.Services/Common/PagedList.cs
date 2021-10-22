using GatePass.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ShopBridge.Services.Common
{
    /// <summary>
    /// Paged List should be Used on Service if Data is allways need to get from DataBase Directly.
    /// if Data need to be chached then Paged List should be used on Controller.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedList<T>
    {
        public List<T> List { get; set; }

        public int PageCount
        {
            get;
            set;
        }

        //public int PageIndex { get; set; }
        public int OffSet { get; set; }

        public int PageSize { get; set; }

        public int ItemCount { get; set; }

        public PagedList(IQueryable<T> list, BaseSearchVM searchParams)
        {
            PageSize = searchParams.PageSize > 0 ? searchParams.PageSize : 10;
            if (searchParams.SortDirections != null && searchParams.SortDirections.Length > 0)
            {
                for (int i = 0; i < searchParams.SortDirections.Length; i++)
                {
                    string methodname = i == 0 ? "OrderBy" : "ThenBy";
                    methodname += searchParams.SortDirections[i] == "desc" ? "Descending" : "";
                    if (!string.IsNullOrEmpty(searchParams.SortNames[i]))
                        list = ApplyOrder<T>(list, searchParams.SortNames[i], methodname);
                }
            }
            List = list.Skip(searchParams.OffSet).Take(PageSize).ToList();
            OffSet = searchParams.OffSet;
            PageSize = PageSize;
            PageCount = (int)Math.Ceiling((float)list.Count() / Convert.ToInt32(searchParams.PageSize));
            ItemCount = list.Count();
        }
        public IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
        {
            string[] props = property.Split('.');
            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (string prop in props)
            {
                // use reflection (not ComponentModel) to mirror LINQ
                PropertyInfo pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            object result = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                            && method.IsGenericMethodDefinition
                            && method.GetGenericArguments().Length == 2
                            && method.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), type)
                    .Invoke(null, new object[] { source, lambda });
            return (IOrderedQueryable<T>)result;
        }
    }
}
