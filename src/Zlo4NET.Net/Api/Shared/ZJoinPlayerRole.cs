﻿namespace Zlo4NET.Api.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public enum ZJoinPlayerRole
    {
        /// <summary>
        /// Soldier role. Supports on All games
        /// </summary>
        Soldier,
        /// <summary>
        /// Commander role. Supports on Battlefield 4 *only (*Hardline maybe to)
        /// </summary>
        Commander,
        /// <summary>
        /// Spectator role. Supports on Battlefield 4 and *Hardline (* not implemented on ZLO)
        /// </summary>
        Spectator
    }
}