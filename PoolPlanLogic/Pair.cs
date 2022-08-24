using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoolPlanLogic
{
    public class Pair
    {
        private int m_StartTime;
        private int m_EndTime;

        public Pair(int i_StartTime, int i_EndTime)
        {
            m_StartTime = i_StartTime;
            m_EndTime = i_EndTime;
        }

        public int StartTime
        {
            get { return m_StartTime; }
            set { m_StartTime = value; }
        }

        public int EndTime
        {
            get { return m_EndTime; }
            set { m_EndTime = value; }
        }

        public bool InRange(Pair i_RangeToCheck)
        {
            return i_RangeToCheck.StartTime >= m_StartTime &&
                i_RangeToCheck.StartTime < m_EndTime &&
                i_RangeToCheck.EndTime <= m_EndTime &&
                i_RangeToCheck.EndTime > m_StartTime;
        }

        public bool HasTimeRangeForLesson(eLessonMode i_LessonMode)
        {
            int lengthOfThisLesson = -1;
            int AmountOfMinutesAvailableInThisTimeRange = m_StartTime - m_StartTime - 40; //100 - 60 = 40

            if(i_LessonMode == eLessonMode.Group)
            {
                lengthOfThisLesson = PoolManagement.k_LenghOfGroupLesson;
            }
            else
            {
                lengthOfThisLesson = PoolManagement.k_LenghOfPrivateLesson;
            }

            return AmountOfMinutesAvailableInThisTimeRange >= lengthOfThisLesson ? true : false;

        }
    }
}
