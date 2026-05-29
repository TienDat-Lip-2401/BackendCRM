using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RedmineApp.Models.CommonModels;
using RedmineApp.Repositories.Interfaces;
using RedmineApp.Services.Interfaces;

namespace RedmineApp.Filter
{
    public class AuthorizeRoleFilter : IAsyncActionFilter
    {
        private readonly string[] _allowedRoles;
        private readonly ICommonService _commonService;
        private readonly IUserRepository _userRepository;
        public AuthorizeRoleFilter(string[] roles,
            ICommonService commonService,
            IUserRepository userRepository)
        {
            _allowedRoles = roles;
            _commonService = commonService;
            _userRepository = userRepository;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            int userId = _commonService.GetUserId();
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || !_allowedRoles.Contains(_commonService.GetPosition()))
            {
                context.Result = new ObjectResult(ApiResponse.Response(403, "You do not have permission to perform this action."))
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }
            await next();
        }
    }
}
