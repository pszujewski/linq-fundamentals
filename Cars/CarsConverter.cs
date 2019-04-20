using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Xml.Linq;

/*
    When you use Linq with the Entity Framework, the lambda functions that you 
    write to filter, order and operate on a given Table in a Database are not compiled
    into executable (invokable) assemblies, as are regular methods or when you query data in memory
    against the IEnumerable type. Instead, they are compiled into "Expressions." That describe the function 
    you wrote (in other words something parsable into an Abstract Syntax Tree). It is parsed into a data structure
    that describes the func's arguments, it's return type, operations contained within it, etc. This Expression
    Type can be translated into SQL commands that can be sent to SQL server (remote datasource).

    // for IEnumberable -> compiled into executable code that result in "in-memory" operations
    Func<int, int> square = x => x * x;

    // For IQueryable Expression -> Entity Framework can inspect this and translate it into SQL commands
    Expression<Func<int, int, int>> add = (x, y) => x + y;

    "add" cannot be invoked as can "square" but "add" can be parsed and analayzed.
*/

namespace Cars
{
    public class CarsConverter
    {
        public void InsertAndQueryData(List<Car> records)
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<CarDb>());
            InsertData(records);
            QueryData();
        }

        private void QueryData()
        {
            var db = new CarDb();

            db.Database.Log = Console.WriteLine;

            // db.Cars is of type "DbSet" so it implements IQueryable and uses
            // Entity Framework. The lambdas here are "Expressions" of functions
            var query =
                db.Cars.Where(c => c.Manufacturer == "BMW")
                       .OrderByDescending(c => c.Combined)
                       .ThenBy(c => c.Name)
                       .Take(10)
                       .ToList(); // Executes and returns IEnumerable

            foreach (var car in query)
            {
                Console.WriteLine($"{car.Name}: {car.Combined}");
            }
        }

        private void InsertData(List<Car> records)
        {
            var db = new CarDb();

            if (!db.Cars.Any())
            {
                foreach (var car in records)
                {
                    db.Cars.Add(car);
                }
                db.SaveChanges();
            }
        }

        public void ToXML(List<Car> records)
        {
            CreateXML(records);
            QueryXML();
        }

        private void QueryXML()
        {
            // Any time you see IEnumerable, you can use Linq extension methods
            var document = XDocument.Load("fuel.xml");
            var query =
                document
                    .Element("Cars")
                    .Elements("Car")
                    .Where(el => el.Attribute("Manufacturer").Value == "BMW")
                    .Select(el => el.Attribute("Name")); // Just get the Name

            foreach (var name in query)
            {
                Console.WriteLine(name);
            }
        }

        private static void CreateXML(List<Car> records)
        {
            var document = new XDocument();
            var cars = new XElement("Cars",
                from record in records
                select new XElement("Car",
                                     new XAttribute("Name", record.Name),
                                     new XAttribute("Combined", record.Combined),
                                     new XAttribute("Manufacturer", record.Manufacturer))
            );

            document.Add(cars);
            document.Save("fuel.xml");
        }

        private void AsElements(List<Car> records)
        {
            var document = new XDocument();
            var cars = new XElement("Cars");

            foreach (var record in records)
            {
                // These could simply be "XAttribute" instead
                var name = new XElement("Name", record.Name);
                var combined = new XElement("Combined", record.Combined);
                var car = new XElement("Car", name, combined);
                cars.Add(car);
            }

            document.Add(cars);
            document.Save("fuel.xml");
        }
    }
}
