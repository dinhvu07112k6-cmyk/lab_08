/**************************************************
 * Mã sinh viên: 202419021
 * Họ tên: Phạm Đình Vũ
 **************************************************/
using TeachingDeviceAllocation.Enums;
namespace TeachingDeviceAllocation.Events;
public class DeviceAlertEventArgs : EventArgs
{
    public string DeviceId { get; }
    public string Message { get; }
    public AlertSeverity Severity { get; }
    public DateTime OccurredAt { get; }
    public DeviceAlertEventArgs(string deviceId, string message, AlertSeverity severity, DateTime occurredAt)
    { DeviceId = deviceId; Message = message; Severity = severity; OccurredAt = occurredAt; }
}
