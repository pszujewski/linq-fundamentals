using System;
using System.Collections.Generic;
using System.Linq;

namespace Features
{
    class Program
    {
        static void Main(string[] args)
        {
            Func<int, int> square = x => x * x;
            Func<int, int, int> add = (x, y) => x + y;
            Func<int, int, int, int> add3 = (x, y, z) =>
            {
                return x + y + z;
            };

            Console.WriteLine(square(add(3, 5)));

            var developers = new Employee[]
            {
               new Employee { Id = 1, Name = "Scott" },
               new Employee { Id = 2, Name = "Chris" }
            };

           var sales = new List<Employee>()
           {
               new Employee { Id = 3, Name = "Alex" }
           };

            Console.WriteLine(developers.Count());

            // Essentially writes a foreach statement from scratch using IEnumerator
            // IEnumerator<Employee> enumerator = developers.GetEnumerator();
            // while (enumerator.MoveNext())
            // {
            //    Console.WriteLine(enumerator.Current.Name);
            // }

            foreach (var employee in developers.Where(e => e.Name.StartsWith("S")))
            {
                Console.WriteLine(employee.Name);
            }
        }
    }
}
