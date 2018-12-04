using Billing.Data;
using Billing.Domain.Entity;
using Billing.Domain.Service;
using NUnit.Framework;
using System;
using System.Linq;

//billing system features:
//------------------------
//monthly billing
//grace period for missed payments(dunning status)
//not all customers are subscribers
//idle customers should be automatically unsubscribed

namespace Billing.Tests
{
    [TestFixture]
    public class MonthlyBillingTests
    {
        ICustomersRepo customersRepo;
        BillingService billingService;
        DateTime billingDate;

        [SetUp]
        public void SetUp()
        {
            customersRepo = new MockCustomersRepo();
            billingService = new BillingService(customersRepo);
            billingDate = new DateTime(2018, 1, 1);
        }

        [Test]
        public void NonsubscribedCustomerShouldNotGetBilled()
        {
            customersRepo.Add(new Customer { IsSubscribed = false, LastActiveDate = billingDate });

            billingService.ProcessSubscribedCustomers(billingDate);

            var nonsubscribedCustomers = customersRepo.GetNonsubscribedCustomers();
            Assert.IsTrue(nonsubscribedCustomers.Count() > 0);

            var billedCustomers = customersRepo.GetBilledCustomers(billingDate);
            Assert.IsTrue(billedCustomers.Count() == 0);
        }

        [Test]
        public void SubscribedCustomerShouldGetBilled()
        {
            customersRepo.Add(new Customer { IsSubscribed = true, LastActiveDate = billingDate });

            billingService.ProcessSubscribedCustomers(billingDate);

            var subscribedCustomers = customersRepo.GetSubscribedCustomers();
            Assert.IsTrue(subscribedCustomers.Count() > 0);

            var billedCustomers = customersRepo.GetBilledCustomers(billingDate);
            Assert.IsTrue(billedCustomers.Count() > 0);
        }

        [Test]
        public void NonPayingSubscribedCustomerShouldNotGetBilledDuringGracePeriod()
        {
            var lastBillingDate = billingDate.AddMonths(-1 * BillingService.GracePeriodMonths);
            var lastPaymentDate = lastBillingDate;

            customersRepo.Add(new Customer { IsSubscribed = true, LastActiveDate = billingDate, LastBillingDate = lastBillingDate, LastPaymentDate = lastPaymentDate });

            billingService.ProcessSubscribedCustomers(billingDate);

            var subscribedCustomers = customersRepo.GetSubscribedCustomers();
            Assert.IsTrue(subscribedCustomers.Count() > 0);

            var nonBilledCustomers = customersRepo.GetNonBilledCustomers(billingDate);
            Assert.IsTrue(nonBilledCustomers.Count() == 0);
        }

        [Test]
        public void NonPayingSubscribedCustomerShouldBeUnsubscribedAfterGracePeriod()
        {
            var lastBillingDate = billingDate.AddMonths(-1 * BillingService.GracePeriodMonths).AddMonths(-1);
            var lastPaymentDate = lastBillingDate;

            customersRepo.Add(new Customer { IsSubscribed = true, LastActiveDate = billingDate, LastBillingDate = lastBillingDate, LastPaymentDate = lastPaymentDate });

            billingService.ProcessSubscribedCustomers(billingDate);

            var billedCustomers = customersRepo.GetBilledCustomers(billingDate);
            Assert.IsTrue(billedCustomers.Count() == 0);

            var subscribedCustomers = customersRepo.GetSubscribedCustomers();
            Assert.IsTrue(subscribedCustomers.Count() == 0);
        }

        [Test]
        public void IdleSubscribedCustomerShouldNotGetBilledAndShouldBeUnsubscribed()
        {
            customersRepo.Add(new Customer { IsSubscribed = true, LastActiveDate = billingDate.AddMonths(-1).AddDays(-1) });

            billingService.ProcessSubscribedCustomers(billingDate);

            var billedCustomers = customersRepo.GetBilledCustomers(billingDate);
            Assert.IsTrue(billedCustomers.Count() == 0);

            var subscribedCustomers = customersRepo.GetSubscribedCustomers();
            Assert.IsTrue(subscribedCustomers.Count() == 0);
        }
    }

}
