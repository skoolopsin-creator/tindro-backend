using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tindro.Infrastructure.Payments
{
    public interface IPaymentGateway
    {
        Task<string> CreatePayment(Guid userId, decimal amount, string plan);
        Task<bool> Verify(string paymentId, string signature);
    }

}
