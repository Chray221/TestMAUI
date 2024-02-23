//using System.ComponentModel.DataAnnotations.Schema;
using SQLite;

namespace TestMAUI.Data
{
	[Table("entries")]
	public class Dictionary
	{
        [Column("word"), NotNull, Collation("NOCASE")] public string Word { get; set; }
        [Column("wordtype"), NotNull, Collation("NOCASE")] public string WordType { get; set; }
        [Column("definition"), NotNull, Collation("NOCASE")] public string Definition { get; set; }

        public Dictionary()
		{

		}

        public Dictionary(string word, string wordType, string definition)
        {
            Word = word;
            WordType = wordType;
            Definition = definition;
        }
    }
}

