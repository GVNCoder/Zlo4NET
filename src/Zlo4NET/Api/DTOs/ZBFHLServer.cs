using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Api.DTOs
{
    /// <inheritdoc />
    public class ZBFHLServer : ZServerBase
    {
        #region Ctor

        /// <summary>
        /// 
        /// </summary>
        public ZBFHLServer()
        {
            TargetGame = ZGame.BFHL;
        }

        #endregion
    }
}