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
        private eSwimStyle m_StudentSwimStyle; // an option to change during runtime?
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

        public void AddLessonToStudent(Lesson i_NewLessonToStudent) // what happen if i take down the ref
        {
            m_RegisteredLessons.Add(i_NewLessonToStudent);
        }

        public string StudentFirstName
        {
            get { return r_StudentFirstName; }
        }

        public string StudentLastName
        {
            get { return r_StudentLastName; }
        }

        public eSwimStyle RequestedSwimStyle
        {
            get { return m_StudentSwimStyle; }
            set { m_StudentSwimStyle = value; }
        }

        public eLessonMode StudentFirstPriority
        {
            get { return m_StudentLessonMode[0]; }
        }

        public bool DoesStudentHasSecondPriority()
        {
            return m_StudentLessonMode.Count == 2;
        }

        public eLessonMode StudentSecondPriority
        {
            get { return m_StudentLessonMode[1]; } // return the second priority of student
        }
     
    }
}
