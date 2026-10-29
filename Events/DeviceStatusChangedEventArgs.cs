/**************************************************
 * Mã sinh viên: 202419021
 * Họ tên: Phạm Đình Vũ
 **************************************************/
using TeachingDeviceAllocation.Enums;
namespace TeachingDeviceAllocation.Events;
public class DeviceStatusChangedEventArgs : EventArgs
{
    public string DeviceId { get; }
    public DeviceStatus OldStatus { get; }
    public DeviceStatus NewStatus { get; }
    public DateTime ChangedAt { get; }
    public DeviceStatusChangedEventArgs(string deviceId, DeviceStatus oldStatus,
        DeviceStatus newStatus, DateTime changedAt)
    { DeviceId = deviceId; OldStatus = oldStatus; NewStatus = newStatus; ChangedAt = changedAt; }
}
