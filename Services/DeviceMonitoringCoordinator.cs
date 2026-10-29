/**************************************************
 * Mã sinh viên: 202419021
 * Họ tên: Phạm Đình Vũ
 **************************************************/
using TeachingDeviceAllocation.Models;
namespace TeachingDeviceAllocation.Services;
public class DeviceMonitoringCoordinator : IDisposable
{
    private readonly List<TeachingDevice> devices;
    private readonly ConsoleNotificationService notificationService;
    private readonly DeviceAuditLogger auditLogger;
    private readonly MaintenanceService maintenanceService;
    private bool isSubscribed;
    private bool isDisposed;
    public DeviceMonitoringCoordinator(IEnumerable<TeachingDevice> devices,
        ConsoleNotificationService notificationService, DeviceAuditLogger auditLogger,
        MaintenanceService maintenanceService)
    {
        ArgumentNullException.ThrowIfNull(devices);
        this.devices = devices.Distinct().ToList();
        if (this.devices.Any(d => d is null)) throw new ArgumentException("Thiết bị không được null.");
        this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        this.auditLogger = auditLogger ?? throw new ArgumentNullException(nameof(auditLogger));
        this.maintenanceService = maintenanceService ?? throw new ArgumentNullException(nameof(maintenanceService));
    }
    public void Subscribe()
    {
        ObjectDisposedException.ThrowIf(isDisposed, this);
        if (isSubscribed) return;
        foreach (var device in devices)
        {
            device.StatusChanged += auditLogger.HandleStatusChanged;
            device.AlertRaised += notificationService.HandleAlert;
            device.AlertRaised += maintenanceService.HandleCriticalAlert;
        }
        isSubscribed = true;
    }
    public void Unsubscribe()
    {
        if (!isSubscribed) return;
        foreach (var device in devices)
        {
            device.StatusChanged -= auditLogger.HandleStatusChanged;
            device.AlertRaised -= notificationService.HandleAlert;
            device.AlertRaised -= maintenanceService.HandleCriticalAlert;
        }
        isSubscribed = false;
    }
    public void Dispose()
    { Unsubscribe(); isDisposed = true; }
}
