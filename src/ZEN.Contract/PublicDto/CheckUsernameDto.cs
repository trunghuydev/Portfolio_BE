namespace ZEN.Contract.PublicDto;

public class CheckUsernameResponseDto
{
    public bool available { get; set; }
    public string message { get; set; } = default!;
}

