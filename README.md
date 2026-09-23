<div align="center">

# 🎓 Exam Ticket Generator

### Случайная выдача экзаменационных билетов и журнал результатов в Excel

> Веб-приложение на C# для случайной выдачи экзаменационных билетов и ведения журнала в Excel.

![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)
![C%23](https://img.shields.io/badge/C%23-console_app-239120?logo=csharp&logoColor=white)
![ClosedXML](https://img.shields.io/badge/Excel-ClosedXML-217346?logo=microsoftexcel&logoColor=white)

</div>

Приложение помогает быстро выдать студенту случайный билет через браузер и сохранить результат в `journal.xlsx`.

## ✨ Возможности

| Функция | Описание |
| --- | --- |
| 👤 Данные студента | Запрашивает фамилию и имя |
| 🎲 Билет | Генерирует случайный номер от 1 до 20 |
| 📊 Журнал | Сохраняет результат в `journal.xlsx` |
| 🖥️ UI | Веб-форма с адаптивным интерфейсом и таблицей журнала |
| 🧪 Selenium | Проверяет реальный сценарий выдачи билета в Chromium |
| 🔁 Повторная запись | Добавляет новые результаты в свободные строки |

## 🔄 Как это работает

```text
Фамилия и имя
	↓
Случайный билет 1–20
	↓
Запись в journal.xlsx
```

## 🧰 Требования

- Windows, Linux или macOS;
- [.NET SDK 9](https://dotnet.microsoft.com/download/dotnet/9.0);
- доступ к папке проекта для создания и изменения `journal.xlsx`.

## 🛠️ Технологии

- C#
- .NET 9
- ClosedXML
- SQL Server LocalDB
- Entity Framework Core
- ASP.NET Core Minimal API
- HTML, CSS и JavaScript
- Selenium WebDriver

## 🚀 Установка и запуск

1. Убедитесь, что установлен .NET SDK 9.
2. Откройте папку проекта.
3. Выполните команду:

```bash
git clone https://github.com/DaniilPeatcov2003/ExamTicketGenerator.git
cd ExamTicketGenerator
dotnet restore
dotnet run
```

После запуска откройте адрес, который показан в терминале, например `http://localhost:5080`.

Для фиксированного локального адреса в PowerShell:

```powershell
$env:ASPNETCORE_URLS="http://localhost:5080"
dotnet run
```

После этого оставьте терминал запущенным и откройте в браузере:

<http://localhost:5080>

Для проверки сборки без запуска:

```bash
dotnet build
```

## 🗄️ SQL Server LocalDB

Приложение автоматически создаёт базу `ExamTickets` и таблицу `Tickets` при первом запуске. Строка подключения находится в `appsettings.json` и использует стандартный экземпляр LocalDB:

```text
Server=(localdb)\MSSQLLocalDB;Database=ExamTickets;Trusted_Connection=True;TrustServerCertificate=True;
```

Чтобы посмотреть данные в SQL Server Management Studio:

1. Откройте SSMS.
2. В поле **Server type** выберите `Database Engine`.
3. В поле **Server name** укажите `(localdb)\MSSQLLocalDB`.
4. Выберите **Windows Authentication**.
5. Подключитесь к серверу.
6. Откройте `Databases → ExamTickets → Tables → dbo.Tickets`.

Запрос для просмотра выданных билетов:

```sql
SELECT Id, LastName, FirstName, TicketNumber, CreatedAt
FROM dbo.Tickets
ORDER BY CreatedAt DESC;
```

SQL Server является основным хранилищем данных. `journal.xlsx` используется как дополнительный Excel-журнал. Если Excel-файл открыт, запись всё равно сохраняется в SQL Server, а интерфейс показывает предупреждение.

## 🧪 Selenium-тест

Selenium-тест находится в папке `SeleniumTests/`. Он открывает приложение в headless Chromium, заполняет форму, проверяет номер билета от 1 до 20 и убеждается, что новая запись появилась в таблице.

Для Selenium используются отдельная база `ExamTicketsTest` и отдельный файл `selenium-journal.xlsx`. Это важнее, чем отдельная схема `dbo`: `dbo` является схемой внутри базы, а отдельная база полностью изолирует тестовые записи от журнала студентов.

Сначала остановите обычный запуск приложения сочетанием `Ctrl+C`, затем запустите его с отдельной тестовой базой и журналом:

```powershell
$env:ASPNETCORE_URLS="http://localhost:5080"
$env:ConnectionStrings__DefaultConnection="Server=(localdb)\MSSQLLocalDB;Database=ExamTicketsTest;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
dotnet run -- --JournalFile=selenium-journal.xlsx
```

Не закрывая этот терминал, откройте второй терминал в папке проекта и выполните:

```powershell
dotnet run --project SeleniumTests/SeleniumTests.csproj -- http://localhost:5080
```

После теста остановите сервер через `Ctrl+C`. Для обычной работы запустите новый терминал с настройками студентов:

```powershell
Remove-Item Env:ConnectionStrings__DefaultConnection -ErrorAction SilentlyContinue
$env:ASPNETCORE_URLS="http://localhost:5080"
dotnet run
```

В результате:

| Назначение | База | Excel-файл |
| --- | --- | --- |
| Реальные студенты | `ExamTickets` | `journal.xlsx` |
| Selenium-тесты | `ExamTicketsTest` | `selenium-journal.xlsx` |

Для работы теста нужен установленный Chrome или Chromium. Selenium Manager автоматически подбирает совместимый ChromeDriver.

## 📁 Формат файла Excel

Файл `journal.xlsx` создаётся автоматически. В нём используются следующие колонки:

| Колонка | Значение |
| --- | --- |
| `Last name` | Фамилия студента |
| `First name` | Имя студента |
| `Номер билета` | Случайный номер от 1 до 20 |
| `Дата и время` | Момент выдачи билета |

## 🖥️ Пример работы

```text
Откройте веб-страницу, введите имя и фамилию, затем нажмите **Выдать билет**.

После отправки интерфейс показывает номер билета, статус сохранения и обновлённую таблицу журнала.
```

## ⚠️ Важно

Если файл `journal.xlsx` открыт в Excel, программа попросит закрыть его перед повторной попыткой сохранения.
