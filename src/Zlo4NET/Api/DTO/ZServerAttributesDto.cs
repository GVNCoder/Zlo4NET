namespace Zlo4NET.Api.DTO
{
    /// <summary>
    /// Represents server attributes
    /// </summary>
    public class ZServerAttributesDto
    {
        /// <summary>
        /// Gets banner url
        /// </summary>
        public string BannerUrl { get; set; }
        /// <summary>
        /// Gets country code
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// Gets server message
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Gets ???
        /// </summary>
        public string Mod { get; set; }
        /// <summary>
        /// Gets preset name
        /// </summary>
        public string Preset { get; set; }
        /// <summary>
        /// Gets PunkBuster version
        /// </summary>
        public string PunkBusterVersion { get; set; }
        /// <summary>
        /// Gets region code
        /// </summary>
        public string Region { get; set; }
        /// <summary>
        /// Gets description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Gets an indication of the presence of a PunkBuster
        /// </summary>
        public string PunkBuster { get; set; }
        /// <summary>
        /// Gets an indication of the presence of a FairFight
        /// </summary>
        public string FairFight { get; set; }
        /// <summary>
        /// Gets type of server
        /// </summary>
        public string ServerType { get; set; }
        /// <summary>
        /// Gets tick rate
        /// </summary>
        public string TickRate { get; set; }
    }
}