using IncidentAPI;
using IncidentAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/incident", async (string url) =>
{
    if (!url.Contains("https://report.bitninja.com/incident-report/"))
    {
        return Results.BadRequest("Bad link");
    }


    var id = url.Split("incident-report/")[1].Split('?')[0];

    var htmlContext = await GetWebsiteBody.GetHtmlContextByUrl($"https://admin.bitninja.io/iphistory/incidentReport?details={id}");
    var htmlNodes = await GetHtmlNodesByTag.FromHtmlText(htmlContext, "td");

    var incidentList = new List<Incident>();

    htmlNodes.ForEach(n =>
    {
        Incident incident = new(n);
        if (Incident.IsComplete(incident))
        {
            var incidentInLast14Days = Incident.IsInLast14Days(incident);

            if (incidentInLast14Days)
            {
                incidentList.Add(incident);
            }
        }
    });

    return Results.Ok(incidentList);
})
.WithName("GetIncidens");

app.Run();