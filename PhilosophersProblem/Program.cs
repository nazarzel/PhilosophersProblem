using System;
using System.Threading;

namespace DinningPhilosophers
{
    class Program
    {
        static void Main(string[] args)
        {
            Philosopher aristotle = new Philosopher(Table.Plastikowy, Table.Platinowy, "Aristotle", 4);
            Philosopher confucius = new Philosopher(Table.Platinowy, Table.Zloty, "Confucius", 5);
            Philosopher kant = new Philosopher(Table.Zloty, Table.Srebny, "Immanuel Kant", 6);
            Philosopher augustine = new Philosopher(Table.Srebny, Table.Drzewniany, "Augustine", 4);
            Philosopher machiavelli = new Philosopher(Table.Drzewniany, Table.Plastikowy, "Niccolo Machiavelli", 7);

            new Thread(aristotle.Think).Start();
            new Thread(confucius.Think).Start();
            new Thread(kant.Think).Start();
            new Thread(augustine.Think).Start();
            new Thread(machiavelli.Think).Start();

            Console.ReadKey();
        }

    }



    enum PhilosopherState { Eating, Thinking }


    class Philosopher
    {
        public string Name { get; set; }

        public PhilosopherState State { get; set; }

        readonly int StarvationThreshold;

        public readonly Widelec PrawyWidelec;
        public readonly Widelec LewyWidelec;

        Random rand = new Random();

        int contCount = 0;

        public Philosopher(Widelec PrawyWidelec, Widelec LewyWidelec, string name, int starvThreshold)
        {
            this.PrawyWidelec = PrawyWidelec;
            this.LewyWidelec = LewyWidelec;
            Name = name;
            State = PhilosopherState.Thinking;
            StarvationThreshold = starvThreshold;
        }

        public void Eat()
        {
            if (TakeWidelecInPrawyHand())
            {
                if (TakeWidelecInLewyHand())
                {
                    this.State = PhilosopherState.Eating;
                    Console.WriteLine("{0} je z {1} i {2}", Name, PrawyWidelec.WidelecID, LewyWidelec.WidelecID);
                    Thread.Sleep(rand.Next(5000, 10000));

                    contCount = 0;
                    PrawyWidelec.Put();
                    LewyWidelec.Put();
                }
                else
                {
                    Thread.Sleep(rand.Next(100, 400));
                    if (TakeWidelecInLewyHand())
                    {
                        this.State = PhilosopherState.Eating;
                        Console.WriteLine("{0} je z {1} i {2}", Name, PrawyWidelec.WidelecID, LewyWidelec.WidelecID);
                        Thread.Sleep(rand.Next(5000, 10000));

                        contCount = 0;

                        PrawyWidelec.Put();
                        LewyWidelec.Put();
                    }
                    else
                    {
                        PrawyWidelec.Put();
                    }
                }
            }
            else
            {
                if (TakeWidelecInLewyHand())
                {
                    Thread.Sleep(rand.Next(100, 400));
                    if (TakeWidelecInPrawyHand())
                    {
                        this.State = PhilosopherState.Eating;
                        Console.WriteLine("{0} je z {1} i {2}", Name, PrawyWidelec.WidelecID, LewyWidelec.WidelecID);
                        Thread.Sleep(rand.Next(5000, 10000));

                        contCount = 0;

                        PrawyWidelec.Put();
                        LewyWidelec.Put();
                    }
                    else
                    {
                        LewyWidelec.Put();
                    }
                }
            }

            Think();
        }

        public void Think()
        {
            this.State = PhilosopherState.Thinking;
            Console.WriteLine("{0} mysli o {1}", Name, Thread.CurrentThread.Priority.ToString());
            Thread.Sleep(rand.Next(2500, 20000));
            contCount++;

            if (contCount > StarvationThreshold)
            {
                Console.WriteLine("{0} gloduje", Name);
            }

            Eat();
        }

        private bool TakeWidelecInLewyHand()
        {
            return LewyWidelec.Take(Name);
        }

        private bool TakeWidelecInPrawyHand()
        {
            return PrawyWidelec.Take(Name);
        }

    }
    enum WidelecState { Taken, OnTheTable }
    class Widelec
    {
        public string WidelecID { get; set; }
        public WidelecState State { get; set; }
        public string TakenBy { get; set; }

        public bool Take(string takenBy)
        {
            lock (this)
            {
                if (this.State == WidelecState.OnTheTable)
                {
                    State = WidelecState.Taken;
                    TakenBy = takenBy;
                    Console.WriteLine("{0} wzieto przez {1}", WidelecID, TakenBy);
                    return true;
                }

                else
                {
                    State = WidelecState.Taken;
                    return false;
                }
            }
        }

        public void Put()
        {
            State = WidelecState.OnTheTable;
            Console.WriteLine("{0} miejsce na stolu {1}", WidelecID, TakenBy);
            TakenBy = String.Empty;
        }
    }



    class Table
    {
        internal static Widelec Platinowy = new Widelec() { WidelecID = "Platinowy Widelec", State = WidelecState.OnTheTable };
        internal static Widelec Zloty = new Widelec() { WidelecID = "Zloty Widelec", State = WidelecState.OnTheTable };
        internal static Widelec Srebny = new Widelec() { WidelecID = "Srebny Widelec", State = WidelecState.OnTheTable };
        internal static Widelec Drzewniany = new Widelec() { WidelecID = "Drzewniany Widelec", State = WidelecState.OnTheTable };
        internal static Widelec Plastikowy = new Widelec() { WidelecID = "Plastikowy Widelec", State = WidelecState.OnTheTable };
    }
}