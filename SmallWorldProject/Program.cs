using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;


namespace SmallWorldProject
{
    /// Small World Phemonenon Project
    class Program
    {

        public class visited_node
        {
            public int relation_strength;  ////O(1)
            public int distance;  ////O(1)
            public int node_index;  ////O(1)
            public int parent_index;  ////O(1)

            public visited_node()
            {

            }
            public visited_node(int distance, int node_index, int parent_index)
            {
                this.distance = distance;  ////O(1)
                this.node_index = node_index;  ////O(1)
                this.parent_index = parent_index;  ////O(1)
            }
            public visited_node(int relation_strength, int distance, int node_index, int parent_index)
            {
                this.relation_strength = relation_strength; ////O(1)
                this.distance = distance;  ////O(1)
                this.node_index = node_index;  ////O(1)
                this.parent_index = parent_index;  ////O(1)
            }

        }
        public struct Movies
        {
            public string movie_name; ////O(1)
            public List<string> actors_name;  ////O(1)
        }


        //// initialization of objects and variables is O(1)
        public static string moviesspath, pairrs_path, pairrsol_path;
        public static string[] read_movies;
        public static string[] read_pairs;
        public static int index = 0;
        public static Movies[] movies;
        public static HashSet<string> actorss = new HashSet<string>();
        public static List<Dictionary<int, int>> neighbors = new List<Dictionary<int, int>>();
        public static List<Dictionary<string, string>> common_movies = new List<Dictionary<string, string>>();
        public static Dictionary<int, string> actors_name = new Dictionary<int, string>();
        public static Dictionary<string, int> actors_index = new Dictionary<string, int>();

        /*
         
         small -> Movies187 , queries50 
         small -> Movies193 , queries110s 
         medium -> Movies967 , queries85, queries4000
         medium -> Movies4736 , queries110, queries2000
         large -> Movies14129 , queries26, queries600
         large -> Movies122806 , queries22, queries200

         */

