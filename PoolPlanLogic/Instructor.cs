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
        private List<List<Lesson>> m_InstructorsLessonsThisWeek;

        // get weekDaysFromUser function. for each day get also an hours range

        public Instructor(string i_InstructorName, List<eSwimStyle> i_SwimStyles) // other data will be added by functions
        {
            r_InstructorName = i_InstructorName;
            m_InstructorStyles = i_SwimStyles;
            m_InstructorAvailability = new List<List<Pair>>();
            m_InstructorsLessonsThisWeek = new List<List<Lesson>>();
            m_InstructorAvailability = createAvailabilityBoardForInstructor(); // sets an array of array of Pairs
            m_InstructorsLessonsThisWeek = createLessonsScheduluForInstructor(); // sets an array of array of Lessons
        }

        public void AddAvailability(eWeekDay i_AvailableDay, Pair i_RangeOfHours)
        {
            Pair newAvailableHoursToAdd = new Pair(i_RangeOfHours.StartTime, i_RangeOfHours.EndTime);
            if(m_InstructorAvailability[(int)i_AvailableDay] == null) // this day haven't been initialized
            {
                m_InstructorAvailability[(int)i_AvailableDay] = new List<Pair>();
            }

            m_InstructorAvailability[(int)i_AvailableDay].Add(newAvailableHoursToAdd);
            m_InstructorAvailability[(int)i_AvailableDay] = m_InstructorAvailability[(int)i_AvailableDay].OrderBy(p => p.StartTime).ToList();
        }

        public List<eWeekDay> DaysCurrentInstructorBookedLesson()
        {
            List<eWeekDay> workingDays = new List<eWeekDay>();
            for (int day = 0; day < PoolManagement.k_AmountOfDaysInWeek; day++)
            {
                if(m_InstructorsLessonsThisWeek[day] != null)
                {
                    workingDays.Add((eWeekDay)day);
                }
            }
            return workingDays;
        }

        public Tuple<bool, eWeekDay, int> InstructorCanBookThisLesson(eLessonMode i_LessonMode, List<eWeekDay> i_Days)
        {
            //fix this problem here
            bool instructorAlreadyBookedForSomeDays = i_Days.Count != 0;
            Tuple<bool, eWeekDay, int> result;

            for (int day = 0; day < PoolManagement.k_AmountOfDaysInWeek; day++)
            {
                if(instructorAlreadyBookedForSomeDays && i_Days.Contains((eWeekDay)day))
                {
                    continue;
                }
                if (m_InstructorAvailability[day] == null) // check in necessary
                {
                    continue;
                }
                for (int TimeRange = 0; TimeRange < m_InstructorAvailability[day].Count; TimeRange++)
                {
                    // are there 45/60 minutes that day
                    // ensuring that the lesson will be early as possible today!
                    if(m_InstructorAvailability[day][TimeRange].HasTimeRangeForLesson(i_LessonMode)) // success
                    {
                        result = new Tuple<bool, eWeekDay, int>( true, (eWeekDay)day, TimeRange);
                        return result;
                    }
                }
                
            }
            result = new Tuple<bool, eWeekDay, int>(false, (eWeekDay)0, 0);
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
            return m_InstructorsLessonsThisWeek[(int)i_Day] != null;
        }

        public string InstructorName
        {
            get { return r_InstructorName; }
        }

        public List<eSwimStyle> InstructorsSwimStyles
        {
            get { return m_InstructorStyles; }
        }


        public void c()
        {
            m_InstructorAvailability = new List<List<Pair>>(); // must in c'tor

            m_InstructorAvailability.Add(new List<Pair>()); // needs to be done 7 times anyway in the c'tor
            m_InstructorAvailability.Add(new List<Pair>()); // 
            m_InstructorAvailability.Add(new List<Pair>());
            // where m_InstructorAvailability[k].count == 0 ---> this instructor now works in this day.
            // need to create another one with hours of lessons. 
            // create lesson class and each instructor must have a list (size 7) of lessons
            // and isAvailable function see if the instructor already have a lesson in this day
            // shibuts student to courses is first of all by the instuctor ability to learn the specific style
            // afterwards we try to put them in a group that already exist, else - we open another group for them
            // if they want a private lesson - it's in the last priority - we first want to create as many groups as possible
            // each student will have a field of -- booked, and to which class
            //

            m_InstructorAvailability[(int)eWeekDay.Sunday] = new List<Pair>(); 
            m_InstructorAvailability[(int)(eWeekDay.Sunday)].Add(new Pair(800, 900));

            m_InstructorAvailability[(int)eWeekDay.Tuesday] = new List<Pair>();
            m_InstructorAvailability[(int)(eWeekDay.Tuesday)].Add(new Pair(800, 2000));

            //m_AvailableHours = new List<Pair>();
            //m_AvailableHours.Add(new Pair(3,5));

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
