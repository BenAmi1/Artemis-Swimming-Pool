
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoolPlanLogic;

namespace PoolPlanUI
{
    public class UserInterface
    {
        private readonly PoolManagement r_RunPool;
        private readonly int r_AmountOfSwimStyles;
        private const int k_AmountOfWorkingDays = 5;

        private readonly string k_Blank = " ";

        public UserInterface(string i_Mode)
        {
            r_RunPool = new PoolManagement();
            r_AmountOfSwimStyles = Enum.GetNames(typeof(eSwimStyle)).Length;
            if (i_Mode == "admin")
            {
                ForceInsertion();
            }
            MainMenu();
        }

        public void MainMenu()
        {
            int userInput = (int)eMenuOptions.UnDefined;
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Welcome to Asgard's Pool!\n");
            while (userInput != (int)eMenuOptions.Exit)
            {
                Console.WriteLine("Please choose one of the following options:\n");
                Console.WriteLine("press (1) to add new student to the pool");
                Console.WriteLine("press (2) to add instructor to the staff");
                Console.WriteLine("press (3) to add availability to existent instructor");
                Console.WriteLine("press (4) to get next week's agenda");
                Console.WriteLine("press (5) to display the booked lesson of a student");
                Console.WriteLine("press (6) to display the booked lessons of instructor");
                Console.WriteLine("press (7) to display the participents in a lesson");
                Console.WriteLine("press (0) to exit\n");

                userInput = UserInput.GetNumber(0, 7);
                Console.Clear();
                executeAction(userInput);
                Console.Clear();
            }
        }

        private void executeAction(int i_UserInput)
        {
            switch ((eMenuOptions)i_UserInput)
            {
                case eMenuOptions.Exit:
                    break;
                case eMenuOptions.AddStudent:
                    addNewStudent();
                    break;
                case eMenuOptions.AddInstructor:
                    addInstructor();
                    break;
                case eMenuOptions.addAvailabilityToExistingInstructor:
                    addAvailabilityToExistingInstructor();
                    break;
                case eMenuOptions.GetWeekAgenda:
                    generateAndPrintWeekAgenda();
                    break;
                case eMenuOptions.ShowLessonsOfStudent:
                    ShowLessonsOfStudent();
                    break;
                case eMenuOptions.GetLessonsOfInstructor:
                    DisplayLessonsOfInstructor();
                    break;
                case eMenuOptions.ParticipantsInALesson:
                    displayParticipantsOfLesson();
                    break;
            }
        }

        private void displayParticipantsOfLesson()
        {
            int index = 0, counter = 0, chosenDay = 0;
            Lesson chosenLesson;
            Console.Clear();

            chosenDay = UserInput.GetDay();
            Console.Clear();
            Console.WriteLine("Please choose a lesson to show it's Participants:");
            if (r_RunPool.WeekAgenda[chosenDay] == null)
            {
                Console.WriteLine("There are no lessons on this day");
                PressAnyKeyToConitnue();
            }

            foreach (Lesson lesson in r_RunPool.WeekAgenda[chosenDay])
            {
                Console.WriteLine(@"({0}) {1}, [{2} - {3}], {4} {5}", index + 1, lesson.LessonDay,
                                                                        lesson.HourToDisplay[0],
                                                                        lesson.HourToDisplay[1],
                                                                        lesson.LessonMode, lesson.SwimStyle);
                index++;
                counter++;
            }
            index = UserInput.GetNumber(1, counter);
            chosenLesson = r_RunPool.WeekAgenda[chosenDay][index - 1];
            Console.Clear();
            printParticipantsInLesson(chosenLesson);
        }

        private void printParticipantsInLesson(Lesson i_ChosenLesson)
        {
            Console.WriteLine(@"({0}) {1}, [{2} - {3}], {4} {5}", i_ChosenLesson.LessonDay,
                                                                  i_ChosenLesson.HourToDisplay[0],
                                                                  i_ChosenLesson.HourToDisplay[1],
                                                                  i_ChosenLesson.LessonMode, i_ChosenLesson.SwimStyle);
            Console.WriteLine("Registered students:");
            foreach (Student currentStudent in i_ChosenLesson.RegisteredStudents)
            {
                Console.WriteLine(@"{0}, {1}", currentStudent.FirstName, currentStudent.LastName);
            }

            PressAnyKeyToConitnue();
        }


