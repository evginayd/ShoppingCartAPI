namespace ShoppingCartAPI.Dto
{
    public class AddToCartDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class DeleteCartItemDto
    {
        public int CartItemId { get; set; }
    }

}
