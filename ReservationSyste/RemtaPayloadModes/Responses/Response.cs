namespace ReservationSyste.RemtaPayloadModes.Responses
{
    public class Response
    {
        public string status { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string batchRef { get; set; }
        public int totalAmount { get; set; }
        public string authorizationId { get; set; }
        public string transactionDate { get; set; }
    }

}
