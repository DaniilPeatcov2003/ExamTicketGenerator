# Exam Ticket Generator

> Консольное приложение на C# для случайной выдачи экзаменационных билетов и ведения журнала в Excel.

![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)
![C%23](https://img.shields.io/badge/C%23-console_app-239120?logo=csharp&logoColor=white)
![ClosedXML](https://img.shields.io/badge/Excel-ClosedXML-217346?logo=microsoftexcel&logoColor=white)

Приложение помогает быстро выдать студенту случайный билет и сохранить результат в `journal.xlsx`.

## Что делает программа

- запрашивает фамилию и имя пользователя;
- случайным образом генерирует номер билета от 1 до 20;
- сохраняет запись в файл `journal.xlsx`;
- поддерживает чтение с клавиатуры и обработку `ESC` для выхода;
- автоматически создаёт таблицу Excel и добавляет новые строки.

## Как это работает

```text
Фамилия и имя
	↓
Случайный билет 1–20
	↓
Запись в journal.xlsx
```

## Требования

- Windows, Linux или macOS;
- [.NET SDK 9](https://dotnet.microsoft.com/download/dotnet/9.0);
- доступ к папке проекта для создания и изменения `journal.xlsx`.

## Технологии

- C#
- .NET 9
- ClosedXML

## Установка и запуск

1. Убедитесь, что установлен .NET SDK 9.
2. Откройте папку проекта.
3. Выполните команду:

```bash
git clone https://github.com/DaniilPeatcov2003/ExamTicketGenerator.git
cd ExamTicketGenerator
dotnet restore
dotnet run
```

Для проверки сборки без запуска:

```bash
dotnet build
```

## Формат файла Excel

Файл `journal.xlsx` создаётся автоматически. В таблице будут колонки:

- Last name
- First name
- Номер билета
- Дата и время

## Пример работы

```text
Генератор экзаменационных билетов
Для выхода нажмите ESC.

Last name: Иванов
First name: Петр
Билет № 7
Запись сохранена в journal.xlsx.
```

## Примечание

Если файл `journal.xlsx` открыт в Excel, программа попросит закрыть его перед повторной попыткой сохранения.
