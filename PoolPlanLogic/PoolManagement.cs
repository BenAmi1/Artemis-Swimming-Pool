using System;
using System.Collections.Generic;
using System.Linq;

namespace PoolPlanLogic
{
    public class PoolManagement
    {
        private readonly List<Student> r_RegisteredStudents;
        private List<Instructor> m_Instructors;
        private readonly List<List<Lesson>> r_WeeklyLessonsSchedule;
        private readonly List<Student> r_ConflictedStudents;
        private const int k_NotFound = -1;
        public  const int k_PrivateLessonLength = 45;
        public  const int k_GroupLessonLength = 60;
        public  const int k_AmountOfDaysInWeek = 5;
        public  const int k_GroupLessonCapacity = 3;

        public PoolManagement()
        {
            r_RegisteredStudents = new List<Student>();
            m_Instructors = new List<Instructor>();
            r_WeeklyLessonsSchedule = new List<List<Lesson>>(); 
            r_WeeklyLessonsSchedule = createAvailabilityBoardForInstructor(); 
            r_ConflictedStudents = new List<Student>();
        }

        public List<List<Lesson>> WeekAgenda
        {
            get { return r_WeeklyLessonsSchedule; }
        }

        public List<Instructor> InstructorsList
        {
            get { return m_Instructors; }
        }

        public List<Student> RegisteredStudents
        {
            get { return r_RegisteredStudents; }
        }

        public List<Student> ConflictedStudents
        {
            get { return r_ConflictedStudents; }
        }

        public Student LasStudentInList
        {
            get { return RegisteredStudents[RegisteredStudents.Count - 1]; }
        }

        public void AddStudent(string i_FirstName, string i_LastName, eSwimStyle i_Style, List<eLessonMode> i_Mode)
        {
            r_RegisteredStudents.Add(new Student(i_FirstName, i_LastName, i_Style, i_Mode));
        }

        public void AddInstructorToStaff(string i_InstructorName, List<eSwimStyle> i_SwimStyles) // adding instructor
        {
            m_Instructors.Add(new Instructor(i_InstructorName, i_SwimStyles));
            m_Instructors = m_Instructors.OrderBy(p => p.InstructorsSwimStyles.Count).ToList();
        }

        private void sortLessonsByStartTime(int i_Day) // sorting lessons list, by start time of lesson
        {
            r_WeeklyLessonsSchedule[i_Day] = r_WeeklyLessonsSchedule[i_Day].OrderBy(p => p.LessonHour.Start).ToList();
        }

        public bool DayIsEmpty(int i_Day) // is that day contains no lessons
        {
            return WeekAgenda[i_Day] == null;
        }

