namespace IMS.Application.Products.Queries.CountByStatus;

public sealed class CountProductsByStatusQueryResponse
{
    public int InStock { get; set; }
    public int Sold { get; set; }
    public int Damaged { get; set; }
}
