using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tindro.Application.Common.Interfaces
{
    public interface IFileStorage
    {
        Task<string> UploadAsync(IFormFile file, string folder);
    }
}
