using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tindro.Infrastructure.Storage
{
    public class StorageSettings
    {
        public string Bucket { get; set; } = null!;
        public string ApiKey { get; set; } = null!;
        public string Secret { get; set; } = null!;
    }

}
