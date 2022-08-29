﻿
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

        public UserInterface()
        {
            r_RunPool = new PoolManagement();
            r_AmountOfSwimStyles = Enum.GetNames(typeof(eSwimStyle)).Length;
            MainMenu();
        }

        public void MainMenu()
        {
            int userInput = (int)eMenuOptions.UnDefined;

            Console.WriteLine("Welcome to Asgard's Pool!\n");
            while (userInput != (int)eMenuOptions.Exit)
            {
                Console.WriteLine("Please choose one of the following options:\n");
                Console.WriteLine("press (1) to add new student to the pool");
                Console.WriteLine("press (2) to add instructor to the staff");
                Console.WriteLine("press (3) to add availability to instructor");
                Console.WriteLine("press (4) to get next week's agenda");
                Console.WriteLine("press (5) to display the booked lesson of a student");
                Console.WriteLine("press (6) to display the booked lesson of a instructor");
                Console.WriteLine("press (7) to show conflicts and recommandations");
                Console.WriteLine("press (0) to exit\n");

                userInput = UserInput.GetNumber(0,7);
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
                case eMenuOptions.AddInstructorAvailability:
                    AddInstructorAvailability();
                    break;
                case eMenuOptions.GetWeekAgenda:
                    displayWeekAgenda2();
                    break;
                    //case eMenuOptions.ShowLessonsOfStudent:
                    //    refuelVehicle();
                    //    break;
                    //case eMenuOptions.GetLessonsOfInstructor:
                    //    rechargeVehicle();
                    //    break;
                    //case eMenuOptions.ShowConflicts:
                    //    showDataOfVehicle();
                    //    break;
            }
        }

        private void displayWeekAgenda2()
        {
            Lesson lesson;
            int verticaloffset = 0, screenHorizontalOffset = 0;
            r_RunPool.AssignWeekAgenda();
            for (int day = 0; day < k_AmountOfWorkingDays; day++)
            {
                verticaloffset = 0;
                if (r_RunPool.WeekAgenda[day] != null)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.SetCursorPosition(screenHorizontalOffset, verticaloffset);
                    Console.Write(@"{0}: ", ((eWeekDay)day).ToString());
                    Console.WriteLine(@"{0} lessons", r_RunPool.WeekAgenda[day].Count,((eWeekDay)day).ToString());
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
            Console.ReadLine();
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

        private void displayWeekAgenda()
        {
            int ScreenHorizontalOffset=0;
            Lesson lesson;
            r_RunPool.AssignWeekAgenda();
            for (int day = 0; day < k_AmountOfWorkingDays; day++)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                //Console.SetCursorPosition(ScreenHorizontalOffset, 0);
                Console.WriteLine(@"{0}'s Lessons:", ((eWeekDay)day).ToString());
                Console.ForegroundColor = ConsoleColor.White;

                if (r_RunPool.WeekAgenda[day] == null)
                {
                    //Console.SetCursorPosition(ScreenHorizontalOffset +8, 1);
                    Console.WriteLine("None");
                    ScreenHorizontalOffset += 30;
                }
                else
                {
                    Console.WriteLine(@"{0} Lessons have been schedualed on {1}", r_RunPool.WeekAgenda[day].Count,
                                     ((eWeekDay)day).ToString());
                    for (int index = 0; index < r_RunPool.WeekAgenda[day].Count; index++)
                    {
                        lesson = r_RunPool.WeekAgenda[day][index];
                        Console.WriteLine(@"Lesson #{0}: {1} lesson", index + 1, lesson.LessonMode.ToString());
                        Console.WriteLine(@"Instructor: {0}", lesson.LessonInstructor);
                        Console.WriteLine(@"Swimming style: {0}", lesson.SwimStyle.ToString());

                        Console.WriteLine(@"Lesson appointed time: {0} - {1}", lesson.HourToDisplay[0], lesson.HourToDisplay[1]);
                        Console.WriteLine(@"{0} students are registered to the lesson", lesson.RegisteredStudents.Count);
                        Console.WriteLine();
                    }
                    ScreenHorizontalOffset += 50;
                }
            }
            Console.WriteLine("h");

        }

        private void AddInstructorAvailability()
        {
            int chosenInstructorIndex;

            Console.WriteLine("Please Choose one of the following days:\n");
            for (int i = 0; i < PoolManagement.k_AmountOfDaysInWeek; i++)
            {
                Console.WriteLine(@"Press ({0}) for {1}", i + 1, r_RunPool.InstructorsList[i].InstructorName);
            }
            chosenInstructorIndex = UserInput.GetNumber(1, r_RunPool.InstructorsList.Count) -1;
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


    }
}
