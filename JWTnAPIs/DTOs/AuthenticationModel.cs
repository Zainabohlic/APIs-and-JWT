namespace Project2.DTOs
{
    public class AuthenticationModel
    {
        public string? token_type { get; set; } 
        public string? expires_in { get; set; }
        public string? access_token { get; set; }
    }
}
