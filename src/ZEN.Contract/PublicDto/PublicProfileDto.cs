namespace ZEN.Contract.PublicDto;

public class PublicProfileDto
{
    public string username { get; set; } = default!;
    public string fullname { get; set; } = default!;
    public string? email { get; set; }
    public string? phone_number { get; set; }
    public string? address { get; set; }
    public string? position_career { get; set; }
    public string? background { get; set; }
    public string? mindset { get; set; }
    public string? avatar { get; set; }
    public string? github { get; set; }
    public string? linkedin_url { get; set; }
    public string? facebook_url { get; set; }
    public string? university_name { get; set; }
    public double? gpa { get; set; }
    public string? expOfYear { get; set; }
    public string? dob { get; set; }
    public List<PublicCertificateDto> certificates { get; set; } = [];
}

public class PublicCertificateDto
{
    public string id { get; set; } = default!;
    public string certificate_name { get; set; } = default!;
}

