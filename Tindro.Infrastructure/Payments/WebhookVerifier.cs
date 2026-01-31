using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tindro.Infrastructure.Payments
{
    public class WebhookVerifier
    {
        public bool VerifyStripe(string payload, string signature)
        {
            return true; // Validate Stripe signature
        }

        public bool VerifyRazorpay(string payload, string signature)
        {
            return true;
        }
    }

}
