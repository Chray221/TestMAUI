using System;
using SQLite;

namespace TestMAUI.Data
{
	[Table("entities")]
	public class Dictionary
	{
        [Column("word")] public string Word { get;}
        [Column("wordtype")] public string WordType { get;}
        [Column("defination")] public string Defination { get; }

		public Dictionary()
		{

		}
    }
}

