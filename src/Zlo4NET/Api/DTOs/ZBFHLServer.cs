using Zlo4NET.Api.Shared;

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