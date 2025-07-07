using Ascendion.Products.Dashboard.Common;
using Ascendion.Products.Dashboard.DTO.Auth;
using Ascendion.Products.Dashboard.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace Ascendion_Dashboard_Api.Controllers;


// Controller for Authentication
[Route("api/[controller]")]
[ApiController]

// As I have used Microsoft.Identity framework which consist built in UserManager 
// UserManager is used for doing CRUD operations in database level for IdentityUser
// _tokenRepository is used to generate tokens
public class AuthController(UserManager<IdentityUser> _userManager,
    ITokenRepository _tokenRepository,
    IMapper _mapper,
    ILogger<AuthController> _logger) : ControllerBase

{
    public static ActivitySource activitySource = new ActivitySource("Dashboard.Api.Services");

    //Logic for Registering a user
    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto,CancellationToken token)
    {
        using var activity = activitySource.StartActivity("Register");
        OpenTelemetricsMeters.start(activity, HttpContext);
        try
        {
            _logger.LogInformation("LogInformation: Registering a user - {User}", registerRequestDto);

            // Create a new IdentityUser Object
            var identityUser = _mapper.Map<IdentityUser>(registerRequestDto);

            // Check whether the user already exists or not
            var user = await _userManager.FindByEmailAsync(registerRequestDto.Email);
            if (user is not null)
            {
                _logger.LogWarning("User Already exists");
                activity?.Stop();
                OpenTelemetricsMeters.authenticationCounter.Add(1, new("auth_method", "Register by Email"), new("status", "failure"));
                OpenTelemetricsMeters.IncrementError("error", $"{StatusCodes.Status400BadRequest} - Bad Request");
                return BadRequest("Email already taken");
            }

            _logger.LogDebug("Adding the user in the backend");

            // Add the user to the backend
            var identityResult = await _userManager.CreateAsync(identityUser, registerRequestDto.Password);

            OpenTelemetricsMeters.authenticationCounter.Add(1, new("auth_method", "Register by Email"), new("status", identityResult.Succeeded ? "success" : "failure"));

            // If it passes all the validations then return Message for Successful registration
            if (identityResult.Succeeded)
            {
                _logger.LogInformation("Registration successuful for {Name}", registerRequestDto.Username);
                activity?.Stop();
                return Ok("User was registered! Please login.");
            }

            _logger.LogWarning("Unable to Register User");
            activity?.Stop();
            OpenTelemetricsMeters.IncrementError("error",$"{StatusCodes.Status400BadRequest} - Bad Request");
            // Return Bad Request
            return BadRequest("Something went wrong");

        }
        catch (Exception ex) 
        {
            _logger.LogError("LogError: Failed to Register with email {Email} -  with an error - {ex}",registerRequestDto.Email, ex.Message);
            activity?.Stop();
            OpenTelemetricsMeters.IncrementError("error",$"{StatusCodes.Status500InternalServerError} - Internal Server Error");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    // Login Logic 
    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto,CancellationToken ctk)
    {
        using var activity = activitySource.StartActivity("Login");

        OpenTelemetricsMeters.start(activity, HttpContext);

        try
        {
            _logger.LogInformation("LogInformation: Logining a user with credentials - {User}", loginRequestDto);

            // Get user by email
            var user = await _userManager.FindByEmailAsync(loginRequestDto.Email);

            // If user exists
            if (user is not null)
            {
                // Comapring the passwords
                var checkPasswordResult = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

                // If Password matches then Create a Jwt Token using _tokenRepository
                if (checkPasswordResult)
                {

                    var authToken = _tokenRepository.CreateJWTToken(user, []);
                    OpenTelemetricsMeters.authenticationCounter.Add(1, new("auth_method", "Login by Email"), new("status", "success"));

                    // Create a login Response
                    var response = new LoginResponseDto
                    {
                        AuthToken = authToken,
                        Email = user.Email
                    };

                    _logger.LogInformation("The User with Email -  {Email} is susccesfully Logged in", user.Email);
                    // Return that Response
                    activity?.Stop();
                    return Ok(response);
                }
            }
            OpenTelemetricsMeters.authenticationCounter.Add(1, new("auth_method", "Login by Email"), new("status", "failure"));
            OpenTelemetricsMeters.IncrementError("error", $"{StatusCodes.Status400BadRequest} - Bad Request");
            activity?.Stop();
            return BadRequest("Email or password incorrect");
        }
        catch (Exception ex) 
        {
            _logger.LogError("LogError: Failed to Register with email {Email} -  with an error - {ex}", loginRequestDto.Email, ex.Message);
            activity?.Stop();
            OpenTelemetricsMeters.IncrementError("error", $"{StatusCodes.Status500InternalServerError} - Internal Server Error");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPost]
    [Route("ForgotPassword")]
    public async Task<IActionResult> ForgotPassword([FromBody] string Email)
    {
        var user = await _userManager.FindByEmailAsync(Email);
        if (user is null)
        {
            return BadRequest("The Email doesnot exists");
        }
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        return Ok("Reset code sent to email.");
    }
}
