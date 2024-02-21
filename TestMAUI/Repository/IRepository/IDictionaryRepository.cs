using System;
using TestMAUI.Data;

namespace TestMAUI.Repository.IRepository
{
	public interface IDictionaryRepository
	{
        Task<IEnumerable<Dictionary>> GetTextTwistWords();
    }
}

