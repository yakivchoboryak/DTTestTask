namespace DTTestTask.Models
{
    public class Trip
    {
        public int Id { get; set; }
        public DateTime PickupDateTime { get; set; }
        public DateTime DropoffDateTime { get; set; }
        public int PassengerCount { get; set; }
        public double TripDistance { get; set; }
        public string ?StoreAndForwardFlag { get; set; }
        public int PickupLocationID { get; set; }
        public int DropoffLocationID { get; set; }
        public decimal FareAmount { get; set; }
        public decimal TipAmount { get; set; }
    }
}
