using Microsoft.AspNetCore.Identity;
using MystiickWeb.Shared.Models.User;

namespace MystiickWeb.Clients.Identity;

public partial class MystiickUserStore : IUserLockoutStore<User>
{
    async Task<int> IUserLockoutStore<User>.GetAccessFailedCountAsync(User user, CancellationToken cancellationToken) => (await _userLockoutDataClient.GetUserLockoutData(user, cancellationToken)).FailedAttempts;
    async Task<bool> IUserLockoutStore<User>.GetLockoutEnabledAsync(User user, CancellationToken cancellationToken) => (await _userLockoutDataClient.GetUserLockoutData(user, cancellationToken)).AccountLocked;
    async Task<DateTimeOffset?> IUserLockoutStore<User>.GetLockoutEndDateAsync(User user, CancellationToken cancellationToken) => (await _userLockoutDataClient.GetUserLockoutData(user, cancellationToken)).LockoutEndDate;

    async Task<int> IUserLockoutStore<User>.IncrementAccessFailedCountAsync(User user, CancellationToken cancellationToken)
    {
        int output = await _userLockoutDataClient.IncrementAccessFailedCountAsync(user, cancellationToken);

        if (output >= _identity.MaxSignInAttempts)
            await SetLockoutEnabledAsync(user, true, cancellationToken);

        return output;
    }

    Task IUserLockoutStore<User>.ResetAccessFailedCountAsync(User user, CancellationToken cancellationToken) =>  _userLockoutDataClient.ResetAccessFailedCount(user, cancellationToken);
    
    Task IUserLockoutStore<User>.SetLockoutEnabledAsync(User user, bool enabled, CancellationToken cancellationToken) => SetLockoutEnabledAsync(user, enabled, cancellationToken);
    async Task  SetLockoutEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
    {
        if (enabled)
            await _userLockoutDataClient.SetLockout(user, DateTime.UtcNow.AddMinutes(30), cancellationToken);
        else
            await _userLockoutDataClient.SetLockout(user, null, cancellationToken);
    }

    Task IUserLockoutStore<User>.SetLockoutEndDateAsync(User user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken) => _userLockoutDataClient.SetLockoutEndDate(user, lockoutEnd, cancellationToken);
}