        private void DisplayLessonsOfInstructor()
        {
            int index = 0;
            Instructor chosenInstructor;
            Console.Clear();
            Console.WriteLine("Please choose an instructor to show his lessons:");
            foreach (Instructor instructor in r_RunPool.InstructorsList)
            {
                Console.WriteLine(@"({0}) {1}", index + 1, instructor.InstructorName);
                index++;
            }

            index = UserInput.GetNumber(1, r_RunPool.InstructorsList.Count);
            Console.Clear();
            chosenInstructor = r_RunPool.InstructorsList[index - 1];
            int daysCounter = 0;
            foreach(List<Lesson> day in chosenInstructor.instructorLessonsSchedule)
            {
                if (day == null)
                {
                    daysCounter++;
                    continue;
                }
                else
                {
                    Console.WriteLine(@"{0}'s Lessons:", ((eWeekDay)daysCounter).ToString());
                    foreach (Lesson lesson in day)
                    {
                        Console.WriteLine(@"[{0} - {1}], {2} - {3}", lesson.HourToDisplay[0], lesson.HourToDisplay[1],
                                                                     lesson.LessonMode, lesson.SwimStyle);
                        Console.WriteLine();
                    }
                }
                daysCounter++;
            }
            PressAnyKeyToConitnue();
        }

        void ShowLessonsOfStudent()
        {
            int index = 0;
            Student chosenStudent;
            Console.Clear();
            Console.WriteLine("Please choose a student to show his lessons:");
            foreach (Student student in r_RunPool.RegisteredStudents)
            {
                Console.WriteLine(@"Press ({0}) {1} {2}", index + 1, student.FirstName, student.LastName);
                index++;
            }
            index = UserInput.GetNumber(1, r_RunPool.RegisteredStudents.Count);

            chosenStudent = r_RunPool.RegisteredStudents[index - 1];
            if (chosenStudent.studentlLessons.Count == 0)
            {
                Console.WriteLine(@"Unfortunately, {0} {1} was not able to adress a lesson");
            }
            else
            {
                Console.WriteLine(@"{0} {1} lessons:", chosenStudent.FirstName, chosenStudent.LastName);
                foreach (Lesson lesson in chosenStudent.studentlLessons)
                {
                    Console.WriteLine(@"#{0} - {1} lesson, Instructor: {2}", lesson.LessonMode,
                        lesson.SwimStyle, lesson.LessonInstructor);
                }
            }
            PressAnyKeyToConitnue();
        }

        void printWeekAgenda()
        {
            int verticaloffset = 0, screenHorizontalOffset = 0;
            for (int day = 0; day < k_AmountOfWorkingDays; day++)
            {
                verticaloffset = 0;
                if (r_RunPool.WeekAgenda[day] != null)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.SetCursorPosition(screenHorizontalOffset, verticaloffset);
                    Console.Write(@"{0}: ", ((eWeekDay)day).ToString());
                    Console.WriteLine(@"{0} lessons", r_RunPool.WeekAgenda[day].Count, ((eWeekDay)day).ToString());
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine();
                    verticaloffset = Console.CursorTop;
                    for (int index = 0; index < r_RunPool.WeekAgenda[day].Count; index++)
                    {
                        verticaloffset = Console.CursorTop;
                        printLessonData(day, index, screenHorizontalOffset, verticaloffset);
                    }
                    verticaloffset = Console.CursorTop;
                    screenHorizontalOffset += 24;
                }
            }
        }

        private void generateAndPrintWeekAgenda()
        {
            r_RunPool.AssignWeekAgenda();
            printWeekAgenda();
            PressAnyKeyToConitnue();
            if (r_RunPool.ConflictedStudents.Count > 0)
            {
                getConflicts();
            }
        }

