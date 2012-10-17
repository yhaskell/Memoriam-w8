using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.System.Threading;
using System.Threading.Tasks;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Memoriam
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class ChangeDifficulty : Memoriam.Common.LayoutAwarePage
    {
        public ChangeDifficulty()
        {
            this.InitializeComponent();
            
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values["level"] = 0;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        bool Do = false;

        private void LevelsView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SaveGoBack();
        }

        private void Levels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SaveGoBack();
        }

        async void SaveGoBack()
        {
            if (!Do) return;

            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            

            roamingSettings.Values["level"] = LevelsView.SelectedIndex;            
            
            Task.Delay(250).ContinueWith(new Action<Task>((x)=>this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, ()=>Frame.GoBack())));

            roamingSettings.Values["first"] = true;

            Do = false;
        }

        private void ChangeDifficulty_Loaded(object sender, RoutedEventArgs e)
        {
            LevelsView.ItemsSource = Levels.Items;

            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;

            if (roamingSettings.Values.ContainsKey("level")) LevelsView.SelectedIndex = (int)roamingSettings.Values["level"];
            else LevelsView.SelectedIndex = 0;

            Do = true;
        }

        private void ResetSettings_Tapped(object sender, RoutedEventArgs e)
        {
            var rs = Windows.Storage.ApplicationData.Current.RoamingSettings.Values;
            rs.Clear();
        }
    }

    public class Level
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class Levels
    {
        static IEnumerable<Level> levels;
        private static readonly object locker = new object();
        public static IEnumerable<Level> Items
        {
            get
            {
                if (levels != null) return levels;
                Monitor.Enter(locker);

                var t = new List<Level>();

                t.Add(new Level { Name = "Novice", Description = "Levels with 3-7 circles (1-7 or A-G)" });
                t.Add(new Level { Name = "Beginner", Description = "8-12 circles (1-12 or A-L)" });
                t.Add(new Level { Name = "Intermediate", Description = "Up to 16 circles (1-16 or A-P)" });
                t.Add(new Level { Name = "Advanced", Description = "3-10 circles (unordered numbers or letters)" });
                t.Add(new Level { Name = "Expert", Description = "11-16 circles (likewize)" });
                t.Add(new Level { Name = "Insane", Description = "Everything possible" });
                t.Add(new Level { Name = "Jedy", Description = "Everything possible, and even more" });

                Interlocked.Exchange(ref levels, t);
                Monitor.Exit(locker);

                return levels;
            }
        }

    }
}
