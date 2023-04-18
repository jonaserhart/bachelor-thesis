namespace backend.Model;

public class AccessToken
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
    public bool IsExpired => Expires.CompareTo(DateTime.Now) >= 0;
}