using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TaskManager.Cloud;
using TaskManager.Data;
using TaskManager.Hubs;
using TaskManager.Security;
using TaskManager.Services.Auth;
using TaskManager.Services.Boards;
using TaskManager.Services.ChatRealtimeNotifier;
using TaskManager.Services.Columns;
using TaskManager.Services.CurrentUser;
using TaskManager.Services.Files;
using TaskManager.Services.MessageReads;
using TaskManager.Services.Notifications;
using TaskManager.Services.ProjectAuthorization;
using TaskManager.Services.ProjectMembers;
using TaskManager.Services.ProjectMessages;
using TaskManager.Services.Projects;
using TaskManager.Services.UserProfile;
using TaskManager.Services.WorkItemAssignees;
using TaskManager.Services.WorkItemMessages;
using TaskManager.Services.WorkItems;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => options
    .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("auth", o =>
    {
        o.PermitLimit = 15;                        
        o.Window = TimeSpan.FromMinutes(1);
        o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        o.QueueLimit = 0; 
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsync(
            "Слишком много попыток. Подождите две минуты.", token);
    };
});

builder.Services.AddHttpContextAccessor(); 

builder.Services.AddHostedService<DeadlineCheckerService>();
builder.Services.AddSignalR();

builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("Cloudinary"));

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IProjectAuthorizationService, ProjectAuthorizationService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IProjectMemberService, ProjectMemberService>();
builder.Services.AddScoped<IProjectMessageService, ProjectMessageService>();
builder.Services.AddScoped<IWorkItemService, WorkItemService>();
builder.Services.AddScoped<IWorkItemAssigneeService, WorkItemAssigneeService>();
builder.Services.AddScoped<IWorkItemMessageService, WorkItemMessageService>();
builder.Services.AddScoped<IBoardService, BoardService>();
builder.Services.AddScoped<IColumnService, ColumnService>();
builder.Services.AddScoped<INotificationSender, NotificationSender>();
builder.Services.AddScoped<IChatRealtimeNotifier, ChatRealtimeNotifier>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IMessageReadService, MessageReadService>();

builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtOptions = builder.Configuration
            .GetSection("JwtOptions")
            .Get<JwtOptions>() ?? throw new Exception("JwtOptions не настроены");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtOptions!.Issuer,
            ValidAudience = jwtOptions.Audience,

            IssuerSigningKey = new SymmetricSecurityKey(Encoding
                .UTF8.GetBytes(jwtOptions.SecretKey))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/hubs/notifications"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5268",
                "http://localhost:5165",
                "https://localhost:5268",
                "https://localhost:5165")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Name = "Bearer",
                In = ParameterLocation.Header,
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();
