using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OpenId
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]UserDto user)
        {
            var identityUser = new User
            {
                UserName = user.UserName
            };

            var res = await _userManager.CreateAsync(identityUser, user.Password);

            if (res.Succeeded)
            {
                return new NoContentResult();
            }
            return NotFound();
        }
    }
}
