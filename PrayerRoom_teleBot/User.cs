using System;
using System.Collections.Generic;
using TimePeriodsLibary;
using System.Text;

namespace PrayerRoom_teleBot
{
    class User
    {
        public long Id { get; }
        public List<TimePeriod> Times = new List<TimePeriod>();

        public User(long id)
        {
            Id = id;
        }
        public User(long id,TimePeriod time)
        {
            Id = id;
            Times.Add(time);
        }
        public User(long id, List<TimePeriod> list)
        {
            Id = id;
            foreach (var period in list)
            {
                Times.Add(period);
            }
        }

        public void addTime(TimePeriod time)
        {
            Times.Add(time);
        }
        
        public void DellTime(TimePeriod dellTime)
        {
           foreach(var time in Times)
            {
                if(time.ToString() == dellTime.ToString())
                {
                    Times.Remove(time);
                    break;
                }
            }
        }
    }
}
