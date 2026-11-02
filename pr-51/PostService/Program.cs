using Microsoft.EntityFrameworkCore;
using PostService.Business;
using PostService.DataAccess;
using PostService.Dtos;
using Swashbuckle.AspNetCore.Annotations;

var builder = WebApplication.CreateBuilder(args);
if (string.IsNullOrWhiteSpace(builder.Configuration["urls"])) builder.WebHost.UseUrls("http://localhost:5211");
builder.Services.AddDbContext<PostingDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("Postings") ?? "Data Source=postings.db"));
builder.Services.AddScoped<IPostingRepository, PostingRepository>();
builder.Services.AddScoped<PostingService>();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.EnableAnnotations());
var app = builder.Build();
app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.MapGet("/postings", (PostingService service) => Results.Ok(service.GetList()))
    .WithName("GetPostings").WithTags("Відправлення")
    .WithSummary("Список відправлень")
    .WithDescription("Повертає всі збережені поштові відправлення з розрахованою ціною доставки.")
    .Produces<List<PostingGetDto>>();

app.MapGet("/postings/{id:int}", ([SwaggerParameter("Унікальний ідентифікатор відправлення.")] int id, PostingService service) =>
    service.GetById(id) is { } posting ? Results.Ok(posting) : Results.NotFound())
    .WithName("GetPosting").WithTags("Відправлення")
    .WithSummary("Пошук відправлення")
    .WithDescription("Повертає відправлення за ідентифікатором із маршруту; якщо запис відсутній, повертає 404.")
    .Produces<PostingGetDto>().Produces(StatusCodes.Status404NotFound);

app.MapPost("/postings", ([SwaggerParameter("Параметри нового поштового відправлення.")] PostingWriteDto model, PostingService service) =>
{
    var errors = PostingRules.Validate(model);
    if (errors.Count > 0) return Results.ValidationProblem(errors);
    var posting = service.Create(model);
    return Results.Created($"/postings/{posting.Id}", posting);
})
    .WithName("CreatePosting").WithTags("Відправлення")
    .WithSummary("Створення відправлення")
    .WithDescription("Приймає JSON із параметрами посилки, перевіряє значення та розраховує ціну. Id, Price та CreatedAt визначає сервер.")
    .Accepts<PostingWriteDto>("application/json")
    .Produces<PostingGetDto>(StatusCodes.Status201Created).ProducesValidationProblem();

app.MapPut("/postings/{id:int}", ([SwaggerParameter("Ідентифікатор відправлення, яке потрібно змінити.")] int id, [SwaggerParameter("Нові значення всіх параметрів відправлення.")] PostingWriteDto model, PostingService service) =>
{
    var errors = PostingRules.Validate(model);
    if (errors.Count > 0) return Results.ValidationProblem(errors);
    return service.Update(id, model) ? Results.NoContent() : Results.NotFound();
})
    .WithName("UpdatePosting").WithTags("Відправлення")
    .WithSummary("Оновлення відправлення")
    .WithDescription("Замінює дані відправлення за id із маршруту та повторно обчислює ціну. Успішна відповідь 204 не містить тіла.")
    .Accepts<PostingWriteDto>("application/json")
    .Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound).ProducesValidationProblem();

app.MapDelete("/postings/{id:int}", ([SwaggerParameter("Ідентифікатор відправлення для видалення.")] int id, PostingService service) =>
    service.Delete(id) ? Results.NoContent() : Results.NotFound())
    .WithName("DeletePosting").WithTags("Відправлення")
    .WithSummary("Видалення відправлення")
    .WithDescription("Видаляє відправлення за ідентифікатором із маршруту. Повертає 204 при успіху або 404, якщо запису немає.")
    .Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound);

using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<PostingDbContext>().Database.EnsureCreated();
app.Run();
