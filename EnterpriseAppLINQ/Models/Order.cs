namespace EnterpriseAppLINQ.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int ClientId { get; set; }
        public DateTime OrderDate { get; set; }

        // Relación con los detalles de la orden (OrderDetails)
        public ICollection<OrderDetail> OrderDetails { get; set; }
        
        // Relación con el cliente (Client)
        public Client Client { get; set; }
    }
}