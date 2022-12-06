using System;
using System.Collections.Generic;
using System.Linq;
using PoolPlanLogic;

namespace PoolPlanUI
{
    public class UserInterface
    {
        private readonly PoolManagement r_RunPool;
        private readonly int r_AmountOfSwimStyles;
        private readonly int r_MenuOptionsSize;
        private const int k_AmountOfWorkingDays = PoolManagement.k_AmountOfDaysInWeek;
        private const string k_Blank = " ";
        private const int k_CurserHorizontalOffset = 24;
        private bool m_AgendaGenerated = false;

        public UserInterface()
        {
            r_RunPool = new PoolManagement();
            r_AmountOfSwimStyles = Enum.GetNames(typeof(eSwimStyle)).Length;
            r_MenuOptionsSize = Enum.GetNames(typeof(eMenuOptions)).Length -2; // Except 'exit', 'unfefined'
            string dataInsertionMode = dataInsertionMethod();

            if (dataInsertionMode == "Test1")
            {
                TestingFunction1();
            }
            else if(dataInsertionMode == "Test2")
            {
                TestingFunction2();
            }
            Console.Clear();
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
                userInput = getNumber(0, r_MenuOptionsSize);
                Console.Clear();
                executeAction(userInput);
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
                    displayParticipantsInLesson();
                    break;
                default:
                    break;
            }
            Console.Clear();
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
            if(m_AgendaGenerated==true)
            {
                r_RunPool.AssignFirstPriorities(r_RunPool.LasStudentInList, r_RunPool.LasStudentInList.FirstPriority);
                r_RunPool.AssignSecondPriorities(r_RunPool.LasStudentInList, r_RunPool.LasStudentInList.FirstPriority);
            }
            System.Threading.Thread.Sleep(500); // pause before clear screen
        }

