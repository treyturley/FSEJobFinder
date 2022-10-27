using System;
using System.Collections.Generic;
using FSEDataFeed;

namespace FSEJobfinderConsoleApp
{
    class Program
    {
        //TODO: migrate to .net 3.0
        //https://docs.microsoft.com/en-us/cpp/build/how-to-modify-the-target-framework-and-platform-toolset?view=msvc-160

        //TODO: manage userkey as a user secret
        //https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows
        //simple secrets in console app:https://makolyte.com/how-to-add-user-secrets-in-a-dotnetcore-console-app/
        //reading a single secret: https://makolyte.com/csharp-how-to-read-custom-configuration-from-appsettings-json/#Reading_a_single_value_from_appsettingsjson

        static void Main(string[] args)
        {
            printWelcomeInstructions();

            //TODO: ask user to enter userkey

            FSEDataAPI fSEData = new FSEDataAPI("userkey");

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
                            Console.WriteLine("Finding the location of the best 737-800 assignment..." + Environment.NewLine);
                            printAssignment(fSEData.getBestCommercialAssignment("Boeing 737-800"));
                            Console.WriteLine();
                            Console.WriteLine();
                            break;
                        case 2:
                            //print top 5 737 assignments
                            Console.WriteLine("Finding the top 25 737-800 assignments..." + Environment.NewLine);
                            printAssignments(fSEData.getCommercialAssignments(AircraftMakeModelStrEnum.Boeing737_800, 25));
                            
                            break;
                        case 3:
                            Console.WriteLine("Finding All 737 Assignments to or from the US..." + Environment.NewLine);
                            printAssignments(fSEData.GetUSAssignments(AircraftMakeModelStrEnum.Boeing737_800));
                            break;
                        case 4:
                            //print the best 747 assignment                            
                            Console.WriteLine("Finding the location of the best 747-400 assignment..." + Environment.NewLine);
                            printAssignment(fSEData.getBestCommercialAssignment("Boeing 747-400"));
                            Console.WriteLine();
                            Console.WriteLine();
                            break;
                        case 5:
                            Console.WriteLine("Finding All 747 Assignments to or from the US..." + Environment.NewLine);
                            printAssignments(fSEData.GetUSAssignments(AircraftMakeModelStrEnum.Boeing747_400));
                            break;
                        case 6:
                            Console.WriteLine("Finding all 747-400 assignments..." + Environment.NewLine);
                            printAssignments(fSEData.getCommercialAssignments(AircraftMakeModelStrEnum.Boeing747_400));
                            break;
                        case 7:
                            //TODO: Test A320 jobs
                            Console.WriteLine("Finding all Airbus A320 (MSFS) assignments..." + Environment.NewLine);
                            printAssignments(fSEData.getCommercialAssignments(AircraftMakeModelStrEnum.AirbusA320_MSFS));
                            break;
                        case 8:
                            //TODO: Test A320 jobs
                            Console.WriteLine("Finding all Airbus A320 (MSFS) assignments that start in the US..." + Environment.NewLine);
                            printAssignments(fSEData.GetUSAssignments(AircraftMakeModelStrEnum.AirbusA320_MSFS));
                            break;
                        default:
                            //invalid input
                            Console.WriteLine("Invalid option selected, try a different option." + Environment.NewLine);
                            break;
                    }

                    //reprint the instructions
                    printWelcomeInstructions();
                    
                }
            }
        }

        //TODO: extract all of this out to its own class for handling the menus
        public static void printWelcomeInstructions()
        {
            Console.WriteLine("FSE Flight Planner" + Environment.NewLine);
            Console.WriteLine("Choose One Option:");
            Console.WriteLine("1: Best Available assignment for a Boeing 737-800");
            Console.WriteLine("2: Top 5 assignments for a Boeing 737-800");
            Console.WriteLine("3: All 737 Assignments to or from the US");
            Console.WriteLine("4: Best Available assignment for a Boeing 747-400");
            Console.WriteLine("5: All 747 Assignments to or from the US");
            Console.WriteLine("6: All 747 Assignments");
            Console.WriteLine("7: All A320 (MSFS) Assignments");
            Console.WriteLine("8: All A320 (MSFS) Assignments to or from the US");
            Console.WriteLine(Environment.NewLine + "Type \"Exit\" or \"Q\" to quit.");
            Console.Write(Environment.NewLine + "Enter your choice: ");
        }

        public static void printAssignments(List<Assignment> assignments)
        {
            if (assignments.Count != 0)
            {
                foreach (Assignment assignment in assignments)
                {
                    printAssignment(assignment);
                }
            }
            else
            {
                Console.WriteLine("No available assignments where found.");
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        public static void printAssignment(Assignment assignment)
        {
            if (assignment != null)
            {
                Console.WriteLine(assignment.ToString());
            }
            else
            {
                Console.WriteLine("There was an error retreiving the assignment. Please try again.");
            }
        }
    }
}
