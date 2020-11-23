namespace Zlo4NET.Api.Models.Shared
{
    /// <summary>
    /// Defines ZClient game run result
    /// </summary>
    public enum ZRunResult : byte
    {
        Success,
        NotFound,
        Error,

        None
    }
}