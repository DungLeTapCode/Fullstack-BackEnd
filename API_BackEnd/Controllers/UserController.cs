using API_BackEnd.Data;
using API_BackEnd.Helper;
using API_BackEnd.Model;
using API_BackEnd.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repo;

        public UserController(IUserRepository  repo) {
         _repo = repo;
        }

        [HttpPost("SignUp")]

        public async Task<IActionResult> SignUp( [FromBody] SignUpModel model)
        {
            var result = await _repo.SignUpAsync(model);
            if (result.Succeeded)
            {
                return Ok(result);
            }
            return StatusCode(500);
        }

        [HttpPost("SingIn")]

        public async Task<IActionResult> SignIn(SingInModel singInModel)
        {
            var result = await _repo.SignInAsync(singInModel);
            if(string.IsNullOrEmpty(result))
            {
                return Unauthorized(result);
            }
            return Ok(result);
        }
        [HttpPost("create-admin")]
        public async Task<IActionResult> SignUpAdmin([FromBody] SignUpModel model)
        {
            var result = await _repo.CreateAdminAsync(model);
            if (result.Succeeded)
            {
                return Ok(result);
            }
            return StatusCode(500);
        }


        [HttpGet("GetAllUser")]
         public async Task<IActionResult> GetAllUser()
        {
            var users = await _repo.GetAllUsersAsync();
            if (users == null || users.Count == 0)
            {
                return NotFound("Không tìm thấy người dùng nào.");
            }
            var userResponses = users.Select(user => new UsersModel
            {   Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                UserName = user.UserName,
                CreatedAt = user.CreatedAt,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,

            }).ToList();

            return Ok(userResponses);
        }
        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _repo.DeleteUserByIdAsync(id);
            if (!result)
            {
                return NotFound($"Không tìm thấy người dùng với ID: {id}");
            }

            return Ok($"Xóa người dùng với ID: {id} thành công");
        }
        [HttpPut("UpdateUserInfo/{id}")]
        public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateUserRequest model)
        {
            if (string.IsNullOrEmpty(model.UserId))
            {
                return BadRequest("UserId không được để trống.");
            }

            var result = await _repo.UpdateUserInfo(model.UserId, model.FullName, model.PhoneNumber, model.Address);

            if (!result)
            {
                return NotFound("Không tìm thấy người dùng hoặc cập nhật thất bại.");
            }

            return Ok("Cập nhật thông tin người dùng thành công.");
        }
        [HttpGet("get-role/{userId}")]
        public async Task<IActionResult> GetRoleByUserId(string userId)
        {
            var role = await _repo.GetRoleByUserIdAsync(userId);

            if (role == null)
            {
                return NotFound("Role not found for this user.");
            }

            return Ok(role); // Trả về vai trò của người dùng
        }
        [HttpGet("roles/{userName}")]
        public async Task<IActionResult> GetRolesByUserName(string userName)
        {
            // Gọi hàm từ repository
            var roles = await _repo.GetRolesByUserNameAsync(userName);

            // Trả về mảng rỗng nếu không tìm thấy người dùng hoặc không có vai trò
            if (roles == null)
            {
                roles = new List<string>(); // Hoặc new List<object>() nếu roles có kiểu khác
            }

            return Ok(roles);
        }




    }
}
