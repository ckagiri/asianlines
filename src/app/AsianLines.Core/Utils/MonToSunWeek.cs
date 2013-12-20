using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Itenso.TimePeriod;

namespace AsianLines.Core.Utils
{
    public class MonToSunWeek
    {
        private readonly CultureInfo _culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        private readonly CultureInfo _uiculture = (CultureInfo)CultureInfo.CurrentUICulture.Clone();
        private readonly Week _week;

        public MonToSunWeek(DateTime? moment = null)
        {
            _culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;
            _uiculture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;

            Thread.CurrentThread.CurrentCulture = _culture;
            Thread.CurrentThread.CurrentUICulture = _uiculture;

            var week = moment != null ? new Week(moment.Value) : new Week();
            _week = week.GetPreviousWeek().GetNextWeek();
        }

        public DateTime Start()
        { 
            var time = _week.Start;
            var year = time.Year;
            var month = time.Month;
            var day = time.Day;
            var hour = time.Hour;
            var minute = time.Minute;
            var second = time.Second;
            return new DateTime(year, month, day, hour, minute, second);
        } 

        public DateTime End()
        {
            var time = _week.End;
            var year = time.Year;
            var month = time.Month;
            var day = time.Day;
            var hour = time.Hour;
            var minute = time.Minute;
            var second = time.Second;
            return new DateTime(year, month, day, hour, minute, second);
        }

        public DateTime FirstDayOfWeek()
        {
            return _week.FirstDayOfWeek;
        }

        public DateTime LastDayOfWeek()
        {
            return _week.LastDayOfWeek;
        }
    }

    public class MonToSunWeeks
    {
        private int _noOfWeeks;
        private DateTime _startDate;
        private DateTime _endDate;
        public MonToSunWeeks(DateTime startDate, DateTime endDate)
        {
            Init(startDate, endDate);
        }

        public MonToSunWeeks(int year, int month)
        {
            var yearmonth = (YearMonth) month;
            var yearMonth = new Month(year, yearmonth);
            var monthStart = yearMonth.FirstDayStart;
            var monthEnd = yearMonth.LastDayStart;
            Init(monthStart, monthEnd);
        }

        private void Init(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                var temp = startDate;
                startDate = endDate;
                endDate = temp;
            }
            _startDate = new MonToSunWeek(startDate).Start();
            _endDate = new MonToSunWeek(endDate).End();

            _noOfWeeks = ((_endDate - _startDate).Days / 7) + 1;
        }

        public List<WeekRange> GetWeeks()
        {
            var weeks = new List<WeekRange>();
            var firstWeek = new MonToSunWeek(_startDate);
            var startOfWeek = firstWeek.Start();
            var endOfWeek = firstWeek.End();
            for (int i = 1; i <= _noOfWeeks; i++)
            {
                weeks.Add(new WeekRange
                              {
                                  StartDate = startOfWeek,
                                  EndDate = endOfWeek
                              });
                startOfWeek = startOfWeek.AddDays(7);
                endOfWeek = endOfWeek.AddDays(7);
            }
            
            return weeks;
        }

        
    }

    public class WeekRange
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
