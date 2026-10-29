/**************************************************
 * Mã sinh viên: 202419021
 * Họ tên: Phạm Đình Vũ
 **************************************************/

using TeachingDeviceAllocation.Exceptions;
using TeachingDeviceAllocation.Models;
using TeachingDeviceAllocation.Repositories;
using TeachingDeviceAllocation.Strategies;

namespace TeachingDeviceAllocation.Services;

public class AllocationService<T> where T : TeachingDevice
{
    private readonly IRepository<T> _repository;
    private ISelectionStrategy<T> _strategy;

    public AllocationService(IRepository<T> repository, ISelectionStrategy<T> strategy)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
    }

    public void ChangeStrategy(ISelectionStrategy<T> newStrategy)
    {
        _strategy = newStrategy ?? throw new ArgumentNullException(nameof(newStrategy));
    }

    public T Allocate(Predicate<T>? additionalCondition = null)
    {
        // 1 & 2 Lấy danh sách và lọc thiết bị sẵn sàng sử dụng
        var allDevices = _repository.GetAll();
        var candidates = allDevices
            .Where(d => d.IsReadyForUse() && (additionalCondition == null || additionalCondition(d)))
            .ToList();

        // 3 Nếu không có thiết bị nào thỏa mãn điều kiện
        if (candidates.Count == 0)
            throw new NoSuitableItemException("NoCandidatesMatchCondition", "Không tìm thấy thiết bị nào sẵn sàng và thỏa mãn điều kiện yêu cầu.");

        // 4 Chọn thiết bị theo chiến lược
        T selected = _strategy.Select(candidates);

        // 5 Cập nhật trạng thái mượn
        selected.Borrow();

        // 6 Trả về thiết bị đã cấp phát
        return selected;
    }
}