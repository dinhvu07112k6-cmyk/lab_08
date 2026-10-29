/**************************************************
 * Mã sinh viên: 202419021
 * Họ tên: Phạm Đình Vũ
 **************************************************/
using TeachingDeviceAllocation.Enums;
namespace TeachingDeviceAllocation.Models;
public class WirelessMicrophone : TeachingDevice
{
    public int BatteryLevel { get; private set; }
    public int SignalQuality { get; private set; }
    public WirelessMicrophone(string id, string name, string storageLocation, int batteryLevel, int signalQuality)
        : base(id, name, storageLocation)
    { ValidateReading(batteryLevel); ValidateReading(signalQuality); BatteryLevel = batteryLevel; SignalQuality = signalQuality; }
    public void UpdateBatteryLevel(int newLevel)
    {
        ValidateReading(newLevel);
        int old = BatteryLevel; BatteryLevel = newLevel;
        AlertReadingChange("Pin micro", old, newLevel, 30);
    }
    public void UpdateSignalQuality(int newQuality)
    {
        ValidateReading(newQuality);
        int old = SignalQuality; SignalQuality = newQuality;
        AlertReadingChange("Tín hiệu micro", old, newQuality, 40);
    }
    public override bool IsReadyForUse() => Status == DeviceStatus.Available && BatteryLevel >= 30 && SignalQuality >= 40;
    public override double GetDefaultPriorityScore() => (BatteryLevel + SignalQuality) / 2.0;
    public override string ToString() => $"{Id} | {Name} | Pin: {BatteryLevel}% | Sóng: {SignalQuality}% | {Status}";
}
