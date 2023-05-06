using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ReservationSyste.Data;
using ReservationSyste.Services.Interfices;

public class BackgroundWorkerService : BackgroundService
{
    private readonly ILogger<BackgroundWorkerService> _logger;
    public IServiceProvider _services { get; }

    public BackgroundWorkerService(IServiceProvider services, ILogger<BackgroundWorkerService> logger
        )
    {
        _logger = logger;
        _services = services;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation($"Worker running at {DateTimeOffset.Now}");
            await GetExpiredReservations(stoppingToken);
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task GetExpiredReservations(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"Worker running at {DateTimeOffset.Now}");

        using (var scope = _services.CreateScope())
        {
            var _DbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var _creatReservationContext = scope.ServiceProvider.GetRequiredService<IReservationService>();

            var profileRooms = await (from pRooms in _DbContext.PersonalProfileRooms
                                      where (pRooms.ProfileStatus == 1 && pRooms.CheckOut < DateTime.Now)
                                      select pRooms).ToListAsync();

            foreach (var pr in profileRooms.Where(w => w.ProfileStatus == 1))
            {
                pr.ProfileStatus = 2;
                _DbContext.PersonalProfileRooms.Update(pr);
                await _DbContext.SaveChangesAsync();

              await  _creatReservationContext.UpdateReservationStatusAsync(pr.RoomId, 2, _DbContext);

            }

        }
    }

}







public class MyBatch : BackgroundService
{
    private readonly ILogger<BackgroundWorkerService> _logger;

    public IServiceProvider Services { get; }
    public MyBatch(IServiceProvider services, ILogger<BackgroundWorkerService> logger)
    {
        Services = services;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stopToken)
    {
        while (!stopToken.IsCancellationRequested)
        {
            await DoWithDb(stopToken);
            await Task.Delay(TimeSpan.FromMinutes(2));
        }
    }

    private async Task DoWithDb(CancellationToken stoppingToken)
    {
        //...
            _logger.LogInformation($"Worker running at {DateTimeOffset.Now}");

        using (var scope = Services.CreateScope())
        {
            var _myDbContext =
                scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

            var result = await _myDbContext.PersonalProfileRooms.ToListAsync();

        }
    }
}