        private bool theraAreConflicts(List<List<int>> i_Conflicts, int lessonMode)
        {
            foreach (int counter in i_Conflicts[lessonMode])
            {
                if (counter != 0)
                {
                    return true;
                }
            }
            return false;
        }

        private void printConflistsInTotal(List<List<int>> i_Conflicts)
        {
            int iteration = 0;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("TOTAL CONFLICTS:");
            Console.ForegroundColor = ConsoleColor.White;
            for (int lessonMode = 0; lessonMode < 2; lessonMode++)
            {
                if (!theraAreConflicts(i_Conflicts, lessonMode))
                {
                    continue;
                }

                Console.WriteLine(@"{0} lessons:", ((eLessonMode)lessonMode).ToString());
                foreach (int styleCounter in i_Conflicts[lessonMode])
                {
                    if (styleCounter != 0)
                    {
                        Console.WriteLine(@"{0} {1} lessons", styleCounter, ((eSwimStyle)iteration).ToString());
                    }
                    iteration++;
                }
                iteration = 0;
            }
            Console.WriteLine();
        }

        private void printConflictedStudents(List<List<int>> i_Conflicts)
        {
            foreach(Student student in r_RunPool.ConflictedStudents)
            {
                Console.WriteLine(@"#{0} {1}: {2} {3} lesson", student.FirstName, student.LastName,
                                                              student.FirstPriority, student.RequestedSwimStyle);

            }
            Console.WriteLine();
        }

        private void getConflicts()
        {
            List<List<int>> conflicts;

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("CONFLICTS: {0} students could not address a lesson:",r_RunPool.ConflictedStudents.Count);
            Console.ForegroundColor = ConsoleColor.White;
            conflicts = r_RunPool.HandleConflicts();
            printConflictedStudents(conflicts);
            printConflistsInTotal(conflicts);
            PressAnyKeyToConitnue();
        }

        private void PressAnyKeyToConitnue()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private void printLessonData(int i_Day, int i_Index, int i_ScreenHorizontalOffset, int i_Verticaloffset)
        {
            Lesson lesson;
            Console.SetCursorPosition(i_ScreenHorizontalOffset + 2, i_Verticaloffset);
            lesson = r_RunPool.WeekAgenda[i_Day][i_Index];
            Console.WriteLine(@"{0}, {1}", lesson.LessonMode.ToString(), lesson.SwimStyle.ToString());
            i_Verticaloffset = Console.CursorTop;
            Console.SetCursorPosition(i_ScreenHorizontalOffset + 2, i_Verticaloffset);
            Console.WriteLine(@"({0} - {1})", lesson.HourToDisplay[0], lesson.HourToDisplay[1]);
            i_Verticaloffset = Console.CursorTop;
            Console.SetCursorPosition(i_ScreenHorizontalOffset + 2, i_Verticaloffset);
            Console.WriteLine(@"   [{0}]", lesson.LessonInstructor);
            Console.WriteLine();
            i_Verticaloffset = Console.CursorTop;
        }

        private void addAvailabilityToExistingInstructor()
        {
            int chosenInstructorIndex;

            Console.WriteLine("Please Choose one of the following instructors:\n");
            for (int i = 0; i < r_RunPool.InstructorsList.Count; i++)
            {
                Console.WriteLine(@"Press ({0}) for {1}", i + 1, r_RunPool.InstructorsList[i].InstructorName);
            }
            chosenInstructorIndex = UserInput.GetNumber(1, r_RunPool.InstructorsList.Count) -1;
            Console.Clear();
            addAvailabilityToInstructor(r_RunPool.InstructorsList[chosenInstructorIndex]);
        }

