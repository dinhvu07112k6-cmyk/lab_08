/**************************************************
 * Mã sinh viên: 202419021
 * Họ tên: Phạm Đình Vũ
 **************************************************/
using TeachingDeviceAllocation.Delegates;
using TeachingDeviceAllocation.Events;
namespace TeachingDeviceAllocation.Services;
public class FilteredAlertSubscriber
{
    private readonly AlertFilter filter;
    private readonly Action<DeviceAlertEventArgs> action;
    public FilteredAlertSubscriber(AlertFilter filter, Action<DeviceAlertEventArgs> action)
    {
        this.filter = filter ?? throw new ArgumentNullException(nameof(filter));
        this.action = action ?? throw new ArgumentNullException(nameof(action));
    }
    public void HandleAlert(object? sender, DeviceAlertEventArgs e)
    { if (filter(e)) action(e); }
}
