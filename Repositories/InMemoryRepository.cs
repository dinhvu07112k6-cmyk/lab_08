/**************************************************
 * Mã sinh viên: 202419021
 * Họ tên: Phạm Đình Vũ
 **************************************************/

using TeachingDeviceAllocation.Exceptions;
using TeachingDeviceAllocation.Models;

namespace TeachingDeviceAllocation.Repositories;

public class InMemoryRepository<T> : IRepository<T> where T : IIdentifiable
{
    private readonly List<T> _items = new();

    public void Add(T item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item), "Không chấp nhận đối tượng null.");

        if (_items.Any(x => x.Id == item.Id))
            throw new DuplicateIdException(item.Id);

        _items.Add(item);
    }

    public bool Remove(string id)
    {
        var item = FindById(id);
        return item != null && _items.Remove(item);
    }

    public T? FindById(string id) => _items.FirstOrDefault(x => x.Id == id);

    public IReadOnlyList<T> GetAll() => _items.AsReadOnly();

    public IReadOnlyList<T> FindAll(Predicate<T> condition)
    {
        if (condition == null)
            return GetAll();

        return _items.FindAll(condition).AsReadOnly();
    }
}