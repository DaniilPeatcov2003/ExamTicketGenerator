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

Для фиксированного локального адреса:

```powershell
$env:ASPNETCORE_URLS="http://localhost:5080"
dotnet run
```

Для проверки сборки без запуска:

```bash
dotnet build
```

## 🧪 Selenium-тест

Selenium-тест находится в папке `SeleniumTests/`. Он открывает приложение в headless Chromium, заполняет форму, проверяет номер билета от 1 до 20 и убеждается, что новая запись появилась в таблице.

Сначала запустите приложение, затем в другом терминале выполните:

```powershell
$env:JOURNAL_FILE="selenium-journal.xlsx"
dotnet run --project SeleniumTests/SeleniumTests.csproj -- http://localhost:5080
```

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
