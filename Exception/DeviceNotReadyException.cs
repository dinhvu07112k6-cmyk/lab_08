/**************************************************
 * Mã sinh viên: 202419021
 * Họ tên: Phạm Đình Vũ 
 **************************************************/

using TeachingDeviceAllocation.Enums;

namespace TeachingDeviceAllocation.Exceptions;

public class DeviceNotReadyException : Exception
{
    public string DeviceId { get; }
    public DeviceStatus CurrentStatus { get; }

    public DeviceNotReadyException(string deviceId, DeviceStatus currentStatus)
        : base($"Thiết bị '{deviceId}' chưa sẵn sàng để cấp phát (Trạng thái hiện tại: {currentStatus}).")
    {
        DeviceId = deviceId;
        CurrentStatus = currentStatus;
    }
}