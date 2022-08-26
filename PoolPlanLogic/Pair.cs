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

        public int Start
        {
            get { return m_StartTime; }
            set { m_StartTime = value; }
        }

        public int End
        {
            get { return m_EndTime; }
            set { m_EndTime = value; }
        }

        public bool InRange(Pair i_RangeToCheck)
        {
            return i_RangeToCheck.Start >= m_StartTime &&
                i_RangeToCheck.Start < m_EndTime &&
                i_RangeToCheck.End <= m_EndTime &&
                i_RangeToCheck.End > m_StartTime;
        }

        public int AmountOfMinutesInTimeRange()
        {
            if(m_StartTime / 100 != m_EndTime / 100)
            {
                return (m_EndTime - m_StartTime) - (40 * ((m_EndTime / 100) - (m_StartTime / 100)));
            }
            else
                return m_EndTime - m_StartTime;
        }

        public bool HasTimeRangeForLesson(eLessonMode i_LessonMode)
        {
            int lengthOfThisLesson = -1;
            int AmountOfMinutesAvailableInThisTimeRange = AmountOfMinutesInTimeRange();

            if (i_LessonMode == eLessonMode.Group)
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
