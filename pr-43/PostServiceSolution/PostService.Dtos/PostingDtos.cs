using System.ComponentModel.DataAnnotations;
namespace PostService.Dtos;
public class PostingWriteDto
{
 [Required(ErrorMessage="Вкажіть відправника"), StringLength(128)] public string From { get; set; } = "";
 [Required(ErrorMessage="Вкажіть отримувача"), StringLength(128)] public string To { get; set; } = "";
 [Required(ErrorMessage="Опишіть вміст"), StringLength(500)] public string Content { get; set; } = "";
 [RegularExpression("^(Standard|Express)$",ErrorMessage="Оберіть Standard або Express")] public string DeliveryType { get; set; } = "Standard";
 [Range(typeof(decimal),"0.001","1000",ParseLimitsInInvariantCulture=true,ErrorMessage="Вага має бути від 0.001 до 1000 кг")] public decimal Weight { get; set; } = 1;
 [Range(typeof(decimal),"0.1","1000",ParseLimitsInInvariantCulture=true,ErrorMessage="Ширина має бути від 0.1 до 1000 см")] public decimal Width { get; set; } = 10;
 [Range(typeof(decimal),"0.1","1000",ParseLimitsInInvariantCulture=true,ErrorMessage="Висота має бути від 0.1 до 1000 см")] public decimal Height { get; set; } = 10;
 [Range(typeof(decimal),"0.1","1000",ParseLimitsInInvariantCulture=true,ErrorMessage="Глибина має бути від 0.1 до 1000 см")] public decimal Depth { get; set; } = 10;
 [Range(typeof(decimal),"0","1000000",ParseLimitsInInvariantCulture=true,ErrorMessage="Цінність має бути від 0 до 1000000 грн")] public decimal Value { get; set; }
}
public class PostingGetDto : PostingWriteDto
{
 public int Id { get; set; }
 public decimal Price { get; set; }
 public DateTime CreatedAt { get; set; }
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
