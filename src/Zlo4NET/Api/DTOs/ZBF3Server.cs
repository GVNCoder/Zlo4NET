using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Api.DTOs
{
    /// <inheritdoc />
    public class ZBF3Server : ZServerBase
    {
        #region Ctor

        /// <summary>
        /// 
        /// </summary>
        public ZBF3Server()
        {
            TargetGame = ZGame.BF3;
        }

        #endregion
    }
}