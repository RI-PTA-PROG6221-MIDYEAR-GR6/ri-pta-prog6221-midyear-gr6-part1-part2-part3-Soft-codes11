using System;
					
public class Program
{
	public static void Main()
	{   //oprators:arithmetic(+,-,/,",%),comparison(==,====,<,>,>=,<=,!==),assignment(=,+=,-=,/=,*=),decrement/increment(||,&&,or,and)
        //Writ a program that asks a user to enter their assgnment 1(25%),assignment 2(30%),assignment 3(35%) and calclte the fnal mark with four ice task(1-4)(10%)
     //EXAMLES : SNGLE INE
    /*MULTI-LINE


     */

     //VARIABLES
        Console.WriteLine("Pease enter marks for assignemt 1");
		int num1=int.Parse(Console.ReadLine());
		double total1 = num1*0.25;
		Console.WriteLine("Please enter marks for assignment 2");
		int num2=int.Parse(Console.ReadLine());
		double total2 = num2*0.30;
		Console.WriteLine("Please enter marks for assignment 3");
		int num3=int.Parse(Console.ReadLine());
        double total3 = num3*0.35;
		
		
		
		
		Console.WriteLine("Pleae enter Ice task 1:");
			int task1=int.Parse(Console.ReadLine());
		double iceTot1 = task1*0.10;
		Console.WriteLine("Please enter Ice Task 2:");
		int task2=int.Parse(Console.ReadLine());
		double iceToat2 = task2*0.10;
		Console.WriteLine("Pleae enter Ice task 3");
		int task3=int.Parse(Console.ReadLine());
		double iceToat3= task3*0.10;
		Console.WriteLine("Pleae enter Ice task 4");
		int task4=int.Parse(Console.ReadLine());
		double iceToat4=task4*0.10;
		
		
		double ftotal=total1+total2+total3+iceTot1+iceToat2+iceToat3+iceToat4;
		Console.WriteLine("The Final Mark:" + ftotal);
		
	
		
		 


     int number1=1;
     string name=" name";
     char c='A';
     double price =13.99;
     bool isTrue=true;
     
		
		Console.Write("NUMBR 1"+number1);
        string[] names ={"",""};//Array
		
	}
}
