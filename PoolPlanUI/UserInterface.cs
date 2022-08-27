
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

        public UserInterface()
        {
            r_RunPool = new PoolManagement();
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
                    //case eMenuOptions.AddInstructor:
                    //    showAllPlateNumbers();
                    //    break;
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

        private void addNewStudent()
        {
            string studentFirstName, studentLastName;
            eSwimStyle swimStyle;
            List<eLessonMode> lessonModePriorities;

            Console.WriteLine("Please enter the student's first name:");
            studentFirstName = UserInput.GetStudentName();
            Console.Clear();
            Console.WriteLine("Please enter the student's last name:");
            studentLastName = UserInput.GetStudentName();
            Console.Clear();
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
