using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Prism.Common;
using Prism.Ioc;
using Prism.Behaviors;
using System.Linq;

namespace Prism.Navigation
{
    /// <summary>
    /// Prism's <see cref="INavigationService"/> for <see cref="ShellNavigationService"/>
    /// </summary>
    public partial class ShellPrismNavigationService : INavigationService
    {
        private readonly IContainerExtension _container;
        private IPageBehaviorFactory _pageBehaviorFactory { get; }

        /// <summary>
        /// Creates an instance of <see cref="ShellPrismNavigationService"/>
        /// </summary>
        /// <param name="containerExtension"></param>
        /// <param name="pageBehaviorFactory"></param>
        public ShellPrismNavigationService(
            IContainerExtension containerExtension,
            IPageBehaviorFactory pageBehaviorFactory)
        {
            _container = containerExtension;
            _pageBehaviorFactory = pageBehaviorFactory;
        }

        protected virtual Page CreatePage(string segmentName)
        {
            try
            {
                return _container.Resolve<object>(segmentName) as Page;
            }
            catch (Exception ex)
            {
                if (((IContainerRegistry)_container).IsRegistered<object>(segmentName))
                    throw new NavigationException(NavigationException.ErrorCreatingPage, null, ex);

                throw new NavigationException(NavigationException.NoPageIsRegistered, null, ex);
            }
        }

        protected virtual Page CreatePageFromSegment(string segment)
        {
            string segmentName = null;
            try
            {
                segmentName = UriParsingHelper.GetSegmentName(segment);
                var page = CreatePage(segmentName);
                if (page == null)
                {
                    var innerException = new NullReferenceException(string.Format("{0} could not be created. Please make sure you have registered {0} for navigation.", segmentName));
                    throw new NavigationException(NavigationException.NoPageIsRegistered, null, innerException);
                }

                PageUtilities.SetAutowireViewModelOnPage(page);
                _pageBehaviorFactory.ApplyPageBehaviors(page);

                // Not Relavent for Shell since we only work with Content Page and not Tabbed or Carousel Pages
                //ConfigurePages(page, segment);

                return page;
            }
            catch (NavigationException)
            {
                throw;
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e);
                System.Diagnostics.Debugger.Break();
#endif
                throw;
            }
        }

        Task<INavigationResult> INavigationService.GoBackAsync()
        {
            throw new NotImplementedException();
        }

        Task<INavigationResult> INavigationService.GoBackAsync(INavigationParameters parameters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The Uri to navigate to</param>
        /// <remarks>Navigation parameters can be provided in the Uri and by using the <paramref name="parameters"/>.</remarks>
        /// <example>
        /// Navigate(new Uri("MainPage?id=3&amp;name=dan", UriKind.RelativeSource), parameters);
        /// </example>
        /// <returns>The <see cref="INavigationResult" /> which will provide a Success == <c>true</c> if the Navigation was successful.</returns>
        public virtual Task<INavigationResult> NavigateAsync(Uri uri) =>
            NavigateAsync(uri, null);

        INavigationParameters _currentParameters;
        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The Uri to navigate to</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <remarks>Navigation parameters can be provided in the Uri and by using the <paramref name="parameters"/>.</remarks>
        /// <example>
        /// Navigate(new Uri("MainPage?id=3&amp;name=dan", UriKind.RelativeSource), parameters);
        /// </example>
        /// <returns>The <see cref="INavigationResult" /> which will provide a Success == <c>true</c> if the Navigation was successful.</returns>
        public virtual async Task<INavigationResult> NavigateAsync(Uri uri, INavigationParameters parameters)
        {
            try
            {
                _currentParameters = parameters;
                await Shell.Current.GoToAsync(uri);
                return new NavigationResult()
                {
                    Success = true
                };
            }
            catch (NavigationException navEx)
            {
                return new NavigationResult
                {
                    Success = false,
                    Exception = navEx
                };
            }
            catch (Exception ex)
            {
                return new NavigationResult
                {
                    Success = false,
                    Exception = new NavigationException(NavigationException.UnknownException, null, ex)
                };
            }
            
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the target to navigate to.</param>
        /// <returns>The <see cref="INavigationResult" /> which will provide a Success == <c>true</c> if the Navigation was successful.</returns>
        public virtual Task<INavigationResult> NavigateAsync(string name) =>
            NavigateAsync(name, null);

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the target to navigate to.</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <returns>The <see cref="INavigationResult" /> which will provide a Success == <c>true</c> if the Navigation was successful.</returns>
        public virtual Task<INavigationResult> NavigateAsync(string name, INavigationParameters parameters) =>
            NavigateAsync(UriParsingHelper.Parse(name), parameters);
    }
}
