using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Api.Service
{
    /// <summary>
    /// Defines game factory
    /// </summary>
    public interface IZGameFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters">Options for creating a game</param>
        /// <returns></returns>
        IZGameProcess CreateSingle(ZSingleLaunchParameters parameters);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters">Options for creating a game</param>
        /// <returns></returns>
        IZGameProcess CreateMulti(ZMultiLaunchParameters parameters);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters">Options for creating a game</param>
        /// <returns></returns>
        IZGameProcess CreateTestRange(ZTestRangeLaunchParameters parameters);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters">Options for creating a game</param>
        /// <returns></returns>
        IZGameProcess CreateCoopClient(ZCoopClientLaunchParameters parameters);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters">Options for creating a game</param>
        /// <returns></returns>
        IZGameProcess CreateCoopHost(ZCoopHostLaunchParameters parameters);
    }
}