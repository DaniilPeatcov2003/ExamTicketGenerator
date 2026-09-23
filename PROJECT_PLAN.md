# План создания Exam Ticket Generator

Пошаговая инструкция по разработке генератора экзаменационных билетов, работе с Excel, Git/GitHub, Pull Request и email-уведомлениями.

## 1. Цель проекта

Создать консольное приложение, которое:

- запрашивает фамилию и имя студента;
- случайно выбирает билет от 1 до 20;
- показывает номер билета в консоли;
- сохраняет студента, билет и дату выдачи в `journal.xlsx`;
- позволяет завершить работу клавишей `Esc`;
- добавляет каждую новую выдачу в отдельную строку Excel-журнала.

## 2. Требования

Установить:

- [.NET SDK 9](https://dotnet.microsoft.com/download/dotnet/9.0);
- Git;
- аккаунт GitHub;
- Excel или другой редактор `.xlsx` для просмотра журнала.

Проверить установку:

```powershell
dotnet --version
git --version
```

## 3. Создание проекта

Создать консольный проект:

```powershell
dotnet new console -n ExamTicketGenerator
Set-Location ExamTicketGenerator
```

Если проект уже открыт в VS Code:

```powershell
dotnet restore
dotnet build
```

## 4. Подключение ClosedXML

`ClosedXML` используется для создания и изменения Excel-файла без ручного редактирования XML.

```powershell
dotnet add package ClosedXML --version 0.105.1
dotnet restore
```

В `.csproj` должна появиться зависимость:

```xml
<PackageReference Include="ClosedXML" Version="0.105.1" />
```

## 5. Построение генератора

### 5.1. Ввод данных

Программа последовательно запрашивает:

1. фамилию;
2. имя.

Пустые значения нужно отклонять. После ввода строки очищаются методом `Trim()`.

### 5.2. Выдача билета

Для выбора номера используется генератор случайных чисел:

```csharp
int ticketNumber = Random.Next(1, 21);
```

Верхняя граница `21` не включается, поэтому возможны значения от 1 до 20.

### 5.3. Выход по Esc

Метод чтения клавиатуры должен обрабатывать:

- `Enter` — завершить ввод текущего значения;
- `Backspace` — удалить последний символ;
- `Esc` — завершить приложение;
- обычные символы — добавить в текущую строку.

### 5.4. Сохранение в Excel

При сохранении:

1. открыть существующий `journal.xlsx` или создать новую книгу;
2. найти первый лист;
3. при необходимости создать заголовки;
4. найти первую свободную строку;
5. записать фамилию, имя, номер билета и дату;
6. настроить формат даты;
7. подогнать ширину столбцов;
8. сохранить файл.

Структура журнала:

| Столбец | Значение |
| --- | --- |
| `Last name` | Фамилия студента |
| `First name` | Имя студента |
| `Номер билета` | Число от 1 до 20 |
| `Дата и время` | Момент выдачи билета |

Если Excel-файл занят, программа должна показать понятное сообщение и предложить повторить сохранение после закрытия файла.

## 6. Проверка программы

Собрать проект:

```powershell
dotnet build
```

Запустить:

```powershell
dotnet run
```

Проверить вручную:

- ввод корректной фамилии и имени;
- отказ при пустой фамилии;
- отказ при пустом имени;
- номер билета находится в диапазоне 1–20;
- `Esc` завершает программу;
- первая запись создаёт `journal.xlsx`;
- следующие записи добавляются ниже предыдущих;
- дата и время записываются корректно;
- открытый в Excel файл обрабатывается без аварийного завершения.

## 7. Первый Git-коммит

Инициализировать репозиторий из папки проекта:

```powershell
git init
git add .
git commit -m "Initial commit"
```

Перед `git add .` проверить состояние:

```powershell
git status
```

В `.gitignore` не следует добавлять результаты сборки:

```gitignore
bin/
obj/
.vs/
*.user
*.suo
```

Временный файл Excel вида `~$journal.xlsx` также не должен попадать в Git. Его можно добавить в `.gitignore`:

```gitignore
~$*.xlsx
```

## 8. Подключение GitHub

Добавить удалённый репозиторий и использовать ветку `main`:

```powershell
git remote add origin https://github.com/DaniilPeatcov2003/ExamTicketGenerator.git
git branch -M main
git push -u origin main
```

Проверить связь:

```powershell
git remote -v
git branch -vv
git status
```

## 9. Обычный рабочий цикл

После изменения кода или журнала:

```powershell
git status
git diff
git add Program.cs journal.xlsx
git commit -m "Describe the change"
git push origin main
```

Примеры сообщений коммитов:

```text
Initial commit
Add Excel ticket journal
Improve ticket input validation
Update exam ticket journal
Polish README presentation
Add push email notification workflow
```

Перед коммитом не добавлять временный файл Excel:

```powershell
git add journal.xlsx
```

Так в коммит попадёт только нужный файл.

## 10. Работа через отдельную ветку и Pull Request

Для отдельной задачи создать ветку:

```powershell
git checkout main
git pull origin main
git checkout -b feature/readme-update
```

После изменений:

```powershell
git add README.md
git commit -m "Improve README documentation"
git push -u origin feature/readme-update
```

Затем открыть Pull Request в GitHub:

```text
https://github.com/DaniilPeatcov2003/ExamTicketGenerator/compare
```

Выбрать:

- base: `main`;
- compare: рабочая ветка;
- добавить заголовок и описание изменений;
- создать Pull Request.

После проверки PR можно слить в `main` кнопкой **Merge pull request**.

## 11. GitHub Actions для email после push

Workflow хранится в:

```text
.github/workflows/push-email.yml
```

Он запускается при push в `main` и использует SMTP-сервис для отправки письма.

В настройках GitHub-репозитория открыть:

```text
Settings → Secrets and variables → Actions → New repository secret
```

Добавить secrets:

| Secret | Назначение |
| --- | --- |
| `MAIL_SERVER` | SMTP-сервер, например `smtp.gmail.com` |
| `MAIL_PORT` | SMTP-порт, обычно `587` |
| `MAIL_USERNAME` | Логин SMTP |
| `MAIL_PASSWORD` | Пароль приложения или SMTP-пароль |
| `MAIL_FROM` | Адрес отправителя |
| `MAIL_TO` | Адрес получателя |

Не записывать пароль SMTP в `.yml`, C#-код или README. Для Gmail обычно требуется включить двухэтапную проверку и создать отдельный пароль приложения.

После добавления secrets выполнить любой новый push в `main`. Результат workflow смотреть во вкладке **Actions**.

## 12. Проверка GitHub Actions

Проверить workflow можно так:

1. открыть репозиторий на GitHub;
2. открыть вкладку **Actions**;
3. выбрать workflow `Email notification on push`;
4. открыть последний запуск;
5. проверить шаг `Send email notification`;
6. проверить входящее письмо.

Если workflow завершился ошибкой, проверить:

- правильность названий secrets;
- SMTP-сервер и порт;
- пароль приложения вместо обычного пароля;
- адреса `MAIL_FROM` и `MAIL_TO`;
- разрешены ли SMTP-подключения у почтового провайдера.

## 13. Email-уведомления самого GitHub

GitHub Actions отправляет письмо через настроенный SMTP. Отдельно можно включить стандартные уведомления GitHub:

1. открыть настройки: <https://github.com/settings/notifications>;
2. убедиться, что почта подтверждена;
3. включить email-уведомления;
4. в репозитории нажать **Watch** и выбрать подходящий уровень уведомлений.

## 14. Финальный чек-лист

- [ ] Проект собирается через `dotnet build`.
- [ ] Приложение запускается через `dotnet run`.
- [ ] Пустые имя и фамилия обрабатываются.
- [ ] Билет генерируется от 1 до 20.
- [ ] Данные сохраняются в `journal.xlsx`.
- [ ] Повторная запись добавляется в новую строку.
- [ ] `bin/` и `obj/` игнорируются Git.
- [ ] Временный `~$journal.xlsx` не добавлен в коммит.
- [ ] Коммит имеет понятное сообщение.
- [ ] `git push` выполнен успешно.
- [ ] README описывает актуальную версию проекта.
- [ ] Workflow находится в `.github/workflows`.
- [ ] Secrets добавлены в настройках GitHub.
- [ ] GitHub Actions завершился успешно.
- [ ] Email-уведомление получено.

## 15. Полезные команды

```powershell
# Состояние репозитория
git status

# История коммитов
git log --oneline --decorate -10

# Проверка удалённых репозиториев
git remote -v

# Получение изменений
git pull origin main

# Отправка изменений
git push origin main

# Просмотр последнего коммита
git show --stat --oneline HEAD
```
