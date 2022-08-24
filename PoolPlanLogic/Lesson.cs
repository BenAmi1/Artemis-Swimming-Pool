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
        private readonly Pair r_LessonAppointedTime;
        private readonly eSwimStyle r_LessonStyle;
        private readonly eLessonMode r_LessonMode;
        private readonly string r_LessonInstructor;
        private readonly List<Student> r_ParticipatingStudentS;

        public Lesson(eWeekDay i_Day, Pair i_Time, eSwimStyle i_Style, eLessonMode i_Mode, string i_InstructorName)
        {
            r_LessonDay = i_Day;
            r_LessonAppointedTime = i_Time;
            r_LessonStyle = i_Style;
            r_LessonMode = i_Mode;
            r_LessonInstructor = i_InstructorName;
            r_ParticipatingStudentS = new List<Student>(); // init only, adding students - in another function
        }

        public void AddStudentToLesson(Student i_RegisteredStudent)
        {
            r_ParticipatingStudentS.Add(i_RegisteredStudent); 
        }

        public eWeekDay LessonDay
        {
            get { return r_LessonDay; }
        }

        public Pair LessonHour
        {
            get {return r_LessonAppointedTime; }
        }

        public eSwimStyle LessonSwimStyle
        {
            get { return r_LessonStyle; }
        }

        public eLessonMode LessonMode
        {
            get { return r_LessonMode; }
        }

        public string LessonInstructor
        {
            get { return r_LessonInstructor; }
        }

        public List<Student> ListOfParticipatingStudents
        {
            get { return r_ParticipatingStudentS; }
        }



        //public void remove


    }
}
