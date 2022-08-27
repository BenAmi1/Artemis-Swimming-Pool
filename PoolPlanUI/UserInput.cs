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
        private readonly int r_MenuOptionsSize = 7;

        public static eMenuOptions GetOptionMainMenu()
        {
            eMenuOptions validInput = eMenuOptions.UnDefined;
            string input = Console.ReadLine();

            if (!eMenuOptions.TryParse(input, out validInput) || !((int)validInput >= 0 && (int)validInput <= 7))
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
            eSwimStyle input;
            int userInput;
            Console.WriteLine("Please Choose one of the following swimming styles:\n");
            foreach(eSwimStyle style in Enum.GetValues(typeof(eSwimStyle)))
            {
                Console.WriteLine(@"Press ({0}) for {1}", (int)style + 1, style.ToString());
            }

            return (eSwimStyle)GetNumber(1, Enum.GetNames(typeof(eLessonMode)).Length) - 1;

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

        public static string GetStudentName()
        {
            string userInput;
            userInput = Console.ReadLine();
            return userInput;
        }
    }
}
