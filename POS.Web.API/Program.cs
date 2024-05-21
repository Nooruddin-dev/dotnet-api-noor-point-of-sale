


// Add services to the container.

using QRCode.Noor.API.Helpers;



var builder = WebApplication.CreateBuilder(args);

ServiceExtensions.ConfigureIServiceCollection(builder);

var app = builder.Build();
ServiceExtensions.ConfigureWebApplication(app);


app.Run();

