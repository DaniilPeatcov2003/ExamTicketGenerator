using ClosedXML.Excel;

class Program
{
    private const string FileName = "journal.xlsx";
    private static readonly Random Random = new Random();

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("Генератор экзаменационных билетов");
        Console.WriteLine("Для выхода нажмите ESC.");
        Console.WriteLine();

        while (true)
        {
            // Проверяем ESC перед началом ввода фамилии
            Console.Write("Last name: ");

            string? lastName = ReadLineWithEsc();

            if (lastName == null)
            {
                Console.WriteLine("\nПриложение завершено.");
                break;
            }

            // Пустая фамилия
            while (string.IsNullOrWhiteSpace(lastName))
            {
                Console.WriteLine("Фамилия не может быть пустой.");
                Console.Write("Last name: ");

                lastName = ReadLineWithEsc();

                if (lastName == null)
                {
                    Console.WriteLine("\nПриложение завершено.");
                    return;
                }
            }

            lastName = lastName.Trim();

            // Имя
            string? firstName;

            while (true)
            {
                Console.Write("First name: ");

                firstName = ReadLineWithEsc();

                if (firstName == null)
                {
                    Console.WriteLine("\nПриложение завершено.");
                    return;
                }

                if (!string.IsNullOrWhiteSpace(firstName))
                    break;

                Console.WriteLine("Имя не может быть пустым.");
            }

            firstName = firstName.Trim();

            // Генерируем номер билета от 1 до 20
            int ticketNumber = Random.Next(1, 21);

            Console.WriteLine($"Билет № {ticketNumber}");

            // Сохраняем запись в Excel
            bool saved = SaveToExcel(
                lastName,
                firstName,
                ticketNumber
            );

            if (saved)
            {
                Console.WriteLine("Запись сохранена в journal.xlsx.");
            }

            Console.WriteLine();
        }
    }

    // Чтение строки с возможностью нажать ESC
    // ESC возвращает null
    private static string? ReadLineWithEsc()
    {
        string result = "";

        while (true)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);

            // ESC
            if (key.Key == ConsoleKey.Escape)
            {
                return null;
            }

            // Enter
            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                return result;
            }

            // Backspace
            if (key.Key == ConsoleKey.Backspace)
            {
                if (result.Length > 0)
                {
                    result = result[..^1];

                    Console.Write("\b \b");
                }

                continue;
            }

            // Обычный символ
            if (!char.IsControl(key.KeyChar))
            {
                result += key.KeyChar;
                Console.Write(key.KeyChar);
            }
        }
    }

    private static bool SaveToExcel(
        string lastName,
        string firstName,
        int ticketNumber)
    {
        while (true)
        {
            try
            {
                using (var workbook = File.Exists(FileName)
                    ? new XLWorkbook(FileName)
                    : new XLWorkbook())
                {
                    IXLWorksheet worksheet;

                    if (workbook.Worksheets.Count == 0)
                    {
                        worksheet = workbook.Worksheets.Add("Journal");
                    }
                    else
                    {
                        worksheet = workbook.Worksheet(1);
                    }

                    // Если файл новый — создаём шапку
                    if (worksheet.Cell("A1").IsEmpty())
                    {
                        worksheet.Cell("A1").Value = "Last name";
                        worksheet.Cell("B1").Value = "First name";
                        worksheet.Cell("C1").Value = "Номер билета";
                        worksheet.Cell("D1").Value = "Дата и время";

                        worksheet.Row(1).Style.Font.Bold = true;
                    }

                    // Находим первую свободную строку
                    int row = worksheet.LastRowUsed()?.RowNumber() + 1 ?? 2;

                    // Записываем данные
                    worksheet.Cell(row, 1).Value = lastName;
                    worksheet.Cell(row, 2).Value = firstName;
                    worksheet.Cell(row, 3).Value = ticketNumber;
                    worksheet.Cell(row, 4).Value = DateTime.Now;

                    // Формат даты и времени
                    worksheet.Cell(row, 4)
                        .Style.DateFormat.Format = "dd.MM.yyyy HH:mm:ss";

                    // Немного улучшаем ширину столбцов
                    worksheet.Columns("A:D").AdjustToContents();

                    // Сохраняем сразу после каждой записи
                    workbook.SaveAs(FileName);
                }

                return true;
            }
            catch (IOException)
            {
                Console.WriteLine();
                Console.WriteLine(
                    "Ошибка: файл journal.xlsx сейчас недоступен для записи."
                );
                Console.WriteLine(
                    "Возможно, файл открыт в Excel."
                );
                Console.WriteLine(
                    "Закройте journal.xlsx и нажмите Enter, чтобы повторить."
                );

                Console.ReadLine();
                Console.WriteLine();
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine();
                Console.WriteLine(
                    "Ошибка: нет доступа для записи в journal.xlsx."
                );
                Console.WriteLine(
                    "Закройте файл в Excel или проверьте права доступа."
                );
                Console.WriteLine(
                    "После этого нажмите Enter для повторной попытки."
                );

                Console.ReadLine();
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine(
                    $"Не удалось сохранить запись: {ex.Message}"
                );

                return false;
            }
        }
    }
}