        private void addInstructor()
        {
            string firstName;
            List<eSwimStyle> swimStyles;

            Console.WriteLine("Please insert the instructor's first name:");
            firstName = getName();
            swimStyles = getSwimStyle();
            r_RunPool.AddInstructorToStaff(firstName, swimStyles);
            Console.WriteLine(@"{0} added successfully to the stuff of the pool!", firstName);
            Console.Clear();
            addAvailabilityToInstructor(r_RunPool.InstructorsList.Find(e => e.InstructorName == firstName));

            if (m_AgendaGenerated == true)
            {
                checkIfHoursAdditionHelped();
            }
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

        private void generateAndPrintWeekAgenda()
        {
            if (!m_AgendaGenerated)
            {
                r_RunPool.AssignWeekAgenda(r_RunPool.RegisteredStudents);
            }

            printWeekAgenda();
            if (r_RunPool.ConflictedStudents.Count > 0)
            {
                getConflicts();
            }

            m_AgendaGenerated = true;
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

        private void displayParticipantsInLesson()
        {
            int index = 0, counter = 0, chosenDay = 0;
            Lesson chosenLesson;

            Console.Clear();
            chosenDay = getDay();
            Console.Clear();
            Console.WriteLine("Please choose a lesson to show it's Participants:");
            if (r_RunPool.WeekAgenda[chosenDay] == null) // There are no lessons that day
            {
                Console.Clear();
                Console.WriteLine("There are no lessons on this day");
                PressAnyKeyToConitnue();
                return;
            }

            foreach (Lesson lesson in r_RunPool.WeekAgenda[chosenDay]) // display lesson in chosen day
            {
                Console.WriteLine(@"({0}) {1}, [{2} - {3}], {4}, {5}", index + 1, lesson.LessonDay,
                                                                       lesson.HourToDisplay[0],
                                                                       lesson.HourToDisplay[1],
                                                                       lesson.LessonMode, lesson.SwimStyle);
                index++;
                counter++;
            }

            index = getNumber(1, counter); // choose one of the lessons
            chosenLesson = r_RunPool.WeekAgenda[chosenDay][index - 1];
            Console.Clear();
            printParticipantsInLesson(chosenLesson);
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



        private void printParticipantsInLesson(Lesson i_ChosenLesson)
        {
            Console.WriteLine(@"{0}, [{1} - {2}], {3} {4}", i_ChosenLesson.LessonDay,
                                                            i_ChosenLesson.HourToDisplay[0],
                                                            i_ChosenLesson.HourToDisplay[1],
                                                            i_ChosenLesson.LessonMode, i_ChosenLesson.SwimStyle);
            Console.WriteLine("Registered students:");
            foreach (Student currentStudent in i_ChosenLesson.RegisteredStudents) // print students in lesson
            {
                Console.WriteLine(@"{0} {1}", currentStudent.FirstName, currentStudent.LastName);
            }

            PressAnyKeyToConitnue();
        }


        void printWeekAgenda()
        {
            int verticaloffset = 0, horizontalOffset = 0, maxVerticalOffset = 0;

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
                    Console.SetCursorPosition(horizontalOffset, verticaloffset);
                    Console.Write(@"{0}: ", ((eWeekDay)day).ToString());
                    Console.WriteLine(@"{0} lessons", r_RunPool.WeekAgenda[day].Count, ((eWeekDay)day).ToString());
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine();
                    verticaloffset = Console.CursorTop;
                    for (int index = 0; index < r_RunPool.WeekAgenda[day].Count; index++)
                    {
                        verticaloffset = Console.CursorTop;
                        printLessonData(day, index, horizontalOffset, verticaloffset);
                    }

                    verticaloffset = Console.CursorTop;
                    if (verticaloffset > maxVerticalOffset)
                    {
                        maxVerticalOffset = verticaloffset;
                    }
                    horizontalOffset += k_CurserHorizontalOffset;
                }
            }

            Console.SetCursorPosition(0, maxVerticalOffset+2); // set curser down the screen for next fucntion call
            PressAnyKeyToConitnue();
        }

        private bool areThereConflicts(List<List<int>> i_Conflicts, int lessonMode)
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
                if (!areThereConflicts(i_Conflicts, lessonMode))
                {
                    continue;
                }

                Console.WriteLine(@"{0} lessons:", ((eLessonMode)lessonMode).ToString());
                foreach (int styleCounter in i_Conflicts[lessonMode])
                {
                    if (styleCounter != 0)
                    {
                        if(lessonMode==0) // Group: [X students != X lessons] often, as 3 student can assign one lesson
                        {
                            Console.WriteLine(@"{0} {1} students", styleCounter, ((eSwimStyle)iteration).ToString());
                        }
                        else
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
            // calc how much hours need to be added to eliminate conflicts
            // also, look for the qualified instructor who works less than the others

            List<int> additionsToInstructors = createAdditionsArray();
            int timeAdditionToStyle = 0;
            List<int> qualifiedInstructorIndexes;
            int chosenInstructorIndex = 0;

            for (int style = 0; style < r_AmountOfSwimStyles; style++) 
            {
                if (i_Conflicts[0][style] % PoolManagement.k_GroupLessonCapacity == 0) 
                {
                    //calc how much group lesson hours need to be added, as each lesson can contain X students
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
                    additionsToInstructors[chosenInstructorIndex] += timeAdditionToStyle;
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
                    //convert minutes to hours
                    hours = (i_AdditionsToInstructors[i] - (i_AdditionsToInstructors[i] % 60)) / 60; 
                    minutes = i_AdditionsToInstructors[i] % 60;
                    Console.WriteLine(@"Consider adding {0} {1} hours and {2} minutes",
                                      r_RunPool.InstructorsList[i].InstructorName,
                                      hours, minutes);
                }
            }

            suggestSpecificTimeRange();
            Console.WriteLine();
        }

        private void suggestSpecificTimeRange()
        {
            Console.WriteLine("\nSuggestions:");
            List<Lesson> listOfLessons;
            for (int day = 0; day < k_AmountOfWorkingDays; day++)
            {
                if(r_RunPool.WeekAgenda[day]==null) // considering day starts at 8:00 and ends at 20:00
                {
                    Console.WriteLine(@"{0}, 8:00 - 20:00", ((eWeekDay)day).ToString());
                }
                else // looking for specific extra time range to recommand on
                {
                    listOfLessons = r_RunPool.WeekAgenda[day];
                    if (listOfLessons[0].LessonHour.Start - 800 > 100) 
                    {
                        Console.WriteLine(@"{0}, 8:00 - {1}", ((eWeekDay)day),
                                                              listOfLessons[0].HourToDisplay[0]);
                    }

                    for (int index=0; index < r_RunPool.WeekAgenda[day].Count-1; index++) 
                    {
                        if(listOfLessons[index+1].LessonHour.Start - listOfLessons[index].LessonHour.End >100)
                        {
                            Console.WriteLine(@"{0}, {1} - {2}", ((eWeekDay)day),
                                                                 listOfLessons[index].HourToDisplay[1],
                                                                 listOfLessons[index + 1].HourToDisplay[0]);
                        }
                    }

                    if (2000 - listOfLessons[listOfLessons.Count-1].LessonHour.End > 100)
                    {
                        Console.WriteLine(@"{0}, {1} - 20:00", ((eWeekDay)day),
                                                                listOfLessons[listOfLessons.Count - 1].HourToDisplay[1]); 
                    }
                }
            }

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
            int chosenDay = 0;
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
                    if (!r_RunPool.IsSyncedWithWeekSchedule((eWeekDay)(chosenDay), newPair))
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
            return ((hour[0] > '2' || hour[3] > '5') || (hour[0] == '2' && hour[1] > '3')); // hour validation check
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

        private void PressAnyKeyToConitnue() // utility fucntion
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        // For presentation only -- allows input insertion //
        /// </summary>

        private void TestingFunction1()
        {
            TestAddStudents1();
            TestAddInstructors1();
            Console.Clear();
        }

        private string dataInsertionMethod()
        {
            Console.WriteLine("Type one of the following:\n(1) Test1\n" +
                                                            "(2) Test2\n" +
                                                            "(3) Type anything to insert data manually");
            return Console.ReadLine();
        }

        public void TestAddStudents1()
        {
            int index = 0;
            Random rand = new Random();
            string[] names = System.IO.File.ReadAllLines(@"C:\Users\amirb\source\repos\Asgard's Pool\PoolPlanUI\Test_Names.txt");

            List<eSwimStyle> swimStyle = new List<eSwimStyle>();
            List<List<eLessonMode>> lessonModes = new List<List<eLessonMode>>();
            List<eLessonMode> mode1 = new List<eLessonMode>();
            List<eLessonMode> mode2 = new List<eLessonMode>();
            List<eLessonMode> mode3 = new List<eLessonMode>();
            List<eLessonMode> mode4 = new List<eLessonMode>();

            swimStyle.Add(eSwimStyle.Chest);
            swimStyle.Add(eSwimStyle.Butterfly);
            swimStyle.Add(eSwimStyle.Hatira);
            mode1.Add(eLessonMode.Private);
            mode1.Add(eLessonMode.Group);
            mode2.Add(eLessonMode.Private);
            mode2.Add(eLessonMode.None);
            mode3.Add(eLessonMode.Group);
            mode3.Add(eLessonMode.None);
            mode4.Add(eLessonMode.Group);
            mode4.Add(eLessonMode.Private);
            lessonModes.Add(mode1);
            lessonModes.Add(mode2);
            lessonModes.Add(mode3);
            lessonModes.Add(mode4);

            for (index = 0; index < 60; index += 2)
            {
                r_RunPool.AddStudent(names[index], names[index + 1], swimStyle[rand.Next() % 3], lessonModes[rand.Next() % 4]);
            }
        }


        public void TestAddInstructors1()
        {
            List<eSwimStyle> eSwimStylesYOTAM = new List<eSwimStyle>();
            List<eSwimStyle> eSwimStylesYONY = new List<eSwimStyle>();
            List<eSwimStyle> eSwimStylesJOHNNY = new List<eSwimStyle>();

            // Creating swim styles
            eSwimStylesYOTAM.Add(eSwimStyle.Chest);
            eSwimStylesYOTAM.Add(eSwimStyle.Hatira);
            eSwimStylesYOTAM.Add(eSwimStyle.Butterfly);
            eSwimStylesYONY.Add(eSwimStyle.Chest);
            eSwimStylesYONY.Add(eSwimStyle.Butterfly);
            eSwimStylesJOHNNY.Add(eSwimStyle.Chest);
            eSwimStylesJOHNNY.Add(eSwimStyle.Hatira);
            eSwimStylesJOHNNY.Add(eSwimStyle.Butterfly);

            // Adding instructors
            r_RunPool.AddInstructorToStaff("Yotam", eSwimStylesYOTAM);
            r_RunPool.AddInstructorToStaff("Yoni", eSwimStylesYONY);
            r_RunPool.AddInstructorToStaff("Johnny", eSwimStylesJOHNNY);

            // Adding constraints to instructors
            r_RunPool.addAvailablityToInstructor("Yotam", eWeekDay.Monday, new TimeRange(1600, 2000));
            r_RunPool.addAvailablityToInstructor("Yotam", eWeekDay.Thursday, new TimeRange(1600, 2000));
            r_RunPool.addAvailablityToInstructor("Yoni", eWeekDay.Tuesday, new TimeRange(800, 1500));
            r_RunPool.addAvailablityToInstructor("Yoni", eWeekDay.Wedensday, new TimeRange(800, 1500));
            r_RunPool.addAvailablityToInstructor("Yoni", eWeekDay.Thursday, new TimeRange(800, 1500));
            r_RunPool.addAvailablityToInstructor("Johnny", eWeekDay.Sunday, new TimeRange(1000, 1900));
            r_RunPool.addAvailablityToInstructor("Johnny", eWeekDay.Tuesday, new TimeRange(1000, 1900));
            r_RunPool.addAvailablityToInstructor("Johnny", eWeekDay.Thursday, new TimeRange(1000, 1900));
        }


        private void TestingFunction2()
        {
            TestAddStudents2();
            TestAddInstructors2();
            Console.Clear();
        }

        private void TestAddStudents2()
        {
            int index = 0;
            Random rand = new Random();
            string[] names = System.IO.File.ReadAllLines(@"C:\Users\amirb\source\repos\Asgard's Pool\PoolPlanUI\Test_Names.txt");

            List<eSwimStyle> swimStyle = new List<eSwimStyle>();
            List<List<eLessonMode>> lessonModes = new List<List<eLessonMode>>();
            List<eLessonMode> mode1 = new List<eLessonMode>();
            List<eLessonMode> mode2 = new List<eLessonMode>();
            List<eLessonMode> mode3 = new List<eLessonMode>();
            List<eLessonMode> mode4 = new List<eLessonMode>();

            swimStyle.Add(eSwimStyle.Chest);
            swimStyle.Add(eSwimStyle.Butterfly);
            swimStyle.Add(eSwimStyle.Hatira);
            mode1.Add(eLessonMode.Private);
            mode1.Add(eLessonMode.Group);
            mode2.Add(eLessonMode.Private);
            mode2.Add(eLessonMode.None);
            mode3.Add(eLessonMode.Group);
            mode3.Add(eLessonMode.None);
            mode4.Add(eLessonMode.Group);
            mode4.Add(eLessonMode.Private);
            lessonModes.Add(mode1);
            lessonModes.Add(mode2);
            lessonModes.Add(mode3);
            lessonModes.Add(mode4);

            for (index = 0; index < 60; index += 2)
            {
                r_RunPool.AddStudent(names[index], names[index + 1], swimStyle[rand.Next() % 3], lessonModes[rand.Next() % 4]);
            }
        }

        private void TestAddInstructors2() // only Yotam and Yoni
        {
            List<eSwimStyle> eSwimStylesYOTAM = new List<eSwimStyle>();
            List<eSwimStyle> eSwimStylesYONY = new List<eSwimStyle>();

            // Creating swim styles
            eSwimStylesYOTAM.Add(eSwimStyle.Chest);
            eSwimStylesYOTAM.Add(eSwimStyle.Hatira);
            eSwimStylesYOTAM.Add(eSwimStyle.Butterfly);
            eSwimStylesYONY.Add(eSwimStyle.Chest);
            eSwimStylesYONY.Add(eSwimStyle.Hatira);

            // Adding instructors
            r_RunPool.AddInstructorToStaff("Yotam", eSwimStylesYOTAM);
            r_RunPool.AddInstructorToStaff("Yoni", eSwimStylesYONY);

            // Adding constraints to instructors
            r_RunPool.addAvailablityToInstructor("Yotam", eWeekDay.Monday, new TimeRange(1600, 1900));
            r_RunPool.addAvailablityToInstructor("Yotam", eWeekDay.Thursday, new TimeRange(1600, 1900));
            r_RunPool.addAvailablityToInstructor("Yoni", eWeekDay.Tuesday, new TimeRange(800, 1200));
            r_RunPool.addAvailablityToInstructor("Yoni", eWeekDay.Wedensday, new TimeRange(800, 1200));
            r_RunPool.addAvailablityToInstructor("Yoni", eWeekDay.Thursday, new TimeRange(800, 1200));
        }
    }
}
