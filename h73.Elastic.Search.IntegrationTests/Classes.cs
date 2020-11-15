using System;
using System.Collections.Generic;

namespace h73.Elastic.Search.IntegrationTests
{
    public class Customer
    {
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string Id { get; set; }
        public string LastName { get; set; }
        public string TelephoneNumber { get; set; }
        public DateTime Created { get; set; }
        public int Age { get; set; }
        public List<Product> Products { get; set; }
    }

    public class Product
    {
        public string Description { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

}