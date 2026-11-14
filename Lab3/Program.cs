using System;
using System.IO;

class Program
{
    const string StartMarker = "Событие началось: Восстание Ледяного Пламени";
    const string EndMarker = "Событие \"Восстание Ледяного Пламени\" закрыто";

    static DateTime GetEventDate(string[] lines)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains(StartMarker))
            {
                return DateTime.Parse(lines[i].Substring(0, 10));
            }
        }
        return DateTime.MinValue;
    }

    static string GetWinner(string[] lines)
    {
        bool isEventRunning = false;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            if (line.Contains(StartMarker))
                isEventRunning = true;

            if (isEventRunning && line.Contains("объявлены победителями события"))
            {
                int messageStart = line.IndexOf("] ") + 2;
                int winnerEnd = line.IndexOf(" объявлены победителями");
                return line.Substring(messageStart, winnerEnd - messageStart);
            }

            if (line.Contains(EndMarker))
                isEventRunning = false;
        }
        return "";
    }

    static int GetPoints(string[] lines, string winner)
    {
        bool isEventRunning = false;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            if (line.Contains(StartMarker))
                isEventRunning = true;

            if (isEventRunning && line.Contains("[Reward]")
                && line.Contains(winner + " получили ") && line.Contains(" очков события"))
            {
                string startText = winner + " получили ";
                int start = line.IndexOf(startText) + startText.Length;
                int end = line.IndexOf(" очков", start);
                return int.Parse(line.Substring(start, end - start));
            }

            if (line.Contains(EndMarker))
                isEventRunning = false;
        }
        return 0;
    }

    static string GetItem(string[] lines, string winner)
    {
        bool isEventRunning = false;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            if (line.Contains(StartMarker))
                isEventRunning = true;

            string startText = winner + " получили ивентовый предмет:";
            if (isEventRunning && line.Contains(startText))
            {
                int start = line.IndexOf(startText) + startText.Length;
                return line.Substring(start).Trim();
            }

            if (line.Contains(EndMarker))
                isEventRunning = false;
        }
        return "";
    }

    static string GetConsolationTeam(string[] lines)
    {
        bool isEventRunning = false;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            if (line.Contains(StartMarker))
                isEventRunning = true;

            if (isEventRunning && line.Contains("утешительную награду:"))
            {
                int messageStart = line.IndexOf("] ") + 2;
                int teamEnd = line.IndexOf(" получили утешительную награду:");
                return line.Substring(messageStart, teamEnd - messageStart);
            }

            if (line.Contains(EndMarker))
                isEventRunning = false;
        }
        return "";
    }

    static int GetConsolationPoints(string[] lines)
    {
        bool isEventRunning = false;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            if (line.Contains(StartMarker))
                isEventRunning = true;

            if (isEventRunning && line.Contains("утешительную награду:"))
            {
                string startText = "утешительную награду: ";
                int start = line.IndexOf(startText) + startText.Length;
                int end = line.IndexOf(" очков", start);
                return int.Parse(line.Substring(start, end - start));
            }

            if (line.Contains(EndMarker))
                isEventRunning = false;
        }
        return 0;
    }

    static int CountLevel(string[] lines, string level)
    {
        bool isEventRunning = false;
        int count = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            if (line.Contains(StartMarker))
                isEventRunning = true;

            if (isEventRunning && line.Contains(level))
                count++;

            if (line.Contains(EndMarker))
                isEventRunning = false;
        }
        return count;
    }

    static string ToGenitive(string name)
    {
        string[] words = name.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            if (words[i].EndsWith("ые"))
                words[i] = words[i].Substring(0, words[i].Length - 2) + "ых";
            else if (words[i].EndsWith("ки"))
                words[i] = words[i].Substring(0, words[i].Length - 2) + "ков";
        }
        return string.Join(" ", words);
    }

    static void Main()
    {
        string[] lines = File.ReadAllLines("event_server.log");

        DateTime date = GetEventDate(lines);
        string winner = GetWinner(lines);
        int points = GetPoints(lines, winner);
        string item = GetItem(lines, winner);
        string consolationTeam = GetConsolationTeam(lines);
        int consolationPoints = GetConsolationPoints(lines);
        int warnings = CountLevel(lines, "[Warning]");
        int errors = CountLevel(lines, "[Error]");

        Console.WriteLine("# Итоги события: Восстание Ледяного Пламени");
        Console.WriteLine();
        Console.WriteLine("Дата: " + date.ToString("dd.MM.yyyy"));
        Console.WriteLine("Победитель: " + winner);
        Console.WriteLine("Очки победителя: " + points);
        Console.WriteLine("Ивентовый предмет: " + item);
        Console.WriteLine("Утешительная награда " + ToGenitive(consolationTeam) + ": " + consolationPoints + " очков");
        Console.WriteLine("Предупреждений во время события: " + warnings);
        Console.WriteLine("Ошибок во время события: " + errors);
    }
}
