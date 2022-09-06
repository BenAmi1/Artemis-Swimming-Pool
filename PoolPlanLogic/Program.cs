using System;
using System.Collections.Generic;

namespace PoolPlanLogic
{
    class Program
    {
        static void Main(string[] args)
        {
            PoolManagement pm = new PoolManagement();

            // adding students
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
            int i = 0;

            for (i = 0; i < 30; i++)
            {
                string firstname = Console.ReadLine();
                string lastName = Console.ReadLine();
                pm.AddStudent(firstname, lastName, swimStyle[i % 3], lessonModes[i%4]);
            }
            Console.WriteLine("student added");

            /// finish add students


            //ADDING INSTRUCTORS


    

            //pm.AssignWeekAgenda();

            //pm.testingFunction();


            //for instructor
            //Pair p1 = new Pair(800, 900);
            //Pair p2 = new Pair(1500, 2200);
            //Pair p3 = new Pair(1700, 1800);

            //List<eSwimStyle> sw = new List<eSwimStyle>();
            //sw.Add(eSwimStyle.Chest);
            //sw.Add(eSwimStyle.Hatira);
            //sw.Add(eSwimStyle.Butterfly);
            //Instructor i = new Instructor("yotam", sw);
            //i.AddAvailableDayAndHours(eWeekDay.Monday, new Pair(800,900));
            //i.AddAvailableDayAndHours(eWeekDay.Monday, new Pair(1600, 2200));
            //i.AddAvailableDayAndHours(eWeekDay.Monday, new Pair(1400, 1500));
            //i.AddAvailableDayAndHours(eWeekDay.Monday, new Pair(1100, 1200));


            //Pair does1 = i.IsAvailableToBookALesson(eWeekDay.Monday, p1);
            //Pair does2 = i.IsAvailableToBookALesson(eWeekDay.Monday, p2);
            //Pair does3 = i.IsAvailableToBookALesson(eWeekDay.Monday, p3);

            //Console.WriteLine(does1.StartTime.ToString());
            //if(does2 !=null)
            //{
            //    Console.WriteLine(does2.StartTime.ToString());

            //}
            //Console.WriteLine(does3.StartTime.ToString());


            //List<eLessonMode> ss = new List<eLessonMode>();
            //ss.Add(eLessonMode.Private);
            //Student s = new Student("amir", "ben ami", eSwimStyle.Chest, ss);
            //List<Lesson> lsn = new List<Lesson>();
            //Pair p = new Pair(4, 5);
            //lsn.Add(new Lesson(eWeekDay.Sunday, p, eSwimStyle.Chest, eLessonMode.Group, eInstructorName.Johnny));
            //s.AddLessonToStudent(lsn[0]);            

        }





    }
}
