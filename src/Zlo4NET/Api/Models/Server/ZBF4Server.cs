using Zlo4NET.Core.Data.Attributes;

namespace Zlo4NET.Api.Models.Server
{
    /// <summary>
    /// Defines the Battlefield 4 server
    /// </summary>
    public class ZBF4Server : ZServerBase
    {
        /// <inheritdoc />
        [ZObservableProperty]
        [ZMapperProperty]
        public override byte SpectatorsCapacity { get; set; }
    }
}