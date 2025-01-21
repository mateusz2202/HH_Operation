using BlazorApp.Application;
using BlazorApp.Server.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp.Server;

public class Startup
{
   
    public void ConfigureServices(IServiceCollection services)
    {             
        services.AddSignalR();
        services.AddApplicationLayer();    
        services.AddControllers();
        services.AddRazorPages();            
        services.AddLazyCache();
        services.AddCurrentUserService();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {       
        app.UseExceptionHandling(env);
        app.UseHttpsRedirection();     
        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();
        app.UseRequestLocalizationByCulture();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();          
        app.UseEndpoints();      
        app.UseCors("CorsPolicy");        
    }
}