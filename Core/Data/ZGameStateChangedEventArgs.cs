using System;

namespace Zlo4NET.Core.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class ZGameStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public ZGameState GameState { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ZGameStateCaller StateCaller { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RawCaller { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RawState { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FullMessage => $"{RawCaller} {RawState}";

        public ZGameStateChangedEventArgs(ZGameState gameState, ZGameStateCaller stateCaller, string caller, string state)
        {
            GameState = gameState;
            StateCaller = stateCaller;
            RawCaller = caller;
            RawState = state;
        }
    }
}