using System.Collections.Generic;

namespace PoolPlanLogic
{
    public class Student
    {
        private readonly string r_StudentFirstName;
        private readonly string r_StudentLastName;
        private readonly eSwimStyle r_StudentSwimStyle;
        private readonly List<eLessonMode> r_StudentLessonMode;
        private readonly List<Lesson> r_RegisteredLessons;

        public Student(string i_FirstName, string i_LastName, eSwimStyle i_Style, List<eLessonMode> i_Mode)
        {
            r_StudentFirstName = i_FirstName;
            r_StudentLastName = i_LastName;
            r_StudentSwimStyle = i_Style;
            r_StudentLessonMode = i_Mode;
            r_RegisteredLessons = new List<Lesson>();
        }

        public List<Lesson> studentlLessons
        {
            get { return r_RegisteredLessons; }
        }
         
        public void AddLessonToStudentAgenda(Lesson i_NewLessonToStudent)
        {
            r_RegisteredLessons.Add(i_NewLessonToStudent);
        }

        public string FirstName
        {
            get { return r_StudentFirstName; }
        }

        public string LastName
        {
            get { return r_StudentLastName; }
        }

        public eSwimStyle RequestedSwimStyle
        {
            get { return r_StudentSwimStyle; }
        }

        public eLessonMode FirstPriority
        {
            get { return r_StudentLessonMode[0]; }
        }

        public bool IsBooked()
        {
            return r_RegisteredLessons.Count > 0;
        }

        public eLessonMode SecondPriority
        {
            get { return r_StudentLessonMode[1]; }
        }
     
    }
}
