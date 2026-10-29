/**************************************************
 * Mã sinh viên: 202419021
 * Họ tên: Phạm Đình Vũ
 **************************************************/
using TeachingDeviceAllocation.Enums;
using TeachingDeviceAllocation.Events;
using TeachingDeviceAllocation.Exceptions;
namespace TeachingDeviceAllocation.Models;
public abstract class TeachingDevice : IIdentifiable
{
    public string Id { get; }
    public string Name { get; }
    public string StorageLocation { get; }
    public DeviceStatus Status { get; private set; }
    public event EventHandler<DeviceStatusChangedEventArgs>? StatusChanged;
    public event EventHandler<DeviceAlertEventArgs>? AlertRaised;

    protected TeachingDevice(string id, string name, string storageLocation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(storageLocation);
        Id = id; Name = name; StorageLocation = storageLocation;
        Status = DeviceStatus.Available;
    }
    public abstract bool IsReadyForUse();
    public abstract double GetDefaultPriorityScore();
    public void Borrow()
    {
        if (!IsReadyForUse()) throw new DeviceNotReadyException(Id, Status);
        ChangeStatus(DeviceStatus.Borrowed);
    }
    public void Return()
    {
        if (Status != DeviceStatus.Borrowed)
            throw new InvalidOperationException($"Thiết bị {Id} chưa được mượn.");
        ChangeStatus(DeviceStatus.Available);
    }
    public void ChangeStatus(DeviceStatus newStatus)
    {
        if (!Enum.IsDefined(newStatus)) throw new ArgumentOutOfRangeException(nameof(newStatus));
        if (newStatus == Status) return;
        var oldStatus = Status;
        Status = newStatus; // Subscriber phải nhìn thấy trạng thái mới.
        OnStatusChanged(new(Id, oldStatus, newStatus, DateTime.Now));
    }
    protected void RaiseAlert(string message, AlertSeverity severity)
        => OnAlertRaised(new(Id, message, severity, DateTime.Now));
    protected virtual void OnStatusChanged(DeviceStatusChangedEventArgs e)
        => StatusChanged?.Invoke(this, e);
    protected virtual void OnAlertRaised(DeviceAlertEventArgs e)
        => AlertRaised?.Invoke(this, e);
    protected void ValidateReading(int value)
    {
        if (value < 0 || value > 100) throw new InvalidDeviceReadingException(Id, value);
    }
    // Chỉ cảnh báo khi đi vào một vùng lỗi mới: an toàn, Warning, Critical.
    // Hồi phục về vùng an toàn không phát cảnh báo; tái phát sau hồi phục có phát.
    protected void AlertReadingChange(string reading, int oldValue, int newValue, int threshold)
    {
        int Region(int value) => value == 0 ? 2 : value < threshold ? 1 : 0;
        int region = Region(newValue);
        if (region == 0 || region == Region(oldValue)) return;
        RaiseAlert($"{reading}: {newValue}%", region == 2 ? AlertSeverity.Critical : AlertSeverity.Warning);
    }
}
