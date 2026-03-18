using api.Data;
using api.Interfaces;
using api.Models;
using api.Repository;
using api.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

//Stock <-> Comment 순환방지 로직
builder.Services.AddControllers().AddNewtonsoftJson(options => {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

//DB에 접속하는 주소문법 - DB접속정보는 appsettings.json의 ConnectionStrings:DefaultConnection에서 가져와라의 뜻
//builder.Configuration - appsettings.json 전체를 메모리로 읽어온 설정 객체
builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
   options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//ASP.NET Identity 인증 시스템을 설정하는 코드
//User -> AppUser, Role -> IdentityRole로 Identity 시스템에 등록
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true; //비밀번호 숫자 최소 1개 포함
    options.Password.RequireLowercase = true; //소문자 최소 1개 포함
    options.Password.RequireUppercase = true; //대문자 최소 1개 포함
    options.Password.RequireNonAlphanumeric = true; //특수문자 최소 1개 포함
    options.Password.RequiredLength = 12; //비밀번호 최소 길이 12자
})
.AddEntityFrameworkStores<ApplicationDBContext>();
//Identity의 사용자 정보 저장을 ApplicationDBContext(db)로 하겠다라는 의미

//스키마 추가
//ASP.NET 서버가 인증을 할 때 JWT 방식으로 인증을 처리하겠다
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = 
    options.DefaultChallengeScheme =
    options.DefaultForbidScheme =
    options.DefaultScheme = 
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme; //내부적으로 "Bearer" 의미
}).AddJwtBearer(options =>
{   //JWT 토큰을 어떻게 검증할지 설정(검사하는 규칙)
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, //이 토큰을 발급한 서버가 맞는지 검사
        ValidIssuer = builder.Configuration["JWT:Issuer"], //이 서버가 발급한 토큰만 인정
        ValidateAudience = true, //이 토큰을 사용할 대상이 맞는지 검사
        ValidAudience = builder.Configuration["JWT:Audience"], //이 API용 토큰만 허용
        ValidateIssuerSigningKey = true, //JWT 서명이 위조되지 않았는지 검사(검증)
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"]!) //!는 null-forgiving 연산자
        ) //JWT 서명 검증에 사용하는 비밀키 즉, header + payload + swordfish
    };
});


//AddScoped<Interface, Implementation> - 인터페이스 -> 구현체 매핑
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<ICommnetRepository, CommentRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPortfolioRepository, PortfolioRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthentication(); //요청이 들어올 때 JWT 토큰을 검사한다
app.UseAuthorization(); //토큰 검증이 끝난 후 권한 체크 진행

app.MapControllers();

app.Run();

