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

        static void addPost(BlogsConsole.BloggingContext db){
            //If the post input is valid
            bool validPost = false;
            //If the user wants to add more posts or not
            bool lastPost = false;
            //All blogs will be displayed, ordered by their ID
            var blogQuery = db.Blogs.OrderBy(b => b.BlogId);
            string blogAns = null;
            //Prompt the user for a Blog number until a valid one is entered
            do{
                try{
                    Console.WriteLine("Which blog do you wish to post to?: ");
                    foreach (var blog in blogQuery){
                        Console.WriteLine($"[{blog.BlogId}] {blog.Name}");
                    }
                blogAns = Console.ReadLine();
                } catch (Exception ex){
                    logger.Error(ex.Message);
                }
                int blogNum = 0;
                //If the blogAns is not a number...
                if(!Int32.TryParse(blogAns, out blogNum)){
                    logger.Info("Non-integer answer");
                    //Tell the user such
                    Console.WriteLine("Please enter a valid number\n");
                    //Break
                    validPost = false;
                    continue;
                } else{
                    //If we know that it's a number, check if it exists in the table
                    //Make a query that contains all the values that have this number as the ID
                    var idQuery = db.Blogs.Where(b => b.BlogId == blogNum);
                    //If the id is empty, the key doesn't exist
                    if(!idQuery.Any()){
                        logger.Info("Blog id does not exist");
                        //Tell the user the blog doesn't exist
                        Console.WriteLine("There is no blog with the given ID.\n");
                        //Continue
                        continue;
                    } else {
                        logger.Info("Valid blog id given");
                        //If it's a valid id...
                        //Prompt the user for the Title
                        Console.WriteLine("What is the title of the post?");
                        string title = Console.ReadLine();
                        //Prompt the user for theContent
                        Console.WriteLine("What is the content of the post?");
                        string content = Console.ReadLine();
                        //Add to the post with the given ID
                        Post post = new Post();
                        post.BlogId = blogNum;
                        post.Title = title;
                        post.Content = content;
                        db.AddPost(post);

                        //Ask if the user wants to add another post
                        Console.WriteLine("Add another post? [Y/N]: ");
                        string contAns = Console.ReadLine();
                        if(contAns.ToLower()[0] == 'y'){
                            //If the user answered yes, continue
                            continue;
                        } else {
                            //Otherwise, set lastPost to true
                            lastPost = true;
                        }
                    }
                }
                logger.Info("Valid post value: " + validPost);
                logger.Info("Last post value: " + lastPost);
            } while(!validPost && !lastPost);     
        }
        
        //This method will get a non-null, non-empty answer that goes into either a post or a blog
        static string getFilledAnswer(string varNeeded, string blogOrPost){
            string ans = null;
            bool filledAnswer = false;
            //While not having a valid answer...
            do{
                //Prompt the user for the value
                Console.WriteLine($"What is the {varNeeded} of the {blogOrPost}?");
                //If the answer is not empty...
                if(ans.Trim() != null){
                    //Set filledAnswer to true
                    filledAnswer = true;
                }
            } while (!filledAnswer);
            return ans;
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
                        addPost(db);
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