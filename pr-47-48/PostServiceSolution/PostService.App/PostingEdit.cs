using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using PostService.Dtos;
namespace PostService.App;
public class PostingEdit:Form
{
 private readonly HttpClient client;private readonly int? id;
 private readonly Dictionary<string,Control> fields=new();
 private readonly ErrorProvider errors=new();
 private readonly Button save=new(){Text="Зберегти",AutoSize=true};
 private readonly Label price=new(){AutoSize=true};
 public PostingEdit(HttpClient client,PostingGetDto? posting)
 {
  this.client=client;id=posting?.Id;Text=id is null?"Нове відправлення":"Редагування відправлення";Width=750;Height=660;MinimumSize=new Size(700,600);StartPosition=FormStartPosition.CenterParent;errors.ContainerControl=this;
  var layout=new TableLayoutPanel{Dock=DockStyle.Fill,AutoScroll=true,ColumnCount=2,Padding=new Padding(20)};layout.ColumnStyles.Add(new(SizeType.Absolute,175));layout.ColumnStyles.Add(new(SizeType.Percent,100));Controls.Add(layout);
  var labels=new Dictionary<string,string>{{"From","Відправник"},{"To","Отримувач"},{"Content","Вміст"},{"DeliveryType","Тип доставки"},{"Weight","Вага кг"},{"Width","Ширина см"},{"Height","Висота см"},{"Depth","Глибина см"},{"Value","Оголошена цінність грн"}};
  PostingWriteDto data=posting??new PostingWriteDto(); int row=0;
  foreach(var (key,label) in labels)
  {
   Control input;
   var value=typeof(PostingWriteDto).GetProperty(key)!.GetValue(data);
   if(key=="DeliveryType"){var cb=new ComboBox{DropDownStyle=ComboBoxStyle.DropDownList};cb.Items.AddRange(["Standard","Express"]);cb.SelectedItem=value;input=cb;cb.SelectedIndexChanged+=(_,_)=>UpdatePrice();}
   else if(value is decimal number){var n=new NumericUpDown{DecimalPlaces=3,Minimum=0,Maximum=1000000,Value=number,ThousandsSeparator=true};input=n;n.ValueChanged+=(_,_)=>UpdatePrice();}
   else input=new TextBox{Text=value?.ToString(),MaxLength=key=="Content"?500:128};
   input.Name=key;input.Dock=DockStyle.Fill;input.Margin=new Padding(4,8,30,8);fields[key]=input;layout.Controls.Add(new Label{Text=label,AutoSize=true,Margin=new Padding(4,12,4,4)},0,row);layout.Controls.Add(input,1,row++);
  }
  layout.Controls.Add(new Label{Text="Вартість грн",AutoSize=true},0,row);layout.Controls.Add(price,1,row++);
  var buttons=new FlowLayoutPanel{AutoSize=true};var cancel=new Button{Text="Скасувати",DialogResult=DialogResult.Cancel,AutoSize=true};buttons.Controls.AddRange([save,cancel]);layout.Controls.Add(buttons,1,row);AcceptButton=save;CancelButton=cancel;save.Click+=async(_,_)=>await SaveAsync();UpdatePrice();
 }
 private PostingWriteDto Read()=>new(){From=fields["From"].Text,To=fields["To"].Text,Content=fields["Content"].Text,DeliveryType=fields["DeliveryType"].Text,Weight=((NumericUpDown)fields["Weight"]).Value,Width=((NumericUpDown)fields["Width"]).Value,Height=((NumericUpDown)fields["Height"]).Value,Depth=((NumericUpDown)fields["Depth"]).Value,Value=((NumericUpDown)fields["Value"]).Value};
 private void UpdatePrice(){if(fields.Count==9)price.Text=PostingRules.CalculatePrice(Read()).ToString("F2")+" (обчислюється сервером)";}
 private async Task SaveAsync()
 {
  errors.Clear();var data=Read();var issues=PostingRules.Validate(data);
  foreach(var (key,messages) in issues)if(fields.TryGetValue(key,out var control))errors.SetError(control,string.Join("; ",messages));
  if(issues.Count>0)return;save.Enabled=false;
  try
  {
   using var response=id is null?await client.PostAsJsonAsync("/postings",data):await client.PutAsJsonAsync($"/postings/{id}",data);
   if(!response.IsSuccessStatusCode){MessageBox.Show(this,"Сервер відхилив зміни: "+await response.Content.ReadAsStringAsync(),"Помилка");return;}
   DialogResult=DialogResult.OK;Close();
  }
  catch(Exception e) when(e is HttpRequestException or TaskCanceledException){MessageBox.Show(this,"Не вдалося зберегти. "+e.Message);}
  finally{save.Enabled=true;}
 }
 protected override void Dispose(bool disposing){if(disposing)errors.Dispose();base.Dispose(disposing);}
}
