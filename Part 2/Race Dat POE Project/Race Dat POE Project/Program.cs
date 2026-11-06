//------------------------------------------------------------------------
//Code written by Claude AI (Anthropic, 2026).
//Reference: Anthropic. 2026. Claude Sonnet 5.5 [Large language model].
//Available at: https://claude.ai [Accessed 16 October 2026].
//------------------------------------------------------------------------

//these are the libraries (using statements) that this file needs
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Race_Dat_POE_Project.Data;
using Race_Dat_POE_Project.Services;

//Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
//create the builder, it is used to add all the services the api needs
var builder = WebApplication.CreateBuilder(args);

//Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
//add the controllers and make enums show as words (Run, Walk) instead of numbers (0, 1)
//this is also what gives the dropdown lists in swagger
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

//Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
//register my Db class (it holds the database connection)
builder.Services.AddSingleton<Db>();

//Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
//register the repositories
//each line says: when a controller asks for the interface, give it the real SQL class
//the unit tests replace these with fake classes
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IEventRepository, EventRepository>();
builder.Services.AddSingleton<ICategoryRepository, CategoryRepository>();
builder.Services.AddSingleton<IEnrolmentRepository, EnrolmentRepository>();
builder.Services.AddSingleton<IResultRepository, ResultRepository>();

//Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
//register my TokenService class so the login can create a token
builder.Services.AddSingleton<TokenService>();

//Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
//get the secret key from appsettings.json, it is used to check the login token
string jwt_key = builder.Configuration["Jwt:Key"] ?? "";

//Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
//set up authentication so only logged in users can use the protected endpoints
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        //rules that the token must pass to be accepted
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt_key)),
            //tell the api where to find the role and the user id inside the token
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });

//Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
//turn on authorization (the [Authorize] attributes on the controllers)
builder.Services.AddAuthorization();

//Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
//add swagger so I can test the api in the browser
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    //the title shown at the top of the swagger page
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Race Dat POE Project", Version = "v1" });

    //add the Authorize button to swagger so I can paste my login token
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Login first, then paste the token here (do not type the word Bearer)."
    });

    //tell swagger to send the token with every request
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] { }
        }
    });

    //load the descriptions I wrote above each endpoint (the /// comments)
    //swagger shows them as the purpose, the data expected and the response
    string xml_file = typeof(Program).Assembly.GetName().Name + ".xml";
    string xml_path = Path.Combine(AppContext.BaseDirectory, xml_file);
    if (File.Exists(xml_path))
    {
        options.IncludeXmlComments(xml_path);
    }
});

//Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
//build the app
var app = builder.Build();

//Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
//turn on swagger and the swagger page
app.UseSwagger();
app.UseSwaggerUI();

//Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
//make the api use https
app.UseHttpsRedirection();

//Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
//authentication must come BEFORE authorization (first who are you, then what can you do)
app.UseAuthentication();
app.UseAuthorization();

//Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
//connect the controllers to the app
app.MapControllers();

//Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
//start the app
app.Run();

//Code written by Claude AI (Anthropic, 2026) - full reference in References.txt
//this line lets the unit test project start the api (it needs to see the Program class)
public partial class Program { }