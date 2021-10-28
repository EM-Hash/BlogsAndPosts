using System;
using NLog.Web;
using System.IO;
using System.Linq;

namespace BlogsConsole
{
    class Program
    {
        //This method will allow the user to view all blogs
        static void viewBlogs(){
            logger.Info("viewBlogs method start");
            //Display number of blogs
            int count = db.Blogs.Count();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Blog Count:" + count.ToString());
            Console.ForegroundColor = ConsoleColor.Black;
            //If there are no blogs to display, break here
            if(!(count > 0)){
                return;
            }
            // Otherwise, Display all Blogs from the database
            var query = db.Blogs.OrderBy(b => b.Name);

            logger.Info("Start viewing blogs...");
            Console.WriteLine("All blogs in the database:");
            foreach (var item in query)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(item.Name);
                Console.ForegroundColor = ConsoleColor.Black;
            }
            logger.Info("viewBlogs method complete");
        }
        
        //This method will allow the user to add a blog
        static void addBlog(){
            logger.Info("addBlog method start");
            try
            {
                logger.Info("Creating blog...");
                // Create and save a new Blog
                string name = getFilledAnswer("name", "blog");
                var blog = new Blog { Name = name };
                
                db.AddBlog(blog);
                logger.Info("Blog added - {name}", name);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                logger.Error(ex.Message);
                Console.ForegroundColor = ConsoleColor.Black;
            }
            logger.Info("addBlog method complete");
        }

        //This method will allow the user to add a post to a selected blog
        static void addPost(){
            logger.Info("addPost method start");
            //If the user wants to add more posts or not
            bool lastPost = false;
            int blogId = 0;
            //Prompt the user for a Blog number until a valid one is entered
            do{
                logger.Info("Prompting user for valid ID");
                try{
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Which blog do you wish to post to?: ");
                    Console.ForegroundColor = ConsoleColor.Black;
                    blogId = getValidBlogId();
                } catch (Exception ex){
                    logger.Error(ex.Message);
                }
                //Once the user has given a valid id, start prompting for post info
                logger.Info("Valid blog id given");

                logger.Info("Prompting user for post info");
                //Prompt the user for the Title
                Console.ForegroundColor = ConsoleColor.Green;
                string title = getFilledAnswer("title", "post");
                Console.ForegroundColor = ConsoleColor.Black;

                //Prompt the user for theContent
                Console.ForegroundColor = ConsoleColor.Green;
                string content = getFilledAnswer("content","post");
                Console.ForegroundColor = ConsoleColor.Black;
                logger.Info("Post information gathered");

                //Add to the post with the given ID
                Post post = new Post();
                post.BlogId = blogId;
                post.Title = title;
                post.Content = content;
                db.AddPost(post);
                logger.Info("New post created - {post}", title);

                //Ask if the user wants to add another post
                logger.Info("Checking if user wants to create another post");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Add another post? [Y/N]: ");
                Console.ForegroundColor = ConsoleColor.Black;
                string contAns = Console.ReadLine();
                if(contAns.ToLower()[0] == 'y'){
                    logger.Info("User will create another post");
                    //If the user answered yes, continue
                    continue;
                } else {
                    logger.Info("User has created last post");
                    //Otherwise, set lastPost to true
                    lastPost = true;
                }
            } while(!lastPost);   
            logger.Info("addPost method complete");  
        }
        
