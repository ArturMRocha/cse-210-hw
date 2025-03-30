using System;
using System.Collections.Generic;

public class Comment
{
    private string _commenterName;
    private string _commentText;

    public Comment(string commenterName, string commentText)
    {
        _commenterName = commenterName;
        _commentText = commentText;
    }

    public string GetCommentInfo()
    {
        return $"{_commenterName}: {_commentText}";
    }
}

public class Video
{
    private string _title;
    private string _author;
    private int _lengthInSeconds;
    private List<Comment> _comments;

    public Video(string title, string author, int lengthInSeconds)
    {
        _title = title;
        _author = author;
        _lengthInSeconds = lengthInSeconds;
        _comments = new List<Comment>();
    }

    public void AddComment(string commenterName, string commentText)
    {
        _comments.Add(new Comment(commenterName, commentText));
    }

    public int GetNumberOfComments()
    {
        return _comments.Count;
    }

    public void DisplayVideoInfo()
    {
        Console.WriteLine($"Title: {_title}");
        Console.WriteLine($"Author: {_author}");
        Console.WriteLine($"Length: {_lengthInSeconds} seconds");
        Console.WriteLine($"Number of comments: {GetNumberOfComments()}");
        Console.WriteLine("Comments:");
        
        foreach (Comment comment in _comments)
        {
            Console.WriteLine($"- {comment.GetCommentInfo()}");
        }
        
        Console.WriteLine();
    }
}

class Program
{
    static void Main()
    {
        // Create videos
        Video video1 = new Video("C# Tutorial for Beginners", "ProgrammingMaster", 720);
        Video video2 = new Video("ASP.NET Core Web API", "WebDevPro", 1500);
        Video video3 = new Video("Entity Framework Core", "DataWizard", 2100);

        // Add comments to videos
        video1.AddComment("JohnDoe", "Great tutorial!");
        video1.AddComment("JaneSmith", "Very helpful!");
        video1.AddComment("CodeNewbie", "Learned a lot!");

        video2.AddComment("BackendDev", "Excellent content");
        video2.AddComment("FullStacker", "When part 2?");
        video2.AddComment("DotNetFan", "Best explanation");
        video2.AddComment("JuniorDev", "Some parts were fast");

        video3.AddComment("DBAnalyst", "Great examples");
        video3.AddComment("ORMExpert", "Need more tips");
        video3.AddComment("CodeOptimizer", "Perfect!");

        // Create list of videos
        List<Video> videos = new List<Video> { video1, video2, video3 };

        // Display video information
        foreach (Video video in videos)
        {
            video.DisplayVideoInfo();
        }
    }
}