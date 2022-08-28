using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoolPlanLogic;

namespace PoolPlanUI
{
    public class UserInput
    {
        private const int r_MenuOptionsSize = 7;

        public static eMenuOptions GetOptionMainMenu()
        {
            eMenuOptions validInput = eMenuOptions.UnDefined;
            string input = Console.ReadLine();

            if (!eMenuOptions.TryParse(input, out validInput) || !((int)validInput >= 0 && (int)validInput <= r_MenuOptionsSize))
            {
                throw new ArgumentException();
            }
            
            Console.Clear();
            return validInput;
        }

        public static int GetNumber(int i, int j)
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

        public static eSwimStyle GetSwimStyle()
        {
            eSwimStyle requestedSwimStyle;

            Console.WriteLine("Please Choose one of the following swimming styles:\n");
            foreach(eSwimStyle style in Enum.GetValues(typeof(eSwimStyle)))
            {
                Console.WriteLine(@"Press ({0}) for {1}", (int)style + 1, style.ToString());
            }


            requestedSwimStyle =(eSwimStyle)GetNumber(1, Enum.GetNames(typeof(eSwimStyle)).Length) - 1;
            Console.Clear();
            return requestedSwimStyle;
        }

        public static List<eLessonMode> GetLessonModePriorities()
        {
            List<eLessonMode> priorities = new List<eLessonMode>();
            int input;

            Console.WriteLine("Please choose which lesson you would like to have:\n");

            Console.WriteLine("Please press (1) for group lesson only");
            Console.WriteLine("Please press (2) for private lesson only");
            Console.WriteLine("Please press (3) for group lesson as first priority, and private as the second priority");
            Console.WriteLine("Please press (4) for private lesson as first priority, and group as the second priority");
            input = GetNumber(1, 4);

            switch(input)
            {
                case 1:
                    priorities.Add(eLessonMode.Group);
                    priorities.Add(eLessonMode.None);
                    return priorities;
                case 2:
                    priorities.Add(eLessonMode.Private);
                    priorities.Add(eLessonMode.None);
                    return priorities;
                case 3:
                    priorities.Add(eLessonMode.Group);
                    priorities.Add(eLessonMode.Private);
                    return priorities;
                case 4:
                    priorities.Add(eLessonMode.Private);
                    priorities.Add(eLessonMode.Group);
                    return priorities;
                default:
                    return priorities;
            }
        }

        public static void AddDaysAndHours(Instructor i_Instructor)
        {
            int chosenDay;
            string from, to;
            Pair newPair = null;
            Console.WriteLine("Please Choose one of the following days:\n");
            for (int day = 0; day < PoolManagement.k_AmountOfDaysInWeek; day++)
            {
                Console.WriteLine(@"Press ({0}) for {1}", day + 1, ((eWeekDay)day).ToString());
            }
            chosenDay = GetNumber(1, PoolManagement.k_AmountOfDaysInWeek);

            while (newPair == null)
            {
                Console.Clear();
                Console.WriteLine("Please Choose an hour in this format: XX:XX\n");
                Console.Write("FROM: ");
                from = Console.ReadLine();
                Console.Write("TO: ");
                to = Console.ReadLine();
                newPair = validateHoursFormat(from, to);
                if(newPair == null)
                {
                    Console.WriteLine("Wrong input. Please press any key to continue.");
                    Console.ReadLine();
                }
            }
            i_Instructor.AddAvailability((eWeekDay)(chosenDay-1), newPair);
            Console.Clear();
        }

        private static Pair validateHoursFormat(string from, string to)
        {
            string elementToCheck = from;
            Pair hoursRange;
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
            if (!validHour(from) || !validHour(to))
                return null;

            from =from.Remove(2, 1); // insert the hours into int, as it managed in pool schedule
            to=to.Remove(2, 1);

            if(!int.TryParse(from, out startingTime) || !int.TryParse(to, out endingTime) || startingTime >=endingTime)
                return null;
       
            hoursRange = new Pair(startingTime, endingTime);
            return hoursRange;
        }

        static bool validHour(string hour)
        {
            return !(hour[0] > '2' || hour[3] > '5');
        }

        public static string GetName()
        {
            string userInput;
            userInput = Console.ReadLine();
            Console.Clear();
            return userInput;
        }
    }
}