        public bool addAvailablityToInstructor(string i_InstructorName, eWeekDay i_Day, TimeRange i_RangeOfHours)
        {
            //Assigning available hours to instructor
            int instructorIndex = GetInstructorIndexInList(i_InstructorName);

            if (instructorIndex != k_NotFound)
            {
                m_Instructors[instructorIndex].AddAvailability(i_Day, i_RangeOfHours);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AssignWeekAgenda(List<Student> i_ListOfStudents) // Creating schedule for next week
        {
            eLessonMode lessonMode = eLessonMode.Group;

            for (int iteration = 1; iteration <= 2; iteration++) 
            {
                foreach (Student currentStudent in i_ListOfStudents)
                {
                    AssignFirstPriorities(currentStudent, lessonMode);
                }
                lessonMode = eLessonMode.Private;
            }

            lessonMode = eLessonMode.Group;
            for (int iteration = 1; iteration <= 2; iteration++)
            {
                foreach (Student currentStudent in i_ListOfStudents)
                {
                    if (!currentStudent.IsBooked())
                    {
                        AssignSecondPriorities(currentStudent, lessonMode);
                    }
                }
                lessonMode = eLessonMode.Private;
            }
        }

        public void AssignFirstPriorities(Student i_CurrentStudent, eLessonMode i_LessonMode)
        {
            if (i_CurrentStudent.FirstPriority == i_LessonMode)// first: groups lesson, afterward: privates
            {
                if (i_LessonMode == eLessonMode.Group && AttemptToAssignToExistentLesson(i_CurrentStudent))
                    return;
                else
                {
                    if (!TryToCreateNewLessonAndAssignStudent(i_CurrentStudent, i_LessonMode) &&
                                                              i_CurrentStudent.SecondPriority == eLessonMode.None &&
                                                              !r_ConflictedStudents.Contains(i_CurrentStudent))
                    {
                        r_ConflictedStudents.Add(i_CurrentStudent);
                    }
                }
            }
        }

        public void AssignSecondPriorities(Student i_CurrentStudent, eLessonMode i_LessonMode)
        {
            if (i_CurrentStudent.SecondPriority == i_LessonMode)
            {
                if (i_LessonMode == eLessonMode.Group && AttemptToAssignToExistentLesson(i_CurrentStudent))
                    return;
                else
                {
                    if (!TryToCreateNewLessonAndAssignStudent(i_CurrentStudent, i_LessonMode) &&
                                                             !r_ConflictedStudents.Contains(i_CurrentStudent))
                    {
                        r_ConflictedStudents.Add(i_CurrentStudent);
                    }
                }
            }
        }

        public List<List<int>> HandleConflicts() // Get conflicts: students who could not register a lesson
        {
            List<List<int>> conflicts = new List<List<int>>();
            conflicts = Enumerable.Repeat(default(List<int>), 2).ToList();
            for (int i = 0; i < 2; i++) // init gap list
            {
                conflicts[i] = Enumerable.Repeat(default(int),3).ToList();
                foreach (eSwimStyle style in (eSwimStyle[])Enum.GetValues(typeof(eSwimStyle)))
                {
                    conflicts[i][(int)style] = new int();
                }
            }

            foreach (Student student in r_ConflictedStudents)
            {
                if (student.FirstPriority == eLessonMode.Group)
                    conflicts[(int)eLessonMode.Group][(int)student.RequestedSwimStyle]++;
                else
                    conflicts[(int)eLessonMode.Private][(int)student.RequestedSwimStyle]++;
            }

            return conflicts;
        }

        public bool TryToCreateNewLessonAndAssignStudent(Student i_CurrentStudent, eLessonMode i_LessonMode)
        {
            // Trying to book a lesson, on a busy day, or in any other day as second priority
            List<int> qualifiedInstructorsIndex;
            eLessonMode lessonMode = i_CurrentStudent.FirstPriority;
            qualifiedInstructorsIndex = IndexesOfQualifiedInstructors(i_CurrentStudent.RequestedSwimStyle);

            if (TryBookLessonOnIdealDay(qualifiedInstructorsIndex, i_CurrentStudent, lessonMode, false))
            {
                return true;
            }
            else
            {
                return TryBookLessonOnIdealDay(qualifiedInstructorsIndex, i_CurrentStudent, lessonMode, true);
            }
        }

        private bool TryBookLessonOnIdealDay(List<int> i_QualifiedInstructorsIndex, Student i_Student,
                                             eLessonMode i_Mode, bool i_CheckForAnyDay)
        {
            Tuple<bool, eWeekDay, int> result;
            Instructor currentInstructor;
            List<eWeekDay> daysToCheck;

            for (int instructorIndex = 0; instructorIndex < i_QualifiedInstructorsIndex.Count; instructorIndex++)
            {
                currentInstructor = m_Instructors[i_QualifiedInstructorsIndex[instructorIndex]];
                if (i_CheckForAnyDay == false)
                {
                    daysToCheck = currentInstructor.DaysCurrentInstructorBookedLesson();
                }
                else
                {
                    daysToCheck = GetAllWeekDays(); // unable to find a busy day, let's try to book on any day
                }

                if ((i_CheckForAnyDay == false && daysToCheck.Count != 0) || (i_CheckForAnyDay==true))
                {
                    result = currentInstructor.DoesInstructorCanBookThisLesson(i_Mode, daysToCheck);
                    if (result.Item1 == true)
                    {
                        CreateNewLesson(result, currentInstructor, i_Student, i_Mode);
                        return true;
                    }
                }
            }
            return false;
        }

        public void CreateNewLesson(Tuple<bool, eWeekDay, int> i_LessonData, Instructor i_Instructor, Student i_Student,
                                    eLessonMode i_LessonMode)
        {
            Lesson newLesson;
            newLesson = new Lesson(i_LessonData.Item2,
                                   i_Instructor.InstructorAvailability[(int)i_LessonData.Item2][i_LessonData.Item3],
                                   i_Student.RequestedSwimStyle, i_LessonMode, i_Instructor.InstructorName);
            i_Student.AddLessonToStudentAgenda(newLesson);
            i_Instructor.AddLessonToInstructor(newLesson, i_LessonData.Item3);
            newLesson.AddStudentToLesson(i_Student);
            if(r_WeeklyLessonsSchedule[(int)i_LessonData.Item2] == null) // setting new list of lesson on that day
            {
                r_WeeklyLessonsSchedule[(int)i_LessonData.Item2] = new List<Lesson>();
            }

            r_WeeklyLessonsSchedule[(int)i_LessonData.Item2].Add(newLesson); // adding new lesson on that day 
            foreach(Instructor instructor in m_Instructors)
            {
                if (instructor.InstructorAvailability[(int)newLesson.LessonDay] != null) 
                {
                    instructor.SyncInstructorsToPoolSchedule(newLesson); // Syncs pool schedule with all instructors
                }
            }
            sortLessonsByStartTime((int)newLesson.LessonDay);
        }

        public bool AttemptToAssignToExistentLesson(Student i_CurrentStudent) 
        {
            // Try to assign student to group lesson that already run
            for (int day = 0; day < k_AmountOfDaysInWeek; day++)
            {
                if(r_WeeklyLessonsSchedule[day] ==null)
                {
                    continue;
                }
                foreach (Lesson currentLesson in r_WeeklyLessonsSchedule[day])
                {
                    if (currentLesson.SwimStyle == i_CurrentStudent.RequestedSwimStyle &&
                        currentLesson.LessonMode == eLessonMode.Group &&
                        currentLesson.RegisteredStudents.Count < k_GroupLessonCapacity) 
                    {
                        i_CurrentStudent.AddLessonToStudentAgenda(currentLesson);
                        currentLesson.AddStudentToLesson(i_CurrentStudent);
                        return true;
                    }
                }
            }
            return false;
        }

        public List<int> IndexesOfQualifiedInstructors(eSwimStyle i_RequestedSwimStyle)
        {
            // Return the indexes of the instructor able to teach that specific swimming style
            List<int> indexesOfQualifiedInstructors = new List<int>();

            for (int instructorIndex = 0; instructorIndex < m_Instructors.Count; instructorIndex++)
            {
                foreach (eSwimStyle swimStyle in m_Instructors[instructorIndex].InstructorsSwimStyles)
                {
                    if (swimStyle == i_RequestedSwimStyle) // instructor is professionally fit
                    {
                        indexesOfQualifiedInstructors.Add(instructorIndex);
                        break;
                    }
                }
            }
            return indexesOfQualifiedInstructors;
        }

        public int GetTheLessBusyInstructor(List<int> i_IndexesListOfInsturctor)
        {
            int min = int.MaxValue, chosenIndex = 0, workingHours = 0;

            foreach(int index in i_IndexesListOfInsturctor)
            {
                workingHours = m_Instructors[index].AmountOfWorkingHours();
                if (workingHours < min)
                {
                    min = workingHours;
                    chosenIndex = index;
                }
            }

            return chosenIndex;
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

        public int GetInstructorIndexInList(string i_InstructorName) // for testing
        {
            int wantedIndex = 0;
            foreach (Instructor instructor in m_Instructors)
            {
                if (instructor.InstructorName == i_InstructorName)
                {
                    return wantedIndex;
                }
                wantedIndex++;
            }
            return k_NotFound;
        }

        public void testingFunction()
        {
            int counter = 0;
            foreach (Instructor instructor in m_Instructors)
            {
                foreach (List<Lesson> lst in instructor.instructorLessonsSchedule)
                {
                    if (lst != null)
                    {
                        counter += lst.Count;
                    }
                }
            }
            Console.WriteLine(counter);
        }

        public bool WeekIsEmpty()
        {
            int emptyDaysCounter = 0;

            for (int day = 0; day < k_AmountOfDaysInWeek; day++)
            {
                if (DayIsEmpty(day))
                {
                    emptyDaysCounter++;
                }
            }

            return emptyDaysCounter == k_AmountOfDaysInWeek;
        }

        public bool IsSyncedWithWeekSchedule(eWeekDay i_ChosenDay, TimeRange i_TimeRangeToCheck)
        {
            if(r_WeeklyLessonsSchedule[(int)i_ChosenDay] == null)
            {
                return true; // no lessons this day --> no collision
            }

            foreach(Lesson lesson in r_WeeklyLessonsSchedule[(int)i_ChosenDay])
            {
                if (lesson.LessonHour.CongruenceInHours(i_TimeRangeToCheck))
                {
                    return false;
                }
            }
            
            return true;
        }

        private List<List<Lesson>> createAvailabilityBoardForInstructor()
        {
            return Enumerable.Repeat(default(List<Lesson>), k_AmountOfDaysInWeek).ToList();
            // NOTE: this function initializing only the main list representing the days!
            //Therefore --> days that are not available will remain null and will not be allocated!
        }
    }
}
