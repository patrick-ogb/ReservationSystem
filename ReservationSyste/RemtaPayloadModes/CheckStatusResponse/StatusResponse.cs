namespace ReservationSyste.RemtaPayloadModes.CheckStatusResponse
{
    public class StatusResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string batchRef { get; set; }
        public float totalAmount { get; set; }
        public float feeAmount { get; set; }
        public string authorizationId { get; set; }
        public string transactionDate { get; set; }
        public string transactionDescription { get; set; }
        public string sourceAccount { get; set; }
        public string currency { get; set; }
        public string paymentStatus { get; set; }
        public Transaction[] transactions { get; set; }
    }

    public class Transaction
    {
        public float amount { get; set; }
        public string transactionRef { get; set; }
        public string paymentDate { get; set; }
        public string paymentStatus { get; set; }
        public string statusMessage { get; set; }
    }


}
