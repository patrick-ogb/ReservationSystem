using ReservationSyste.RemtaPayloadModes;
using System.Collections.Generic;

namespace ReservationSyste.Models
{
    public class CustomBankList
    {
        public string bankCode { get; set; }
        public string bankName { get; set; }
        public string bankAccronym { get; set; }
        public string type { get; set; }
        public string update { get; set; }

        public static List<CustomBankList> GetCustomBankList()
        {
            List<CustomBankList> customBankLists = new List<CustomBankList>();

            customBankLists.Add(new CustomBankList
            {
                bankCode = "511080016",
                bankName = "ABUJA MICROFINANCE BANKS",
                bankAccronym = "ASO",
                type = "MFB",
                update =""
            });
            customBankLists.Add(new CustomBankList
            {
                bankCode = "511080026",
                bankName = "ABUJA MICROFINANCE BANKS",
                bankAccronym = null,
                type = "MFB",
                update = ""
            });
            customBankLists.Add(new CustomBankList
            {
                bankCode = "511080036",
                bankName = "ABUJA MICROFINANCE BANKS",
                bankAccronym = null,
                type = "MFB",
                update = ""
            });
            customBankLists.Add(new CustomBankList
            {
                bankCode = "044",
                bankName = "ACCESS BANK PLC",
                bankAccronym = "ACCESSBANK",
                type = "DMB",
                update = ""
            });
            return customBankLists;
        }

        internal static  List<CustomBankList> CompareBankList(List<CustomBankList> customBankList, List<Bank> bankList)
        {
            List<CustomBankList> cbankList = new List<CustomBankList>();
            foreach (CustomBankList cbank in customBankList)
            {
               var result =  bankList.Where(b => b.bankCode == cbank.bankCode).FirstOrDefault();
                cbank.update = result.bankCode + "_" + result.bankCode;
                cbankList.Add(cbank);
            }

            return cbankList;
        }
    }

}
