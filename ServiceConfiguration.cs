using Microsoft.EntityFrameworkCore;
using RedmineApp.Models.EntityModels;
using RedmineApp.Repositories.Implement;
using RedmineApp.Repositories.Interfaces;
using RedmineApp.Services.Implement;
using RedmineApp.Services.Interfaces;

namespace RedmineApp
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {   
            //Database
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            //Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IPositionRepository, PositionRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IProjectMemberRepository, ProjectMemberRepository>();
            //
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<IPositionService, PositionService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IProjectMemberService, ProjectMemberService>();
            services.AddScoped<IEmailService, EmailService>();
            return services;
        }
    }
}
