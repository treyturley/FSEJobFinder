using System;
using FSEDataFeed;

namespace FSEJobfinderConsoleApp
{
    class Program
    {
        //TODO: migrate to .net 3.0
        //https://docs.microsoft.com/en-us/cpp/build/how-to-modify-the-target-framework-and-platform-toolset?view=msvc-160
        static void Main(string[] args)
        {
            Console.WriteLine("FSE Flight Planner" + Environment.NewLine);
            Console.WriteLine("Choose One Option:");
            Console.WriteLine("1: Best Available assignment for a Boeing 737-800");
            Console.WriteLine("2: Top 5 assignments for a Boeing 737-800");
            Console.WriteLine("3: All 737 Assignments to or from the US");
            Console.WriteLine("4: Best Available assignment for a Boeing 747-400");
            Console.WriteLine("5: All 747 Assignments to or from the US");
            Console.WriteLine("6: All 747 Assignments");
            Console.WriteLine(Environment.NewLine + "Type \"Exit\" or \"Q\" to quit.");
            Console.Write(Environment.NewLine + "Enter your choice: ");

            FSEDataAPI fSEData = new FSEDataAPI();

            string consoleInput = "";
            bool running = true;

            while (running)
            {
                consoleInput = Console.ReadLine();

                //check for back input or the quit command
                if (consoleInput.ToLower() == "q" || consoleInput.ToLower() == "exit")
                {
                    running = false;
                }
                else if (!int.TryParse(consoleInput, out int n))
                {
                    //invalid input, found a char where there should be a number
                    Console.WriteLine("Invalid input, try a different option.");
                }
                else
                {
                    //check to see which option was selected and perform required tasks to generate output
                    switch (int.Parse(consoleInput))
                    {
                        case 1:
                            //print the best 737 assignment
                            Console.WriteLine("Finding the location of the best 737-800 assignment...");
                            Console.WriteLine("Location: " + fSEData.getBestCommercialAssignment("Boeing 737-800").FromIcao);
                            break;
                        case 2:
                            //print top 5 737 assignments
                            Console.WriteLine("Finding the top 25 737-800 assignments...");
                            foreach (Assignment assignment in fSEData.getCommercialAssignments(AircraftMakeModelStrEnum.Boeing737_800, 25))
                            {
                                Console.WriteLine("Assignment: " + assignment.FromIcao + " to " + assignment.ToIcao + " - Pays: " + assignment.Pay);
                            }
                            
                            break;
                        case 3:
                            Console.WriteLine("Finding All 737 Assignments to or from the US...");
                            foreach(Assignment assignment in fSEData.GetUSAssignments(AircraftMakeModelStrEnum.Boeing737_800))
                            {
                                Console.WriteLine("Assignment: " + assignment.FromIcao + " to " + assignment.ToIcao + " - Pays: " + assignment.Pay);
                            }
                            break;
                        case 4:
                            //print the best 747 assignment                            
                            Console.WriteLine("Finding the location of the best 747-400 assignment...");
                            Console.WriteLine("Location: " + fSEData.getBestCommercialAssignment("Boeing 747-400").FromIcao);
                            break;
                        case 5:
                            Console.WriteLine("Finding All 747 Assignments to or from the US...");
                            foreach (Assignment assignment in fSEData.GetUSAssignments(AircraftMakeModelStrEnum.Boeing747_400))
                            {
                                Console.WriteLine("Assignment: " + assignment.FromIcao + " to " + assignment.ToIcao + " - Pays: " + assignment.Pay);
                            }
                            break;
                        case 6:
                            Console.WriteLine("Finding all 747-400 assignments...");
                            foreach (Assignment assignment in fSEData.getCommercialAssignments(AircraftMakeModelStrEnum.Boeing747_400))
                            {
                                Console.WriteLine("Assignment: " + assignment.FromIcao + " to " + assignment.ToIcao + " - Pays: " + assignment.Pay);
                            }
                            break;
                        default:
                            //invalid input
                            Console.WriteLine("Invalid option selected, try a different option.");
                            break;
                    }

                    //reprint the instructions
                    printWelcomeInstructions();
                    
                }
            }
        }

        public static void printWelcomeInstructions()
        {
            Console.WriteLine(Environment.NewLine + Environment.NewLine);
            Console.WriteLine("Choose One Option:");
            Console.WriteLine("1: Best Available assignment for a Boeing 737-800");
            Console.WriteLine("2: Top 5 assignments for a Boeing 737-800");
            Console.WriteLine("3: Best Available assignment for a Boeing 747-400");
            Console.WriteLine(Environment.NewLine + "Type \"Exit\" to quite.");
            Console.Write(Environment.NewLine + "Enter your choice: ");
        }
    }
}
