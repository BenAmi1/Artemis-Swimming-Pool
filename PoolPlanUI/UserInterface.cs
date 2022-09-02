
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
        private bool m_AgendaGenerated = false;
        private const int k_AmountOfWorkingDays = 5;
        private readonly int r_MenuOptionsSize;
        private const string k_Blank = " ";

        public UserInterface(string i_Mode)
        {
            r_RunPool = new PoolManagement();
            r_AmountOfSwimStyles = Enum.GetNames(typeof(eSwimStyle)).Length;
            r_MenuOptionsSize = Enum.GetNames(typeof(eMenuOptions)).Length -2; // Except 'exit', 'unfefined'
            if (i_Mode == "admin")
            {
                insertionByForce();
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
                userInput = getNumber(0, 7);
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
                    displayLessonsOfStudent();
                    break;
                case eMenuOptions.GetLessonsOfInstructor:
                    displayLessonsOfInstructor();
                    break;
                case eMenuOptions.ParticipantsInALesson:
                    displayParticipantsOfLesson();
                    break;
                default:
                    break;
            }
        }

        private void addNewStudent()
        {
            string studentFirstName, studentLastName;
            eSwimStyle swimStyle;
            List<eLessonMode> lessonModePriorities;

            if(m_AgendaGenerated == true)
            {
                if(r_RunPool.ConflictedStudents.Count > 0) // There are already conflicted students!
                {
                    Console.WriteLine("Unable to add more students - no available lessons");
                    PressAnyKeyToConitnue();
                    return;
                }
            }

            Console.WriteLine("Please insert the student's first name:");
            studentFirstName = getName();
            Console.WriteLine("Please enter the student's last name:");
            studentLastName = getName();
            swimStyle = userInputGetSwimStyle();
            Console.Clear();
            lessonModePriorities = getLessonModePriorities();
            Console.Clear();
            r_RunPool.AddStudent(studentFirstName, studentLastName, swimStyle, lessonModePriorities);
            Console.WriteLine(@"{0} {1} registered successfully!", studentFirstName, studentLastName);
            //System.Threading.Thread.Sleep(500); // pause before clear screen
        }

        private void addInstructor()
        {
            string firstName;
            List<eSwimStyle> swimStyles;

            //if (m_AgendaGenerated == true)
            //{
            //    Console.WriteLine("Unable to add more instructors. Week shcedule have been generated already");
            //    PressAnyKeyToConitnue();
            //    return;
            //}

            Console.WriteLine("Please insert the instructor's first name:");
            firstName = getName();
            swimStyles = getSwimStyle();
            r_RunPool.AddInstructorToStaff(firstName, swimStyles);
            Console.WriteLine(@"{0} added successfully to the stuff of the pool!", firstName);
            //System.Threading.Thread.Sleep(500); // pause before clear screen
            Console.Clear();
            addAvailabilityToInstructor(r_RunPool.InstructorsList[r_RunPool.InstructorsList.Count - 1]);

            if (m_AgendaGenerated == true)
            {
                checkIfHoursAdditionHelped();
            }
        }

        private List<eSwimStyle> getSwimStyle()
        {
            string userInput = "yes";
            List<eSwimStyle> swimStyles = new List<eSwimStyle>();
            eSwimStyle chosenSwimStyle;

            while (userInput == "yes")
            {
                chosenSwimStyle = userInputGetSwimStyle();
                while (swimStyles.Contains(chosenSwimStyle))
                {
                    Console.WriteLine(@"{0} style is already in list!", chosenSwimStyle.ToString());
                    chosenSwimStyle = userInputGetSwimStyle();
                }
                swimStyles.Add(chosenSwimStyle);
                if (swimStyles.Count == r_AmountOfSwimStyles)
                {
                    break;
                }
                Console.WriteLine("Swim style added. Type 'yes' to add another style, or any other key to continue");
                userInput = Console.ReadLine();
                Console.Clear();
            }

            return swimStyles;
        }
        

        private void addAvailabilityToExistingInstructor()
        {
            int chosenInstructorIndex;

            Console.WriteLine("Please Choose one of the following instructors:\n");
            for (int i = 0; i < r_RunPool.InstructorsList.Count; i++)
            {
                Console.WriteLine(@"Press ({0}) for {1}", i + 1, r_RunPool.InstructorsList[i].InstructorName);
            }
            chosenInstructorIndex = getNumber(1, r_RunPool.InstructorsList.Count) - 1;
            Console.Clear();

            addDaysAndHours(r_RunPool.InstructorsList[chosenInstructorIndex]);
            if (m_AgendaGenerated == true)
            {
                checkIfHoursAdditionHelped();
            }
        }

        private void checkIfHoursAdditionHelped()
        {
            string userInput;

            if (TryToReduceNumberOfConflicts())
            {
                Console.WriteLine("Adding hours reduced conflicts! Press (1) to see updated schedule" +
                                  " and conflicts, or any other key to continue");
                userInput = Console.ReadLine();
                if (userInput == "1")
                {
                    generateAndPrintWeekAgenda();
                }
            }
        }

        private bool TryToReduceNumberOfConflicts()
        {
            int numberOfConflicted = r_RunPool.ConflictedStudents.Count;
            bool success = false;

            r_RunPool.AssignWeekAgenda(r_RunPool.ConflictedStudents);
            for (int i = 0; i < numberOfConflicted; i++)
            {
                if (r_RunPool.ConflictedStudents[i].IsBooked())
                {
                    r_RunPool.ConflictedStudents.RemoveAt(i);
                    numberOfConflicted--;
                    i--;
                    success = true;
                }
            }
            return success;
        }

        private void generateAndPrintWeekAgenda()
        {
            if (!m_AgendaGenerated)
                r_RunPool.AssignWeekAgenda(r_RunPool.RegisteredStudents);
            printWeekAgenda();
            if (r_RunPool.ConflictedStudents.Count > 0)
            {
                getConflicts();
            }
            m_AgendaGenerated = true;
        }

        void displayLessonsOfStudent()
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

            index = getNumber(1, r_RunPool.RegisteredStudents.Count);
            Console.Clear();
            chosenStudent = r_RunPool.RegisteredStudents[index - 1];
            if (chosenStudent.studentlLessons.Count == 0)
            {
                Console.WriteLine(@"{0} {1} was not able to adress a lesson");
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

        private void printLessonOfInstructor(List<Lesson> i_Day)
        {
            foreach (Lesson lesson in i_Day)
            {
                Console.WriteLine(@"[{0} - {1}], {2} - {3}", lesson.HourToDisplay[0], lesson.HourToDisplay[1],
                                                             lesson.LessonMode, lesson.SwimStyle);
            }
        }

        private void printInstructorsLesson(Instructor i_Insturctor)
        {
            int daysCounter = 0, emptyDaysCounter = 0;

            foreach (List<Lesson> day in i_Insturctor.instructorLessonsSchedule)
            {
                if (day == null)
                {
                    emptyDaysCounter++;
                }
                else
                {
                    Console.WriteLine(@"{0}'s Lessons:", ((eWeekDay)daysCounter).ToString());
                    printLessonOfInstructor(day);
                    Console.WriteLine();
                }
                daysCounter++;
            }

            if (emptyDaysCounter == daysCounter)
            {
                Console.WriteLine("No lesson were scheduled to {0} this week", i_Insturctor.InstructorName);
            }
        }

        private void displayLessonsOfInstructor()
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

            index = getNumber(1, r_RunPool.InstructorsList.Count);
            Console.Clear();
            chosenInstructor = r_RunPool.InstructorsList[index - 1];
            printInstructorsLesson(chosenInstructor);
            PressAnyKeyToConitnue();
        }

        private void displayParticipantsOfLesson()
        {
            int index = 0, counter = 0, chosenDay = 0;
            Lesson chosenLesson;

            Console.Clear();
            chosenDay = getDay();
            Console.Clear();
            Console.WriteLine("Please choose a lesson to show it's Participants:");
            if (r_RunPool.WeekAgenda[chosenDay] == null)
            {
                Console.Clear();
                Console.WriteLine("There are no lessons on this day");
                PressAnyKeyToConitnue();
                return;
            }

            foreach (Lesson lesson in r_RunPool.WeekAgenda[chosenDay])
            {
                Console.WriteLine(@"({0}) {1}, [{2} - {3}], {4}, {5}", index + 1, lesson.LessonDay,
                                                                        lesson.HourToDisplay[0],
                                                                        lesson.HourToDisplay[1],
                                                                        lesson.LessonMode, lesson.SwimStyle);
                index++;
                counter++;
            }

            index = getNumber(1, counter);
            chosenLesson = r_RunPool.WeekAgenda[chosenDay][index - 1];
            Console.Clear();
            printParticipantsInLesson(chosenLesson);
        }

        private void printParticipantsInLesson(Lesson i_ChosenLesson)
        {
            Console.WriteLine(@"{0}, [{1} - {2}], {3} {4}", i_ChosenLesson.LessonDay,
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


        void printWeekAgenda()
        {
            int verticaloffset = 0, screenHorizontalOffset = 0, maxVerticalOffset = 0;

            Console.Clear();
            if (r_RunPool.WeekIsEmpty())
            {
                Console.WriteLine("No lessons scheduled this week");
                return;
            }

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
                    if (verticaloffset > maxVerticalOffset)
                        maxVerticalOffset = verticaloffset;
                    screenHorizontalOffset += 24;
                }
            }
            Console.SetCursorPosition(0, maxVerticalOffset+2);

            PressAnyKeyToConitnue();
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

        private void printConflictedStudents(List<List<int>> i_Conflicts)
        {
            int index = 1;
            foreach(Student student in r_RunPool.ConflictedStudents)
            {
                Console.WriteLine(@"#{0}: {1} {2}: {3} {4} lesson", index, student.FirstName, student.LastName,
                                                                    student.FirstPriority, student.RequestedSwimStyle);
                index++;

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
            TryToDealWithConflicts(conflicts);
            PressAnyKeyToConitnue();
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
                Console.WriteLine();

            }
            Console.WriteLine();
        }

        public void TryToDealWithConflicts(List<List<int>> i_Conflicts)
        {
            List<int> additionsToInstructors = createAdditionsArray();
            int timeAdditionToStyle = 0;
            List<int> qualifiedInstructorIndexes;
            int chosenInstructorIndex = 0;

            for (int style = 0; style < r_AmountOfSwimStyles; style++)
            {
                if (i_Conflicts[0][style] % PoolManagement.k_GroupLessonCapacity == 0)
                {
                    timeAdditionToStyle = i_Conflicts[0][style] / PoolManagement.k_GroupLessonCapacity * 60;
                }
                else
                {
                    timeAdditionToStyle = (((i_Conflicts[0][style] / PoolManagement.k_GroupLessonCapacity) + 1) * 60);
                }

                timeAdditionToStyle +=  + i_Conflicts[1][style] * 45;
                if (timeAdditionToStyle != 0) 
                {
                    qualifiedInstructorIndexes = r_RunPool.IndexesOfQualifiedInstructors((eSwimStyle)style);
                    chosenInstructorIndex = r_RunPool.GetTheLessBusyInstructor(qualifiedInstructorIndexes);
                    additionsToInstructors[chosenInstructorIndex] = timeAdditionToStyle;
                }

                timeAdditionToStyle = 0;
            }

            printSuggestions(additionsToInstructors);
        }

        private void printSuggestions(List<int> i_AdditionsToInstructors)
        {
            int hours = 0, minutes = 0;
            for (int i = 0; i < i_AdditionsToInstructors.Count; i++)
            {
                if (i_AdditionsToInstructors[i] != 0)
                {
                    hours = (i_AdditionsToInstructors[i] - (i_AdditionsToInstructors[i] % 60)) / 60;
                    minutes = i_AdditionsToInstructors[i] % 60;
                    Console.WriteLine("Consider adding {0} {1} hours and {2} minutes",
                                      r_RunPool.InstructorsList[i].InstructorName,
                                      hours, minutes);
                }
            }
            Console.WriteLine();
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

        private void addAvailabilityToInstructor(Instructor i_Instructor)
        {
            string userInput = "yes";

            Console.WriteLine("Now choose days and hours {0} can work:", i_Instructor.InstructorName);

            while(userInput == "yes")
            {
                addDaysAndHours(i_Instructor);
                Console.WriteLine("Availability added. Press 'yes' to add availability, or press other key to continue");
                userInput = Console.ReadLine();
                Console.Clear();
            }
        }

        private int getNumber(int i, int j)
        {
            string input = Console.ReadLine();
            int validInput = 0;

            while (!int.TryParse(input, out validInput) || !(validInput >= i && validInput <= j))
            {
                Console.WriteLine(@"Wrong input. please insert a number between {0} and {1}", i, j);
                input = Console.ReadLine();
            }

            return validInput;
        }

        private eSwimStyle userInputGetSwimStyle()
        {
            eSwimStyle requestedSwimStyle;

            Console.WriteLine("Please Choose one of the following swimming styles:\n");
            foreach (eSwimStyle style in Enum.GetValues(typeof(eSwimStyle)))
            {
                Console.WriteLine(@"Press ({0}) for {1}", (int)style + 1, style.ToString());
            }

            requestedSwimStyle = (eSwimStyle)getNumber(1, r_AmountOfSwimStyles) - 1;
            Console.Clear();
            return requestedSwimStyle;
        }

        private List<eLessonMode> getLessonModePriorities()
        {
            List<eLessonMode> priorities = new List<eLessonMode>();
            int input;

            Console.WriteLine("Please choose which lesson you would like to have:\n");
            Console.WriteLine("Please press (1) for group lesson only");
            Console.WriteLine("Please press (2) for private lesson only");
            Console.WriteLine("Please press (3) for group lesson as first priority, and private as the second priority");
            Console.WriteLine("Please press (4) for private lesson as first priority, and group as the second priority");
            input = getNumber(1, 4);

            return lessonModesPriorities(input, priorities);
        }
        
        private List<eLessonMode> lessonModesPriorities(int i_input, List<eLessonMode> i_PrioritiesList )
        {
            switch (i_input)
            {
                case 1:
                    i_PrioritiesList.Add(eLessonMode.Group);
                    i_PrioritiesList.Add(eLessonMode.None);
                    return i_PrioritiesList;
                case 2:
                    i_PrioritiesList.Add(eLessonMode.Private);
                    i_PrioritiesList.Add(eLessonMode.None);
                    return i_PrioritiesList;
                case 3:
                    i_PrioritiesList.Add(eLessonMode.Group);
                    i_PrioritiesList.Add(eLessonMode.Private);
                    return i_PrioritiesList;
                case 4:
                    i_PrioritiesList.Add(eLessonMode.Private);
                    i_PrioritiesList.Add(eLessonMode.Group);
                    return i_PrioritiesList;
                default:
                    return i_PrioritiesList;
            }

        }

        private int getDay()
        {
            int chosenDay;

            Console.WriteLine("Please Choose one of the following days:\n");
            for (int day = 0; day < PoolManagement.k_AmountOfDaysInWeek; day++)
            {
                Console.WriteLine(@"Press ({0}) for {1}", day + 1, ((eWeekDay)day).ToString());
            }
            chosenDay = getNumber(1, PoolManagement.k_AmountOfDaysInWeek) - 1;

            return chosenDay;
        }

        private void addDaysAndHours(Instructor i_Instructor)
        {
            int chosenDay=0;
            string from, to;
            TimeRange newPair = null;

            while (newPair == null)
            {
                chosenDay = getDay();
                Console.Clear();
                Console.WriteLine("Please Choose an hour in this format: XX:XX\n");
                Console.Write("FROM: ");
                from = Console.ReadLine();
                Console.Write("TO:   ");
                to = Console.ReadLine();
                newPair = validateHoursFormat(from, to);
                if (newPair == null)
                {
                    Console.WriteLine("Wrong input. Please press any key to continue.");
                    Console.ReadLine();
                }

                if (m_AgendaGenerated == true)
                {
                    if (!r_RunPool.DoesHoursAdditionSyncWithSchedule((eWeekDay)(chosenDay), newPair))
                    {
                        Console.WriteLine("Unable. Hours collides with scheduled lessons!");
                        PressAnyKeyToConitnue();
                        newPair=null;
                    }
                }
            }

            i_Instructor.AddAvailability((eWeekDay)(chosenDay), newPair);
            Console.Clear();
        }

        private TimeRange validateHoursFormat(string from, string to)
        {
            string elementToCheck = from;
            TimeRange hoursRange;
            int startingTime, endingTime;

            for (int check = 0; check < 2; check++)
            {
                if (elementToCheck.Length != 5)
                    return null;
                for (int indexInString = 0; indexInString < 5; indexInString++)
                {
                    if (indexInString == 2 && elementToCheck[indexInString] != ':')
                        return null;
                    if (indexInString != 2 && !char.IsDigit(from[indexInString]))
                        return null;
                }
                elementToCheck = to;
            }
            if (validHour(from) || validHour(to))
                return null;

            from = from.Remove(2, 1);// insert the hours into int, as it managed in pool schedule
            to = to.Remove(2, 1);// insert the hours into int, as it managed in pool schedule

            if (!int.TryParse(from, out startingTime) || !int.TryParse(to, out endingTime) || startingTime >= endingTime)
                return null;

            hoursRange = new TimeRange(startingTime, endingTime);
            return hoursRange;
        }

        private bool validHour(string hour)
        {
            return ((hour[0] > '2' || hour[3] > '5') || (hour[0] == '2' && hour[1] > '3')); // hour validation
        }

        private string getName()
        {
            string userInput;
            userInput = Console.ReadLine();
            Console.Clear();

            return userInput;
        }

        private List<int> createAdditionsArray()
        {
            return Enumerable.Repeat(default(int), r_RunPool.InstructorsList.Count).ToList();
        }

        // For presentation only -- allows quick insertion of input

        private void insertionByForce()
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
