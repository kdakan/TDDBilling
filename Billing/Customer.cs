using System;

namespace Billing.Domain.Entity
{
    public class Customer
    {
        public DateTime? LastBillingDate { get; set; }
        public bool IsSubscribed { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public DateTime? LastActiveDate { get; set; }

        public void Unsubscribe()
        {
            IsSubscribed = false;
        }

        public void Bill(DateTime billingDate)
        {
            LastBillingDate = billingDate;
        }
    }

}
