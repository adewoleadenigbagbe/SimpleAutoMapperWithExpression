using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = new A { Id = 1, Name = "John" };
            var b = Mapper.Map<A, B>(a);

            Console.WriteLine(b.Id);
            Console.WriteLine(b.Name);

            Console.ReadKey();
        }

    }

    public class A
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class B
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Mapper
    {
        public static TTarget Map<TSource, TTarget>(TSource source)
        {
            var sourceType = typeof(TSource);
            var targetType = typeof(B);

            var targetTypeConstructor = targetType.GetConstructor(Type.EmptyTypes);
            var newExpression = Expression.New(targetTypeConstructor);

            var bindings = new List<MemberBinding>();
            var parameterExpression = Expression.Parameter(sourceType);

            foreach (var sourcePropertyInfo in sourceType.GetProperties())
            {
                var targetProperty = targetType.GetProperty(sourcePropertyInfo.Name);
                if (targetProperty == null)
                {
                    continue;
                }

                var propertyExpression = Expression.MakeMemberAccess(parameterExpression, sourcePropertyInfo);
                var memberAssignment = Expression.Bind(targetProperty, propertyExpression);
                bindings.Add(memberAssignment);
            }

            var body = Expression.MemberInit(newExpression, bindings);
            var obj = Expression.Lambda(body, false, parameterExpression).Compile().DynamicInvoke(source);

            return (TTarget)obj;
        }

    }
}
