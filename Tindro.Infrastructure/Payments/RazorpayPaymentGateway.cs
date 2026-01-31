using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tindro.Infrastructure.Payments
{
    public class RazorpayPaymentGateway : IPaymentGateway
    {
        public Task<string> CreatePayment(Guid userId, decimal amount, string plan)
        {
            // Call Razorpay API here
            return Task.FromResult("razorpay_order_id");
        }

        public Task<bool> Verify(string paymentId, string signature)
        {
            // Validate webhook
            return Task.FromResult(true);
        }
    }

}
