using System.Threading.Tasks;
using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Core.Services
{
    internal interface IZUserService
    {
        Task<ZUser> GetAuthorizedUserAsync();
        ZUser AuthorizedUser { get; }
    }
}