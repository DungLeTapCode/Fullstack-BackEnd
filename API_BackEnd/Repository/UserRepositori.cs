using API_BackEnd.Data;
using API_BackEnd.Helper;
using API_BackEnd.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace API_BackEnd.Repository
{
    public class UserRepositori : IUserRepository
    {
        private readonly UserManager<ApplicationUsers> userManager;
        private readonly SignInManager<ApplicationUsers> signInManager;
        private readonly IConfiguration configuration;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly AppDbContext context;

        public UserRepositori(UserManager<ApplicationUsers> userManager,SignInManager<ApplicationUsers>signInManager,
           IConfiguration configuration,RoleManager<IdentityRole> roleManager,AppDbContext context ) 
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.roleManager = roleManager;
            this.context = context;


        }

    public async Task<List<ApplicationUsers>> GetAllUsersAsync()
    {
        // Lấy tất cả người dùng từ cơ sở dữ liệu
        var users = await userManager.Users.ToListAsync();
        return users;
    }

        public async Task<string> SignInAsync(SingInModel model)
        {
              var user = await userManager.FindByEmailAsync( model.Email );
              var passwordVaid = await userManager.CheckPasswordAsync(user,model.PassWord);
            if (user == null || !passwordVaid) { 
                  return string.Empty;
            }
           
                var authClaims = new List<Claim>
                {   

                    new Claim(ClaimTypes.Email,model.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            
                };

             var userRoles = await userManager.GetRolesAsync(user);
            foreach (var role in userRoles) { 
                   authClaims.Add(new Claim(ClaimTypes.Role,role.ToString()));
            }
            var authenkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));
            var token = new JwtSecurityToken(
                issuer: configuration["JWT:ValidIssuer"],
                audience: configuration["JWT:ValidAudience"],
                expires:DateTime.Now.AddHours(1),
                claims:authClaims,
                signingCredentials: new SigningCredentials(authenkey,SecurityAlgorithms.HmacSha256)
                );  
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

      

        public async Task<IdentityResult> SignUpAsync(SignUpModel model)
        {
            var user = new ApplicationUsers
            {
                FullName = model.FullName,
                Email = model.Email,
                UserName = model.Email,
                CreatedAt = DateTime.Now,
                Address = null,


            };
             var results = await userManager.CreateAsync(user,model.PassWord);
            if (results.Succeeded) { 
                 if(!await roleManager.RoleExistsAsync(ApplicationRole.Customer))
                {
                    await roleManager.CreateAsync(new IdentityRole(ApplicationRole.Customer));
                }
                 await userManager.AddToRoleAsync(user, ApplicationRole.Customer);
                var cart = new Cart
                {
                    UserId = user.Id, // Liên kết UserId với Id của ApplicationUsers
                    Items = new List<CartItem>() // Khởi tạo danh sách rỗng
                };

                // Lưu Cart vào cơ sở dữ liệu
                await context.Carts.AddAsync(cart);
                await context.SaveChangesAsync();
            }
            return results;
        }

        public async Task<bool> DeleteUserByIdAsync(string userId)
        {
            // Tìm người dùng theo ID
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false; // Người dùng không tồn tại
            }

            // Xóa người dùng
            var result = await userManager.DeleteAsync(user);
            return result.Succeeded; // Trả về kết quả xóa thành công hoặc không
        }
        public async Task<bool> UpdateUserInfo(string userId, string? fullName, string? phoneNumber, string? address)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return false;

            if (!string.IsNullOrEmpty(fullName))
            {
                user.FullName = fullName;
            }

            if (!string.IsNullOrEmpty(phoneNumber))
            {
                user.PhoneNumber = phoneNumber;
            }

            if (!string.IsNullOrEmpty(address))
            {
                user.Address = address;
            }

            var result = await userManager.UpdateAsync(user);
            return result.Succeeded;
        }
        public async Task<IdentityResult> CreateAdminAsync(SignUpModel model)
        {
            var user = new ApplicationUsers
            {
                FullName = model.FullName,
                Email = model.Email,
                UserName = model.Email,
                CreatedAt = DateTime.Now,
                Address = null
            };

            // Tạo tài khoản Admin
            var result = await userManager.CreateAsync(user, model.PassWord);
            if (result.Succeeded)
            {
                // Kiểm tra và tạo vai trò Admin nếu chưa tồn tại
                if (!await roleManager.RoleExistsAsync(ApplicationRole.Admin))
                {
                    await roleManager.CreateAsync(new IdentityRole(ApplicationRole.Admin));
                }

                // Gán vai trò Admin cho tài khoản
                await userManager.AddToRoleAsync(user, ApplicationRole.Admin);
            }

            return result;
        }
        public async Task<string> GetRoleByUserIdAsync(string userId)
        {
            // Tìm người dùng theo UserId
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null; // Nếu không tìm thấy người dùng, trả về null
            }

            // Lấy danh sách các vai trò của người dùng
            var roles = await userManager.GetRolesAsync(user);

            // Nếu người dùng có vai trò, trả về vai trò đầu tiên (hoặc xử lý theo yêu cầu)
            return roles.FirstOrDefault(); 
        }

        public async Task<IList<string>> GetRolesByUserNameAsync(string userName)
        {
            // Tìm người dùng theo userName
            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return new List<string>(); // Hoặc trả về danh sách rỗng: new List<string>();
            }

            // Lấy danh sách các vai trò của người dùng
            var roles = await userManager.GetRolesAsync(user);
            return roles; // Trả về danh sách vai trò
        }


    }
}
