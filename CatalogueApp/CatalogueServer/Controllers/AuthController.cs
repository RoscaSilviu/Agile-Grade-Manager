namespace CatalogueServer.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using CatalogueServer.Helpers.Repositories;
    using CatalogueServer.Helpers;

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserRepository _userRepository;

        public AuthController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {

            var user = _userRepository.GetUserByEmail(request.Email);
            //user.email= request.newemail
            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }

            var hashedPassword = PasswordHelper.HashPassword(request.Password);
            if (user.Password != hashedPassword)
            {
                return Unauthorized("Invalid email or password");
            }

            user.Token = Guid.NewGuid().ToString();
            user.LastLogin = DateTime.Now;

            _userRepository.UpdateUser(user);

            return Ok(new { Token = user.Token, Role = user.Role });
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
