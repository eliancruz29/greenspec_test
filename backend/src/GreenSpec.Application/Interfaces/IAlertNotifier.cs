using GreenSpec.Application.DTOs;

namespace GreenSpec.Application.Interfaces;

public interface IAlertNotifier
{
    Task NotifyAlertAsync(AlertDto alert, CancellationToken cancellationToken = default);
}
