using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace linq_slideviews
{
	public class ParsingTask
	{
		/// <param name="lines">все строки файла, которые нужно распарсить. Первая строка заголовочная.</param>
		/// <returns>Словарь: ключ — идентификатор слайда, значение — информация о слайде</returns>
		/// <remarks>Метод должен пропускать некорректные строки, игнорируя их</remarks>
		public static IDictionary<int, SlideRecord> ParseSlideRecords(IEnumerable<string> lines)
		{
			return lines.Select(line => line.Split(';'))
				.Where(data => data.Count() == 3
				&& !string.IsNullOrEmpty(data[1])
				&& Enum.IsDefined(typeof(SlideType), char.ToUpper(data[1][0]) + data[1].Substring(1))
				&& int.TryParse(data[0], out int id))
				.Select(data => new SlideRecord
				(
					int.Parse(data[0]), 
					(SlideType) Enum.Parse(typeof(SlideType), char.ToUpper(data[1][0]) + data[1].Substring(1)), 
					data[2]))
				.ToDictionary(slides => slides.SlideId, slides => slides);
		}

		/// <param name="lines">все строки файла, которые нужно распарсить. Первая строка — заголовочная.</param>
		/// <param name="slides">Словарь информации о слайдах по идентификатору слайда. 
		/// Такой словарь можно получить методом ParseSlideRecords</param>
		/// <returns>Список информации о посещениях</returns>
		/// <exception cref="FormatException">Если среди строк есть некорректные</exception>
		public static IEnumerable<VisitRecord> ParseVisitRecords(
			IEnumerable<string> lines, IDictionary<int, SlideRecord> slides)
		{
			return lines.Skip(1)
				.Select(line => line.Split(';'))
				.Where(data => data.Count() != 4 
				|| !int.TryParse(data[0], out int slideId)
				|| !int.TryParse(data[1], out int studentId)
				|| !slides.ContainsKey(studentId)
				|| !DateTime.TryParseExact 
				(
					data[2] + " " + data[3] + "Z",
					"u", CultureInfo.InvariantCulture, DateTimeStyles.None,
					out DateTime dateTime
				)
				? throw new FormatException("Wrong line [" + string.Join(";", data) + "]")
				: true)
                .Select(data => new VisitRecord
                (
                    int.Parse(data[0]),
                    int.Parse(data[1]),
                    DateTime.ParseExact(data[2] + " " + data[3] + "Z", "u", CultureInfo.InvariantCulture),
                    slides[int.Parse(data[1])].SlideType)
                );
        }
	}
}