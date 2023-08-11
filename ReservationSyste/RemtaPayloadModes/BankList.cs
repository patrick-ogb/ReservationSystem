namespace ReservationSyste.RemtaPayloadModes
{

    public class BankList
    {
        public string status { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public Bank[] banks { get; set; }
    }

    public class Bank
    {
        public string bankCode { get; set; }
        public string bankName { get; set; }
        public string bankAccronym { get; set; }
        public string type { get; set; }
    }


    public class ApiResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public Datum[] data { get; set; }
    }

    public class Datum
    {
        public string accessToken { get; set; }
        public int expiresIn { get; set; }
    }



    public class Transaction
    {
        public int amount { get; set; }
        public string transactionRef { get; set; }
        public string destinationAccount { get; set; }
        public string destinationAccountName { get; set; }
        public string destinationBankCode { get; set; }
        public string destinationNarration { get; set; }
    }

    public class RemitalBulkPayload
    {
        public string batchRef { get; set; }
        public int totalAmount { get; set; }
        public string sourceAccount { get; set; }
        public string sourceAccountName { get; set; }
        public string sourceBankCode { get; set; }
        public string currency { get; set; }
        public string sourceNarration { get; set; }
        public string originalAccountNumber { get; set; }
        public string originalBankCode { get; set; }
        public string customReference { get; set; }
        public List<Transaction> transactions { get; set; }
    }

    public class Payloads
    {
        public static int Generate()
        {
            Random rand = new Random((int)DateTime.Now.Ticks);
            // var tricks = DateTime.Now.Ticks;
           // var random = rand.Next(100000000, 999999999);
            return rand.Next(100000000, 999999999);
        }
        public static RemitalBulkPayload GetRemitalBulkPayloads()
        {
            RemitalBulkPayload remitas = new RemitalBulkPayload
            {
                batchRef = Payloads.Generate().ToString(),
                totalAmount = 4500,
                sourceAccount = "8909090989",
                sourceAccountName = "ABC",
                sourceBankCode = "058",
                currency = "NGN",
                sourceNarration = "Bulk Transfer",
                originalAccountNumber = "8909090989",
                originalBankCode = "058",
                customReference = "8909090989111",
                transactions = new List<Transaction>()
                {
                    new Transaction()
                    {
                        amount = 2500,
                        transactionRef = "8909090989222",
                        destinationAccount = "0037475942",
                        destinationAccountName = "Kelvin John",
                        destinationBankCode = "058",
                        destinationNarration = "Bulk Transfer"
                    },
                    new Transaction()
                    {
                        amount = 1500,
                        transactionRef ="8909090989333",
                        destinationAccount = "0037475942",
                        destinationAccountName = "Martin John",
                        destinationBankCode = "058",
                        destinationNarration = "Bulk Transfer"
                    },
                    new Transaction()
                    {
                         amount = 500,
                         transactionRef ="8909090989444",
                         destinationAccount = "0037475942",
                         destinationAccountName = "Mike John",
                         destinationBankCode = "058",
                         destinationNarration = "Bulk Transfer"
                    }
                }
            };

            return remitas;
        }
    }

    public class RemitaObj
    {
        public string batchRef { get; set; }
        public int totalAmount { get; set; }
        public string authorizationId { get; set; }
        public string transactionDate { get; set; }
    }

    public class RemitaReponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }


}


//AuthorizationId
//    BeneficiaryPaymentStatus
//    BeneficiaryStatusMessage
//    FeeAmount
//    PaymentStatus