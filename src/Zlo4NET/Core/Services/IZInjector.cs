using System.Collections.Generic;

using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Core.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IZInjector
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        /// <param name="dlls"></param>
        void Inject(ZGame game, IEnumerable<string> dlls);
    }
}