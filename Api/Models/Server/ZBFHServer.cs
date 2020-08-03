namespace Zlo4NET.Api.Models.Server
{
    /// <summary>
    /// Defines the Battlefield Hardline server
    /// </summary>
    public class ZBFHServer : ZServerBase
    {
        /// <inheritdoc />
        public override byte SpectatorsCapacity { get; set; }
    }
}