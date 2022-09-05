using System;
using System.Collections.Generic;
using System.Linq;

namespace PoolPlanLogic
{
    public class Instructor
    {
        private readonly string r_InstructorName;
        private readonly List<eSwimStyle> r_InstructorStyles;
        private readonly List<List<TimeRange>> r_InstructorAvailability;
        private readonly List<List<Lesson>> r_InstructorsLessonsSchedule;

        public Instructor(string i_InstructorName, List<eSwimStyle> i_SwimStyles) 
        {
            r_InstructorName = i_InstructorName;
            r_InstructorStyles = i_SwimStyles;
            r_InstructorAvailability = new List<List<TimeRange>>();
            r_InstructorsLessonsSchedule = new List<List<Lesson>>();
            r_InstructorAvailability = createAvailabilityBoardForInstructor(); // Sets an array of array of Pairs
            r_InstructorsLessonsSchedule = createLessonsScheduluForInstructor(); // Sets an array of array of Lessons
        }

        public List<List<Lesson>> instructorLessonsSchedule
        {
            get { return r_InstructorsLessonsSchedule; }
        }

        public void AddLessonToInstructor(Lesson i_NewLesson, int i_TimeRangeIndex)
        {
            int lessonDayIndex = (int)i_NewLesson.LessonDay, endOfLesson;
            int startOfLesson = r_InstructorAvailability[lessonDayIndex][i_TimeRangeIndex].Start;

            if (r_InstructorsLessonsSchedule[lessonDayIndex] == null)
            {
                r_InstructorsLessonsSchedule[lessonDayIndex] = new List<Lesson>();
            }

            r_InstructorsLessonsSchedule[lessonDayIndex].Add(i_NewLesson);
            endOfLesson= clearLessonFromAvailability(lessonDayIndex, i_TimeRangeIndex, i_NewLesson.LengthOfLesson);
            i_NewLesson.LessonHour = new TimeRange(startOfLesson, endOfLesson); // Set The Hour of the lesson
        }

