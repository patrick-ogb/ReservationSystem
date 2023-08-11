namespace ReservationSyste.ViewModels
{
    public class ChartDataViewModel
    {
        public List<ChartData> chartData { get; set; } = ChartData.GetChartDatas();
    }
    public class ChartData
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string Department { get; set; }

        public static List<ChartData> GetChartDatas()
        {
            return new List<ChartData>
            {
                new ChartData { Id = 1, Name = "Sir", Department = "IT" },
                new ChartData { Id = 2, Name = "Noble", Department = "IT" },
                new ChartData { Id = 3, Name = "James", Department = "IT" },
                new ChartData { Id = 4, Name = "Paul", Department = "HR" },
                new ChartData { Id = 5, Name = "Sam", Department = "IT" },
                new ChartData { Id = 6, Name = "Ben", Department = "IT" },
                new ChartData { Id = 7, Name = "Tina", Department = "HR" },
                new ChartData { Id = 8, Name = "Rose", Department = "IT" },
                new ChartData { Id = 9, Name = "May", Department = "HR" },
                new ChartData { Id = 10, Name = "Jane", Department = "Admin" },
                new ChartData { Id = 11, Name = "Mark", Department = "Admin" },
                new ChartData { Id = 12, Name = "Look", Department = "HR" },
                new ChartData { Id = 13, Name = "King", Department = "Admin" },
            };
        }
    }

   
}
