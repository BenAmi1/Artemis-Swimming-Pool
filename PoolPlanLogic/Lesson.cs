using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoolPlanLogic
{
    public class Lesson
    {
        private readonly eWeekDay r_LessonDay;
        private TimeRange m_LessonAppointedTime; 
        private readonly int r_lessonLength;
        private readonly TimeRange r_LessonInTimeRange;
        private readonly eSwimStyle r_LessonStyle;
        private readonly eLessonMode r_LessonMode;
        private readonly string r_LessonInstructorName;
        private readonly List<Student> r_ParticipatingStudents;

        public Lesson(eWeekDay i_Day, TimeRange i_Time, eSwimStyle i_Style, eLessonMode i_Mode, string i_InstructorName)
        {
            r_LessonDay = i_Day;
            r_LessonInTimeRange = i_Time;
            r_LessonStyle = i_Style;
            r_LessonMode = i_Mode;
            r_LessonInstructorName = i_InstructorName;
            r_ParticipatingStudents = new List<Student>(); // init only, adding students - in another function
            r_lessonLength = setLessonLength();
            // whats with the lessons hour
        }

        private int setLessonLength()
        {
            return r_LessonMode == eLessonMode.Private ?
                PoolManagement.k_PrivateLessonLength : PoolManagement.k_GroupLessonLength;
        }

        public int LengthOfLesson
        {
            get { return r_lessonLength; }
        }

        public void AddStudentToLesson(Student i_RegisteredStudent)
        {
            r_ParticipatingStudents.Add(i_RegisteredStudent); 
        }

        public eWeekDay LessonDay
        {
            get { return r_LessonDay; }
        }

        public TimeRange LessonTimeRange
        {
            get { return r_LessonInTimeRange; }
        }

        public TimeRange LessonHour
        {
            get {return m_LessonAppointedTime; }
            set { m_LessonAppointedTime = value; }
        }

        public eSwimStyle SwimStyle
        {
            get { return r_LessonStyle; }
        }

        public eLessonMode LessonMode
        {
            get { return r_LessonMode; }
        }

        public string LessonInstructor
        {
            get { return r_LessonInstructorName; }
        }

        public List<Student> RegisteredStudents
        {
            get { return r_ParticipatingStudents; }
        }

        public List<string> HourToDisplay
        {
            get { return setHoursInString(); }
        }


        public List<string> setHoursInString()
        {
            List<string> hourRange = new List<string>();
            hourRange.Add(m_LessonAppointedTime.Start.ToString()); // check
            hourRange.Add(m_LessonAppointedTime.End.ToString());

            for (int i = 0; i < 2; i++)
            {
                if (hourRange[i].Length == 3)
                {
                    hourRange.Add(hourRange[i].Insert(1, ":"));
                }
                else
                {
                    hourRange.Add(hourRange[i].Insert(2, ":"));
                }
            }

            hourRange.RemoveAt(0);
            hourRange.RemoveAt(0);

            return hourRange;
        }

        //public void remove


    }
}
