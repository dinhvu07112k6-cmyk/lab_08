/**************************************************
 * Mã sinh viên: 202419021
 * Họ tên: Phạm Đình Vũ
 **************************************************/
using System.Text;
using TeachingDeviceAllocation.Enums;
using TeachingDeviceAllocation.Events;
using TeachingDeviceAllocation.Exceptions;
using TeachingDeviceAllocation.Models;
using TeachingDeviceAllocation.Repositories;
using TeachingDeviceAllocation.Services;
using TeachingDeviceAllocation.Strategies;

Console.OutputEncoding = Encoding.UTF8;
int passed = 0, failed = 0;
void Check(string name, Action test)
{
    try { test(); passed++; Console.WriteLine($"PASS | {name}"); }
    catch (Exception ex) { failed++; Console.WriteLine($"FAIL | {name}: {ex.Message}"); }
}
void Expect(bool condition)
{ if (!condition) throw new Exception("Kết quả không đúng kỳ vọng."); }
void Throws<T>(Action action) where T : Exception
{
    try { action(); }
    catch (T) { return; }
    throw new Exception($"Cần phát sinh {typeof(T).Name}.");
}
ProjectorRemote Projector() => new("PR01", "Máy chiếu", "Kho A", 80);

Check("Trạng thái được cập nhật trước event; không phát khi giữ nguyên", () =>
{
    var p = Projector(); int count = 0;
    p.StatusChanged += (sender, e) =>
    {
        Expect(ReferenceEquals(sender, p) && e.DeviceId == p.Id);
        Expect(e.OldStatus == DeviceStatus.Available && e.NewStatus == DeviceStatus.Borrowed);
        Expect(p.Status == e.NewStatus); count++;
    };
    p.ChangeStatus(DeviceStatus.Available); p.Borrow(); p.ChangeStatus(DeviceStatus.Borrowed);
    Expect(count == 1);
});
Check("Mượn/trả giữ quy tắc W07 và phát hai sự kiện", () =>
{
    var p = Projector(); int count = 0; p.StatusChanged += (_, _) => count++;
    Throws<InvalidOperationException>(() => p.Return());
    p.Borrow(); Throws<DeviceNotReadyException>(() => p.Borrow()); p.Return();
    Expect(count == 2 && p.Status == DeviceStatus.Available);
});
Check("Máy chiếu: 20 an toàn; 19 Warning; 0 Critical; không lặp", () =>
{
    var p = Projector(); var alerts = new List<AlertSeverity>();
    p.AlertRaised += (_, e) => alerts.Add(e.Severity);
    foreach (int value in new[] { 20, 19, 10, 0, 0, 5, 20, 19 }) p.UpdateBatteryLevel(value);
    Expect(alerts.SequenceEqual(new[] { AlertSeverity.Warning, AlertSeverity.Critical,
        AlertSeverity.Warning, AlertSeverity.Warning }));
});
Check("Micro: ngưỡng pin 30, tín hiệu 40 và Critical độc lập", () =>
{
    var m = new WirelessMicrophone("M1", "Micro", "Kho", 80, 80);
    var alerts = new List<AlertSeverity>(); m.AlertRaised += (_, e) => alerts.Add(e.Severity);
    m.UpdateBatteryLevel(30); m.UpdateSignalQuality(40); Expect(alerts.Count == 0);
    m.UpdateBatteryLevel(29); m.UpdateBatteryLevel(28);
    m.UpdateSignalQuality(39); m.UpdateSignalQuality(38);
    m.UpdateBatteryLevel(0); m.UpdateSignalQuality(0);
    Expect(alerts.SequenceEqual(new[] { AlertSeverity.Warning, AlertSeverity.Warning,
        AlertSeverity.Critical, AlertSeverity.Critical }));
});
Check("Dữ liệu sai không sửa pin/tín hiệu và không phát cảnh báo", () =>
{
    var p = Projector(); var m = new WirelessMicrophone("M1", "Micro", "Kho", 80, 70);
    int count = 0; p.AlertRaised += (_, _) => count++; m.AlertRaised += (_, _) => count++;
    foreach (int value in new[] { -1, 101 })
    {
        Throws<InvalidDeviceReadingException>(() => p.UpdateBatteryLevel(value));
        Throws<InvalidDeviceReadingException>(() => m.UpdateBatteryLevel(value));
        Throws<InvalidDeviceReadingException>(() => m.UpdateSignalQuality(value));
    }
    Expect(p.BatteryLevel == 80 && m.BatteryLevel == 80 && m.SignalQuality == 70 && count == 0);
    try { p.UpdateBatteryLevel(150); }
    catch (InvalidDeviceReadingException e) { Expect(e.DeviceId == p.Id && e.InvalidValue == 150); }
});
Check("Constructor từ chối dữ liệu sai; chấp nhận biên 0 và 100", () =>
{
    Throws<InvalidDeviceReadingException>(() => new ProjectorRemote("P", "P", "Kho", -1));
    Throws<InvalidDeviceReadingException>(() => new WirelessMicrophone("M", "M", "Kho", 100, 101));
    var m = new WirelessMicrophone("M", "M", "Kho", 0, 100);
    Expect(m.BatteryLevel == 0 && m.SignalQuality == 100);
});
Check("Điều hòa: chỉ báo khi mất phản hồi; hồi phục rồi lỗi lại có báo", () =>
{
    var ac = new AirConditionerRemote("AC", "Điều hòa", "Kho", new[] { "A203" }, "A203");
    int count = 0; ac.AlertRaised += (_, e) => { Expect(e.Severity == AlertSeverity.Critical); count++; };
    ac.UpdateResponseState(false); ac.UpdateResponseState(false);
    Expect(!ac.IsReadyForUse() && count == 1);
    ac.UpdateResponseState(true); Expect(ac.IsReadyForUse());
    ac.UpdateResponseState(false); Expect(count == 2);
});
Check("Coordinator: nhiều subscriber, đăng ký lặp không nhân đôi", () =>
{
    var p = Projector(); var logger = new DeviceAuditLogger(); var maintenance = new MaintenanceService();
    using var c = new DeviceMonitoringCoordinator(new[] { p, p }, new(), logger, maintenance);
    c.Subscribe(); c.Subscribe(); p.Borrow(); p.UpdateBatteryLevel(19);
    Expect(logger.Entries.Count == 1 && maintenance.Requests.Count == 0);
    p.UpdateBatteryLevel(0); Expect(maintenance.Requests.Count == 1);
});
Check("Unsubscribe giữ handler bên ngoài; có thể đăng ký lại", () =>
{
    var p = Projector(); var logger = new DeviceAuditLogger(); int external = 0;
    p.StatusChanged += (_, _) => external++;
    using var c = new DeviceMonitoringCoordinator(new[] { p }, new(), logger, new());
    c.Subscribe(); p.Borrow(); c.Unsubscribe(); c.Unsubscribe(); p.Return();
    Expect(external == 2 && logger.Entries.Count == 1);
    c.Subscribe(); p.Borrow(); Expect(external == 3 && logger.Entries.Count == 2);
});
Check("Dispose hủy handler; gọi nhiều lần an toàn; không đăng ký lại", () =>
{
    var p = Projector(); var logger = new DeviceAuditLogger(); var maintenance = new MaintenanceService();
    var c = new DeviceMonitoringCoordinator(new[] { p }, new(), logger, maintenance);
    c.Subscribe(); c.Dispose(); c.Dispose(); p.Borrow(); p.UpdateBatteryLevel(0);
    Expect(logger.Entries.Count == 0 && maintenance.Requests.Count == 0);
    Throws<ObjectDisposedException>(() => c.Subscribe());
});
Check("Delegate AlertFilter và Action chỉ xử lý Critical", () =>
{
    var p = Projector(); int count = 0;
    var filtered = new FilteredAlertSubscriber(e => e.Severity == AlertSeverity.Critical, _ => count++);
    p.AlertRaised += filtered.HandleAlert;
    p.UpdateBatteryLevel(19); p.UpdateBatteryLevel(0); Expect(count == 1);
    p.AlertRaised -= filtered.HandleAlert; p.UpdateBatteryLevel(80); p.UpdateBatteryLevel(0);
    Expect(count == 1);
});
Check("Không có subscriber vẫn cập nhật được; enum sai bị từ chối", () =>
{
    var p = Projector(); p.UpdateBatteryLevel(0); p.ChangeStatus(DeviceStatus.UnderMaintenance);
    Throws<ArgumentOutOfRangeException>(() => p.ChangeStatus((DeviceStatus)999));
    Expect(p.Status == DeviceStatus.UnderMaintenance && p.BatteryLevel == 0);
});
Check("Handler ném ngoại lệ: trạng thái đã cập nhật, lỗi truyền về caller", () =>
{
    var p = Projector(); bool nextCalled = false;
    p.StatusChanged += (_, _) => throw new InvalidOperationException("Subscriber lỗi");
    p.StatusChanged += (_, _) => nextCalled = true;
    Throws<InvalidOperationException>(() => p.Borrow());
    Expect(p.Status == DeviceStatus.Borrowed && !nextCalled);
});
Check("Tái sử dụng repository và strategy W07, cấp phát phát event", () =>
{
    var repo = new InMemoryRepository<ProjectorRemote>();
    var a = new ProjectorRemote("A", "A", "Kho", 50); var b = new ProjectorRemote("B", "B", "Kho", 90);
    repo.Add(a); repo.Add(b); Throws<DuplicateIdException>(() => repo.Add(a));
    var logger = new DeviceAuditLogger();
    using var c = new DeviceMonitoringCoordinator(repo.GetAll(), new(), logger, new()); c.Subscribe();
    var service = new AllocationService<ProjectorRemote>(repo, new HighestPriorityStrategy<ProjectorRemote>(d => d.GetDefaultPriorityScore()));
    Expect(ReferenceEquals(service.Allocate(), b));
    service.ChangeStrategy(new FirstAvailableStrategy<ProjectorRemote>());
    Expect(ReferenceEquals(service.Allocate(), a));
    Throws<NoSuitableItemException>(() => service.Allocate());
    Expect(logger.Entries.Count == 2);
});
Console.WriteLine($"\nKẾT QUẢ: {passed} PASS, {failed} FAIL");
Environment.ExitCode = failed == 0 ? 0 : 1;
