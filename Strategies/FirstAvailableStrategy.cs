/**************************************************
 * Mã sinh viên: 202419021
 * Họ tên: Phạm Đình Vũ
 **************************************************/

using TeachingDeviceAllocation.Exceptions;

namespace TeachingDeviceAllocation.Strategies;

public class FirstAvailableStrategy<T> : ISelectionStrategy<T>
{
    public T Select(IReadOnlyList<T> candidates)
    {
        if (candidates == null || candidates.Count == 0)
            throw new NoSuitableItemException("EmptyCandidates", "Danh sách ứng viên rỗng, không thể chọn thiết bị.");

        return candidates[0];
    }
}