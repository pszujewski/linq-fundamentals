using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Cars
{
    class Program
    {
        static void Main(string[] args)
        {
            CarsConverter converter = new CarsConverter();

            var cars = ProcessFile("fuel.csv");
            var manufacturers = ProcessManufacturers("manufacturers.csv");

            converter.InsertAndQueryData(cars);

            GroupJoinCarsIntoManufacturers(cars, manufacturers);
            FindMostFuelEfficientCarsByCountry(cars, manufacturers);

            // Grouping
            var carsByManufacturer =
                from car in cars
                group car by car.Manufacturer.ToUpper() into manufacturer
                orderby manufacturer.Key // By default here, will order in ascending alphabetical order
                select manufacturer;

            // Same query just in method syntax
            var carsByManufacturer2 =
                cars.GroupBy(c => c.Manufacturer.ToUpper())
                    .OrderBy(g => g.Key); // the key being the manufacturer name

            foreach (var carGroup in carsByManufacturer)
            {
                Console.WriteLine("");
                Console.WriteLine($"{carGroup.Key} group"); // Key is the value I grouped by (i.e Kia)
                Console.WriteLine($"{carGroup.Key} has {carGroup.Count()} cars.");
                foreach (var car in carGroup.OrderByDescending(c => c.Combined).Take(2))
                {
                    Console.WriteLine($"\t{car.Name} : {car.Combined}");
                }
            }

            var querySyntaxExample =
                from car in cars
                join manufacturer in manufacturers
                    on new { car.Manufacturer, car.Year }
                        equals
                        new { Manufacturer = manufacturer.Name, manufacturer.Year }
                orderby car.Combined descending, car.Name ascending
                select new
                {
                    manufacturer.Headquarters,
                    car.Name,
                    car.Combined
                };

            // join 'manufacturers' to 'cars' where c.Manufacturer == m.Name
            // now we are joining on a composite operator.
            // and select new anonomyous object projection from the join
            var query = 
                cars.Join(manufacturers, 
                            c => new { c.Manufacturer, c.Year }, // defines how to match a given car to a manufacturer
                            m => new { Manufacturer = m.Name, m.Year }, // property names must match
                            (c, m) => new
                            {
                                m.Headquarters,
                                c.Name,
                                c.Combined,
                            })
                    .OrderByDescending(c => c.Combined)
                    .ThenBy(c => c.Name);

            var hasFord = cars.Any(c => c.Manufacturer == "Ford");

            foreach (var carSummary in query.Take(10))
            {
                Console.WriteLine($"{carSummary.Name} in {carSummary.Headquarters}: {carSummary.Combined}");
                
            }

            Console.WriteLine($"Does the list have any Ford cars? {hasFord}");
        }

        private static List<Manufacturer> ProcessManufacturers(string path)
        {
            return File.ReadAllLines(path)
                .Where(l => l.Length > 1)
                .Select(l =>
                {
                    var columns = l.Split(',');
                    return new Manufacturer
                    {
                        Name = columns[0],
                        Headquarters = columns[1],
                        Year = int.Parse(columns[2]),
                    };
                })
                .ToList();
        }

        private static List<Car> ProcessFile(string path)
        {
            return File.ReadAllLines(path)
                .Skip(1)
                .Where(line => line.Length > 1)
                .Select(Car.ParseFromCsv)
                .ToList();
        }

        public static void GroupJoinCarsIntoManufacturers(List<Car> cars, List<Manufacturer> manufacturers)
        {
            var query =
                from manufacturer in manufacturers
                join car in cars on manufacturer.Name equals car.Manufacturer
                    into carGroup
                select new
                {
                    Manufacturer = manufacturer,
                    Cars = carGroup
                };

            foreach (var group in query)
            {
                Console.WriteLine($"{group.Manufacturer.Name}: {group.Manufacturer.Headquarters}");
                foreach (var car in group.Cars.OrderByDescending(c => c.Combined).Take(2))
                {
                    Console.WriteLine($"\t{car.Name}: {car.Combined}");
                }
            }

            Console.WriteLine($"");
            Console.WriteLine($"===============================================================");
            Console.WriteLine($"");
        }

        public static void FindMostFuelEfficientCarsByCountry(List<Car> cars, List<Manufacturer> manufacturers)
        {
            // Use a regular 'join' to stictch together a manufacturer with a car and then group those results
            // and use ordering and Take.
            var carGroups =
                manufacturers.Join(cars,
                                   m => m.Name,
                                   c => c.Manufacturer,
                                   (m, c) => new
                                   {
                                       Country = m.Headquarters,
                                       Car = c
                                   })
                               .GroupBy(c => c.Country)
                               .OrderBy(g => g.Key);

            foreach (var carGroup in carGroups)
            {
                Console.WriteLine($"Country: {carGroup.Key}");
                foreach (var container in carGroup.OrderByDescending(cn => cn.Car.Combined).Take(3))
                {
                    Console.WriteLine($"\t{container.Car.Name}: {container.Car.Combined}");
                }
            }
        }
    }

    public static class CarExtensions
    {
        public static IEnumerable<Car> ToCar(this IEnumerable<string> source)
        {
            foreach (var line in source)
            {
                var columns = line.Split(",");

                yield return new Car
                {
                    Year = int.Parse(columns[0]),
                    Manufacturer = columns[1],
                    Name = columns[2],
                    Displacement = double.Parse(columns[3]),
                    Cylinders = int.Parse(columns[4]),
                    City = int.Parse(columns[5]),
                    Highway = int.Parse(columns[6]),
                    Combined = int.Parse(columns[7]),
                };
            }
        }
    }
}
