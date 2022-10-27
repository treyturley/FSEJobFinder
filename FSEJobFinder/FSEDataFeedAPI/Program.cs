using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using System.Reflection;

using FSEDataFeedAPI;

// Web API Docs here:https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-6.0

// TODO: review logging
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-6.0

// TODO: review caching
// https://learn.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-6.0

// TODO: review HTTPS docs
// https://learn.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?source=recommendations&view=aspnetcore-6.0&tabs=visual-studio
// https://learn.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-6.0

// TODO: more help with swagger doc
// https://learn.microsoft.com/en-us/training/modules/improve-api-developer-experience-with-swagger/?ranMID=46131&ranEAID=a1LgFw09t88&ranSiteID=a1LgFw09t88-ReOh2nMazBzo9sGFOckRRw&epi=a1LgFw09t88-ReOh2nMazBzo9sGFOckRRw&irgwc=1&OCID=AID2200057_aff_7806_1243925&tduid=(ir__d3pswffmyckf6gzkowcr2wotf32xqrrarh1g0ybx00)(7806)(1243925)(a1LgFw09t88-ReOh2nMazBzo9sGFOckRRw)()&irclickid=_d3pswffmyckf6gzkowcr2wotf32xqrrarh1g0ybx00

// NOTE: to allow the app to be accessed by IP, run from commandline with:
//       dotnet run --urls http://0.0.0.0:7152

// NOTE: https://letsencrypt.org/ 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin();
            policy.AllowAnyHeader();
        });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo {
        Version = "v1",
        Title = "FSEDataFeedAPI",
        Description = "An ASP.NET Core Web API for getting job information from the FSE Game World."
        // TODO: once the github.io page has a contact link add it here
        //Contact = ""
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    //adds filter to check for fse-access-key header in all requests
    options.OperationFilter<AddHeaderParameter>();
});

var app = builder.Build();

// Toggle envs
//app.Environment.EnvironmentName = "Production";

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    //in prod, errors that cause exceptions go to this exception handler.
    app.UseExceptionHandler("/error");
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
