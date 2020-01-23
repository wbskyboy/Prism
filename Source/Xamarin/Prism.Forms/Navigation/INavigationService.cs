using System;
using System.Threading.Tasks;

namespace Prism.Navigation
{
    /// <summary>
    /// Provides page based navigation for ViewModels.
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Navigates to the most recent entry in the back navigation history by popping the calling Page off the navigation stack.
        /// </summary>
        /// <returns><see cref="INavigationResult" />. If the Success parameter is <c>true</c>, then the Page successfully was popped. Otherwise you may have an Exception in the result.</returns>
        Task<INavigationResult> GoBackAsync();

        /// <summary>
        /// Navigates to the most recent entry in the back navigation history by popping the calling Page off the navigation stack.
        /// </summary>
        /// <param name="parameters">The navigation parameters</param>
        /// <returns><see cref="INavigationResult" />. If the Success parameter is <c>true</c>, then the Page successfully was popped. Otherwise you may have an Exception in the result.</returns>
        Task<INavigationResult> GoBackAsync(INavigationParameters parameters);

        /// <summary>
        /// Navigates to the most recent entry in the back navigation history by popping the calling Page off the navigation stack.
        /// </summary>
        /// <param name="parameters">The navigation parameters</param>
        /// <param name="useModalNavigation">If set to <c>true</c>, or <c>false</c>, it will explicitly control whether or not to use Modal Navigation. Otherwise Prism will determine the proper context.</param>
        /// <param name="animated">If <c>true</c>, animations will be enabled for the pop.</param>
        /// <returns><see cref="INavigationResult" />. If the Success parameter is <c>true</c>, then the Page successfully was popped. Otherwise you may have an Exception in the result.</returns>
        /// <remarks>You shouldn't need to use this. If you have to explicitly set <paramref name="useModalNavigation"/>, please file a bug.</remarks>
        Task<INavigationResult> GoBackAsync(INavigationParameters parameters, bool? useModalNavigation, bool animated);

        /// <summary>
        /// Pops back to the root of the current context.
        /// </summary>
        /// <param name="parameters">The navigation parameters</param>
        /// <returns><see cref="INavigationResult" />. If the Success parameter is <c>true</c>, then the Page successfully was popped. Otherwise you may have an Exception in the result.</returns>
        Task<INavigationResult> GoBackToRootAsync(INavigationParameters parameters);

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The Uri to navigate to</param>
        /// <returns><see cref="INavigationResult" />. If the Success parameter is <c>true</c>, then the Page successfully was popped. Otherwise you may have an Exception in the result.</returns>
        /// <remarks>Navigation parameters can be provided in the Uri.</remarks>
        /// <example>
        /// Navigate(new Uri("MainPage?id=3&amp;name=brian", UriKind.RelativeSource), parameters);
        /// </example>
        Task<INavigationResult> NavigateAsync(Uri uri);

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The Uri to navigate to</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <returns><see cref="INavigationResult" />. If the Success parameter is <c>true</c>, then the Page successfully was popped. Otherwise you may have an Exception in the result.</returns>
        /// <remarks>Navigation parameters can be provided in the Uri and by using the <paramref name="parameters"/>.</remarks>
        /// <example>
        /// Navigate(new Uri("MainPage?id=3&amp;name=brian", UriKind.RelativeSource), parameters);
        /// </example>
        Task<INavigationResult> NavigateAsync(Uri uri, INavigationParameters parameters);

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="name"/>.
        /// </summary>
        /// <returns><see cref="INavigationResult" />. If the Success parameter is <c>true</c>, then the Page successfully was popped. Otherwise you may have an Exception in the result.</returns>
        /// <param name="name">The name of the target to navigate to.</param>
        Task<INavigationResult> NavigateAsync(string name);

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the target to navigate to.</param>
        /// <param name="parameters">The navigation parameters</param>
        Task<INavigationResult> NavigateAsync(string name, INavigationParameters parameters);

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the target to navigate to.</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <param name="useModalNavigation">If set to <c>true</c>, or <c>false</c>, it will explicitly control whether or not to use Modal Navigation. Otherwise Prism will determine the proper context.</param>
        /// <param name="animated">If <c>true</c>, animations will be enabled for the pop.</param>
        /// <returns><see cref="INavigationResult" />. If the Success parameter is <c>true</c>, then the Page successfully was popped. Otherwise you may have an Exception in the result.</returns>
        Task<INavigationResult> NavigateAsync(string name, INavigationParameters parameters, bool? useModalNavigation, bool animated);

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The Uri to navigate to</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <param name="useModalNavigation">If set to <c>true</c>, or <c>false</c>, it will explicitly control whether or not to use Modal Navigation. Otherwise Prism will determine the proper context.</param>
        /// <param name="animated">If <c>true</c>, animations will be enabled for the pop.</param>
        /// <returns><see cref="INavigationResult" />. If the Success parameter is <c>true</c>, then the Page successfully was popped. Otherwise you may have an Exception in the result.</returns>
        Task<INavigationResult> NavigateAsync(Uri uri, INavigationParameters parameters, bool? useModalNavigation, bool animated);
    }
}
