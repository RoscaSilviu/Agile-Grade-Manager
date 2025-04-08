namespace CatalogueServer.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using CatalogueServer.Repositories;

    /// <summary>
    /// Controller responsible for user authentication operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the AuthController.
        /// </summary>
        /// <param name="userRepository">The repository for user operations.</param>
        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Authenticates a user and generates an authentication token.
        /// </summary>
        /// <param name="request">The login credentials.</param>
        /// <returns>
        /// 200 OK with token and role if authentication successful;
        /// 401 Unauthorized if credentials are invalid.
        /// </returns>
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

    /// <summary>
    /// Represents the login request data.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the user's password.
        /// </summary>
        public string Password { get; set; }
    }
}