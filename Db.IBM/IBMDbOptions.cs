namespace Services
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "IBM is an abbreviation")]
    public class IBMDbOptions
    {
        public string? Database { get; set; }

        public string? DBName { get; set; }

        public string? UserId { get; set; }

        public string? Password { get; set; }
    }
}
