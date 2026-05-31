using System;
using System.Xml.Serialization;

namespace Model.Data
{
    public class HighScore : IComparable<HighScore>
    {
        public string PlayerName { get; set; }
        public int Score { get; set; }
        public int Difficulty { get; set; }
        public DateTime Date { get; set; }

        public HighScore()
        {
            Date = DateTime.Now;
        }

        public HighScore(string playerName, int score, int difficulty)
        {
            PlayerName = playerName;
            Score = score;
            Difficulty = difficulty;
            Date = DateTime.Now;
        }
        public int CompareTo(HighScore other)
        {
            if (other == null) return 1;
            return other.Score.CompareTo(this.Score);
        }
    
        public static bool operator >(HighScore a, HighScore b) => a?.Score > b?.Score;
        public static bool operator <(HighScore a, HighScore b) => a?.Score < b?.Score;
        public static bool operator ==(HighScore a, HighScore b) => a?.Score == b?.Score;
        public static bool operator !=(HighScore a, HighScore b) => !(a == b);
  
        public override bool Equals(object obj)
        {
            // Проверяем, является ли переданный объект объектом типа HighScore
            if (!(obj is HighScore))
            {
                return false; 
            }

            // Приводим obj к типу HighScore
            HighScore other = (HighScore)obj;

            // Используем перегруженный оператор == для сравнения
            return this == other;
        }
     
        public bool Equals(HighScore other)
        {
            if (other is null) return false;
            return this.Score == other.Score; // или другое правило сравнения
        }
        public override string ToString()
        {
            return $"{PlayerName}: {Score} (Уровень: {Difficulty}) - {Date:dd.MM.yyyy}";
        }
    }
}