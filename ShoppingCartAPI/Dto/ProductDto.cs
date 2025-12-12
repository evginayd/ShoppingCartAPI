namespace ShoppingCartAPI.Dto
{
    public class FilterProductDto
    {
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }
        public string? Category { get; set; }
        public string? Name { get; set; }
    }
}
