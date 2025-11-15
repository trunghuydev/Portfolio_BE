using System.ComponentModel.DataAnnotations;

namespace ZEN.Contract.ProfileDto;

public class UpdateUsernameDto
{
    [Required]
    [StringLength(30, MinimumLength = 3)]
    public string username { get; set; } = default!;
}

public class UpdateVisibilityDto
{
    [Required]
    public bool is_public { get; set; }
}

