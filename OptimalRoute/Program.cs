using System;
using System.Collections.Generic;

namespace OptimalRoute
{
    class Program
    {
        static void Main(string[] args)
        {
            //все это лучше хранить в бд, но в целях упрощения оставим в памяти
            var MyQueue = new MyPriorityQueue();
            MyQueue.Enqueue(new TravelNode("Исаакиевский собор", 5, 10));
            MyQueue.Enqueue(new TravelNode("Эрмитаж", 8, 11));
            MyQueue.Enqueue(new TravelNode("Кунсткамера", 3.5, 4));
            MyQueue.Enqueue(new TravelNode("Петропавловская крепость", 10, 7));
            MyQueue.Enqueue(new TravelNode("Ленинградский зоопарк", 9, 15));
            MyQueue.Enqueue(new TravelNode("Медный всадник", 1, 17));
            MyQueue.Enqueue(new TravelNode("Казанский собор", 4, 3));
            MyQueue.Enqueue(new TravelNode("Спас на Крови", 2, 9));
            MyQueue.Enqueue(new TravelNode("Зимний дворец Петра I", 7, 12));
            MyQueue.Enqueue(new TravelNode("Зоологический музей", 5.5, 6));
            MyQueue.Enqueue(new TravelNode("Музей обороны и блокады Ленинграда", 2, 19));
            MyQueue.Enqueue(new TravelNode("Русский музей", 5, 8));
            MyQueue.Enqueue(new TravelNode("Навестить друзей", 12, 20));
            MyQueue.Enqueue(new TravelNode("Музей восковых фигур", 2, 13));
            MyQueue.Enqueue(new TravelNode("Литературно-мемориальный музей Ф.М. Достоевского", 4, 2));
            MyQueue.Enqueue(new TravelNode("Екатерининский дворец", 1.5, 5));
            MyQueue.Enqueue(new TravelNode("Петербургский музей кукол", 1, 14));
            MyQueue.Enqueue(new TravelNode("Музей микроминиатюры «Русский Левша»", 3, 18));
            MyQueue.Enqueue(new TravelNode("Всероссийский музей А.С. Пушкина и филиалы", 6, 1));
            MyQueue.Enqueue(new TravelNode("Музей современного искусства Эрарта", 7, 16));

            ShowOptimalRoute(MyQueue, 48);
        }

        public static void ShowOptimalRoute(MyPriorityQueue queue, double tripHours)
        {
            //время на сон
            const double dailySleepAmount = 8;
            const double hoursInDay = 24;
            //потраченное время в поездке
            double timeSpent = 0;
            double timeAfterSleep;


            int highPriorityIndex;
            TravelNode node;

            //количество полных дней (предположим, что спать мы будем 1 раз за полный день)
            int daysInTrip = (int)(tripHours / hoursInDay);

            //добавляем активность сна с нулевым приоритетом согласно количеству дней
            for (int i = 0; i < daysInTrip; i++)
            {
                queue.Enqueue(new TravelNode("Сон", dailySleepAmount, 0));
            }

            Console.WriteLine($"{"Activity",-55}  {"Priority",-10:0}  {"Time",-5} {"Total Time",-5}");
            do
            {
                //получаем индекс самого приоритетного занятия
                highPriorityIndex = queue.IndexOfHighestPriorityPerTime();
                //успеваем ли поспать
                timeAfterSleep = timeSpent + dailySleepAmount;

                //ставим сон в приоритет если пора отдохнуть 
                if ((hoursInDay - timeAfterSleep % hoursInDay <= dailySleepAmount) && daysInTrip > 0)
                {
                    queue.ChangePriority("Сон", dailySleepAmount * queue[highPriorityIndex].priority);
                    daysInTrip--;
                }

                //выполняем (достаём из очереди с записью времени) активность, если вписываемся по времени 
                if ((timeSpent + queue[highPriorityIndex].time) <= tripHours)
                {
                    node = queue.Dequeue();                    
                    timeSpent += node.time;
                    Console.WriteLine($"{node.nodeName}");
                }
                //пропускаем (достаём из очереди без выполнения) активность
                else
                {
                    node = queue.Dequeue();
                    Console.WriteLine($"---{node.nodeName}");
                }
            } while (timeSpent <= tripHours && queue.Count() != 0);

            Console.WriteLine("Total time spent: " + timeSpent);
        }
        /// <summary>
        /// Класс-контейнер для очереди с приоритетом
        /// </summary>
        public class TravelNode
        {
            public string nodeName;
            public double time;
            public double priority;
            public TravelNode(string nodeName, double time, double priority)
            {
                this.nodeName = nodeName;
                this.time = time;
                this.priority = priority;
            }
        }
        /// <summary>
        /// Очередь с приоритетом
        /// </summary>
        public class MyPriorityQueue
        {
            private List<TravelNode> list;
            public MyPriorityQueue()
            {
                this.list = new List<TravelNode>();
            }
            public TravelNode Dequeue()
            {
                int i = IndexOfHighestPriorityPerTime();
                TravelNode result = list[i];
                list.RemoveAt(i);
                return result;
            }
            //поиск самой приоритетной активности (относительно затраченного времени)
            public int IndexOfHighestPriorityPerTime()
            {
                double highPriority = list[0].time / list[0].priority;
                int highIndex = 0;
                for (int i = 0; i < list.Count; ++i)
                {
                    if (list[i].time / list[i].priority < highPriority)
                    {
                        highPriority = list[i].time / list[i].priority;
                        highIndex = i;
                    }
                }
                return highIndex;
            }

            public TravelNode this[int index]
            {
                get => list[index];
                set => list[index] = value;
            }
            public void Enqueue(TravelNode tnd)
            {
                list.Add(tnd);
            }

            //возможность изменить приоритет
            public void ChangePriority(string nodeName, double priority)
            {
                int i = IndexOf(nodeName);
                list[i].priority = priority;
            }

            private int IndexOf(string nodeName)
            {
                for (int i = 0; i < list.Count; ++i)
                    if (list[i].nodeName == nodeName)
                        return i;
                return -1;
            }

            public int Count()
            {
                return list.Count;
            }
        }
    }
}
