using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Cars
{
    public class CarsConverter
    {
        public void InsertAndQueryData(List<Car> records)
        {
            // Database.SetInitializer(new DropCreateDatabaseIfModelChanges<CarDb>());
            InsertData(records);
            // QueryData();
        }

        private void QueryData()
        {
            throw new NotImplementedException();
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
