/**************************************************
 * Mã sinh viên: 202419021
 * Họ tên: Phạm Đình Vũ
 **************************************************/

namespace TeachingDeviceAllocation.Exceptions;

public class NoSuitableItemException : Exception
{
    public string Reason { get; }

    public NoSuitableItemException(string reason, string message) : base(message)
    {
        Reason = reason;
    }

    public NoSuitableItemException(string message = "Không tìm thấy thiết bị nào phù hợp với yêu cầu.")
        : base(message)
    {
        Reason = message;
    }
}