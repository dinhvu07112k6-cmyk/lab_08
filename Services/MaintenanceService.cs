/**************************************************
 * Mã sinh viên: 202419021
 * Họ tên: Phạm Đình Vũ
 **************************************************/
using TeachingDeviceAllocation.Enums;
using TeachingDeviceAllocation.Events;
namespace TeachingDeviceAllocation.Services;
public class MaintenanceService
{
    private readonly List<string> requests = new();
    public IReadOnlyList<string> Requests => requests.AsReadOnly();
    public void HandleCriticalAlert(object? sender, DeviceAlertEventArgs e)
    {
        if (e.Severity == AlertSeverity.Critical)
            requests.Add($"Maintenance request for {e.DeviceId}: {e.Message}");
    }
}
