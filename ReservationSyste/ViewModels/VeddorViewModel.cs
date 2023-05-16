namespace ReservationSyste.ViewModels
{
    public class VeddorViewModel
    {
        public VendorModel[] VeddorViewl { get; set; }
    }

    public class VendorModel
    {
        public int regionId { get; set; }
        public string regionCode { get; set; }
        public string regionName { get; set; }
    }

}
