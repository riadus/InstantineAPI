namespace InstantineAPI.Formatter
{
    public class CsvFormatterOptions
    {
        public bool UseSingleLineHeaderInCsv { get; set; } = true;

        public string CsvDelimiter { get; set; } = ";";

        public string Encoding { get; set; } = "UTF-8";
        public string EscapeChar { get; set; } = "/";
        public string EscapeLineCharacters { get; set; } = "///";
    }
}
