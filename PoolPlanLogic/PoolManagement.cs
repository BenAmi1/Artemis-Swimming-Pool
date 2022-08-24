using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoolPlanLogic
{
    class PoolManagement
    {
        private readonly List<Student> r_RegisteredStudents;
        private readonly List<Instructor> r_Instructors;
        private readonly List<List<Lesson>> r_WeeklyLessonsSchedule;
        private const int k_NotFound = -1;
        public const int k_LenghOfPrivateLesson = 45;
        public const int k_LenghOfGroupLesson = 60;
        public const int k_AmountOfDaysInWeek = 7;

        public PoolManagement()
        {
            r_RegisteredStudents = new List<Student>();
            r_Instructors = new List<Instructor>();
            r_WeeklyLessonsSchedule = new List<List<Lesson>>(); // need to new list<lesson> and Lesson
            r_WeeklyLessonsSchedule = createAvailabilityBoardForInstructor(); // sets an array of array of Pairs
        }


        public bool addAvailablityToInstructor(string i_InstructorName, eWeekDay i_Day, Pair i_RangeOfHours)
        {
            int instructorIndex = GetInstructorIndexInList(i_InstructorName);
            if (instructorIndex != k_NotFound)
            {
                r_Instructors[instructorIndex].AddAvailability(i_Day, i_RangeOfHours);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AssignWeekAgenda()
        {
            AssignGroupLessons();
            assignPrivateLessons();
        }

        public void AssignGroupLessons()
        {
            foreach (Student currentStudent in r_RegisteredStudents)
            {
                if (currentStudent.StudentFirstPriority == eLessonMode.Group)
                {
                    if (AttemptToAssignToExistentLesson(currentStudent))
                    {
                        return;
                    }
                    else
                    {
                        //CreateNewLessonAndAssignStudent(currentStudent);
                    }

                }
            }

        }
        
        public bool AttemptToAssignToExistentLesson(Student i_CurrentStudent)
        {
            // need to understand wether there is a collision between two instructors
            for (int day = 0; day < k_AmountOfDaysInWeek; day++)
            {
                if(r_WeeklyLessonsSchedule[day] ==null) // to check if neccessary
                {
                    continue;
                }
                foreach (Lesson currentLesson in r_WeeklyLessonsSchedule[day])
                {
                    if (currentLesson.LessonSwimStyle == i_CurrentStudent.RequestedSwimStyle &&
                        currentLesson.LessonMode == eLessonMode.Group)
                    {
                        i_CurrentStudent.AddLessonToStudent(currentLesson);
                        return true;
                    }
                }
            }

            return false;
        }

        public List<int> indexesOfQualifiedInstructors(eSwimStyle i_RequestedSwimStyle)
        {
            List<int> indexesOfQualifiedInstructors = new List<int>();

            for (int instructorIndex = 0; instructorIndex < r_Instructors.Count; instructorIndex++)
            {
                foreach (eSwimStyle swimStyle in r_Instructors[instructorIndex].InstructorsSwimStyles)
                {
                    if (swimStyle == i_RequestedSwimStyle) // instructor is professionally fit
                    {
                        indexesOfQualifiedInstructors.Add(instructorIndex);
                        break; // does it fine?
                    }
                }
            }

        }

        public bool CreateNewLessonAndAssignStudent(Student i_CurrentStudent, eLessonMode i_LessonMode)
        {
            // dont forget to update the lessons list in pool management class
            // try to find the instructor that already works that day

            List<eWeekDay> daysCurrentInstructorWorks;
            List<int> qualifiedInstructorsIndex;
            int indexOfAvailableInstructor = 0; // to correct
            Tuple<bool, eWeekDay, int> result;
            Instructor currentInstructor;
            int numberOfTriesToAssign = 0;

            qualifiedInstructorsIndex = indexesOfQualifiedInstructors(i_CurrentStudent.RequestedSwimStyle);

            for (int instructorIndex = 0; instructorIndex < qualifiedInstructorsIndex.Count; instructorIndex++)
            {
                currentInstructor = r_Instructors[instructorIndex]; // check if ok (modified by value or ref...)
                daysCurrentInstructorWorks = currentInstructor.DaysCurrentInstructorBookedLesson();
                if (daysCurrentInstructorWorks.Count != 0 || 1 == numberOfTriesToAssign) 
                {
                    result = currentInstructor.InstructorCanBookThisLesson(i_LessonMode, daysCurrentInstructorWorks); // need to be in amother place, change this loop --> coliision between this list and the if
                    if (result.Item1 == true) // success! instructor can take lesson in day he works!
                    {
                        // make shiboots!
                    }
                }
                if(instructorIndex == qualifiedInstructorsIndex.Count -1)//unable to assign instructor to his busy day
                {
                    numberOfTriesToAssign++;
                    instructorIndex = 0; // redo the loop - but now try to assign any instructor!
                    if (2 == numberOfTriesToAssign) 
                    {
                        return false;
                    }
                }
            }
        }

        

        public void assignPrivateLessons()
        {

        }
       



        public int GetInstructorIndexInList(string i_InstructorName)
        {
            int wantedIndex = 0;
            foreach (Instructor instructor in r_Instructors)
            {
                if (instructor.InstructorName == i_InstructorName)
                {
                    return wantedIndex;
                }
                wantedIndex++;
            }
            return k_NotFound;
        }

        public void AddStudent(string i_FirstName, string i_LastName, eSwimStyle i_Style, List<eLessonMode> i_Mode)
        {
            r_RegisteredStudents.Add(new Student(i_FirstName, i_LastName, i_Style, i_Mode));
        }

        public void AddInstructorToStaff(string i_InstructorName, List<eSwimStyle> i_SwimStyles)
        {
            r_Instructors.Add(new Instructor(i_InstructorName, i_SwimStyles));
        }


        private List<List<Lesson>> createAvailabilityBoardForInstructor()
        {
            return Enumerable.Repeat(default(List<Lesson>), k_AmountOfDaysInWeek).ToList();
            // NOTE: this function initializing only the main list representing the days!
            //Therefore --> days that are not available will remain null and will not be allocated!
        }


        // think of the algorithm
        // working by priorities -- as early as possible
        // first of all - fill the groups
        // afterwards - fill in the private
        // createLesson(...)
    }
}
