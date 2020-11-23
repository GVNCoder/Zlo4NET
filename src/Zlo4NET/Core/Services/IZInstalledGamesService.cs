using System.Threading.Tasks;
using Zlo4NET.Core.Data;

namespace Zlo4NET.Core.Services
{
    internal interface IZInstalledGamesService
    {
        Task<ZInstalledGames> GetInstalledGamesAsync();
    }
}