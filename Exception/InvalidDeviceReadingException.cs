/**************************************************
 * Mã sinh viên: 202419021
 * Họ tên: Phạm Đình Vũ
 **************************************************/
namespace TeachingDeviceAllocation.Exceptions;
public class InvalidDeviceReadingException : Exception
{
    public string DeviceId { get; }
    public int InvalidValue { get; }
    public InvalidDeviceReadingException(string deviceId, int invalidValue)
        : base($"Thiết bị {deviceId}: giá trị {invalidValue} ngoài khoảng 0–100.")
    { DeviceId = deviceId; InvalidValue = invalidValue; }
}
