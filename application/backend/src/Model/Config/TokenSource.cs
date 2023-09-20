using backend.Model.Enum;

namespace backend.Model.Config;

public class TokenSource
{
    public TokenSourceType Type { get; set; }
    public string? Token { get; set; }
    public string? VariableName { get; set; }
}