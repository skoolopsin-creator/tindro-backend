using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tindro.Application.Users.Dtos;
using Tindro.Application.Common.Models;

namespace Tindro.Application.Admin.Interfaces
{
    public interface IAdminUserService
    {
        Task<PagedResult<UserSummaryDto>> SearchUsersAsync(UserSearchParameters query);
        Task<UserDetailDto> GetUserDetailAsync(Guid userId);
    }

}
