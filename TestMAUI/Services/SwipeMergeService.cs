using System;
using TestMAUI.CustomViews.SwipeMerge;
using TestMAUI.Models;
using TestMAUI.Services.Interfaces;

namespace TestMAUI.Services
{
	public class SwipeMergeService : ISwipeMergeService
	{
		static SwipeMergeService _instance;
        public static SwipeMergeService Instance { get { return _instance ??= new SwipeMergeService(); } }

        SwipeMergeView _swipeMergeView;
        private SwipeMergeService()
		{
		}

		public void SetService(SwipeMergeView swipeMergeView)
		{
			_swipeMergeView = swipeMergeView;
        }

		public void Reset()
		{
			_swipeMergeView?.Reset();
        }

		public bool IsPlaying() => _swipeMergeView?.IsPlaying() ?? false;

		public Task SwipeAsync(SwipeDirection direction) => _swipeMergeView?.SwipeToAsync(direction);

        public GameStatus GameStatus => _swipeMergeView?.GameStatus ?? GameStatus.GameOver;
		public int Score => _swipeMergeView?.GetScore() ?? 0;
        
	}
}