        private void addInstructor()
        {
            string userInput;
            string firstName;
            List<eSwimStyle> swimStyles = new List<eSwimStyle>();
            Console.WriteLine("Please insert the instructor's first name:");
            firstName = UserInput.GetName();
            swimStyles.Add(UserInput.GetSwimStyle());
            Console.WriteLine("Would you like to add another swimming style?");

            do
            {
                Console.WriteLine("Write one of the following (yes:no)");
                userInput = Console.ReadLine();
                Console.Clear();
                while (userInput == "yes" && swimStyles.Count < r_AmountOfSwimStyles)
                {
                    swimStyles.Add(UserInput.GetSwimStyle());
                    Console.Clear();
                    if(swimStyles.Count< r_AmountOfSwimStyles)
                        Console.WriteLine("Would you like to add another swimming style?");
                    userInput = k_Blank;
                }
                if (userInput == "no" || swimStyles.Count == r_AmountOfSwimStyles)
                    break;

            } while (userInput != "yes" && userInput != "no");

            r_RunPool.AddInstructorToStaff(firstName, swimStyles);
            Console.WriteLine(@"{0} added successfully to the stuff of the pool!", firstName);
            //System.Threading.Thread.Sleep(500); // pause before clear screen
            Console.Clear();
            addAvailabilityToInstructor(r_RunPool.InstructorsList[r_RunPool.InstructorsList.Count-1]);
        }

        private void addAvailabilityToInstructor(Instructor i_Instructor)
        {
            string userInput;

            Console.WriteLine("Now choose days and hours {0} can work:", i_Instructor.InstructorName);
            UserInput.AddDaysAndHours(i_Instructor);
            Console.WriteLine("Availability added successfully. Would you like to add another day and hours range?");

            do
            {
                Console.WriteLine("Write one of the following (yes:no)");
                userInput = Console.ReadLine();
                Console.Clear();
                while (userInput == "yes")
                {
                    UserInput.AddDaysAndHours(i_Instructor);
                   //System.Threading.Thread.Sleep(500); // pause before clear screen
                    Console.WriteLine("Would you like to add more days and hours range?");
                    userInput = k_Blank;
                }
                if (userInput == "no")
                    break;

            } while (userInput != "yes" && userInput != "no");
        }

        private void addNewStudent()
        {
            string studentFirstName, studentLastName;
            eSwimStyle swimStyle;
            List<eLessonMode> lessonModePriorities;

            Console.WriteLine("Please insert the student's first name:");
            studentFirstName = UserInput.GetName();
            Console.WriteLine("Please enter the student's last name:");
            studentLastName = UserInput.GetName();
            swimStyle = UserInput.GetSwimStyle();
            Console.Clear();
            lessonModePriorities = UserInput.GetLessonModePriorities();
            Console.Clear();
            r_RunPool.AddStudent(studentFirstName, studentLastName, swimStyle, lessonModePriorities);
            Console.WriteLine(@"{0} {1} registered successfully!", studentFirstName, studentLastName);
            //System.Threading.Thread.Sleep(1500); // pause before clear screen

        }

        private void ForceInsertion()
        {
            int i = 0;

            List<eSwimStyle> swimStyle = new List<eSwimStyle>();
            swimStyle.Add(eSwimStyle.Chest);
            swimStyle.Add(eSwimStyle.Butterfly);
            swimStyle.Add(eSwimStyle.Hatira);
            List<List<eLessonMode>> lessonModes = new List<List<eLessonMode>>();
            List<eLessonMode> lm1 = new List<eLessonMode>();
            lm1.Add(eLessonMode.Private);
            lm1.Add(eLessonMode.Group);
            List<eLessonMode> lm2 = new List<eLessonMode>();
            lm2.Add(eLessonMode.Private);
            lm2.Add(eLessonMode.None);
            List<eLessonMode> lm3 = new List<eLessonMode>();
            lm3.Add(eLessonMode.Group);
            lm3.Add(eLessonMode.None);
            List<eLessonMode> lm4 = new List<eLessonMode>();
            lm4.Add(eLessonMode.Group);
            lm4.Add(eLessonMode.Private);
            lessonModes.Add(lm1);
            lessonModes.Add(lm2);
            lessonModes.Add(lm3);
            lessonModes.Add(lm4);

            for (i = 0; i < 30; i++)
            {
                string firstname = Console.ReadLine();
                string lastName = Console.ReadLine();
                r_RunPool.AddStudent(firstname, lastName, swimStyle[i % 3], lessonModes[i % 4]);
            }
            Console.Clear();
        }


    }
}
