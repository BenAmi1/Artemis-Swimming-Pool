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
            // to check about the priorities!!

            bool assignSucceed = false;
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
                        assignSucceed=CreateNewLessonAndAssignStudent(currentStudent, eLessonMode.Group);
                        //return popup;
                    }
                }
                else
                {
                    assignSucceed=CreateNewLessonAndAssignStudent(currentStudent, eLessonMode.Private);
                    // return popup
                }
            }

        }

        public bool CreateNewLessonAndAssignStudent(Student i_CurrentStudent, eLessonMode i_LessonMode)
        {
            // dont forget to update the lessons list in pool management class
            List<eWeekDay> daysCurrentInstructorWorks;
            List<eWeekDay> allDaysOfWeek;
            List<int> qualifiedInstructorsIndex;
            Tuple<bool, eWeekDay, int> result;
            Instructor currentInstructor;
            eLessonMode lessonMode = i_CurrentStudent.StudentFirstPriority;

            for (int tryOutToAssign = 1; tryOutToAssign <= 2; tryOutToAssign++)
            {
                qualifiedInstructorsIndex = IndexesOfQualifiedInstructors(i_CurrentStudent.RequestedSwimStyle);
                for (int instructorIndex = 0; instructorIndex < qualifiedInstructorsIndex.Count; instructorIndex++)
                {
                    currentInstructor = r_Instructors[instructorIndex]; // check if ok (modified by value or ref...)
                    daysCurrentInstructorWorks = currentInstructor.DaysCurrentInstructorBookedLesson();
                    if (daysCurrentInstructorWorks.Count != 0)
                    {
                        // does this 'result' can be modified
                        result = currentInstructor.InstructorCanBookThisLesson(lessonMode, daysCurrentInstructorWorks); // need to be in amother place, change this loop --> coliision between this list and the if
                        if (result.Item1 == true) // success! instructor can take lesson in day he works!
                        {
                            CreateNewLesson(result, currentInstructor, i_CurrentStudent, lessonMode);

                            // considering that the lesson percise hours changed in addlessonToInstructor (to check), now
                            // we combine it in the pools schedule

                            // to add student to this lesson
                            // to add this lesson to the list of lesson if instructor
                            // to add this lesson to the general schedule
                        }
                        // make shiboots!
                    
                        // return -- must!
                    }
                }


                allDaysOfWeek = GetAllWeekDays();
                for (int instructorIndex = 0; instructorIndex < qualifiedInstructorsIndex.Count; instructorIndex++)
                {
                    currentInstructor = r_Instructors[instructorIndex]; // check if ok (modified by value or ref...)
                    result = currentInstructor.InstructorCanBookThisLesson(lessonMode, allDaysOfWeek); // need to be in amother place, change this loop --> coliision between this list and the if
                    if (result.Item1 == true) // success! instructor can take lesson in day he works!
                    {
                        // make shiboots!
                        // return -- must!
                    }
                }


                if (i_CurrentStudent.StudentSecondPriority != eLessonMode.None && tryOutToAssign == 1) // lets try to assingn second priority
                {
                    lessonMode = i_CurrentStudent.StudentSecondPriority;
                    if (lessonMode == eLessonMode.Group && AttemptToAssignToExistentLesson(i_CurrentStudent))
                    {
                        return true;
                    }
                }
                else
                {
                    return false; // return pop-up
                }
            }

            return false; // return pop up
            // not Found!
        }

        public void CreateNewLesson(Tuple<bool, eWeekDay, int> i_LessonData, Instructor i_Instructor, Student i_Student, eLessonMode i_LessonMode)
        {
            Lesson newLesson;
            newLesson = new Lesson(i_LessonData.Item2,
                        i_Instructor.InstructorAvailability[(int)i_LessonData.Item2][i_LessonData.Item3],
                        i_Student.RequestedSwimStyle, i_LessonMode, i_Instructor.InstructorName);
            i_Student.AddLessonToStudent(newLesson);
            i_Instructor.AddLessonToInstructor(newLesson, i_LessonData.Item3);
            newLesson.AddStudentToLesson(i_Student);
            if(r_WeeklyLessonsSchedule[(int)i_LessonData.Item2] == null)
            {
                r_WeeklyLessonsSchedule[(int)i_LessonData.Item2] = new List<Lesson>();
            }
            r_WeeklyLessonsSchedule[(int)i_LessonData.Item2].Add(newLesson); // allow me to know if pool is vacant

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


        

        public List<int> IndexesOfQualifiedInstructors(eSwimStyle i_RequestedSwimStyle)
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
            return indexesOfQualifiedInstructors;
        }

        public List<eWeekDay> GetAllWeekDays()
        {
            List<eWeekDay> allOperatingWeekDays = new List<eWeekDay>();
            for (int i = 0; i < PoolManagement.k_AmountOfDaysInWeek; i++)
            {
                allOperatingWeekDays.Add((eWeekDay)i);
            }
            return allOperatingWeekDays;
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
