using System.Threading.Tasks;

namespace Flower.Backend.Services.Interfaces
{
    public interface INotificationService
    {
        Task NotifyEntityChanged(string entityName);
    }
}
