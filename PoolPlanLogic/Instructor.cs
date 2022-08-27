using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoolPlanLogic
{
    public class Instructor
    {
        private readonly string r_InstructorName;
        private List<eSwimStyle> m_InstructorStyles;
        private List<List<Pair>> m_InstructorAvailability;
        private List<List<Lesson>> m_InstructorsLessonsSchedule;

        // get weekDaysFromUser function. for each day get also an hours range

        public Instructor(string i_InstructorName, List<eSwimStyle> i_SwimStyles) // other data will be added by functions
        {
            r_InstructorName = i_InstructorName;
            m_InstructorStyles = i_SwimStyles;
            m_InstructorAvailability = new List<List<Pair>>();
            m_InstructorsLessonsSchedule = new List<List<Lesson>>();
            m_InstructorAvailability = createAvailabilityBoardForInstructor(); // sets an array of array of Pairs
            m_InstructorsLessonsSchedule = createLessonsScheduluForInstructor(); // sets an array of array of Lessons
        }

        // to delete
        public List<List<Lesson>> instructorLessonsSchedule
        {
            get { return m_InstructorsLessonsSchedule; }
        }

        public void AddLessonToInstructor(Lesson i_NewLesson, int i_TimeRangeIndex)
        {
            int lessonDayIndex = (int)i_NewLesson.LessonDay;
            int startOfLesson = m_InstructorAvailability[lessonDayIndex][i_TimeRangeIndex].Start;
            int endOfLesson;
            if (m_InstructorsLessonsSchedule[lessonDayIndex] == null)
            {
                m_InstructorsLessonsSchedule[lessonDayIndex] = new List<Lesson>();
            }
            m_InstructorsLessonsSchedule[lessonDayIndex].Add(i_NewLesson);

            endOfLesson= clearLessonFromAvailability(lessonDayIndex, i_TimeRangeIndex, i_NewLesson.LengthOfLesson);
            i_NewLesson.LessonHour = new Pair(startOfLesson, endOfLesson); // set The Hour of the lesson
        }

        public void SyncInstructorsToPoolSchedule(Lesson i_Lesson)
        {
            int lessonStart = i_Lesson.LessonHour.Start, lessonEnd = i_Lesson.LessonHour.End;
            int i_DayIndex = (int)i_Lesson.LessonDay, indexToDelete = 0;
            bool availabilityModified = false;

            foreach (Pair i_HourRange in m_InstructorAvailability[i_DayIndex])
            {
                if (lessonStart > i_HourRange.Start && lessonEnd < i_HourRange.End) 
                {
                    m_InstructorAvailability[i_DayIndex].Add(new Pair(lessonEnd, i_HourRange.End));
                    i_HourRange.End = lessonStart;
                    availabilityModified = true;
                }
                else if (lessonStart <= i_HourRange.Start && lessonEnd > i_HourRange.Start && lessonEnd < i_HourRange.End)
                {
                    i_HourRange.Start = lessonEnd;
                    availabilityModified = true;
                }
                else if (lessonStart > i_HourRange.Start && lessonStart < i_HourRange.End && lessonEnd >= i_HourRange.End)
                {
                    i_HourRange.End = lessonStart;
                    availabilityModified = true;
                }
                else if (lessonStart <= i_HourRange.Start && lessonEnd >= i_HourRange.End)
                {
                    m_InstructorAvailability[i_DayIndex].RemoveAt(indexToDelete);
                    availabilityModified = true;
                }
                indexToDelete++;
                if (availabilityModified == true)
                {
                    sortListByStartTime(i_DayIndex);
                    return;
                }
            }
        }

        private void sortListByStartTime(int i_DayIndex)
        {
            m_InstructorAvailability[i_DayIndex]= m_InstructorAvailability[i_DayIndex].OrderBy(p => p.Start).ToList();
        }

        private int clearLessonFromAvailability(int i_DayIndex, int i_TimeRangeIndex, int i_LessonLength)
        {
            int startTime = m_InstructorAvailability[i_DayIndex][i_TimeRangeIndex].Start;
            if(startTime + i_LessonLength >= ((startTime / 100)*100)+60)
            {
                // we pass to the next hour
                m_InstructorAvailability[i_DayIndex][i_TimeRangeIndex].Start = startTime + i_LessonLength + 40;
            }
            else
            {
                m_InstructorAvailability[i_DayIndex][i_TimeRangeIndex].Start = startTime + i_LessonLength;
            }
            if(m_InstructorAvailability[i_DayIndex][i_TimeRangeIndex].Start ==
               m_InstructorAvailability[i_DayIndex][i_TimeRangeIndex].End)
            {
                m_InstructorAvailability[i_DayIndex].RemoveAt(i_TimeRangeIndex); // we filled the whole hour
            }
            sortListByStartTime(i_DayIndex);

            return m_InstructorAvailability[i_DayIndex][i_TimeRangeIndex].Start;
        }

        public void AddAvailability(eWeekDay i_AvailableDay, Pair i_RangeOfHours)
        {
            Pair newAvailableHoursToAdd = new Pair(i_RangeOfHours.Start, i_RangeOfHours.End);
            int dayIndex = (int)i_AvailableDay;

            if (m_InstructorAvailability[dayIndex] == null) // this day haven't been initialized
            {
                m_InstructorAvailability[dayIndex] = new List<Pair>();
            }

            m_InstructorAvailability[dayIndex].Add(newAvailableHoursToAdd);
            sortListByStartTime(dayIndex);
        }

        public List<eWeekDay> DaysCurrentInstructorBookedLesson()
        {
            List<eWeekDay> workingDays = new List<eWeekDay>();
            for (int day = 0; day < PoolManagement.k_AmountOfDaysInWeek; day++)
            {
                if(m_InstructorsLessonsSchedule[day] != null)
                {
                    workingDays.Add((eWeekDay)day);
                }
            }
            return workingDays;
        }

        public Tuple<bool, eWeekDay, int> InstructorCanBookThisLesson(eLessonMode i_LessonMode, List<eWeekDay> i_Days)
        {
            Tuple<bool, eWeekDay, int> result;
            int currentDayIndex = -1;

            foreach (eWeekDay currentDay in i_Days)
            {
                currentDayIndex = (int)currentDay;
                
                if (m_InstructorAvailability[currentDayIndex] == null) // the worker in not in the pool that day
                {
                    continue;
                }
                for (int TimeRange = 0; TimeRange < m_InstructorAvailability[currentDayIndex].Count; TimeRange++)
                {
                    // are there 45/60 minutes aviailable that day
                    // ensuring that the lesson will be early as possible today due to the sorted time-ranges
                    if (m_InstructorAvailability[currentDayIndex][TimeRange].HasTimeRangeForLesson(i_LessonMode)) // success
                    {
                        // to add: and if pool is vaccant also!
                        result = new Tuple<bool, eWeekDay, int>(true, (eWeekDay)currentDayIndex, TimeRange);
                        return result;
                    }
                }
            }

            result = new Tuple<bool, eWeekDay, int>(false, (eWeekDay)0, 0); // not found!
            return result;
        }


        public Pair IsAvailableToBookALesson(eWeekDay i_Day, Pair i_HoursRangeToCheck)
        {
            List<Pair> listOfHours = m_InstructorAvailability[(int)i_Day];
            if (m_InstructorAvailability[(int)i_Day] == null)
            {
                return null;
            }
            else
            {
                foreach (Pair availableHours in listOfHours)
                {
                    if (availableHours.InRange(i_HoursRangeToCheck))
                    {
                        return availableHours;
                    }
                }
                return null;
            }
        }

        public bool IsScheduledForThisDay(eWeekDay i_Day)
        {
            return m_InstructorsLessonsSchedule[(int)i_Day] != null;
        }

        public string InstructorName
        {
            get { return r_InstructorName; }
        }

        public  List<List<Pair>> InstructorAvailability
        {
            get { return m_InstructorAvailability; }
        }


        public List<eSwimStyle> InstructorsSwimStyles
        {
            get { return m_InstructorStyles; }
        }

        private List<List<Pair>> createAvailabilityBoardForInstructor()
        {
            return Enumerable.Repeat(default(List<Pair>), PoolManagement.k_AmountOfDaysInWeek).ToList();

            // NOTE: this function initializing only the main list representing the days!
            //Therefore --> days that are not available will remain null and will not be allocated!
        }

        private List<List<Lesson>> createLessonsScheduluForInstructor()
        {
            return Enumerable.Repeat(default(List<Lesson>), PoolManagement.k_AmountOfDaysInWeek).ToList();
            // NOTE: this function initializing only the main list representing the days!
            //Therefore --> days that are not booked with lesson will remain null and will not be allocated!
        }




    }
}
