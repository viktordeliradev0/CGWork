using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CGWork.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace CGWork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        // Добавихме IConfiguration, за да можем да вземем тайния ключ от appsettings.json
        public AuthController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return NotFound();

            return Ok(new
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,             // <-- НОВО
                PhoneNumber = user.PhoneNumber,
                City = user.City ?? "Пловдив",
                Role = User.IsInRole("Manager") ? "Мениджър" : "Учител"
            });
        }
        // ... (ТУК ОСТАВА ТВОЯТ СТАР МЕТОД ЗА REGISTER, НЕ ГО ТРИЙ) ...

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                // Взимаме ролите на потребителя
                var userRoles = await _userManager.GetRolesAsync(user);

                // Създаваме списък с информация, която ще сложим в токена (име, имейл, роля)
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName!),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                // Генерираме самия токен
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    expires: DateTime.Now.AddHours(3), // Токенът важи 3 часа
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized("Невалиден имейл или парола!");
        }
        [HttpPost("register")]
        [Authorize]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            // 1. Проверяваме дали вече няма такъв имейл
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                return BadRequest("Вече съществува потребител с този имейл!");
            }
            if (!User.IsInRole("Manager"))
            {
                return Forbid("Само мениджъри могат да създават нови профили.");
            }

            // 2. Създаваме новия потребител
            var user = new ApplicationUser()
            {
                Email = model.Email,
                UserName = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                City = model.City,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            // 3. Записваме го в базата с паролата
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                // Ако гръмне заради слаба парола, взимаме точните съобщения и ги връщаме
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                return BadRequest(errors);
            }

            // 4. Добавяме му ролята (Teacher или Manager)
            await _userManager.AddToRoleAsync(user, model.Role);

            return Ok(new { Message = "Успешна регистрация!" });
        }
        // GET: api/Auth/teachers
        [HttpGet("teachers")]
        public async Task<IActionResult> GetTeachers()
        {
            // Взимаме всички потребители, които са в роля "Teacher"
            var teachers = await _userManager.GetUsersInRoleAsync("Teacher");
            var managers = await _userManager.GetUsersInRoleAsync("Manager");

            var allStaff = teachers.Concat(managers)
                 .GroupBy(u => u.Id) // Застраховаме се да няма дублажи, ако някой има и двете роли
                 .Select(g => g.First())
                 .Select(u => new
                 {
                     Id = u.Id,
                     FullName = u.FullName + (User.IsInRole("Manager") && managers.Contains(u) ? " (Мениджър)" : ""),
                     Email = u.Email,               // <-- ТОВА ТРЯБВА ДА ГО ИМА
                     PhoneNumber = u.PhoneNumber,
                     City = u.City ?? "Пловдив"
                 });

            return Ok(allStaff);
        }
        [HttpPut("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto model)
        {
            // Взимаме ID-то на логнатия потребител
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            // Намираме го в базата
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("Потребителят не е намерен.");

            // Обновяваме му данните
            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            
            if (User.IsInRole("Manager"))
            {
                user.City = model.City;
            }

            // Записваме промените
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest("Възникна грешка при запазването.");
        }
        [HttpPut("teachers/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateTeacher(string id, [FromBody] UpdateTeacherDto model)
        {
            if (!User.IsInRole("Manager"))
            {
                return Forbid("Само мениджъри могат да редактират профили!");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("Учителят не е намерен.");

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.City = model.City;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest("Грешка при обновяването на учителския профил.");
        }

        // DELETE: api/Auth/teachers/{id}
        [HttpDelete("teachers/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTeacher(string id)
        {
            if (!User.IsInRole("Manager"))
            {
                return Forbid("Само мениджъри могат да изтриват профили!");
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == id)
            {
                return BadRequest("Не можете да изтриете собствения си профил!");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("Учителят не е намерен.");

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest("Грешка при изтриването на учителския профил.");
        }


        // DTO клас за редакция на учител
        public class UpdateTeacherDto
        {
            public string FullName { get; set; } = "";
            public string PhoneNumber { get; set; } = "";
            public string City { get; set; } = "Пловдив";
        }
    }


    // Помощен клас за данните при регистрация
    public class RegisterModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = "Teacher";
        public string City { get; set; } = "Пловдив";
    }

    // НОВ помощен клас за данните при логин
    public class LoginModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class RegisterRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string City { get; set; } = "Пловдив";

        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = "Teacher";
    }
    public class UpdateProfileDto
    {
        public string FullName { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
      
        public string City { get; set; }
    }
}