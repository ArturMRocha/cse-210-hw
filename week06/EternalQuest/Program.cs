using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace EternalQuest
{
    // Base Goal class
    public abstract class Goal
    {
        protected string _name;
        protected string _description;
        protected int _points;
        protected bool _isComplete;

        public Goal(string name, string description, int points)
        {
            _name = name;
            _description = description;
            _points = points;
            _isComplete = false;
        }

        public abstract int RecordEvent();
        public abstract string GetProgress();
        public abstract string GetGoalType();
        
        public string GetName() => _name;
        public string GetDescription() => _description;
        public bool IsComplete() => _isComplete;
        
        public virtual string GetDetails() => $"{_name} ({_description})";
        
        // For serialization
        public virtual Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>
            {
                { "Type", GetGoalType() },
                { "Name", _name },
                { "Description", _description },
                { "Points", _points },
                { "IsComplete", _isComplete }
            };
        }
    }

    // Simple Goal (one-time completion)
    public class SimpleGoal : Goal
    {
        public SimpleGoal(string name, string description, int points) 
            : base(name, description, points) { }

        public override int RecordEvent()
        {
            if (!_isComplete)
            {
                _isComplete = true;
                return _points;
            }
            return 0;
        }

        public override string GetProgress() => _isComplete ? "[X]" : "[ ]";

        public override string GetGoalType() => "Simple";

        public override string GetDetails() => $"{base.GetDetails()} - Points: {_points}";
    }

    // Eternal Goal (never complete, points each time)
    public class EternalGoal : Goal
    {
        private int _timesCompleted;

        public EternalGoal(string name, string description, int points) 
            : base(name, description, points) 
        {
            _timesCompleted = 0;
        }

        public override int RecordEvent()
        {
            _timesCompleted++;
            return _points;
        }

        public override string GetProgress() => "[∞]";

        public override string GetGoalType() => "Eternal";

        public override string GetDetails() => $"{base.GetDetails()} - Points per completion: {_points} (Completed {_timesCompleted} times)";
        
        public override Dictionary<string, object> ToDictionary()
        {
            var dict = base.ToDictionary();
            dict.Add("TimesCompleted", _timesCompleted);
            return dict;
        }
    }

    // Checklist Goal (complete a certain number of times)
    public class ChecklistGoal : Goal
    {
        private int _timesCompleted;
        private int _targetCount;
        private int _bonusPoints;

        public ChecklistGoal(string name, string description, int points, int targetCount, int bonusPoints) 
            : base(name, description, points) 
        {
            _timesCompleted = 0;
            _targetCount = targetCount;
            _bonusPoints = bonusPoints;
        }

        public override int RecordEvent()
        {
            if (!_isComplete)
            {
                _timesCompleted++;
                
                if (_timesCompleted >= _targetCount)
                {
                    _isComplete = true;
                    return _points + _bonusPoints;
                }
                return _points;
            }
            return 0;
        }

        public override string GetProgress() => $"Completed {_timesCompleted}/{_targetCount} times";

        public override string GetGoalType() => "Checklist";

        public override string GetDetails() => $"{base.GetDetails()} - Points per completion: {_points} - Bonus: {_bonusPoints} when completed {_targetCount} times";

        public override Dictionary<string, object> ToDictionary()
        {
            var dict = base.ToDictionary();
            dict.Add("TimesCompleted", _timesCompleted);
            dict.Add("TargetCount", _targetCount);
            dict.Add("BonusPoints", _bonusPoints);
            return dict;
        }
    }

    // Negative Goal (lose points for bad habits)
    public class NegativeGoal : Goal
    {
        public NegativeGoal(string name, string description, int points) 
            : base(name, description, points) { }

        public override int RecordEvent()
        {
            return -_points; // Deduct points
        }

        public override string GetProgress() => "[⚠]";

        public override string GetGoalType() => "Negative";

        public override string GetDetails() => $"{base.GetDetails()} - Points lost each time: {_points}";
    }

    // Progress Goal (make progress toward a large goal)
    public class ProgressGoal : Goal
    {
        private int _currentProgress;
        private int _targetProgress;
        private int _progressPoints;

        public ProgressGoal(string name, string description, int completionPoints, int targetProgress, int progressPoints) 
            : base(name, description, completionPoints) 
        {
            _currentProgress = 0;
            _targetProgress = targetProgress;
            _progressPoints = progressPoints;
        }

        public override int RecordEvent()
        {
            if (!_isComplete)
            {
                _currentProgress++;
                if (_currentProgress >= _targetProgress)
                {
                    _isComplete = true;
                    return _points; // Full points when completed
                }
                return _progressPoints; // Partial points for progress
            }
            return 0;
        }

        public override string GetProgress() => $"Progress: {_currentProgress}/{_targetProgress}";

        public override string GetGoalType() => "Progress";

        public override string GetDetails() => $"{base.GetDetails()} - Progress points: {_progressPoints} - Completion points: {_points}";

        public override Dictionary<string, object> ToDictionary()
        {
            var dict = base.ToDictionary();
            dict.Add("CurrentProgress", _currentProgress);
            dict.Add("TargetProgress", _targetProgress);
            dict.Add("ProgressPoints", _progressPoints);
            return dict;
        }
    }

    // User profile with gamification elements
    public class UserProfile
    {
        private List<Goal> _goals = new List<Goal>();
        private int _totalPoints = 0;
        private int _level = 1;
        private int _pointsToNextLevel = 1000;
        private List<string> _achievements = new List<string>();

        public void AddGoal(Goal goal) => _goals.Add(goal);
        
        public void RecordEvent(string goalName)
        {
            var goal = _goals.FirstOrDefault(g => g.GetName().Equals(goalName, StringComparison.OrdinalIgnoreCase));
            if (goal != null)
            {
                int pointsEarned = goal.RecordEvent();
                _totalPoints += pointsEarned;
                
                // Check for level up
                while (_totalPoints >= _pointsToNextLevel)
                {
                    _level++;
                    _pointsToNextLevel = _level * 1000;
                    _achievements.Add($"Reached Level {_level}!");
                    Console.WriteLine($"\n⭐ LEVEL UP! You are now Level {_level} ⭐\n");
                }
                
                // Check for special achievements
                CheckAchievements();
                
                Console.WriteLine($"\nEvent recorded for {goalName}. You earned {pointsEarned} points!");
            }
            else
            {
                Console.WriteLine($"Goal '{goalName}' not found.");
            }
        }

        private void CheckAchievements()
        {
            // Check for completion achievements
            int completedGoals = _goals.Count(g => g.IsComplete());
            if (completedGoals >= 5 && !_achievements.Contains("Goal Getter: Complete 5 goals"))
            {
                _achievements.Add("Goal Getter: Complete 5 goals");
                Console.WriteLine("\n🏆 ACHIEVEMENT UNLOCKED: Goal Getter (Complete 5 goals) 🏆\n");
            }
            
            // Check for eternal goal achievements
            int eternalCompletions = _goals
                .Where(g => g is EternalGoal)
                .Sum(g => ((EternalGoal)g).RecordEvent()); // Note: This isn't perfect, would need to track separately
            if (eternalCompletions >= 10 && !_achievements.Contains("Eternal Champion: Record 10 eternal goal completions"))
            {
                _achievements.Add("Eternal Champion: Record 10 eternal goal completions");
                Console.WriteLine("\n🏆 ACHIEVEMENT UNLOCKED: Eternal Champion (Record 10 eternal goal completions) 🏆\n");
            }
        }

        public void DisplayGoals()
        {
            Console.WriteLine("\nYour Goals:");
            if (_goals.Count == 0)
            {
                Console.WriteLine("No goals yet. Create some to get started!");
                return;
            }

            for (int i = 0; i < _goals.Count; i++)
            {
                var goal = _goals[i];
                Console.WriteLine($"{i + 1}. {goal.GetProgress()} {goal.GetDetails()}");
            }
        }

        public void DisplayScore()
        {
            Console.WriteLine($"\nCurrent Score: {_totalPoints} points");
            Console.WriteLine($"Level: {_level}");
            Console.WriteLine($"Points to next level: {_pointsToNextLevel - _totalPoints}");
            
            if (_achievements.Count > 0)
            {
                Console.WriteLine("\nAchievements:");
                foreach (var achievement in _achievements)
                {
                    Console.WriteLine($"- {achievement}");
                }
            }
        }

        public void SaveToFile(string filename)
        {
            var data = new
            {
                Goals = _goals.Select(g => g.ToDictionary()).ToList(),
                TotalPoints = _totalPoints,
                Level = _level,
                PointsToNextLevel = _pointsToNextLevel,
                Achievements = _achievements
            };

            string json = JsonSerializer.Serialize(data);
            File.WriteAllText(filename, json);
            Console.WriteLine($"Progress saved to {filename}");
        }

        public void LoadFromFile(string filename)
        {
            if (File.Exists(filename))
            {
                string json = File.ReadAllText(filename);
                var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

                _totalPoints = data["TotalPoints"].GetInt32();
                _level = data["Level"].GetInt32();
                _pointsToNextLevel = data["PointsToNextLevel"].GetInt32();
                _achievements = data["Achievements"].EnumerateArray().Select(a => a.GetString()).ToList();

                _goals.Clear();
                foreach (var goalElement in data["Goals"].EnumerateArray())
                {
                    var goalDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(goalElement.GetRawText());
                    string type = goalDict["Type"].GetString();
                    string name = goalDict["Name"].GetString();
                    string description = goalDict["Description"].GetString();
                    int points = goalDict["Points"].GetInt32();
                    bool isComplete = goalDict["IsComplete"].GetBoolean();

                    Goal goal = null;
                    switch (type)
                    {
                        case "Simple":
                            goal = new SimpleGoal(name, description, points);
                            break;
                        case "Eternal":
                            goal = new EternalGoal(name, description, points);
                            ((EternalGoal)goal).RecordEvent(); // Hack to set times completed
                            break;
                        case "Checklist":
                            int targetCount = goalDict["TargetCount"].GetInt32();
                            int bonusPoints = goalDict["BonusPoints"].GetInt32();
                            goal = new ChecklistGoal(name, description, points, targetCount, bonusPoints);
                            for (int i = 0; i < goalDict["TimesCompleted"].GetInt32(); i++)
                                ((ChecklistGoal)goal).RecordEvent();
                            break;
                        case "Negative":
                            goal = new NegativeGoal(name, description, points);
                            break;
                        case "Progress":
                            int targetProgress = goalDict["TargetProgress"].GetInt32();
                            int progressPoints = goalDict["ProgressPoints"].GetInt32();
                            goal = new ProgressGoal(name, description, points, targetProgress, progressPoints);
                            for (int i = 0; i < goalDict["CurrentProgress"].GetInt32(); i++)
                                ((ProgressGoal)goal).RecordEvent();
                            break;
                    }

                    if (goal != null)
                    {
                        if (isComplete) goal.RecordEvent(); // Mark complete if needed
                        _goals.Add(goal);
                    }
                }

                Console.WriteLine($"Progress loaded from {filename}");
            }
            else
            {
                Console.WriteLine("No save file found. Starting fresh.");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("🌟 Eternal Quest Program 🌟");
            Console.WriteLine("Track your goals and grow spiritually!");
            
            UserProfile user = new UserProfile();
            user.LoadFromFile("eternalquest.json");

            bool running = true;
            while (running)
            {
                Console.WriteLine("\nMain Menu:");
                Console.WriteLine("1. View Goals");
                Console.WriteLine("2. Create New Goal");
                Console.WriteLine("3. Record Goal Event");
                Console.WriteLine("4. View Score");
                Console.WriteLine("5. Save Progress");
                Console.WriteLine("6. Exit");
                Console.Write("Choose an option: ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        user.DisplayGoals();
                        break;
                    case "2":
                        CreateNewGoal(user);
                        break;
                    case "3":
                        RecordGoalEvent(user);
                        break;
                    case "4":
                        user.DisplayScore();
                        break;
                    case "5":
                        user.SaveToFile("eternalquest.json");
                        break;
                    case "6":
                        running = false;
                        Console.WriteLine("Remember to save before exiting!");
                        Console.Write("Save progress now? (y/n): ");
                        if (Console.ReadLine().ToLower() == "y")
                        {
                            user.SaveToFile("eternalquest.json");
                        }
                        Console.WriteLine("Goodbye! Keep working on your eternal quest!");
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        static void CreateNewGoal(UserProfile user)
        {
            Console.WriteLine("Goal Types:");
            Console.WriteLine("1. Simple Goal (one-time completion)");
            Console.WriteLine("2. Eternal Goal (never complete, points each time)");
            Console.WriteLine("3. Checklist Goal (complete a certain number of times)");
            Console.WriteLine("4. Negative Goal (lose points for bad habits)");
            Console.WriteLine("5. Progress Goal (make progress toward a large goal)");
            Console.Write("Select goal type: ");
            
            string typeChoice = Console.ReadLine();
            Console.Write("Enter goal name: ");
            string name = Console.ReadLine();
            Console.Write("Enter description: ");
            string description = Console.ReadLine();

            Goal goal = null;
            switch (typeChoice)
            {
                case "1":
                    Console.Write("Enter points for completing: ");
                    int points = int.Parse(Console.ReadLine());
                    goal = new SimpleGoal(name, description, points);
                    break;
                case "2":
                    Console.Write("Enter points per completion: ");
                    int eternalPoints = int.Parse(Console.ReadLine());
                    goal = new EternalGoal(name, description, eternalPoints);
                    break;
                case "3":
                    Console.Write("Enter points per completion: ");
                    int checklistPoints = int.Parse(Console.ReadLine());
                    Console.Write("Enter target number of completions: ");
                    int targetCount = int.Parse(Console.ReadLine());
                    Console.Write("Enter bonus points for full completion: ");
                    int bonusPoints = int.Parse(Console.ReadLine());
                    goal = new ChecklistGoal(name, description, checklistPoints, targetCount, bonusPoints);
                    break;
                case "4":
                    Console.Write("Enter points to lose each time: ");
                    int negativePoints = int.Parse(Console.ReadLine());
                    goal = new NegativeGoal(name, description, negativePoints);
                    break;
                case "5":
                    Console.Write("Enter completion points: ");
                    int completionPoints = int.Parse(Console.ReadLine());
                    Console.Write("Enter target progress count: ");
                    int  targetProgress = int.Parse(Console.ReadLine());
                    Console.Write("Enter points for each progress step: ");
                    int progressPoints = int.Parse(Console.ReadLine());
                    goal = new ProgressGoal(name, description, completionPoints, targetProgress, progressPoints);
                    break;
                default:
                    Console.WriteLine("Invalid goal type.");
                    return;
            }

            user.AddGoal(goal);
            Console.WriteLine($"New {goal.GetGoalType()} goal added: {name}");
        }

        static void RecordGoalEvent(UserProfile user)
        {
            Console.Write("Enter the name of the goal you completed: ");
            string goalName = Console.ReadLine();
            user.RecordEvent(goalName);
        }
    }
}