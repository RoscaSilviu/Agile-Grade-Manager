using Microsoft.AspNetCore.Mvc;
using CatalogueServer.Repositories;

namespace CatalogueServer.Controllers
{
    /// <summary>
    /// Controller responsible for managing user profile operations.
    /// Provides endpoints for retrieving and updating user profile information.
    /// </summary>
    /// <remarks>
    /// All endpoints in this controller require authentication via Bearer token.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the ProfileController.
        /// </summary>
        /// <param name="userRepository">Repository for user operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when userRepository is null.</exception>
        public ProfileController(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        /// <summary>
        /// Retrieves the profile information for the authenticated user.
        /// </summary>
        /// <returns>
        /// 200 OK with the user's profile information;
        /// 401 Unauthorized if the authentication token is missing or invalid.
        /// </returns>
        /// <remarks>
        /// The endpoint requires a valid Bearer token in the Authorization header.
        /// Returns a sanitized version of the user profile without sensitive information.
        /// </remarks>
        [HttpGet]
        public IActionResult GetProfile()
        {
            string authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized("Missing or invalid token");
            }

            string token = authHeader.Substring("Bearer ".Length).Trim();

            var user = _userRepository.GetUserByToken(token);
            if (user == null)
            {
                return Unauthorized("Invalid token");
            }

            var result = new UserResult
            {
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Role = user.Role,
                LastLogin = user.LastLogin
            };

            return Ok(result);
        }

        /// <summary>
        /// Changes the password for the authenticated user.
        /// </summary>
        /// <param name="request">The password change request containing current and new passwords.</param>
        /// <returns>
        /// 200 OK if password changed successfully;
        /// 400 BadRequest if the request is invalid or current password is incorrect;
        /// 401 Unauthorized if the authentication token is missing or invalid.
        /// </returns>
        /// <remarks>
        /// The endpoint requires a valid Bearer token in the Authorization header.
        /// The current password must be verified before allowing the change.
        /// </remarks>
        [HttpPost("ChangePassword")]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest("Invalid request");
            }

            string authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized("Missing or invalid token");
            }

            string token = authHeader.Substring("Bearer ".Length).Trim();

            var user = _userRepository.GetUserByToken(token);
            if (user == null)
            {
                return Unauthorized("Invalid token");
            }

            if (!PasswordHelper.ValidatePassword(request.CurrentPassword, user.Password))
            {
                return BadRequest("Current password is incorrect");
            }

            user.Password = PasswordHelper.HashPassword(request.NewPassword);
            _userRepository.Update(user);

            return Ok("Password changed successfully");
        }
    }

    /// <summary>
    /// Represents a request to change a user's password.
    /// </summary>
    public class ChangePasswordRequest
    {
        /// <summary>
        /// Gets or sets the user's current password.
        /// </summary>
        /// <remarks>
        /// Must not be null or whitespace.
        /// </remarks>
        public string CurrentPassword { get; set; }

        /// <summary>
        /// Gets or sets the new password to set.
        /// </summary>
        /// <remarks>
        /// Must not be null or whitespace.
        /// Should meet the system's password complexity requirements.
        /// </remarks>
        public string NewPassword { get; set; }
    }

    /// <summary>
    /// Represents user profile information returned by the API.
    /// </summary>
    /// <remarks>
    /// This class contains only non-sensitive user information suitable for client display.
    /// </remarks>
    public class UserResult
    {
        /// <summary>
        /// Gets or sets the user's first name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the user's surname.
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the user's role in the system.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the user's last login.
        /// </summary>
        public DateTime LastLogin { get; set; }
    }
}