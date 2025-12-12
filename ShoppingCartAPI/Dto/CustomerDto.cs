namespace ShoppingCartAPI.Dto
{
    public class RegisterDto
    {
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class LoginDto
    {
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class UpdateBudgetDto
    {
        public decimal Budget { get; set; }
    }

}
