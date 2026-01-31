using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tindro.Infrastructure.Payments
{
    public class StripePaymentGateway : IPaymentGateway
    {
        public Task<string> CreatePayment(Guid userId, decimal amount, string plan)
        {
            // Create Stripe Checkout Session
            return Task.FromResult("stripe_session_id");
        }

        public Task<bool> Verify(string paymentId, string signature)
        {
            return Task.FromResult(true);
        }
    }

}
