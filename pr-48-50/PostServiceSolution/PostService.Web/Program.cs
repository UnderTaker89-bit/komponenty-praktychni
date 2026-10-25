using System.Net;
var builder=WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();builder.Logging.AddConsole();
builder.Services.AddHttpClient("PostService",c=>{c.BaseAddress=new Uri(builder.Configuration["PostServiceUrl"]??"http://localhost:5211");c.Timeout=TimeSpan.FromSeconds(10);});
var app=builder.Build();app.UseDefaultFiles();app.UseStaticFiles();
app.MapMethods("/api/postings/{id:int?}",["GET","POST","PUT","DELETE"],async(HttpContext ctx,IHttpClientFactory factory,int? id)=>
{
 try{
  using var request=new HttpRequestMessage(new HttpMethod(ctx.Request.Method),"/postings"+(id is null?"":$"/{id}"));
  if(ctx.Request.Method is "POST" or "PUT"){request.Content=new StreamContent(ctx.Request.Body);request.Content.Headers.ContentType=new("application/json");}
  using var response=await factory.CreateClient("PostService").SendAsync(request,ctx.RequestAborted);
  ctx.Response.StatusCode=(int)response.StatusCode;ctx.Response.ContentType=response.Content.Headers.ContentType?.ToString()??"application/json";
  await response.Content.CopyToAsync(ctx.Response.Body);
 }catch(Exception e) when(e is HttpRequestException or TaskCanceledException){ctx.Response.StatusCode=503;await ctx.Response.WriteAsJsonAsync(new{title="Сервер PostService недоступний. Перевірте його запуск."});}
});
app.Run();
