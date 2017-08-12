
/* Tuan Huynh 8/11/2017
 * Exercise taken from https://www.cs.uaf.edu/~cs202/deitel4e/ab2eocexample.html
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Simpletron
{
    class Simpletron
    {
        //variables
        const int OP_READ = 10;
        const int OP_WRITE = 11;
        const int OP_LOAD = 20;
        const int OP_STORE = 21;
        const int OP_ADD = 30;
        const int OP_SUBT = 31;
        const int OP_DIV = 32;
        const int OP_MULT = 33;
        const int OP_BRANCH = 40;
        const int OP_BRANCHNEG = 41;
        const int OP_BRANCHZERO = 42;
        const int OP_HALT = 43;

        private int[] memory;
        private int accumulator = 0;

        public Simpletron() { }

        public int inputCheck(String x)
        {
            //Validates that user inputs are int values
            String input = x;
            int result = 0;
            int temp = 0;
            bool notValid = true;
            while (notValid)
            {
                if (int.TryParse(input, out temp))
                {
                    if ((-10000 < temp && temp < 10000) || temp == -99999)
                    {
                        result = temp;
                        notValid = false;
                    }
                    else
                    {
                        Console.WriteLine("Error: Only numbers between 9999 and -9999 are accepted. Please try again.");
                        input = Console.ReadLine();
                    }
                }

                else
                {
                    Console.WriteLine("Error: Only numbers between 9999 and -9999 are accepted. Please try again.");
                    input = Console.ReadLine();
                }
            }
            return result;

        }

        public void run()
        {   
            //Main method for simpletron

            Console.WriteLine("*** Welcome to Simpletron! ***\r\n\r\n" +
                "*** Please enter your program one instruction ***\r\n" +
                "*** (or data word) at a time. I will type the ***\r\n" +
                "*** location number and a question mark (?).  ***\r\n" +
                "*** You then type the word for that location. ***\r\n" +
                "*** Type the sentinel -99999 to stop entering ***\r\n" +
                "*** your program. ***");
            loadInput();
            Console.WriteLine("*** Program loading completed ***\r\n" +
                "*** Program execution begins  ***");
            calculate();
        }

        public void loadInput()
        {
            //Method to load value into memory, only int values are accepted
            int[] temp = new int[100];
            bool keepGoing = true;
            for(int i = 0; i<100 && keepGoing; i++){
                Console.Write(i.ToString("D2") + " ? ");
                int x = inputCheck(Console.ReadLine());
                temp[i] = x;
                if (x == -99999)
                {
                    keepGoing = false;
                }
            }
            memory = temp;

        }
        public int[] calculate()
        {
            //Method that computes the instructions in the memory array. If successfully ran it will call
            //the dump method which prints the memory's contents
            int counter = 0;
            while (counter < memory.Length)
            {
                int instructionRegister = memory[counter];
                int operationCode =instructionRegister / 100;
                int operand = instructionRegister % 100;
                bool increment = true;

                switch (operationCode)
                {
                    case OP_READ:
                        int x = 0;
                        bool notValid = true;
                        while (notValid)
                        {
                            System.Console.WriteLine("Please enter a number between -9999 and 9999");
                            x = inputCheck(Console.ReadLine());
                            if (x == -99999)
                            {
                                Console.WriteLine("Error input is invald.");

                            }
                            else
                            {
                                
                                notValid = false;
                            }
                        }
                        memory[operand] = x;
                        break;
                    case OP_WRITE:
                        System.Console.WriteLine(memory[operand] + "\r\n");
                        break;
                    case OP_LOAD:
                        accumulator = memory[operand];
                        break;
                    case OP_STORE:
                        memory[operand] = accumulator;
                        break;
                    case OP_ADD:
                        accumulator += memory[operand];
                        if (accumulator < -9999 || accumulator > 9999)
                        {
                            abnormalShutdown("Addition caused accumulator overflow");
                        }
                        break;
                    case OP_SUBT:
                        accumulator -= memory[operand];
                        if (accumulator < -9999 || accumulator > 9999)
                        {
                            abnormalShutdown("Subtraction caused accumulator overflow");
                        }
                        break;
                    case OP_DIV:
                        if (memory[operand] == 0)
                        {
                            abnormalShutdown("Divide by Zero Error");
                        }
                        else
                        {
                            accumulator /= memory[operand];
                        }
                        break;
                    case OP_MULT:
                        accumulator *= memory[operand];
                        if (accumulator < -9999 || accumulator > 9999)
                        {
                            abnormalShutdown("Multiplication caused accumulator overflow");
                        }
                        break;
                    case OP_BRANCH:
                        counter = operand;
                        increment = false;
                        break;
                    case OP_BRANCHNEG:
                        if (accumulator < 0)
                        {
                            counter = operand;
                            increment = false;
                        }
                        break;
                    case OP_BRANCHZERO:
                        if (accumulator == 0)
                        {
                            counter = operand;
                            increment = false;
                        }
                        break;
                    case OP_HALT:
                        System.Console.WriteLine("Operation done. Dumping Data\r\n" );
                        dump(counter, instructionRegister, operationCode, operand);
                        
                        counter = memory.Length;
                        increment = false;
                        break;
                    default:
                        System.Console.WriteLine("Unexpected value in memory:" + " "+ counter +" "+ memory[counter]);
                        break;
                }

                if (increment)
                {
                    counter++;
                }
            }

            return memory;
        }

        public void test(int[] x)
        {
            //Dev testing method so you don't need to type values into the memory on the console, accepts int[]
            if (x.Length > 101)
            {
                Console.WriteLine("Array over 101, program closing");
                int[] temp = { };
                memory = temp;
            }
            else
            {
                int[] temp = new int[100];
                for (int i = 0; i < x.Length; i++)
                {
                    temp[i] = inputCheck(x[i].ToString());
                }
                memory = temp;
            }
        }
        public void abnormalShutdown(String s)
        {
            //Special shutdown method for overflows
            Console.WriteLine("Abonormal Error due to: " + s);
            Console.Read();
            Environment.Exit(0);
        }

        public void dump(int counter, int instructionRegister, int operationCode, int operand)
        {
            //Prints all memory contents and variables when simpletron has finished running

            Console.WriteLine("REGISTERS:\r\n" +
                "accumulator          " + accumulator.ToString("+0000;-0000") + "\r\n" +
                "counter                 " + counter.ToString("D2") + "\r\n" +
                "instructionRegister  " + instructionRegister.ToString("+0000;-0000") + "\r\n" +
                "operationCode           " + operationCode.ToString("D2") + "\r\n" +
                "operand                 " + operand.ToString("D2") + "\r\n");

            Console.WriteLine("MEMORY:");
            Console.WriteLine("        0     1     2     3     4     5     6     7     8     9");
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("  " + i*10 + " " + memory[0 + (i * 10)].ToString("+0000;-0000") + " " +
                memory[1 + (i * 10)].ToString("+0000;-0000") + " " +
                memory[2 + (i * 10)].ToString("+0000;-0000") + " " +
                memory[3 + (i * 10)].ToString("+0000;-0000") + " " +
                memory[4 + (i * 10)].ToString("+0000;-0000") + " " +
                memory[5 + (i * 10)].ToString("+0000;-0000") + " " +
                memory[6 + (i * 10)].ToString("+0000;-0000") + " " +
                memory[7 + (i * 10)].ToString("+0000;-0000") + " " +
                memory[8 + (i * 10)].ToString("+0000;-0000") + " " +
                memory[9 + (i * 10)].ToString("+0000;-0000") + " ");
            }
        }
        static void Main(string[] args)
        {
            int[] test2 = { 1009, 1010, 2009, 3110, 4107, 1109, 4300, 1110, 4300, 0000, 0000 };//test 
            int[] test1 = { +1007, +1008, +2007, +3008, +2109, +1109, +4300, +0000, +0000, +0000 };//test
            int[] partA = { 1008, 2008, 4106, 3009, 2109, 4000, 1109, 4300, 0000, 0000 }; //8.18 part A
            int[] dividetest = { 3201, 0000 };
            
            Simpletron reader = new Simpletron();
            //reader.run();
            reader.test(test1);
            reader.calculate();
            
            Console.ReadLine();
            
        }
    }
}