        static void Main(string[] args)
        {
            Stopwatch sw = Stopwatch.StartNew();
            Console.WriteLine("To Print the Output press 1 to Compare The files press 2 :"); ////O(1)
            char test_choice = (char)Console.ReadLine()[0]; ////O(1)

            //read movies and pairs file and put it in an array
            Console.WriteLine("Enter Movies File"); ////O(1)
            moviesspath = Console.ReadLine(); ////O(1)
            Console.WriteLine("Enter Pairs File"); ////O(1)
            pairrs_path = Console.ReadLine(); ////O(1)
            read_movies = File.ReadAllLines(moviesspath); ////O(n) n is the number of lines in the file
            read_pairs = File.ReadAllLines(pairrs_path); ////O(n) n is the number of lines in the file
            Stopwatch no_optm = new Stopwatch();
            Stopwatch optm = new Stopwatch();

            //// initialization of objects and variables is O(1)
            movies = new Movies[read_movies.Length];
            string[] splitt;
            List<string> actors = new List<string>();
            List<string[]> splitt_pair = new List<string[]>();

            //get the path of the solutions file into an array
            string[] firstwordd = pairrs_path.Split('.'); ////O(1)
            pairrsol_path = firstwordd[0] + " - Solution.txt"; ////O(1)
            // System.Console.WriteLine(pairrsol_path);
            string[] Pairssolution = File.ReadAllLines(pairrsol_path);  ////O(n) n is the number of lines in the file

            File.Delete("solutions.txt"); ////O(n) n is the number of lines in the file

            for (int i = 0; i < read_movies.Length; i++) ////O(n*s) n is the number of lines in the read_movies, s in number of splits
            {
                //dictionary holds each movie and its actors
                actors = new List<string>();   ////O(1)
                splitt = read_movies[i].Split('/');  ////O(1) 
                for (int y = 1; y < splitt.Length; y++) ////O(s) s in number of splits
                {
                    actors.Add(splitt[y]);  ////O(1)
                    actorss.Add(splitt[y]);  ////O(1)

                }

                movies[i].actors_name = actors;  ////O(1)
                movies[i].movie_name = splitt[0];  ////O(1)
            }

            foreach (string actor in actorss)  ////O(v) v is the number of actors
            {
                neighbors.Add(new Dictionary<int, int>());  ////O(1)
                common_movies.Add(new Dictionary<string, string>());  ////O(1)
                actors_index.Add(actor, index);  ////O(1)
                actors_name.Add(index, actor);  ////O(1)
                index++;  ////O(1)
            }


            Construct_graph(); ////O(m*a*a) m in number of movies, a is the number of actors in each movie

            string[] sol = new string[read_pairs.Length];   ////O(1)


            ///bfs and optimization
            Console.WriteLine("To Run Without Optimization press 1 and to Run with Optimization press 2 :");  ////O(1)
            char my_choice = (char)Console.ReadLine()[0];  ////O(1)

            if (my_choice == '1')  ////O(1)
            {
                no_optm = Stopwatch.StartNew();

                for (int q = 0; q < read_pairs.Length; q++) ////O(p*()) p is th number of queries
                {
                    //split the query to 2 actors
                    string[] pair = new string[2];  ////O(1)
                    int pair1_src = 0, pair2_dest = 0;  ////O(1)
                    pair = read_pairs[q].Split('/');  ////O(1)
                    pair1_src = actors_index[pair[0]];  ////O(1)
                    pair2_dest = actors_index[pair[1]];  ////O(1)

                    ///call optimization function
                    sol[q] = BFS(pair1_src, pair2_dest);   /// O(v+e) v is number of actors, e number of edges
                    File.AppendAllText("solutions.txt", sol[q]); ////O(1)
                }
                no_optm.Stop();
            }
            else if (my_choice == '2') ////O(1)
            {
                optm = Stopwatch.StartNew();
                for (int q = 0; q < read_pairs.Length; q++) ////O(p*(v+e)) p is th number of queries
                {

                    //split the query to 2 actors
                    string[] pair = new string[2];  ////O(1)
                    int pair1_src = 0, pair2_dest = 0;  ////O(1)
                    pair = read_pairs[q].Split('/');  ////O(1)
                    pair1_src = actors_index[pair[0]];  ////O(1)
                    pair2_dest = actors_index[pair[1]];  ////O(1)

                    ///call optimization function
                    int degree = BFS_Optimization(pair1_src, pair2_dest); /// O(v+e) the upper bound is v + e where v is number of actors, e number of edges
                    sol[q] = pair[0] + "/" + pair[1] + "\n DoS= " + degree + "\n----------------------------------------------------\n";  ////O(1)
                    File.AppendAllText("solutions.txt", sol[q]);  ////O(1)
                }
                optm.Stop();
            }
            else  ////O(1)
            {
                System.Console.WriteLine("invalid option");  ////O(1)
            }

            ///read the final solutions file
            string[] solutions = File.ReadAllLines("solutions.txt");  ////O(n) n is the number of lines in the file

            if (test_choice == '1') ////O(1)
            {
                for (int p = 0; p < solutions.Length; p++) ////O(n) n is the body of th loop
                {
                    System.Console.WriteLine(solutions[p]); ////O(n) n is the number of lines in the file
                }
            }
            else if (test_choice == '2')  ////O(1)
            {
                int w = 0, c = 0;  ////O(1) 
                for (int p = 0; p < solutions.Length; p += 5) ////O(n/5) n is the number of lines in the file
                {
                    if (solutions[p] != Pairssolution[p])  ////O(1)
                    {
                        c++;
                    }
                    else ////O(1)
                    {
                        w++;
                    }
                    if (solutions[p + 1] != Pairssolution[p + 1])  ////O(1)
                    {
                        c++;
                    }
                    else  ////O(1)
                    {
                        w++;
                    }
                }
                System.Console.WriteLine("Error in " + c + " lines out of " + solutions.Length + " lines");  ////O(1)
                if (w == Pairssolution.Length)   ////O(1)
                {

                    System.Console.WriteLine("Congratulations your Code Passed the Test *********");  ////O(1)
                }

            }

            System.Console.WriteLine((float)(no_optm.ElapsedMilliseconds) + "milisec");   ////O(1)   
            System.Console.WriteLine((float)((no_optm.ElapsedMilliseconds) / 1000) + "sec");   ////O(1)
            System.Console.WriteLine("\n=========================================\n");   ////O(1)

            System.Console.WriteLine(optm.ElapsedMilliseconds + "milisec");   ////O(1)   
            System.Console.WriteLine((float)((optm.ElapsedMilliseconds) / 1000) + "sec");   ////O(1)
            System.Console.WriteLine("\n=========================================\n");   ////O(1)



            System.Console.WriteLine("Do you Want to run the First Bonus (y/n):");    ////O(1)
            Dictionary<int, double> sum_degree = new Dictionary<int, double>();   ////O(1)
            char mybonus_choice = (char)Console.ReadLine()[0];  ////O(1)
            if (mybonus_choice == 'y')  ////O(a)
            {
                System.Console.WriteLine("Enter Actor Name: ");  ////O(1)
                string actor_name = Console.ReadLine();  ////O(1)
                if (actorss.Contains(actor_name))  ////O(a)
                {
                    foreach (var index in actors_index) ////O(a) a is the total number of of actors
                    {

                        int bonus_sol = BFS_Optimization(actors_index[actor_name], index.Value);  /// O(v+e) the upper bound is v + e where v is number of actors, e number of edges
                        if (bonus_sol > 11)   ////O(1)
                        {
                            bonus_sol = 12;
                        }
                        if (sum_degree.ContainsKey(bonus_sol))  ////O(1)
                        {
                            sum_degree[bonus_sol]++;
                        }
                        else  ////O(1)
                        {
                            sum_degree.Add(bonus_sol, 1);
                        }

                    }
                    System.Console.WriteLine("Degree of separation          Frequency");   ////O(1)
                    foreach (var sum in sum_degree)  ////O(k) k is the numbers of degree of separations 
                    {
                        System.Console.WriteLine("      " + sum.Key + "             " + "             " + sum.Value); ////O(1)
                    }
                }
                else   ////O(1)
                {
                    System.Console.WriteLine("There is no actor with this name !!");
                }
            }
            else if (mybonus_choice != 'y' && mybonus_choice != 'n')   ////O(1)
            {
                System.Console.WriteLine("invalid option");
            }

            System.Console.WriteLine("\n=========================================\n");   ////O(1)
            System.Console.WriteLine("Do you Want to run the Second Bonus (y/n):");   ////O(1)
            char mybonus_choice2 = (char)Console.ReadLine()[0];   ////O(1)
            if (mybonus_choice2 == 'y')  
            { 
                System.Console.WriteLine("Enter Actors Query: ");  ////O(1)
                string actors_query = Console.ReadLine();   ////O(1)

                string[] pair = new string[2];    ////O(1)
                int pair1_src = 0, pair2_dest = 0;   ////O(1)
                pair = actors_query.Split('/');   ////O(1)
                pair1_src = actors_index[pair[0]];   ////O(1)
                pair2_dest = actors_index[pair[1]];   ////O(1)
                System.Console.WriteLine(Query_Strongest_Path(pair1_src, pair2_dest));     ////O(v+e) as worst case

            }
            else if (mybonus_choice != 'y' && mybonus_choice != 'n')   ////O(1)
            {
                System.Console.WriteLine("invalid option");
            }
        }

