using System.ComponentModel.DataAnnotations;

namespace Tasks.Application.Dtos.Pagination;

public class PaginationDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Page number can't be less than 1")]
    public int PageNumber { get; init; } = 1;

    [AllowedValues(5, 10, 20, ErrorMessage = "Page size must be 5, 10, or 20")]
    public int PageSize { get; init; } = 10;
}