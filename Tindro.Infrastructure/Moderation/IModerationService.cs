using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tindro.Domain.Moderation;
namespace Tindro.Infrastructure.Moderation
{
    public interface IModerationService
    {
        Task<ModerationResult> CheckText(string text);
        Task<ModerationResult> CheckImage(string imageUrl);
    }

}
