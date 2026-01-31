using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Tindro.Infrastructure.Storage
{
    public interface IStorageService
    {
        Task<string> UploadAsync(IFormFile file, string folder);
    }

}
