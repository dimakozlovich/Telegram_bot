using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace TimePeriodsLibary
{
    public class TimePeriod
    {
        private DateTime Beginning;
        private DateTime End;
        public TimePeriod(DateTime begining)
        {
            Beginning = begining;
            End = begining.AddHours(1);
        }
        public DateTime GetBeginning()
        {
             return Beginning;
        }
        public override string ToString()
        {
            return Beginning.ToString("dd.MM HH:mm") + "-" + End.ToShortTimeString();
        }
    }
    public class ListOfPeriod 
    {
        public List<TimePeriod> listOfperiod = new List<TimePeriod>();
        
        public ListOfPeriod()
        {

        }
        public void FillList(DateTime dtBeg,DateTime dtEnd)
        {
            TimeSpan check = dtEnd - dtBeg;
            // TODO: checking loigc
            if (check!= null)
            {
                int begTime = dtBeg.Hour;
                int endTime = dtEnd.Hour;
                for(int i = 0; i <= check.Hours; i++)
                {
                    var timePeriod = new TimePeriod(dtBeg.AddHours(i));
                    listOfperiod.Add(timePeriod);
                }
            }
            
        }
        public void AddInList(TimePeriod period)
        {
            listOfperiod.Add(period);
            var comparer = new PeriodComparer();
            listOfperiod.Sort(comparer);
        }
        public void DellFromList(TimePeriod Dellperiod)
        {
            
            listOfperiod.Remove(Dellperiod);
            var comparer = new PeriodComparer();
            listOfperiod.Sort(comparer);
        }
        public int PlaceInList(DateTime time)
        {
            foreach (var dataT in listOfperiod)
            {
              if(dataT.GetBeginning() == time)
                { 
                    return listOfperiod.BinarySearch(dataT);
                }
            }
            return 0;
        }

    }
    public class PeriodComparer : IComparer<TimePeriod>
    {
        public int Compare([AllowNull] TimePeriod time1, [AllowNull] TimePeriod time2)
        {
            if(time1.GetBeginning()> time2.GetBeginning())
            {
                return 1;
            }
            if (time1.GetBeginning() < time2.GetBeginning())
            {
                return -1;
            }
            return 0;
        }
    }


}
