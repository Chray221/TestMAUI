using System;
using System.Linq.Expressions;
using Android.Content;
using SQLite;
using SQLitePCL;
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

        public async Task<IEnumerable<Dictionary>> GetTextTwistWords()
        {
            await Init();

            //(SELECT COUNT(*) FROM entries WHERE length(word) = 6 AND word NOT REGEXP "^[-`']")
            Expression<Func<Dictionary, bool>> findWord = entity =>
                entity.Word.Length == 6 &&
                !entity.Word.StartsWith("-") &&
                !entity.Word.StartsWith("`") &&
                !entity.Word.StartsWith("'");
            int maxWords = await Dictionary.CountAsync(findWord);
            
        GENERATE_WORDS:
            //Dictionary sixLetterWordDict = await Dictionary.FirstOrDefaultAsync(findWord);
            Dictionary sixLetterWordDict = await Dictionary
                .Take(RandomizerHelper.GetInt(0, maxWords))
                .FirstOrDefaultAsync(findWord);
            string word = sixLetterWordDict.Word;

            //WHERE  LOWER('Gentes' )LIKE "%"||LOWER(word)  AND length(word) >= 3 AND length(word) <= 6
            List <Dictionary> dictionaries = await Dictionary.Where(entity =>
                word.ToLower().Contains(entity.Word.ToLower()) &&
                word.Length >= 3 && word.Length <= 6).ToListAsync();
            if (dictionaries.Count < 6)
                goto GENERATE_WORDS;

            return dictionaries;
        }
    }
}

