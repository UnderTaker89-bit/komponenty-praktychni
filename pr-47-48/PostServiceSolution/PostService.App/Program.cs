using System.Net.Http.Json;
using PostService.Dtos;
namespace PostService.App;
internal static class Program
{
 [STAThread] static void Main(){ApplicationConfiguration.Initialize();Application.Run(new PostingList());}
}
public class PostingList:Form
{
 private readonly HttpClient client=new(){BaseAddress=new Uri(Environment.GetEnvironmentVariable("POSTSERVICE_URL")??"http://localhost:5211"),Timeout=TimeSpan.FromSeconds(10)};
 private readonly DataGridView gridPostings=new(){Name="gridPostings",Dock=DockStyle.Fill,ReadOnly=true,AllowUserToAddRows=false,AllowUserToDeleteRows=false,SelectionMode=DataGridViewSelectionMode.FullRowSelect,MultiSelect=false,AutoGenerateColumns=false,AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.DisplayedCells};
 private readonly Button btnRefresh=new(){Name="btnRefresh",Text="Оновити",AutoSize=true};
 private readonly Button btnDelete=new(){Name="btnDelete",Text="Видалити",AutoSize=true};
 private readonly FlowLayoutPanel actions=new(){Dock=DockStyle.Top,Height=48,Padding=new Padding(8)};
 public PostingList()
 {
  Text="PostService — Список поштових відправлень";Width=1250;Height=650;MinimumSize=new Size(850,450);StartPosition=FormStartPosition.CenterScreen;
  Controls.Add(gridPostings);Controls.Add(actions);actions.Controls.AddRange([btnRefresh,btnDelete]);
  var columns=new Dictionary<string,string>{{"Id","Код"},{"From","Відправник"},{"To","Отримувач"},{"Content","Вміст"},{"DeliveryType","Доставка"},{"Weight","Вага кг"},{"Width","Ширина см"},{"Height","Висота см"},{"Depth","Глибина см"},{"Value","Цінність грн"},{"Price","Вартість грн"},{"CreatedAt","Створено"}};
  foreach(var (key,title) in columns)gridPostings.Columns.Add(new DataGridViewTextBoxColumn{Name=key,DataPropertyName=key,HeaderText=title,Visible=key!="Id"&&key!="CreatedAt"});
  Load+=async(_,_)=>await LoadPostingsAsync();btnRefresh.Click+=async(_,_)=>await LoadPostingsAsync();btnDelete.Click+=async(_,_)=>await DeleteAsync();
  var add=new Button{Text="Нове відправлення",AutoSize=true};var edit=new Button{Text="Редагувати",AutoSize=true};actions.Controls.AddRange([add,edit]);add.Click+=async(_,_)=>await EditAsync(true);edit.Click+=async(_,_)=>await EditAsync(false);
 }
 private PostingGetDto? Selected=>gridPostings.CurrentRow?.DataBoundItem as PostingGetDto;
 private async Task LoadPostingsAsync()
 {
  actions.Enabled=false;
  try{gridPostings.DataSource=await client.GetFromJsonAsync<List<PostingGetDto>>("/postings")??[];}
  catch(Exception e) when(e is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException){MessageBox.Show(this,"Не вдалося завантажити список. Перевірте запуск сервера.\n"+e.Message,"Помилка",MessageBoxButtons.OK,MessageBoxIcon.Error);}
  finally{actions.Enabled=true;}
 }
 private async Task DeleteAsync()
 {
  if(Selected is not {} p){MessageBox.Show(this,"Оберіть відправлення");return;}
  if(MessageBox.Show(this,$"Видалити відправлення №{p.Id}?","Підтвердження видалення",MessageBoxButtons.YesNo,MessageBoxIcon.Warning)!=DialogResult.Yes)return;
  actions.Enabled=false;
  try{using var response=await client.DeleteAsync($"/postings/{p.Id}");if(!response.IsSuccessStatusCode)MessageBox.Show(this,$"Не вдалося видалити: {(int)response.StatusCode}");}
  catch(Exception e) when(e is HttpRequestException or TaskCanceledException){MessageBox.Show(this,"Помилка з’єднання: "+e.Message);}
  finally{await LoadPostingsAsync();}
 }
 private async Task EditAsync(bool create)
 {
  if(!create && Selected is null){MessageBox.Show(this,"Оберіть відправлення");return;}
  using var dialog=new PostingEdit(client,create?null:Selected);
  if(dialog.ShowDialog(this)==DialogResult.OK)await LoadPostingsAsync();
 }
 protected override void Dispose(bool disposing){if(disposing)client.Dispose();base.Dispose(disposing);}
}
