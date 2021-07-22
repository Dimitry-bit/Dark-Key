namespace DarkKey.Core.Debugger
{
    [System.Flags]
    public enum DebugLogLevel
    {
        Core = 1 << 0,
        Player = 1 << 1,
        UI = 1 << 2,
        Network = 1 << 3
    }
}