        public static void Construct_graph()
        {

            foreach (var movie in movies) ////O(m*a*a) m is the number of movies, a is the number of actors in each movie
            {
                for (int l = 0; l < movie.actors_name.Count; l++) ////O(a*a) 
                {
                    for (int m = 0; m < movie.actors_name.Count; m++) ////O(a)
                    {
                        string parent_name = movie.actors_name[l];  ////O(1)
                        string child_name = movie.actors_name[m];  ////O(1)
                        int parent_index = actors_index[parent_name];  ////O(1)
                        int child_index = actors_index[child_name];  ////O(1)

                        if (parent_index != child_index)  ////O(1)
                        {
                            if (neighbors[parent_index].ContainsKey(child_index) == false) ////O(1)
                            {
                                common_movies[parent_index].Add(child_name, movie.movie_name);  ////O(1)
                                neighbors[parent_index].Add(child_index, 1);  ////O(1)
                            }
                            else
                            {
                                //increasing el weight
                                neighbors[parent_index][child_index]++;  ////O(1)
                            }
                        }
                    }
                }
            }
        }
        public static string BFS(int pair1_src, int pair2_dest)
        {
            //// initialization of objects and variables is O(1)
            Dictionary<int, visited_node> its_visited_node = new Dictionary<int, visited_node>();
            string final_solutions = " ";
            Queue<int> actors_queue = new Queue<int>();
            visited_node src_colord_node;
            visited_node dest_visited_node = new visited_node();
            bool found_dest = false;
            src_colord_node = new visited_node(0, 0, pair1_src, 0); ////O(1)
            Stack<int> chains = new Stack<int>();
            int parent_actor_index = 0;
            int child_actor_index = 0;
            visited_node parent_node = new visited_node();
            string movie_chain = " ";
            string actor_chain = " ";

            //add the source actor 
            its_visited_node.Add(pair1_src, src_colord_node); ////O(1)

            //push the src index in the queue
            actors_queue.Enqueue(pair1_src); ////O(1)

            while (actors_queue.Count() > 0) ////O(v+e) v is the number of actors, e is the number of edges between the actors
            {
                int parent = actors_queue.Dequeue(); ////O(v) v is the the number of actors as each actor is accessed once

                foreach (var neighbor in neighbors[parent]) //// O(e) e is the number of edges between the actors
                {
                    visited_node neighbor_node = new visited_node(); ////O(1)
                    int max_weight = its_visited_node[parent].relation_strength + neighbor.Value; ////O(1)
                    int max_distance = its_visited_node[parent].distance + 1; ////O(1)

                    if (its_visited_node.ContainsKey(neighbor.Key) == false) ////O(1)
                    {
                        actors_queue.Enqueue(neighbor.Key); ////O(1)
                        neighbor_node = new visited_node(max_weight, max_distance, neighbor.Key, parent); ////O(1)
                        its_visited_node.Add(neighbor.Key, neighbor_node); ////O(1)

                    }
                    if (neighbor.Key == pair2_dest && found_dest == false)  ////O(1)
                    {
                        found_dest = true;  ////O(1)
                        if (dest_visited_node.relation_strength <= neighbor_node.relation_strength)
                            dest_visited_node = neighbor_node;  ////O(1)
                    }
                    else  ////O(1)
                    {
                        continue;
                    }
                    if (dest_visited_node.distance >= max_distance && found_dest == true && neighbor.Key == pair2_dest)  ////O(1)
                    {
                        if (max_weight >= dest_visited_node.relation_strength)
                        {
                            dest_visited_node.relation_strength = max_weight; ////O(1)
                            dest_visited_node.distance = max_distance;  ////O(1)
                        }
                    }
                    else  ////O(1)
                    {
                        continue;
                    }
                }
                if (found_dest == true && dest_visited_node.distance <= its_visited_node[parent].distance &&
                    dest_visited_node.relation_strength >= its_visited_node[parent].relation_strength)   ////O(d+c)
                {
                    if (dest_visited_node.relation_strength >= its_visited_node[parent].relation_strength)  ////O(d+c)
                    {
                        chains.Push(dest_visited_node.node_index); ////O(1)
                        parent_node = dest_visited_node; ////O(1)
                        for (int q = 0; q < dest_visited_node.distance; q++)  ////O(d) d is the number of nodes in the shortest path
                        {
                            parent_node = its_visited_node[parent_node.parent_index];  ////O(1)
                            chains.Push(parent_node.node_index);  ////O(1)
                                                                  // System.Console.WriteLine(parent_node.parent_index);
                        }

                        while (chains.Count > 1)   ////O(c) c is the number of nodes in the stack
                        {
                            parent_actor_index = chains.Pop();  ////O(1)
                            child_actor_index = chains.Peek();  ////O(1)
                            movie_chain += " => " + common_movies[parent_actor_index][actors_name[child_actor_index]];  ////O(1)
                            actor_chain += actors_name[parent_actor_index] + " -> ";  ////O(1)
                        }
                        actor_chain += actors_name[dest_visited_node.node_index];  ////O(1)
                        movie_chain += " =>";  ////O(1)

                        final_solutions = actors_name[pair1_src] + "/" + actors_name[pair2_dest]
                            + "\nDoS = " + dest_visited_node.distance + ", RS = " + dest_visited_node.relation_strength
                            + "\nCHAIN OF ACTORS:" + actor_chain + "\nCHAIN OF MOVIES:" + movie_chain + "\n\n";   ////O(1)

                        return final_solutions;   ////O(1)
                    }
                }
            }
            return null;   ////O(1)
        }
        public static int BFS_Optimization(int pair1_src, int pair2_dest)
        {
            //// initialization of objects and variables is O(1)
            Dictionary<int, visited_node> its_visited_node = new Dictionary<int, visited_node>();
            int final_solutions;
            Queue<int> actors_queue = new Queue<int>();
            visited_node src_colord_node;
            visited_node dest_visited_node = new visited_node();
            bool found_dest = false;
            src_colord_node = new visited_node(0, pair1_src, 0);


            //push the src index in the queue
            actors_queue.Enqueue(pair1_src);  ////O(1)

            its_visited_node.Add(pair1_src, src_colord_node);   ////O(1)

            while (actors_queue.Count() > 0) ////O(v+e) upper bound where v is the the number of actors, e is the number of edges between the actors
            {
                int parent = actors_queue.Dequeue(); ////O(v) v is the number of actors as each actor is accessed once

                foreach (var neighbor in neighbors[parent]) //// O(e) e is the number of edges between the actors
                {
                    visited_node neighbor_node = new visited_node();  ////O(1)
                    int max_distance = its_visited_node[parent].distance + 1;  ////O(1)

                    if (its_visited_node.ContainsKey(neighbor.Key) == false)  ////O(1)
                    {
                        actors_queue.Enqueue(neighbor.Key);   ////O(1)
                        neighbor_node = new visited_node(max_distance, neighbor.Key, parent);  ////O(1)
                        its_visited_node.Add(neighbor.Key, neighbor_node);  ////O(1)
                    }
                    if (neighbor.Key == pair2_dest && found_dest == false)
                    {
                        found_dest = true;  ////O(1)
                        dest_visited_node = neighbor_node;  ////O(1)
                        break;  ////O(1)
                    }
                    else
                    {
                        continue;     ////O(1)
                    }
                }
                if (found_dest == true && dest_visited_node.distance <= its_visited_node[parent].distance)  ////O(1)
                {
                    final_solutions = dest_visited_node.distance;  ////O(1)

                    return final_solutions;  ////O(1)
                }
            }
            return 0;  ////O(1)
        }
        public static string Query_Strongest_Path(int pair1_src, int pair2_dest)
        {
            //// initialization of objects and variables is O(1)
            Dictionary<int, visited_node> its_visited_node = new Dictionary<int, visited_node>();
            string final_solutions;
            Queue<int> actors_queue = new Queue<int>();
            visited_node src_colord_node;
            visited_node dest_visited_node = new visited_node();
            bool found_dest = false;
            src_colord_node = new visited_node(0, 0, pair1_src, 0);


            //push the src index in the queue
            actors_queue.Enqueue(pair1_src);     ////O(1)

            its_visited_node.Add(pair1_src, src_colord_node);   ////O(1)

            /// the while loop may not access all actors as the foreach may break if a certain condition happens
            while (actors_queue.Count() > 0)  ////O(v+e) upper bound where v is the number of actors, e is the number of edges between the actors
            {
                int parent = actors_queue.Dequeue();  ////O(v) v is the the number of actors as each actor is accessed once

                foreach (var neighbor in neighbors[parent])  //// O(e) e is the number of edges between the actors
                {
                    visited_node neighbor_node = new visited_node();    ////O(1)
                    int max_weight = its_visited_node[parent].relation_strength + neighbor.Value;    ////O(1)

                    if (its_visited_node.ContainsKey(neighbor.Key) == false)   ////O(1)
                    {
                        actors_queue.Enqueue(neighbor.Key);
                        neighbor_node = new visited_node(max_weight, 1, neighbor.Key, parent);
                        its_visited_node.Add(neighbor.Key, neighbor_node);

                    }
                    if (neighbor.Key == pair2_dest && found_dest == false)    ////O(1)
                    {
                        found_dest = true;
                        dest_visited_node = neighbor_node;
                    }
                    if (found_dest == true && neighbor.Key == pair2_dest && max_weight >= dest_visited_node.relation_strength)    ////O(1)
                    {
                        dest_visited_node.relation_strength = max_weight;
                        break;
                    }
                }

            }
            final_solutions = "RS = " + dest_visited_node.relation_strength + "\n";      ////O(1)

            return final_solutions;    ////O(1)
        } 

    }
}