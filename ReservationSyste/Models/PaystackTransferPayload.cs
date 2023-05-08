namespace ReservationSyste.Models
{ 
    public class PaystackTransferPayload
    {
        public string currency { get; set; }
        public string source { get; set; }
        public Transfer[] transfers { get; set; }
    }

    public class Transfer
    {
        public int amount { get; set; }
        public string reference { get; set; }
        public string reason { get; set; }
        public string recipient { get; set; }
    }





}
