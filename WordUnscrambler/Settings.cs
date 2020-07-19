namespace WordUnscrambler
{
    internal sealed class Settings
    {
        public bool OutputIsLowerCase { get; set; } = true;
        public int OutputIndentWidth { get; set; } = 2;
        public int MinimumWordLength { get; set; } = 2;
    }
}
