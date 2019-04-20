using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;

/*
 public CarDb(): base("Data Source=(localdb)\\mssqllocaldb;Integrated Security=True")
        {
           
        }
     */

namespace Cars
{
    public class CarDb: DbContext
    {
        public DbSet<Car> Cars { get; set; }
    }
}
