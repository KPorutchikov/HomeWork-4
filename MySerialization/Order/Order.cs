using System;

namespace MySerialization
{
    [Serializable]
    public class Order
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public int Quantity { get; set; }
        public double TotalSum { get; set; }

        public Order() { }

        public Order(int orderID, DateTime orderDate, string productName, string description, string address, int quantity, double totalSum)
        {
            OrderID = orderID;
            OrderDate = orderDate;
            ProductName = productName;
            Description = description;
            Address = address;
            Quantity = quantity;
            TotalSum = totalSum;
        }
    }
}
