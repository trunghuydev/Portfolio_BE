namespace ZEN.Contract.PublicDto;

public class PublicProjectResponseDto
{
    public int total_item { get; set; }
    public List<PublicProjectDto> data { get; set; } = [];
}

public class PublicProjectDto
{
    public string project_id { get; set; } = default!;
    public string project_name { get; set; } = default!;
    public string? description { get; set; }
    public string? project_type { get; set; }
    public bool is_Reality { get; set; }
    public string? duration { get; set; }
    public string? from { get; set; }
    public string? to { get; set; }
    public string? url_project { get; set; }
    public string? url_demo { get; set; }
    public string? url_github { get; set; }
    public string? img_url { get; set; }
    public string? url_contract { get; set; }
    public string? url_excel { get; set; }
    public List<PublicTechDto> teches { get; set; } = [];
}

public class PublicTechDto
{
    public string tech_name { get; set; } = default!;
}

