using System;
using NLog.Web;
using System.IO;
using System.Linq;

namespace BlogsConsole
{
    class Program
    {
        //This method will allow the user to view all blogs
        static void viewBlogs(BlogsConsole.BloggingContext db){
            //Display number of blogs
            int count = db.Blogs.Count();
            Console.WriteLine("Blog Count:" + count.ToString());

            // Display all Blogs from the database
            var query = db.Blogs.OrderBy(b => b.Name);

            Console.WriteLine("All blogs in the database:");
            foreach (var item in query)
            {
                Console.WriteLine(item.Name);
            }
        }
        
        //This method will allow the user to add a blog - pass in the database
        static void addBlog(BlogsConsole.BloggingContext db){
            try
            {
                // Create and save a new Blog
                Console.Write("Enter a name for a new Blog: ");
                var name = Console.ReadLine();
                var blog = new Blog { Name = name };
                
                db.AddBlog(blog);
                logger.Info("Blog added - {name}", name);

                // Display all Blogs from the database
                var query = db.Blogs.OrderBy(b => b.Name);

                Console.WriteLine("All blogs in the database:");
                foreach (var item in query)
                {
                    Console.WriteLine(item.Name);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        // create static instance of Logger
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();
        
        static void Main(string[] args)
        {

            /* OUTLINE:
            Present User with 5 choices: 
            1 - View all Blogs
                Display num of blogs and then all blogs
            2 - Add Blog
            3 - Create Post
                - Prompt user with blog choices, to decide which one to use
                - Save post to Posts table
            4 - View all Posts
                - Prompt user w/ blog choices, to decide which blog's posts to view
                - Display number of posts + all posts
                    Per post, display Blog Name, Post Title, and Post Content
            5 - Quit
            
            Personal Touches:
            - Menus in special color - green
            - Listing of blogs/posts in other special color - cyan
            - User prompting in White
            - Errors in orange?

            Loop until user wants to quit
            */

            //Create the database
            var db = new BloggingContext();
            
            logger.Info("Program started");
            //Is the program still going? 
            bool progRun = true;
            do{
                //Give the user the menu
                logger.Info("Display Menu");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Welcome! What would you like to do?: ");
                Console.WriteLine("[1] View all blogs");
                Console.WriteLine("[2] Add a blog");
                Console.WriteLine("[3] Create a post");
                Console.WriteLine("[4] View all posts");
                Console.WriteLine("[5] Quit");
                Console.ForegroundColor = ConsoleColor.White;
                //Save the answer
                string ans = Console.ReadLine();
                logger.Info("Main Menu switch");
                switch(ans){
                    case "1":
                        logger.Info("View all blogs");
                        viewBlogs(db);
                        break;
                    case "2":
                        logger.Info("Create a blog");   
                        addBlog(db); 
                        break;
                    case "3":
                        logger.Info("Create a post");
                        break;
                    case "4": 
                        logger.Info("View all posts");
                        break;
                    default:
                        logger.Info("Quit program");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Goodbye!");
                        Console.WriteLine("Shutting down...");
                        Console.ForegroundColor = ConsoleColor.White;
                        progRun = false;
                        break;
                }
            } while (progRun);
            

            logger.Info("Program ended");
        }
    }
}