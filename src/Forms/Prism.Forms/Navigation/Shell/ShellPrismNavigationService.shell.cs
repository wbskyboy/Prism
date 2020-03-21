using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Prism.AppModel;
using Prism.Common;
using Prism.Ioc;
using Xamarin.Forms;

namespace Prism.Navigation
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class PathPartExtensions
    {
        // TODO: Remove this as it should be built into Xamarin.Forms
        public static PathPart GetPrevious(this RoutePath routePath)
        {
            var index = routePath.PathParts.Count - 2;
            if(index > 0)
            {
                return routePath.PathParts.ElementAt(index);
            }

            return default;
        }
    }

    public partial class ShellPrismNavigationService : ShellNavigationService
    {
        internal const string RemovePageRelativePath = "../";

        private ShellRouteState _previousRoutePath;

        public override Task NavigatedToAsync(ShellNavigationArgs args)
        {
            var previouslyActivePathPart = _previousRoutePath.CurrentRoute.GetCurrent();

            if (previouslyActivePathPart.ShellPart is ShellContent previousShellContent)
            {
                PageUtilities.OnNavigatedFrom(previousShellContent.Content, _currentParameters);
            }

            PageUtilities.OnNavigatedTo(args.FutureState.CurrentRoute.GetCurrent(), _currentParameters);
            return Task.CompletedTask;
        }

        public sealed override async Task<ShellRouteState> NavigatingToAsync(ShellNavigationArgs args)
        {
            _previousRoutePath = args.Shell.RouteState;

            // args.Shell.RouteState => args.FutureState
            // Destroying logic
            //var diff = _previousRoutePath.CurrentRoute.PathParts.Count - args.FutureState.CurrentRoute.PathParts.Count;
            //if (diff > 0 && _previousRoutePath.CurrentRoute.PathParts[1] == args.FutureState.CurrentRoute.PathParts[1])
            //{

            //    foreach(var destroying in previouslyActivePathPart.PathParts.Take(diff))
            //    {

            //    }
            //} 


            if (_previousRoutePath.CurrentRoute.PathParts[1] == args.FutureState.CurrentRoute.PathParts[1])
            {
                // TODO: Compare Current and Future Route States

                //ViewA
                //ViewA/ViewB/ViewC

                // calling navigatedto on every path that was added during this navigation phase
                for (int i = 0; i < args.FutureState.CurrentRoute.PathParts.Count; i++)
                {
                    bool shouldDo = false;
                    if(i > _previousRoutePath.CurrentRoute.PathParts.Count)
                    {
                        shouldDo = true;
                    }
                    else if(args.FutureState.CurrentRoute.PathParts[i] != _previousRoutePath.CurrentRoute.PathParts[i])
                    {
                        shouldDo = true;
                    }

                    if(shouldDo)
                    {
                        //call prism navigating code on shellContent
                    }
                }
            }

            var foo = args.FutureState.CurrentRoute.PathParts.LastOrDefault();
            var pathPart = args.FutureState.CurrentRoute.GetCurrent();

            // For reference only...
            //if (pathPart.ShellPart is ShellContent shellContent)
            //{
            //    var page = CreatePageFromSegment(shellContent.Route);
            //    shellContent.Content = page;
            //    await PageUtilities.OnInitializedAsync(page,
            //       _currentParameters
            //        );
            //}

            return args.FutureState;
        }

        // Resolve... Set ViewModelLocator... Apply Page Behaviors
        public sealed override Page Create(ShellContentCreateArgs args)
        {
            return CreatePageFromSegment(args.Content.Route);
        }

        // Prism OnInitialized
        public sealed override async Task ApplyParametersAsync(ShellLifecycleArgs args)
        {
            await base.ApplyParametersAsync(args);
            if(!(args.Element is ShellContent shellContent) || !(args.PathPart is PrismPathPart pathPart))
            {
                return;
            }

            await PageUtilities.OnInitializedAsync(shellContent.Content, pathPart.Parameters);
        }

        // First... validates URI
        public sealed override Task<ShellRouteState> ParseAsync(ShellUriParserArgs args)
        {
            ShellRouteState newState = args.Shell.RouteState;

            //var newState = RebuildCurrentState(args.Shell.CurrentState.Location, args.Shell.Items);
            //var currentUri = args.Shell.CurrentState.Location;
            var navigationSegments = UriParsingHelper.GetUriSegments(args.Uri);
            //IEnumerable<ShellItem> items = args.Shell.Items;
            ShellItem rootShellItem = null;
            foreach(var segment in navigationSegments)
            {
                ProcessNavigationSegment(segment, newState, _currentParameters, args.Shell, ref rootShellItem);
            }

            return Task.FromResult(newState);
        }

        private static void ProcessNavigationSegment(string navigationSegment, ShellRouteState newState, INavigationParameters currentParameters, Shell shell, ref ShellItem rootShellItem)
        {
            var segmentName = UriParsingHelper.GetSegmentName(navigationSegment);

            // ../ViewA
            if (segmentName == RemovePageRelativePath && newState.CurrentRoute.PathParts.Count >= 3)
            {
                var currentRoutes = newState.CurrentRoute.PathParts.ToList();
                currentRoutes.RemoveAt(newState.CurrentRoute.PathParts.Count - 1);
                newState.CurrentRoute.PathParts = currentRoutes;
                //return null;
            }
            else
            {
                // TODO: Test these Prism type URI strings
                //ViewA?id=5&grapes=purple/ViewB?id=3
                //ViewA?id=5&id=4&id=2&id=1&grapes=purple
                // navigationService.NavigateAsync("ViewA?id=5&grapes=purple/ViewB?id=3", new NavigationParameters { { "foo", new { Bar = 1 } } });
                //new NavigationParameters { { "foo", new { Bar = 1 } } };

                //// ViewA
                //new NavigationParameters
                //{
                //    { "foo", new { Bar = 1} },
                //    { "id", 5 },
                //    { "grapes", "purple" }
                //};

                //// ViewB
                //new NavigationParameters
                //{
                //    { "foo", new { Bar = 1} },
                //    { "id", 3 }
                //};

                // ViewA?id=5

                BaseShellItem currentItem = rootShellItem is null ?
                    GetShellItem(shell.Items, segmentName) :
                    GetShellItem(rootShellItem.Items, segmentName);

                if (currentItem is null)
                    return;
                else if (currentItem is ShellItem shellItem)
                    rootShellItem = shellItem;

                var navigationParameters = UriParsingHelper.GetSegmentParameters(navigationSegment, currentParameters);

                newState.Add(new PrismPathPart(currentItem, navigationParameters));
            }
        }

        private static BaseShellItem GetShellItem(IList<ShellContent> items, string name)
        {
            var shellItem = items.FirstOrDefault(x => !IsImplicitRoute(x) && UriParsingHelper.GetSegmentName(x.Route) == name);
            if (shellItem != null)
                return shellItem;

            return null;
        }

        // TODO: Determine why routes are coming back with as TabBar4 instead of IMPL_TabBar4
        private static bool IsImplicitRoute(BaseShellItem item) =>
            item.Route.StartsWith("IMPL_");

        private static BaseShellItem GetShellItem(IEnumerable<ShellItem> items, string name)
        {
            var shellItem = items.FirstOrDefault(x => !IsImplicitRoute(x) && UriParsingHelper.GetSegmentName(x.Route) == name);
            if (shellItem != null)
                return shellItem;

            var implicitItems = items.Where(x => IsImplicitRoute(x));
            foreach (var implicitItem in implicitItems)
            {
                var item = GetShellItem(implicitItem.Items, name);
                if (item != null)
                    return item;
            }

            return null;
        }

        private static BaseShellItem GetShellItem(IEnumerable<ShellSection> items, string name)
        {
            var shellItem = items.FirstOrDefault(x => !IsImplicitRoute(x) && UriParsingHelper.GetSegmentName(x.Route) == name);
            if (shellItem != null)
                return shellItem;

            var implicitItems = items.Where(x => IsImplicitRoute(x));
            foreach (var implicitItem in implicitItems)
            {
                var item = GetShellItem(implicitItem.Items, name);
                if (item != null)
                    return item;
            }

            return null;
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public class PrismPathPart : PathPart
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public PrismPathPart(BaseShellItem baseShellItem, INavigationParameters parameters)
            : base(baseShellItem, new Dictionary<string, string>())
        {
            Parameters = parameters;
        }

        public INavigationParameters Parameters { get; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
