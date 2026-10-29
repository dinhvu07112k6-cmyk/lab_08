/**************************************************
 * Mã sinh viên: 202419021
 * Họ tên: Phạm Đình Vũ
 **************************************************/
using System.Collections.Frozen;
using TeachingDeviceAllocation.Enums;
namespace TeachingDeviceAllocation.Models;
public class AirConditionerRemote : TeachingDevice
{
    public IReadOnlySet<string> CompatibleRooms { get; }
    public string AssignedRoom { get; }
    public bool IsResponsive { get; private set; } = true;
    public AirConditionerRemote(string id, string name, string storageLocation,
        IEnumerable<string> compatibleRooms, string assignedRoom) : base(id, name, storageLocation)
    {
        ArgumentNullException.ThrowIfNull(compatibleRooms);
        ArgumentException.ThrowIfNullOrWhiteSpace(assignedRoom);
        var rooms = compatibleRooms.ToArray();
        if (rooms.Any(string.IsNullOrWhiteSpace)) throw new ArgumentException("Mã phòng không hợp lệ.");
        CompatibleRooms = rooms.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
        if (!CompatibleRooms.Contains(assignedRoom)) throw new ArgumentException("Phòng được gán phải tương thích.");
        AssignedRoom = assignedRoom;
    }
    public bool IsCompatibleWith(string roomId) => CompatibleRooms.Contains(roomId);
    public void UpdateResponseState(bool isResponsive)
    {
        if (IsResponsive == isResponsive) return;
        IsResponsive = isResponsive;
        if (!isResponsive) RaiseAlert("Điều khiển điều hòa không phản hồi.", AlertSeverity.Critical);
    }
    public override bool IsReadyForUse() => Status == DeviceStatus.Available && IsResponsive;
    public override double GetDefaultPriorityScore() => 100;
}
