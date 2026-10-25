# Практична робота 43

## Entity Framework Core та узагальнений репозиторій

Предмет Компоненти. Номер у списку 14.

Результат — окремий робочий знімок PostServiceSolution з SQLite й EF Core 10.0.13. Початкову реалізацію ADO.NET залишено в ПР-42. Клас PostingService і DTO однакові в обох роботах; замінюється спосіб збереження, а не тариф чи API.

### Зміни

ModelBase містить Id; Posting успадковує його. PostingDbContext має DbSet<Posting>, отримує налаштування через DI, а OnConfiguring задає запасний SQLite рядок, якщо options не налаштовані. IGenericRepository<TModel> обмежено ModelBase та містить GetList, GetById, Create, Update, Delete. IPostingRepository успадковує IGenericRepository<Posting>. GenericEfRepository працює з контекстом через Set<TModel>(), LINQ і SaveChanges. PostingRepository лише передає PostingDbContext базовому класу. У Program.cs зареєстровано AddDbContext, scoped репозиторій і сервіс; перед app.Run створено scope та викликано Database.EnsureCreated.

Нова база postings-ef.db відокремлена від postings-ado.db. Старі користувацькі дані не видаляються. Кожен клієнт працює через ті самі DTO і REST маршрути.

### Перевірка і запуск

Рішення збирається без помилок. У фактичному тесті POST повернув 201, GET прочитав дані, PUT змінив вміст, від’ємні й нульові неприпустимі параметри дали 400. Після зупинки та повторного запуску запис залишився в SQLite. DELETE повернув 204; наступний GET — 404. Повторити можна командами у README та запитами http-requests.rest, підставивши реальний Id із POST. Протокол — Додаткова перевірка.json у корені.

### Самоконтроль

1. ORM відображає об’єкти в таблиці та приховує рутинне формування SQL і заповнення моделей; ADO.NET дає прямий доступ до команд і читача.
2. DbContext — одиниця роботи з налаштуваннями, запитами й відстеженням; DbSet описує запит до таблиці, а не просто список у пам’яті.
3. EnsureCreated створює схему, якщо вона ще не ініціалізована. За наявності таблиць не оновлює їх відповідно до моделі; для еволюції застосовують міграції. Саме існування порожнього файла не є достатнім критерієм, що база вже готова.
4. Обмеження ModelBase гарантує доступність Id у будь-якого TModel.
5. Set<TModel>() дозволяє одному класу працювати з різними таблицями без жорсткої прив’язки до Postings.
6. Change tracking відстежує стан сутностей; SaveChanges перетворює накопичені зміни на SQL команди.
7. Після вставки EF отримує згенерований ключ і записує його назад у model.Id.
8. PostingRepository став спеціалізацією generic класу, PostingService зберіг свої виклики контракту репозиторію.
9. У DI додано scoped DbContext; ручний CreateDb замінено EnsureCreated у scope перед запуском.

Джерело уточнення EnsureCreated: https://learn.microsoft.com/en-us/ef/core/managing-schemas/ensure-created
