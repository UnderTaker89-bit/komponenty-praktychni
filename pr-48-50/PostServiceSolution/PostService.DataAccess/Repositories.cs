using Microsoft.EntityFrameworkCore;
using PostService.Models;
namespace PostService.DataAccess;
public class PostingDbContext : DbContext
{
 public PostingDbContext(DbContextOptions<PostingDbContext> options):base(options){}
 public DbSet<Posting> Postings => Set<Posting>();
 protected override void OnConfiguring(DbContextOptionsBuilder options)
 { if(!options.IsConfigured) options.UseSqlite("Data Source=postings-ef.db"); }
}
public interface IGenericRepository<TModel> where TModel:ModelBase
{
 List<TModel> GetList(); TModel? GetById(int id); int Create(TModel model); void Update(TModel model); int Delete(int id);
}
public interface IPostingRepository : IGenericRepository<Posting> {}
public class GenericEfRepository<TModel>(DbContext context):IGenericRepository<TModel> where TModel:ModelBase
{
 public List<TModel> GetList()=>context.Set<TModel>().ToList();
 public TModel? GetById(int id)=>context.Set<TModel>().FirstOrDefault(m=>m.Id==id);
 public int Create(TModel model) {context.Set<TModel>().Add(model);context.SaveChanges();return model.Id;}
 public void Update(TModel model){context.Set<TModel>().Update(model);context.SaveChanges();}
 public int Delete(int id){var model=GetById(id);if(model is null)return 0;context.Set<TModel>().Remove(model);return context.SaveChanges();}
}
public class PostingRepository(PostingDbContext context):GenericEfRepository<Posting>(context),IPostingRepository {}
