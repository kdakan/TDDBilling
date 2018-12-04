using Billing.Data;
using System;

namespace Billing.Domain.Service
{
    public class BillingService
    {
        private ICustomersRepo customersRepo;

        public BillingService(ICustomersRepo customersRepo)
        {
            this.customersRepo = customersRepo;
        }

        public static int GracePeriodMonths { get; set; } = 2;

        public void ProcessSubscribedCustomers(DateTime billingDate)
        {
            var subscribedCustomers = customersRepo.GetSubscribedCustomers();
            foreach (var customer in subscribedCustomers)
            {
                if (customer.LastActiveDate < billingDate.AddMonths(-1))
                    customer.Unsubscribe();
                else if (customer.LastPaymentDate < billingDate.AddMonths(-1 * GracePeriodMonths) && customer.LastBillingDate != null)
                    customer.Unsubscribe();
                else if ((customer.LastBillingDate < billingDate || customer.LastBillingDate == null) && customer.LastPaymentDate == customer.LastBillingDate)
                    customer.Bill(billingDate);

                customersRepo.Update(customer);
            }

        }
    }

}
