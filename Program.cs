using System.ComponentModel;

namespace HW_26_09
{
    internal class Program
    {
        static void Main(string[] args)
        {
           Console.Clear();
            Console.SetCursorPosition(0, 0);


            Console.SetCursorPosition(0, 11);
            AntColonySimylation sim = new AntColonySimylation(25,10,10);
            Console.SetCursorPosition(0, 0);
            sim.StartSimulation();
            Console.WriteLine("Sim complete");
        }
    }


    public class Ant
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Energy { get; set; }

        public int Age { get; set; }

        public Role AntRole { get; set; }

        private Random random = new Random();
        private AntColonySimylation simulation;

        public Ant(int x,int y,Role role,AntColonySimylation simylation)
        {
            X = x;
            Y = y;
            AntRole = role;
            Energy = 100;
            Age = 0;
            this.simulation = simylation;
        }

        public void Move()
        {
            X = (X + random.Next(-1, 2)) % simulation.Width;
            Y = (Y + random.Next(-1,2)) % simulation.Height;

            if (X < 0) X = simulation.Width - 1;
            if(Y < 0) Y = simulation.Height - 1;
        }

        public void PerformTask()
        {
            switch (AntRole)
            {
                case Role.Foreager:
                    SearchFood();
                    break;
                case Role.Builder:
                    Build();
                    break;
                case Role.Caretaker:
                    TakeCare();
                    break;

            }
        }

        private void SearchFood()
        {
            Move();
            Energy -= 5;
        }

        private void Build()
        {
            Move();
            Energy -= 5;
        }

        private void TakeCare()
        {
            Energy -= 2;
        }


        private void Rest()
        {
            Energy+= 5;
        }

        public void Work()
        {
            while (Energy > 0)
            {
                PerformTask();
                simulation.UpdateMap();
                simulation.UpdateInfo(this);
                Thread.Sleep(500);
            }
        }
    }


    public enum Role
    {
        Builder,
        Queen,
        Foreager,
        Caretaker
    }

    public class AntColonySimylation
    {
        public int Height { get; set; }
        public int Width { get; set; }
        private char[,] world;
        private List<Ant> ants;
        private List<Thread> antThreads;
        private int infoStartY;

        public AntColonySimylation(int width, int height, int numberOfAnts)
        {
            Width = width;
            Height = height;
            world = new char[Width, Height];
            ants = new List<Ant>();
            antThreads = new List<Thread>();

            infoStartY = height + 2;
            Random random = new Random();

            for (int i = 0; i < numberOfAnts; i++)
            {
                Role role = (Role)random.Next(0, 4);
                var ant = new Ant(random.Next(0, width), random.Next(0, height), role, this);
                ants.Add(ant);

                Thread thread = new Thread(ant.Work);
                antThreads.Add(thread);
            }
        }

        public void UpdateMap()
        {
            lock (world)
            {
                for (int i = 0; i < Width; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        world[i, j] = '.';
                    }
                }
                foreach (var ant in ants)
                {
                    world[ant.X, ant.Y] = 'A';
                }

                DrawMap();
            }
        }


        public void DrawMap()
        {
            Console.SetCursorPosition(0, 0);
            for (int j = 0; j < Height; j++)
            {
                for (int i = 0; i < Width; i++)
                {
                    Console.WriteLine(world[i, j]);
                }
                Console.WriteLine();
            }
        }


        public void UpdateInfo(Ant ant)
        {
            lock (world)
            {
                int infoYpos = infoStartY + ants.IndexOf(ant);
                Console.SetCursorPosition(0, infoYpos);
                Console.WriteLine($"Ant {ants.IndexOf(ant) + 1} Role: {ant.AntRole}, Energy: {ant.Energy}, Age: {ant.Age}");
            }
        }


        public void StartSimulation()
        {
            foreach(var thread in antThreads)
            {
                thread.Start();
            }

            foreach(var thread in antThreads)
            {
                thread.Join();
            }
        }
    }
}
