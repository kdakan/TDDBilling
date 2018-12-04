using Billing.Data;
using Billing.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Billing.Tests
{
    public class MockCustomersRepo : ICustomersRepo
    {
        private List<Customer> customers;

        public MockCustomersRepo()
        {
            customers = new List<Customer>();
        }

        public IEnumerable<Customer> GetNonsubscribedCustomers()
        {
            return customers.Where(c => !c.IsSubscribed);
        }

        public IEnumerable<Customer> GetSubscribedCustomers()
        {
            return customers.Where(c => c.IsSubscribed);
        }

        public IEnumerable<Customer> GetBilledCustomers(DateTime billingDate)
        {
            return customers.Where(c => c.LastBillingDate == billingDate);
        }

        public IEnumerable<Customer> GetNonBilledCustomers(DateTime billingDate)
        {
            return customers.Where(c => c.LastBillingDate != billingDate);
        }

        public void Add(Customer customer)
        {
            customers.Add(customer);
        }

        public void Update(Customer customer)
        {
        }
    }

}
