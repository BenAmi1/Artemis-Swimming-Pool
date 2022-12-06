# Artemis-Swimming-Pool
Welcome to the olympic pool of Artemis!
The following project is to assemble the most efficient weekly-schedule of a swimming pool.
The schedule considering the swimming instructors constrains:
1. Available working days
2. Available working hours
3. Speciallity: which swimming styles the instructor is quallified to instuct
The schedule also considering the students constraints:
1. Which swimming style they wish to learn
2. Do they want private (45 minutes) lesson, or a group (60 minutes) lesson. The students also gets to choose first and second priorities:
if their first priority is not aviailable, the algorithm will try to allow them the socenod priority

In case of conflicts or fully booked schedule, a pop-up will apear showing the unscheduleded-students.

Algorithms modus operandi:
1. The algorithm will set first the group lessons, as it morally justified.
If possible, the private lessong will be setted afterwards.
2. The algorithms will first try to book the instructor whose already working that day as it is more profitable for the employer.
3. In case of conflicts or lacks:
  a. Pop up including students names and a list of lacks will apear (summery of how many hours of each swimming style and group / private lessons - image below)
  b. The algorithms will suggest solutions: which instructors, in which days and for how much hours. In this way the employer can easily ask them to extend
  their working schedule. The algorithms will choose the instructor who works the lease amount of hours in that week as is socially justified.
  
  
The system allows to add students, instuctors and adding availablity to instuctors during run time: the conflicts will also be handled during run time
and a pop-up will be apear accordingly.


Manual:
The program will get an input: a list of X students and their constraints.
The output can be inserted manually, or by a file (files are published in this repository "Test_Names.txt"): the student preferations will be
automatically and randomlly setted.
If the automated method was chosen, make sure to save the attached text file above, in the file of the repository and to change the directory accordingly
in the next 2 methods:(Project: "SchedulingUI")
void TestAddStudents1()
void TestAddStudents2()

Output:
The assambled lessons schedule, for the instuctors and the students, divides by the week days
The system including some other features alse, e.g: displaying lessons of specific student or instructor, adding students etc, availability etc...

Technology:
1. .NET Framework, C#.
1. 100%, Fully supperation between the logic and the user interface - two different projects. The UI can be changes in a blink on an eye with another platform. No need to change
the logic project whatsoever!
2. Extendability: The amount of students and instructors can be changed, also the amount of swimming lessons or the working days of the pool.
3. Readability: emphasizing on a clean - readable, and maintainble code.


Schedule output

![image](https://user-images.githubusercontent.com/75504717/205940287-d5337c4c-9832-4e19-85ad-eeee16f7ab66.png)


Conflicts

![image](https://user-images.githubusercontent.com/75504717/205933838-52804f0f-fb1c-4626-b29b-a37223056404.png)


Reducing conflicts after adding availability to instructos

![image](https://user-images.githubusercontent.com/75504717/205935024-88694de6-cb0c-47cd-b6c5-43a7bb0e46a2.png)
