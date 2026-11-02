namespace PostService.Models;
public class ModelBase { public int Id { get; set; } }
public class Posting : ModelBase
{
 public string From { get; set; } = "";
 public string To { get; set; } = "";
 public string Content { get; set; } = "";
 public string DeliveryType { get; set; } = "Standard";
 public decimal Weight { get; set; }
 public decimal Width { get; set; }
 public decimal Height { get; set; }
 public decimal Depth { get; set; }
 public decimal Value { get; set; }
 public decimal Price { get; set; }
 public DateTime CreatedAt { get; set; }
}
