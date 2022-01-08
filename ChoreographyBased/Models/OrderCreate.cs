namespace Models
{
  public class OrderCreate
  {
    public int ProductId  { get; set; }
    public int CustomerId { get; set; }
    public int Quantity { get; set; }
  }
}
