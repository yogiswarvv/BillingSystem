using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BillingSystem.Data;

namespace BillingSystem.Security
{
    public class SlimUserStore : IUserStore<IdentityUser>, 
                                 IUserPasswordStore<IdentityUser>, 
                                 IUserRoleStore<IdentityUser>,
                                 IUserEmailStore<IdentityUser>,
                                 IUserSecurityStampStore<IdentityUser>
    {
        private readonly HospitalDbContext _context;

        public SlimUserStore(HospitalDbContext context)
        {
            _context = context;
        }

        public async Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync(cancellationToken);
            return IdentityResult.Success;
        }

        public async Task<IdentityUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        }

        public async Task<IdentityUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName, cancellationToken);
        }

        public Task<string?> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken) => Task.FromResult(user.NormalizedUserName);
        public Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken) => Task.FromResult(user.Id);
        public Task<string?> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken) => Task.FromResult(user.UserName);

        public Task SetNormalizedUserNameAsync(IdentityUser user, string? normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(IdentityUser user, string? userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
            return IdentityResult.Success;
        }

        // Password Store
        public Task SetPasswordHashAsync(IdentityUser user, string? passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string?> GetPasswordHashAsync(IdentityUser user, CancellationToken cancellationToken) => Task.FromResult(user.PasswordHash);
        public Task<bool> HasPasswordAsync(IdentityUser user, CancellationToken cancellationToken) => Task.FromResult(user.PasswordHash != null);

        // Role Store
        public async Task AddToRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.NormalizedName == roleName.ToUpper(), cancellationToken);
            if (role != null)
            {
                _context.UserRoles.Add(new IdentityUserRole<string> { UserId = user.Id, RoleId = role.Id });
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task RemoveFromRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.NormalizedName == roleName.ToUpper(), cancellationToken);
            if (role != null)
            {
                var userRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id, cancellationToken);
                if (userRole != null)
                {
                    _context.UserRoles.Remove(userRole);
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
        }

        public async Task<IList<string>> GetRolesAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            var roleIds = await _context.UserRoles.Where(ur => ur.UserId == user.Id).Select(ur => ur.RoleId).ToListAsync(cancellationToken);
            return await _context.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.Name!).ToListAsync(cancellationToken);
        }

        public async Task<bool> IsInRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.NormalizedName == roleName.ToUpper(), cancellationToken);
            if (role == null) return false;
            return await _context.UserRoles.AnyAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id, cancellationToken);
        }

        public async Task<IList<IdentityUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.NormalizedName == roleName.ToUpper(), cancellationToken);
            if (role == null) return new List<IdentityUser>();
            var userIds = await _context.UserRoles.Where(ur => ur.RoleId == role.Id).Select(ur => ur.UserId).ToListAsync(cancellationToken);
            return await _context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync(cancellationToken);
        }

        // Email Store
        public Task SetEmailAsync(IdentityUser user, string? email, CancellationToken cancellationToken) { user.Email = email; return Task.CompletedTask; }
        public Task<string?> GetEmailAsync(IdentityUser user, CancellationToken cancellationToken) => Task.FromResult(user.Email);
        public Task<bool> GetEmailConfirmedAsync(IdentityUser user, CancellationToken cancellationToken) => Task.FromResult(user.EmailConfirmed);
        public Task SetEmailConfirmedAsync(IdentityUser user, bool confirmed, CancellationToken cancellationToken) { user.EmailConfirmed = confirmed; return Task.CompletedTask; }
        public Task<IdentityUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken) => _context.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
        public Task<string?> GetNormalizedEmailAsync(IdentityUser user, CancellationToken cancellationToken) => Task.FromResult(user.NormalizedEmail);
        public Task SetNormalizedEmailAsync(IdentityUser user, string? normalizedEmail, CancellationToken cancellationToken) { user.NormalizedEmail = normalizedEmail; return Task.CompletedTask; }

        // Security Stamp Store
        public Task SetSecurityStampAsync(IdentityUser user, string stamp, CancellationToken cancellationToken) { user.SecurityStamp = stamp; return Task.CompletedTask; }
        public Task<string?> GetSecurityStampAsync(IdentityUser user, CancellationToken cancellationToken) => Task.FromResult(user.SecurityStamp);

        public void Dispose() { }
    }

    public class SlimRoleStore : IRoleStore<IdentityRole>, IQueryableRoleStore<IdentityRole>
    {
        private readonly HospitalDbContext _context;
        public SlimRoleStore(HospitalDbContext context) => _context = context;

        public IQueryable<IdentityRole> Roles => _context.Roles;

        public async Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync(cancellationToken);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync(cancellationToken);
            return IdentityResult.Success;
        }

        public async Task<IdentityRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken) => await _context.Roles.FindAsync(new object[] { roleId }, cancellationToken);
        public async Task<IdentityRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken) => await _context.Roles.FirstOrDefaultAsync(r => r.NormalizedName == normalizedRoleName, cancellationToken);
        public Task<string?> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken) => Task.FromResult(role.NormalizedName);
        public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken) => Task.FromResult(role.Id);
        public Task<string?> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken) => Task.FromResult(role.Name);
        public Task SetNormalizedRoleNameAsync(IdentityRole role, string? normalizedName, CancellationToken cancellationToken) { role.NormalizedName = normalizedName; return Task.CompletedTask; }
        public Task SetRoleNameAsync(IdentityRole role, string? roleName, CancellationToken cancellationToken) { role.Name = roleName; return Task.CompletedTask; }

        public async Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            _context.Roles.Update(role);
            await _context.SaveChangesAsync(cancellationToken);
            return IdentityResult.Success;
        }

        public void Dispose() { }
    }
}
