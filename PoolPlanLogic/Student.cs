using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoolPlanLogic
{
    public class Student
    {
        private readonly string r_StudentFirstName;
        private readonly string r_StudentLastName;
        private eSwimStyle m_StudentSwimStyle; 
        private readonly List<eLessonMode> m_StudentLessonMode;
        private List<Lesson> m_RegisteredLessons;

        public Student(string i_FirstName, string i_LastName, eSwimStyle i_Style, List<eLessonMode> i_Mode)
        {
            r_StudentFirstName = i_FirstName;
            r_StudentLastName = i_LastName;
            m_StudentSwimStyle = i_Style;
            m_StudentLessonMode = i_Mode;
            m_RegisteredLessons = new List<Lesson>();
        }

        //test
        public List<Lesson> studentlLessons
        {
            get { return m_RegisteredLessons; }
        }
         

        public void AddLessonToStudentAgenda(Lesson i_NewLessonToStudent) // what happen if i take down the ref
        {
            m_RegisteredLessons.Add(i_NewLessonToStudent);
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
            get { return m_StudentSwimStyle; }
            set { m_StudentSwimStyle = value; }
        }

        public eLessonMode FirstPriority
        {
            get { return m_StudentLessonMode[0]; }
        }

        public bool IsBooked()
        {
            return m_RegisteredLessons.Count > 0;
        }

        public eLessonMode SecondPriority
        {
            get { return m_StudentLessonMode[1]; } // return the second priority of student
        }
     
    }
}