        //This method displays the blogs as a menu
        static void blogMenu(){
            logger.Info("blogMenu method start");
            //Blog query
            logger.Info("create query holding all blogs, sorted by BlogId");
            var blogQuery = db.Blogs.OrderBy(b => b.BlogId);
            //Display the blogs by order of blog id
            logger.Info("list all blogs in order of BlogId");
            foreach(Blog b in blogQuery){
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"[{b.BlogId}] {b.Name}");
                Console.ForegroundColor = ConsoleColor.Black;
            }
            logger.Info("blogMenu method complete");
            return;
        }
        
        //This method will make sure the user entered a valid choice for a blog, and return said Id
        static int getValidBlogId(){
            logger.Info("getValidBlogId method start");
            //Did the user put in a valid Id?
            bool validId = false;
            int id = 0;
            do{
                logger.Info("Begin Id collection loop");
                //Until the user inputs a valid id...
                //Display their choices
                blogMenu();
                //Take in the id
                string input = Console.ReadLine();
                //Test if the input is an integer
                if(!Int32.TryParse(input, out id)){
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    logger.Warn("User attempted to input non-integer value");
                    Console.ForegroundColor = ConsoleColor.Black;
                    //If not, tell the user it must be
                    Console.WriteLine("The Blog ID must be an integer");
                    //Continue and reloop
                    continue;
                } else {
                    //Otherwise
                    //Check if there's a blog with that id
                    var idQuery = db.Blogs.Where(b => b.BlogId == id);
                    //If there're any values in the query
                    if(idQuery.Any()){
                        logger.Info("User entered valid ID");
                        //Set validID to true and break out of the loop
                        validId = true;
                        continue;
                    } else {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        logger.Warn("User entered an ID for a blog that does not exist");
                        Console.ForegroundColor = ConsoleColor.Black;
                        //Otherwise, tell the user there's no blog with that id and re=loop
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("There is no blog with the given ID");
                        Console.ForegroundColor = ConsoleColor.Black;
                        continue;
                    }
                }
            } while (!validId);
            logger.Info("getValidBlogId method complete");
            return id;
        }
        
        //This method will get a non-null, non-empty answer that goes into either a post or a blog
            //Pass in the name of the variable requested and either "blog" or "post"
        static string getFilledAnswer(string varNeeded, string blogOrPost){
            logger.Info("getFilledAnswer method start");
            string ans = null;
            bool filledAnswer = false;
            //While not having a valid answer...
            do{
                logger.Info("Begin information gathering loop");
                //Prompt the user for the value
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"What is the {varNeeded} of the {blogOrPost}?");
                Console.ForegroundColor = ConsoleColor.Black;
                ans = Console.ReadLine();
                //If the answer is not empty...
                if(ans == null || ans.Trim() == ""){
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    logger.Warn("User attempted to input null/blank answer");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"The {varNeeded} cannot be blank.");
                    Console.ForegroundColor = ConsoleColor.Black;
                } else {
                    //Set filledAnswer to true
                    filledAnswer = true;
                }
            } while (!filledAnswer);
            logger.Info("getFilledAnswer method complete");
            return ans;
        }

        //This method will view posts
        static void viewPosts(){
            logger.Info("viewPosts method start");
            //The blogs ordered by their id
            var blogQuery = db.Blogs.OrderBy(b => b.BlogId);
            //Have user choose which blog to view the posts of
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Which blog's posts do you want to view?");
            Console.ForegroundColor = ConsoleColor.Black;
            int blogId = getValidBlogId();
            //Get the blog's name
            string name = blogQuery.Where(b => b.BlogId == blogId).First().Name;
            //Build a query of posts with the given blogId
            var postQuery = db.Posts.Where(p => p.BlogId == blogId).OrderBy(p => p.Title);
            //Check if that blog has any posts
            if(!postQuery.Any()){
                //If there are no posts, say so
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("This blog doesn't have any posts.");
                Console.ForegroundColor = ConsoleColor.Black;
            } else {
                //Otherwise...

                //Display post count
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Posts: " + postQuery.Count());
                //Display all posts
                foreach(Post p in postQuery){
                    //Display blog name, along with the post info
                    Console.WriteLine($"Blog: {name}");
                    Console.WriteLine($"Post Title: {p.Title}");
                    Console.WriteLine($"{p.Content}");
                    Console.WriteLine("----------");
                }
                Console.ForegroundColor = ConsoleColor.Black;
            }
            logger.Info("viewPosts method complete");
        }
        
        // create static instance of Logger
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();
        //create static instance of database
        private static BloggingContext db = new BloggingContext();


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
            - Default is Black
            - Errors in orange?

            Loop until user wants to quit
            */
            
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
                Console.ForegroundColor = ConsoleColor.Black;
                //Save the answer
                string ans = Console.ReadLine();
                logger.Info("Main Menu switch");
                switch(ans){
                    case "1":
                        logger.Info("View all blogs");
                        viewBlogs();
                        break;
                    case "2":
                        logger.Info("Create a blog");   
                        addBlog(); 
                        break;
                    case "3":
                        logger.Info("Create a post");
                        addPost();
                        break;
                    case "4": 
                        logger.Info("View all posts");
                        viewPosts();
                        break;
                    default:
                        logger.Info("Quit program");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Goodbye!");
                        Console.WriteLine("Shutting down...");
                        Console.ForegroundColor = ConsoleColor.Black;
                        progRun = false;
                        break;
                }
            } while (progRun);
            

            logger.Info("Program ended");
        }
    }
}