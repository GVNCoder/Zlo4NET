using System.Collections.Generic;
using System.Threading.Tasks;

using Zlo4NET.Api.Shared;

namespace Zlo4NET.Api.Service
{
    /// <summary>
    /// 
    /// </summary>
    public interface IZInjector
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetGame"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        Task InjectAsync(ZGame targetGame, string filePath);
    }
}