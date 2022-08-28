
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
        private readonly int k_AmountOfSwimStyles;
        private readonly int k_AmountOfWorkingDays = 7;

        private readonly string k_Blank = " ";

        public UserInterface()
        {
            r_RunPool = new PoolManagement();
            k_AmountOfSwimStyles = Enum.GetNames(typeof(eSwimStyle)).Length;
            MainMenu();
        }

        public void MainMenu()
        {
            eMenuOptions userInput = eMenuOptions.UnDefined;

            Console.WriteLine("Welcome to Asgard's Pool!\n");
            while (userInput != eMenuOptions.Exit)
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
                try
                {
                    userInput = UserInput.GetOptionMainMenu();
                    executeAction(userInput);
                    Console.Clear();
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine("Bed argumant. Please try again!");
                    System.Threading.Thread.Sleep(1500); // pause before clear screen
                    Console.Clear();
                }

                Console.Clear();
            }
        }

        private void executeAction(eMenuOptions i_UserInput)
        {
            switch (i_UserInput)
            {
                case eMenuOptions.Exit:
                    break;
                case eMenuOptions.AddStudent:
                    addNewStudent();
                    break;
                case eMenuOptions.AddInstructor:
                    addInstructor();
                    break;
                    //case eMenuOptions.AddInstructorAvailability:
                    //    changeStatus();
                    //    break;
                    //case eMenuOptions.GetWeekAgenda:
                    //    inflateAirInWheelsToMax();
                    //    break;
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
                while (userInput == "yes" && swimStyles.Count < k_AmountOfSwimStyles)
                {
                    swimStyles.Add(UserInput.GetSwimStyle());
                    Console.Clear();
                    if(swimStyles.Count< k_AmountOfSwimStyles)
                        Console.WriteLine("Would you like to add another swimming style?");
                    userInput = k_Blank;
                }
                if (userInput == "no" || swimStyles.Count == k_AmountOfSwimStyles)
                    break;

            } while (userInput != "yes" && userInput != "no");

            r_RunPool.AddInstructorToStaff(firstName, swimStyles);
            Console.WriteLine(@"{0} added successfully to the stuff of the pool!", firstName);
            System.Threading.Thread.Sleep(500); // pause before clear screen
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
                    Console.Clear();
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
            System.Threading.Thread.Sleep(1500); // pause before clear screen

        }


    }
}
