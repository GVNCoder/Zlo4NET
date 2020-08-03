using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Data;
using Zlo4NET.Core.Data.Attributes;

namespace Zlo4NET.Api.Models.Server
{
    /// <summary>
    /// Defines base server
    /// </summary>
    public abstract class ZServerBase : ZObservableObject
    {
        #region Non-observable

        /// <summary>
        /// Gets the ZloID
        /// </summary>
        public uint Id { get; set; }
        /// <summary>
        /// Gets ip address
        /// </summary>
        public IPAddress Ip { get; set; }
        /// <summary>
        /// Gets port number
        /// </summary>
        public uint Port { get; set; }
        /// <summary>
        /// Gets target game
        /// </summary>
        public ZGame Game { get; set; }
        /// <summary>
        /// Gets settings
        /// </summary>
        public IDictionary<string, string> Settings { get; set; }

        #endregion

        #region Lazy-onservable

        /// <summary>
        /// Gets name
        /// </summary>
        [ZObservableProperty]
        [ZMapperProperty]
        public string Name { get; set; }
        /// <summary>
        /// Gets capacity of players. This field is observable
        /// </summary>
        [ZObservableProperty]
        [ZMapperProperty]
        public byte PlayersCapacity { get; set; }
        /// <summary>
        /// Gets number of current players. This field is observable
        /// </summary>
        [ZObservableProperty]
        public byte CurrentPlayersNumber { get; set; }
        /// <summary>
        /// Gets attributes
        /// </summary>
        [ZObservableProperty]
        [ZMapperProperty]
        public ZAttributesBase Attributes { get; set; }
        /// <summary>
        /// Gets list of players
        /// </summary>
        public ObservableCollection<ZPlayer> Players { get; set; }
        /// <summary>
        /// Gets capacity of spectators
        /// </summary>
        public abstract byte SpectatorsCapacity { get; set; } // observable attr setups in child classes
        /// <summary>
        /// Gets maps rotation
        /// </summary>
        public ZMapRotation MapRotation { get; set; }

        #endregion

        #region Reactive

        private int _ping;
        /// <summary>
        /// Gets signal delay in ms. By default it is not calculated. This field is observable
        /// </summary>
        public int Ping { get => _ping; set => SetProperty(ref _ping, value); }

        #endregion
    }
}