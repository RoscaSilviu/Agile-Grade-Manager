namespace CatalogueServer.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using CatalogueServer.Repositories;

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {

            var user = _userRepository.GetUserByEmail(request.Email);
            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }

            if (!PasswordHelper.ValidatePassword(request.Password, user.Password))
            {
                return Unauthorized("Invalid email or password");
            }

            user.Token = Guid.NewGuid().ToString();
            user.LastLogin = DateTime.Now;

            _userRepository.Update(user);

            return Ok(new { Token = user.Token, Role = user.Role });
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
