using API_BackEnd.Data;
using API_BackEnd.Model;
using Microsoft.AspNetCore.Identity;

namespace API_BackEnd.Repository
{
    public interface IUserRepository
    {
        public Task<IdentityResult> SignUpAsync(SignUpModel model);
        public Task<IdentityResult> CreateAdminAsync(SignUpModel model);
        public Task<String> SignInAsync(SingInModel model);

        Task<List<ApplicationUsers>> GetAllUsersAsync();
        Task<bool> DeleteUserByIdAsync(string userId);
        Task<bool> UpdateUserInfo(string userId, string? fullName, string? phoneNumber, string? address);
        Task<string> GetRoleByUserIdAsync(string userId);

        public Task<IList<string>> GetRolesByUserNameAsync(string userName);



    }
}
