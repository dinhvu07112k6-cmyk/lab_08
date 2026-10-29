/**************************************************
 * Mã sinh viên: 202419021
 * Họ tên: Phạm Đình Vũ
 **************************************************/

using TeachingDeviceAllocation.Exceptions;

namespace TeachingDeviceAllocation.Strategies;

public class HighestPriorityStrategy<T> : ISelectionStrategy<T>
{
    private readonly Func<T, double> _scoreSelector;

    public HighestPriorityStrategy(Func<T, double> scoreSelector)
    {
        _scoreSelector = scoreSelector ?? throw new ArgumentNullException(nameof(scoreSelector));
    }

    public T Select(IReadOnlyList<T> candidates)
    {
        if (candidates == null || candidates.Count == 0)
            throw new NoSuitableItemException("EmptyCandidates", "Danh sách ứng viên rỗng, không thể chọn thiết bị.");

        return candidates.OrderByDescending(_scoreSelector).First();
    }
}