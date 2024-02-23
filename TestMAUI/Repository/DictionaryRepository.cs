using System.Linq.Expressions;
using System.Text;
using SQLite;
using TestMAUI.Data;
using TestMAUI.Helpers;
using TestMAUI.Helpers.Extensions;
using TestMAUI.Repository.IRepository;

namespace TestMAUI.Repository
{
    public class DictionaryRepository : IDictionaryRepository
    {
        SQLiteAsyncConnection Database;

        AsyncTableQuery<Dictionary> Dictionary => Database.Table<Dictionary>();


        public DictionaryRepository()
		{
        }

        async Task Init()
        {
            if (Database is not null) return;

            SQLiteExteionsions.CopyEmbeddedDatabase(
                MyConstants.DictionaryResourceName,
                MyConstants.DictionaryDBPath);
            Database = new SQLiteAsyncConnection(MyConstants.DictionaryDBPath, MyConstants.Flags);
            await Database.EnableWriteAheadLoggingAsync();
            //SQLitePCL.raw.sqlite3_create_function(con.handle, "regexp", 2, null, matchregex);
        }

        //private void matchregex(sqlite3_context ctx, object user_data, sqlite3_value[] args)
        //{
        //    bool ismatched = System.Text.RegularExpressions.Regex.IsMatch(
        //        SQLitePCL.raw.sqlite3_value_text(args[1]).utf8_to_string(),
        //        SQLitePCL.raw.sqlite3_value_text(args[0]).utf8_to_string(),
        //        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        //    if (ismatched)
        //        SQLitePCL.raw.sqlite3_result_int(ctx, 1);
        //    else
        //        SQLitePCL.raw.sqlite3_result_int(ctx, 0);
        //}

        public async Task<IEnumerable<Dictionary>> GetTextTwistWordsAsync()
        {
            await Init();

            //    //(SELECT COUNT(*) FROM entries WHERE length(word) = 6 AND word NOT REGEXP "^[-`']")
            //    Expression<Func<Dictionary, bool>> findWord = entity =>
            //        entity.Word.Length == 6 &&
            //        !entity.Word.StartsWith('-') &&
            //        !entity.Word.StartsWith('`') &&
            //        !entity.Word.StartsWith('\'');

            //    int maxWords = await Database.Table<Dictionary>().CountAsync(findWord);

            //GENERATE_WORDS:
            //    //Dictionary sixLetterWordDict = await Dictionary.FirstOrDefaultAsync(findWord);
            //    Dictionary sixLetterWordDict = await Dictionary
            //        .Take(RandomizerHelper.GetInt(0, maxWords))
            //        .FirstOrDefaultAsync(findWord);

            //    string word = sixLetterWordDict.Word;

            //    //WHERE  LOWER('Gentes') LIKE "%"||LOWER(word)  AND length(word) >= 3 AND length(word) <= 6
            //    List<Dictionary> dictionaries = await Database.Table<Dictionary>().Where(entity =>
            //        word.ToLower().Contains(entity.Word.ToLower()) &&
            //        word.Length >= 3 && word.Length <= 6).ToListAsync();

            //string columns = "word, wordtype, definition";
            string columns = "*";
            string excluddedWordCond =
                    "word NOT LIKE '-' " +
                "AND word NOT LIKE '`' " +
                "AND word NOT LIKE \"'\"";
            string findWordCond = $"length(word) = 6 AND {excluddedWordCond}";
            int maxWords = await Database.ExecuteScalarAsync<int>(
                $"SELECT COUNT(*) " +
                $"FROM entries " +
                $"WHERE {findWordCond}");
        GENERATE_WORDS:
            Dictionary? sixLetterWordDict = (await Database.QueryAsync<Dictionary>(
                @$"SELECT {columns}
                       FROM entries
                       WHERE {findWordCond}
                       LIMIT 1
                       OFFSET {RandomizerHelper.GetInt(0, maxWords)}")).FirstOrDefault();

            if(sixLetterWordDict is null) goto GENERATE_WORDS;

            string word = sixLetterWordDict.Word;
            //string comparer = string.Join(" AND ", word.ToLower().Distinct()
            //                                           .Select(c => $"LOWER(word) LIKE (\"%{c}%\")"));
            //IEnumerable<char> distinctWord = word.ToLower().Distinct();
            //string comparer = "";
            //for(int count = 2; count < word.Length; count++)
            //{
            //    comparer += $"({string.Join(" AND ",
            //                                distinctWord
            //                                    .Take(count)
            //                                    .Select(c => $"LOWER(word) LIKE (\"%{c}%\")"))})"
            //                + (count < word.Length - 1 ? comparer += "\n OR " : "");
            //}
            System.Diagnostics.Debug.WriteLine(
                @$"SELECT upper(word) AS WORD, wordtype, definition
                   FROM entries
                   WHERE {ReplaceString(word)}
	                     AND length(word) >= 3 
	                     AND length(word) <= 6	
						 AND (word NOT glob '[-`'']*')
                   GROUP BY word");
            //List<Dictionary> dictionaries = await Database.QueryAsync<Dictionary>(
            //    @$"SELECT upper(word) AS WORD, wordtype, definition
            //       FROM entries
            //       WHERE ( LOWER(""{word}"")  LIKE (""%"" || LOWER(word) || ""%"" )  
            //            OR ({comparer}))
            //          AND length(word) >= 3 
            //          AND length(word) <= 6	
            //       GROUP BY word");
            List<Dictionary> dictionaries = await Database.QueryAsync<Dictionary>(
                @$"SELECT upper(word) AS WORD, wordtype, definition
                   FROM entries
                   WHERE {ReplaceString(word)}
	                     AND length(word) >= 3 
	                     AND length(word) <= 6	
						 AND (word NOT glob '[-`'']*')
                   GROUP BY word");
            dictionaries.Add(sixLetterWordDict);


            if (dictionaries.Count < 6)
                goto GENERATE_WORDS;

            return dictionaries;
        }

        public static string ReplaceString(string chars)
        {
            StringBuilder replaceCount = new ();
            StringBuilder newChars = new ();
            var charsLookUp = chars.ToLookup(c => c);
            foreach (var look in charsLookUp)
            {
                replaceCount.Append($"AND  LOWER(word) -  LENGTH(REPLACE(LOWER(word),'{look.Key}','')) <= {look.Count()}\n");
                newChars.Append(look.Key);
            }
            return ReplaceString(newChars.Length - 1, newChars.ToString()) + " = ''\n" + replaceCount.ToString();
        }

        public static string ReplaceString(int totalCount, string chars)
        {
            char currentChar = chars[totalCount];
            if (totalCount == 0)
                return $"REPLACE(LOWER(word),'{currentChar}','')";
            return $"REPLACE({ReplaceString(--totalCount, chars)},'{currentChar}','')";
        }
    }
}

