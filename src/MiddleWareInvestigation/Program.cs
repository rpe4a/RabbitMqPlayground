using MiddleWareInvestigation;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseMiddleware<CountMiddleware>();
app.Map("/", () => "Hello world!");
// app.MapGet("/first", () => "Hello first!");
// app.MapGet("/second", () => "Hello second!");

app.Run();