/**************************************************
 * Mã sinh viên: 202419021
 * Họ tên: Phạm Đình Vũ
 **************************************************/
using TeachingDeviceAllocation.Enums;
namespace TeachingDeviceAllocation.Models;
public class ProjectorRemote : TeachingDevice
{
    public int BatteryLevel { get; private set; }
    public ProjectorRemote(string id, string name, string storageLocation, int batteryLevel)
        : base(id, name, storageLocation)
    { ValidateReading(batteryLevel); BatteryLevel = batteryLevel; }
    public void UpdateBatteryLevel(int newLevel)
    {
        ValidateReading(newLevel);
        int old = BatteryLevel; BatteryLevel = newLevel;
        AlertReadingChange("Pin máy chiếu", old, newLevel, 20);
    }
    public override bool IsReadyForUse() => Status == DeviceStatus.Available && BatteryLevel >= 20;
    public override double GetDefaultPriorityScore() => BatteryLevel;
    public override string ToString() => $"{Id} | {Name} | Pin: {BatteryLevel}% | {Status}";
}
