using Auth.Middlewares;
using Auth.Extensions;

namespace Auth;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddApi();
        builder.AddServices();

        builder.AddCorsConfiguration();
        builder.AddCrudClient();
        builder.AddSwaggerAuthorization("Fridge App Auth API");
        builder.ConfigureAuthentication();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.UseHttpsRedirection();
        app.UseCors();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.Run();
    }
}
