using System;
using System.Threading;

namespace tex_3
{
    class Program
    {
        public delegate int DisplayHandler();

        static void Main1(string[] args)
        {
            //try the five second method with a 6 second timeout
            CallWithTimeout(FiveSecondMethod, 6000);

            //try the five second method with a 4 second timeout
            //this will throw a timeout exception
            CallWithTimeout(FiveSecondMethod, 4000);
        }

   

        static void Main(string[] args)
        {
            DisplayHandler handler = new DisplayHandler(Display);
            int result = handler.Invoke();

            Console.WriteLine("Продолжается работа метода Main");
            Console.WriteLine("Результат равен {0}", result);

            Console.ReadLine();
        }

        static void CallWithTimeout(Action action, int timeoutMilliseconds)
        {
            Thread threadToKill = null;
            Action wrappedAction = () =>
            {
                threadToKill = Thread.CurrentThread;
                try
                {
                    action();
                }
                catch (ThreadAbortException ex)
                {
                    Thread.ResetAbort();// cancel hard aborting, lets to finish it nicely.
                }
            };

            IAsyncResult result = wrappedAction.BeginInvoke(null, null);
            if (result.AsyncWaitHandle.WaitOne(timeoutMilliseconds))
            {
                wrappedAction.EndInvoke(result);
            }
            else
            {
                threadToKill.Abort();
                throw new TimeoutException();
            }
        }


        static void FiveSecondMethod()
        {
            Thread.Sleep(5000);
        }

        static int Display()
        {
            Console.WriteLine("Начинается работа метода Display....");
            Random random = new Random();
            int a = random.Next(3, 43);
            int[] mas = new int[a];
            Console.WriteLine($"Числа, делящиеся на 3:");
            int b = 0;


            for (int i = 0; b < mas.Length; i++)
            {
                if (i % 3 == 0)
                {
                    mas[b] = i;
                    Console.WriteLine(mas[b]);
                    b++;
                }
            }

            Thread.Sleep(3000);
            Console.WriteLine("Завершается работа метода Display....");
            return a;
        }
    }
}