using Billing.Domain.Entity;
using System;
using System.Collections.Generic;

namespace Billing.Data
{
    public interface ICustomersRepo
    {
        void Add(Customer customer);
        IEnumerable<Customer> GetBilledCustomers(DateTime billingDate);
        IEnumerable<Customer> GetNonBilledCustomers(DateTime billingDate);
        IEnumerable<Customer> GetNonsubscribedCustomers();
        IEnumerable<Customer> GetSubscribedCustomers();
        void Update(Customer customer);
    }

}
