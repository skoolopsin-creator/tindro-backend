public class RegisterFcmTokenRequest
{
    public required string Token { get; set; }
    public string? Platform { get; set; }   // android / ios
}
