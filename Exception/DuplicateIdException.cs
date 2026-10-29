/**************************************************
 * Mã sinh viên: 202419021
 * Họ tên: Phạm Đình Vũ
 **************************************************/

namespace TeachingDeviceAllocation.Exceptions;

public class DuplicateIdException : Exception
{
    public string DuplicateId { get; }

    public DuplicateIdException(string duplicateId)
        : base($"Lỗi: Thiết bị có mã định danh '{duplicateId}' đã tồn tại trong kho.")
    {
        DuplicateId = duplicateId;
    }
}