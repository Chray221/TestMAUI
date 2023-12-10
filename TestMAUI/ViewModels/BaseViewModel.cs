using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using TestMAUI.Services.Interfaces;

namespace TestMAUI.ViewModels
{
    public class BaseViewModel : ObservableObject, IPageLifeCycle
	{
		public readonly ILogger _logger;

		public BaseViewModel(ILogger logger)
		{
            _logger = logger;
        }

        #region Logger
        public void Log(string msg, [CallerMemberName] string memberName = "")
        {
            //msg = string.Format("", );
#if DEBUG
            //System.Diagnostics.Debug.WriteLine(msg);
            _logger.LogInformation("{DateTime:HH:mm:ss:ff tt} [{MethodName}]: {Message}", DateTime.Now, memberName, msg);
            //#elif RELEASE
            //            msg = string.Format("{0:HH:mm:ss:ff tt} [{1}]-[{2}]: {3}",DateTime.Now,AppNamespace,memberName,msg);
            //            Console.WriteLine(msg);
#else
            msg = string.Format("{0:HH:mm:ss:ff tt} [{1}]-[{2}]: {3}",DateTime.Now,AppNamespace,memberName,msg);
            Console.WriteLine(msg);
#endif
        }

        public void LogException(Exception exception,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            switch (exception)
            {
                // for Prism
                //case ContainerResolutionException crException:
                //    HandleContainerException(crException, memberName, filePath, lineNumber);
                //    break;
                //case NavigationException ne when ne.InnerException is ContainerResolutionException cre:
                //    HandleContainerException(cre, memberName, filePath, lineNumber);
                //    break;
                //case NavigationException ne:
                //    //HandleNavigationException(ne);
                //    App.LogException(ne, memberName, filePath, lineNumber);
                //    break;
                default:
                    // Report a bug to the Prism Team
                    Log(string.Format("TITLE:{0} \n\tMESSAGE: {1} \n\tSTACKTRACE: {2} \n\tFROM:{3} \n\tLINE:{4}", exception.GetType(), exception.Message, exception.StackTrace, filePath, lineNumber),
                        string.Format("Exception]-[{0}", memberName));
                    _logger.LogWarning(exception, "From ({MethodName})", memberName);
                    break;
            }
        }
        #endregion

        #region LifeCycle
        public virtual void OnNavigatingTo()
        {

        }

        public virtual void OnNavigatedTo()
        {

        }

        public virtual void OnNavigatingFrom()
        {

        }

        public virtual void OnNavigatedFrom()
        {

        }

        public virtual void OnDisappearing()
        {

        }
        #endregion

        #region Navigation
        public static Task NavigateToAsync<TPage>(params KeyValuePair<string, object>[] parameters)
        {
            return NavigateToAsync(typeof(TPage).Name, parameters);
        }

        public static Task NavigateToAsync(string page, params KeyValuePair<string, object>[] parameters)
        {
            return Shell.Current.GoToAsync(page, parameters?.ToDictionary(p => p.Key, p => p.Value));
        }

        public static Task GoBackAsync(params KeyValuePair<string, object>[] parameters)
        {
            return NavigateToAsync("..", parameters);
        }

        public static Task<bool> DisplayAlertAsync(string title, string message, string acceptButton = "Okay", string cancelButton = "Cancel")
        {
            return Shell.Current.DisplayAlert(title, message, acceptButton, cancelButton);
        }

        public static Task DisplayAlertAsync(string title, string message, string cancelButton = "Cancel")
        {
            return Shell.Current.DisplayAlert(title, message, cancelButton);
        }
        #endregion
    }
}

