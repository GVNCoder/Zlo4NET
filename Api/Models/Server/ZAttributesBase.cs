using System.Collections.Generic;
using Zlo4NET.Core.Data;
using Zlo4NET.Core.Data.Attributes;
using Zlo4NET.Core.Helpers;

namespace Zlo4NET.Api.Models.Server
{
    /// <summary>
    /// Defines basic server attributes
    /// </summary>
    public abstract class ZAttributesBase : ZObservableObject
    {
        protected readonly IDictionary<string, string> _attributes;

        protected ZAttributesBase(IDictionary<string, string> attributes)
        {
            _attributes = attributes;
        }

        protected string _getValue(string key)
        {
            _attributes.TryGetValue(key, out var value);
            return string.IsNullOrWhiteSpace(value) ? ZStringConstants.NotReceived : value;
        }

        /// <summary>
        /// Gets the banner url. If the value is not defined, the default value "Not Received" will be returned.
        /// </summary>
        [ZObservableProperty]
        public string BannerUrl => _getValue("bannerurl");
        /// <summary>
        /// Gets country code. If the value is not defined, the default value "Not Received" will be returned.
        /// </summary>
        public string Country =>  _getValue("country");
        /// <summary>
        /// Gets server message. If the value is not defined, the default value "Not Received" will be returned.
        /// </summary>
        [ZObservableProperty]
        public string Message => _getValue("message");
        /// <summary>
        /// Gets ???. If the value is not defined, the default value "Not Received" will be returned.
        /// </summary>
        [ZObservableProperty]
        public string Mod => _getValue("mod");
        /// <summary>
        /// Gets preset name. If the value is not defined, the default value "Not Received" will be returned.
        /// </summary>
        [ZObservableProperty]
        public string Preset => _getValue("preset");
        /// <summary>
        /// Gets PunkBuster version. If the value is not defined, the default value "Not Received" will be returned.
        /// </summary>
        public string PunkBusterVersion => _getValue("punkbusterversion");
        /// <summary>
        /// Gets region code. If the value is not defined, the default value "Not Received" will be returned.
        /// </summary>
        public string Region => _getValue("region");
        /// <summary>
        /// Gets description. If the value is not defined, the default value "Not Received" will be returned.
        /// </summary>
        [ZObservableProperty]
        public string Description => _getValue("description");
        /// <summary>
        /// Gets an indication of the presence of a PangBuster. If the value is not defined, the default value "Not Received" will be returned.
        /// </summary>
        public string PunkBuster => _getValue("punkbuster");
        /// <summary>
        /// Gets an indication of the presence of a FairFight. If the value is not defined, the default value "Not Received" will be returned.
        /// </summary>
        public abstract string FairFight { get; }
        /// <summary>
        /// Gets type of server. If the value is not defined, the default value "Not Received" will be returned.
        /// </summary>
        public abstract string ServerType { get; }
        /// <summary>
        /// Gets tick rate. If the value is not defined, the default value "Not Received" will be returned.
        /// </summary>
        public abstract string TickRate { get; }
    }
}