namespace ZEN.Contract.PublicDto;

public class PublicWorkExpResponseDto
{
    public int total_item { get; set; }
    public List<PublicWorkExpDto> data { get; set; } = [];
}

public class PublicWorkExpDto
{
    public string we_id { get; set; } = default!;
    public string company_name { get; set; } = default!;
    public string? position { get; set; }
    public string? duration { get; set; }
    public string? description { get; set; }
    public string? project_id { get; set; }
    public List<PublicTaskDto> tasks { get; set; } = [];
}

public class PublicTaskDto
{
    public string mt_id { get; set; } = default!;
    public string? task_description { get; set; }
}

