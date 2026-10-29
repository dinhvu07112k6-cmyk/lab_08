/**************************************************
 * Mã sinh viên: 202419021
 * Họ tên: Phạm Đình Vũ
 **************************************************/
using TeachingDeviceAllocation.Events;
namespace TeachingDeviceAllocation.Services;
public class ConsoleNotificationService
{
    public void HandleAlert(object? sender, DeviceAlertEventArgs e)
        => Console.WriteLine($"[{e.Severity}] {e.DeviceId}: {e.Message}");
}
