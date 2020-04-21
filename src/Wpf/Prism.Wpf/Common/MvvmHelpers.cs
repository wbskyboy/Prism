using System;
using System.Collections.Generic;
using System.Linq;

#if HAS_WINUI
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

namespace Prism.Common
{
    public static class MvvmHelpers
    {
        public static void ViewAndViewModelAction<T>(object view, Action<T> action) where T : class
        {
            if (view is T viewAsT)
                action(viewAsT);

            if (view is FrameworkElement element && element.DataContext is T viewModelAsT)
            {
                action(viewModelAsT);
            }
        }

        public static T GetImplementerFromViewOrViewModel<T>(object view) where T : class
        {
            if (view is T viewAsT)
            {
                return viewAsT;
            }

            if (view is FrameworkElement element && element.DataContext is T vmAsT)
            {
                return vmAsT;
            }

            return null;
        }
    }
}
