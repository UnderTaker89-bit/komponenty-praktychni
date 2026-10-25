using PostService.Models;
using PostService.Dtos;
using PostService.DataAccess;
namespace PostService.Business;
public class PostingService(IPostingRepository repository)
{
 public List<PostingGetDto> GetList()=>repository.GetList().Select(Map).ToList();
 public PostingGetDto? GetById(int id)=>repository.GetById(id) is {} p?Map(p):null;
 public PostingGetDto Create(PostingWriteDto d){var p=new Posting{CreatedAt=DateTime.UtcNow};Apply(p,d);repository.Create(p);return Map(p);}
 public bool Update(int id,PostingWriteDto d){var p=repository.GetById(id);if(p is null)return false;Apply(p,d);repository.Update(p);return true;}
 public bool Delete(int id)=>repository.Delete(id)>0;
 private static void Apply(Posting p,PostingWriteDto d){p.From=d.From.Trim();p.To=d.To.Trim();p.Content=d.Content.Trim();p.DeliveryType=d.DeliveryType;p.Weight=d.Weight;p.Width=d.Width;p.Height=d.Height;p.Depth=d.Depth;p.Value=d.Value;p.Price=PostingRules.CalculatePrice(d);}
 private static PostingGetDto Map(Posting p)=>new(){Id=p.Id,From=p.From,To=p.To,Content=p.Content,DeliveryType=p.DeliveryType,Weight=p.Weight,Width=p.Width,Height=p.Height,Depth=p.Depth,Value=p.Value,Price=p.Price,CreatedAt=p.CreatedAt};
}
