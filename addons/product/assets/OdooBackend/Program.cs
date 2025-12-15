var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();

// CORS : autorise tous les origines (utile si la page est ouverte en file://)
var allowLocal = "_allowLocal";
builder.Services.AddCors(o =>
    o.AddPolicy(allowLocal, p => p
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()));

var app = builder.Build();

// Middleware pipeline
app.UseStaticFiles();

app.UseRouting();
app.UseCors(allowLocal);
app.UseAuthorization();
app.MapControllers();

app.Run();
