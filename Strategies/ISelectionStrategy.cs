/**************************************************
 * Mã sinh viên: 202419021
 * Họ tên: Phạm Đình Vũ
 **************************************************/

namespace TeachingDeviceAllocation.Strategies;

public interface ISelectionStrategy<T>
{
    T Select(IReadOnlyList<T> candidates);
}