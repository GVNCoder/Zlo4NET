namespace Zlo4NET.Data
{
    internal class ZActionState<TResult, TState>
    {
        public TResult Result { get; set; }
        public TState State { get; set; }
    }
}