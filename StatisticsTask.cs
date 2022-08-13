using System;
using System.Collections.Generic;
using System.Linq;

namespace linq_slideviews
{
	public class StatisticsTask
	{
		public static double GetMedianTimePerSlide(List<VisitRecord> visits, SlideType slideType)
		{
			var timeBySlideType = visits.OrderBy(record => record.UserId).ThenBy(record => record.DateTime)
				.Bigrams()
				.Where(bigram => bigram.Item1.SlideType == slideType && bigram.Item1.UserId == bigram.Item2.UserId)
				.Select(bigram => (bigram.Item2.DateTime - bigram.Item1.DateTime).TotalSeconds / 60)
				.Where(min => min >= 1 && min < 120);
			return timeBySlideType.Count() > 0 ? timeBySlideType.Median() : 0;
		}
	}
}