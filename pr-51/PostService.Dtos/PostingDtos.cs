using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
namespace PostService.Dtos;
public class PostingWriteDto
{
 [Required(ErrorMessage="Вкажіть відправника"), StringLength(128)] [SwaggerSchema("Відправник або адреса відправлення, до 128 символів.")] public string From { get; set; } = "";
 [Required(ErrorMessage="Вкажіть отримувача"), StringLength(128)] [SwaggerSchema("Одержувач або адреса доставки, до 128 символів.")] public string To { get; set; } = "";
 [Required(ErrorMessage="Опишіть вміст"), StringLength(500)] [SwaggerSchema("Опис вмісту посилки, до 500 символів.")] public string Content { get; set; } = "";
 [RegularExpression("^(Standard|Express)$",ErrorMessage="Оберіть Standard або Express")] [SwaggerSchema("Спосіб доставки: Standard або Express.")] public string DeliveryType { get; set; } = "Standard";
 [Range(typeof(decimal),"0.001","1000",ParseLimitsInInvariantCulture=true,ErrorMessage="Вага має бути від 0.001 до 1000 кг")] [SwaggerSchema("Фактична вага посилки в кілограмах, від 0.001 до 1000.")] public decimal Weight { get; set; } = 1;
 [Range(typeof(decimal),"0.1","1000",ParseLimitsInInvariantCulture=true,ErrorMessage="Ширина має бути від 0.1 до 1000 см")] [SwaggerSchema("Ширина посилки в сантиметрах, від 0.1 до 1000.")] public decimal Width { get; set; } = 10;
 [Range(typeof(decimal),"0.1","1000",ParseLimitsInInvariantCulture=true,ErrorMessage="Висота має бути від 0.1 до 1000 см")] [SwaggerSchema("Висота посилки в сантиметрах, від 0.1 до 1000.")] public decimal Height { get; set; } = 10;
 [Range(typeof(decimal),"0.1","1000",ParseLimitsInInvariantCulture=true,ErrorMessage="Глибина має бути від 0.1 до 1000 см")] [SwaggerSchema("Довжина посилки в сантиметрах, від 0.1 до 1000.")] public decimal Depth { get; set; } = 10;
 [Range(typeof(decimal),"0","1000000",ParseLimitsInInvariantCulture=true,ErrorMessage="Цінність має бути від 0 до 1000000 грн")] [SwaggerSchema("Оголошена цінність у гривнях, від 0 до 1000000.")] public decimal Value { get; set; }
}
public class PostingGetDto : PostingWriteDto
{
 [SwaggerSchema("Унікальний ідентифікатор, який призначає сервер.")] public int Id { get; set; }
 [SwaggerSchema("Розрахована сервером ціна доставки у гривнях.")] public decimal Price { get; set; }
 [SwaggerSchema("Дата і час створення відправлення в UTC.")] public DateTime CreatedAt { get; set; }
}
public static class PostingRules
{
 public static Dictionary<string,string[]> Validate(PostingWriteDto dto)
 {
  var results=new List<ValidationResult>();
  Validator.TryValidateObject(dto,new ValidationContext(dto),results,true);
  if(string.IsNullOrWhiteSpace(dto.DeliveryType)) results.Add(new("Оберіть тип доставки",new[]{"DeliveryType"}));
  return results.SelectMany(r=>r.MemberNames.Select(n=>(Name:n,Message:r.ErrorMessage??"Некоректне значення"))).GroupBy(r=>r.Name).ToDictionary(g=>g.Key,g=>g.Select(r=>r.Message).ToArray());
 }
 public static decimal CalculatePrice(PostingWriteDto d) => decimal.Round((40m+d.Weight*10m)*(d.DeliveryType=="Express"?1.5m:1m)+d.Value*0.01m,2,MidpointRounding.AwayFromZero);
}