        public void SyncInstructorsToPoolSchedule(Lesson i_Lesson)
        {
            int lessonStart = i_Lesson.LessonHour.Start, lessonEnd = i_Lesson.LessonHour.End;
            int i_DayIndex = (int)i_Lesson.LessonDay, indexToDelete = 0;
            bool availabilityModified = false;

            foreach (TimeRange i_HourRange in r_InstructorAvailability[i_DayIndex])
            {
                if (lessonStart > i_HourRange.Start && lessonEnd < i_HourRange.End) 
                {
                    r_InstructorAvailability[i_DayIndex].Add(new TimeRange(lessonEnd, i_HourRange.End));
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
                    r_InstructorAvailability[i_DayIndex].RemoveAt(indexToDelete);
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
            r_InstructorAvailability[i_DayIndex]= r_InstructorAvailability[i_DayIndex].OrderBy(p => p.Start).ToList();
        }

        private int clearLessonFromAvailability(int i_DayIndex, int i_TimeRangeIndex, int i_LessonLength)
        {
            int hourRangeStartTime = r_InstructorAvailability[i_DayIndex][i_TimeRangeIndex].Start;
            int endTimOfLesson = 0;

            if(hourRangeStartTime + i_LessonLength >= ((hourRangeStartTime / 100)*100)+60)
            {
                // Passing to the next hour.
                r_InstructorAvailability[i_DayIndex][i_TimeRangeIndex].Start = hourRangeStartTime + i_LessonLength + 40;
            }
            else
            {
                r_InstructorAvailability[i_DayIndex][i_TimeRangeIndex].Start = hourRangeStartTime + i_LessonLength;
            }

            endTimOfLesson = r_InstructorAvailability[i_DayIndex][i_TimeRangeIndex].Start;
            if (r_InstructorAvailability[i_DayIndex][i_TimeRangeIndex].Start ==
               r_InstructorAvailability[i_DayIndex][i_TimeRangeIndex].End)
            {
                r_InstructorAvailability[i_DayIndex].RemoveAt(i_TimeRangeIndex); // The whole hour is filled: remove it
            }

            sortListByStartTime(i_DayIndex);
            return endTimOfLesson;
        }

        public void AddAvailability(eWeekDay i_AvailableDay, TimeRange i_RangeOfHours)
        {
            TimeRange newAvailableHoursToAdd = new TimeRange(i_RangeOfHours.Start, i_RangeOfHours.End);
            int dayIndex = (int)i_AvailableDay;

            if (r_InstructorAvailability[dayIndex] == null) // this day haven't been initialized
            {
                r_InstructorAvailability[dayIndex] = new List<TimeRange>();
            }

            r_InstructorAvailability[dayIndex].Add(newAvailableHoursToAdd);
            sortListByStartTime(dayIndex);
        }

        public List<eWeekDay> DaysCurrentInstructorBookedLesson()
        {
            List<eWeekDay> workingDays = new List<eWeekDay>();
            for (int day = 0; day < PoolManagement.k_AmountOfDaysInWeek; day++)
            {
                if(r_InstructorsLessonsSchedule[day] != null)
                {
                    workingDays.Add((eWeekDay)day);
                }
            }
            return workingDays;
        }

        public Tuple<bool, eWeekDay, int> DoesInstructorCanBookThisLesson(eLessonMode i_LessonMode, List<eWeekDay> i_Days)
        {
            Tuple<bool, eWeekDay, int> result;
            int currentDayIndex = -1;

            foreach (eWeekDay currentDay in i_Days)
            {
                currentDayIndex = (int)currentDay;
                if (r_InstructorAvailability[currentDayIndex] == null) // the worker in not in the pool that day
                    continue;
                for (int TimeRange = 0; TimeRange < r_InstructorAvailability[currentDayIndex].Count; TimeRange++)
                {
                    // are there 45/60 minutes aviailable that day
                    // ensuring that the lesson will be early as possible today due to the sorted time-ranges
                    if (r_InstructorAvailability[currentDayIndex][TimeRange].HasTimeRangeForLesson(i_LessonMode))
                    {
                        result = new Tuple<bool, eWeekDay, int>(true, (eWeekDay)currentDayIndex, TimeRange);
                        return result;
                    }
                }
            }
            result = new Tuple<bool, eWeekDay, int>(false, (eWeekDay)0, 0); // not found!
            return result;
        }

        public TimeRange IsAvailableToBookALesson(eWeekDay i_Day, TimeRange i_HoursRangeToCheck)
        {
            List<TimeRange> listOfHours = r_InstructorAvailability[(int)i_Day];
            if (r_InstructorAvailability[(int)i_Day] == null)
            {
                return null;
            }
            else
            {
                foreach (TimeRange availableHours in listOfHours)
                {
                    if (availableHours.InRange(i_HoursRangeToCheck))
                    {
                        return availableHours;
                    }
                }
                return null;
            }
        }

        public int AmountOfWorkingHours()
        {
            int counter = 0;
            for (int day = 0; day < PoolManagement.k_AmountOfDaysInWeek; day++)
            {
                if (r_InstructorsLessonsSchedule[day] == null)
                    continue;

                foreach (Lesson lesson in r_InstructorsLessonsSchedule[day])
                {
                    counter += lesson.LengthOfLesson;
                }
            }

            return counter;
        }

        public bool IsScheduledForThisDay(eWeekDay i_Day)
        {
            return r_InstructorsLessonsSchedule[(int)i_Day] != null;
        }

        public string InstructorName
        {
            get { return r_InstructorName; }
        }

        public  List<List<TimeRange>> InstructorAvailability
        {
            get { return r_InstructorAvailability; }
        }

        public List<eSwimStyle> InstructorsSwimStyles
        {
            get { return r_InstructorStyles; }
        }

        private List<List<TimeRange>> createAvailabilityBoardForInstructor()
        {
            return Enumerable.Repeat(default(List<TimeRange>), PoolManagement.k_AmountOfDaysInWeek).ToList();

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
