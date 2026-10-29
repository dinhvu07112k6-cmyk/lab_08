/**************************************************
 * Mã sinh viên: 202419021
 * Họ tên: Phạm Đình Vũ
 **************************************************/
using TeachingDeviceAllocation.Events;
namespace TeachingDeviceAllocation.Services;
public class DeviceAuditLogger
{
    private readonly List<string> entries = new();
    public IReadOnlyList<string> Entries => entries.AsReadOnly();
    public void HandleStatusChanged(object? sender, DeviceStatusChangedEventArgs e)
        => entries.Add($"{e.ChangedAt:O}: {e.DeviceId}, {e.OldStatus} -> {e.NewStatus}");
}
