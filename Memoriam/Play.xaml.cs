using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Memoriam
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class Play : Memoriam.Common.LayoutAwarePage
    {
        public Play()
        {
            this.InitializeComponent();
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

        PlayStyle playStyle { get; set; }
        int[] GameTable { get; set; }
        int[] Lives { get; set; }
        int Count { get; set; }
        bool LastFail { get; set; }
        Random seed = new Random();
        bool Cleaned { get; set; }
        int Opened { get; set; }
        string[] Tags { get; set; }
        int Difficulty;
        int m_currLives, m_currLevel, m_Levels;
        int CurrentLives { get { return m_currLives; } set { m_currLives = value; LivesView.Text = value.ToString(); } }
        int CurrentLevel { get { return m_currLevel; } set { m_currLevel = value; LevelView.Text = (1 + value).ToString(); ApplicationData.Current.RoamingSettings.Values[(playStyle == PlayStyle.GeneralRun ? "g" : "t") + Difficulty.ToString()] = value; } }
        int OverallLevels { get { return m_Levels; } set { m_Levels = value; LevelsView.Text = value.ToString(); } }

        private void SelectPlayStyle()
        {
            if (!ApplicationData.Current.RoamingSettings.Values.ContainsKey("style")) { Frame.GoBack(); return; }
            playStyle = ((string) ApplicationData.Current.RoamingSettings.Values["style"]) == "g" ? PlayStyle.GeneralRun : PlayStyle.TimedRun;
            ReadyTip.Visibility = (playStyle == PlayStyle.GeneralRun) ? Visibility.Visible : Visibility.Collapsed;
        }

        private bool InitializeLevelSequence()
        {
            int i = 0;           

            Difficulty = (int)ApplicationData.Current.RoamingSettings.Values["level"];

            switch (Difficulty)
            {
                case 0:

                    GameTable = new int[OverallLevels = 13];
                    GameTable[0] = 3;
                    GameTable[1] = 4;
                    GameTable[2] = GameTable[3] = 5;
                    GameTable[4] = GameTable[5] = GameTable[6] = GameTable[7] = 6;
                    GameTable[8] = GameTable[9] = GameTable[10] = GameTable[11] = GameTable[12] = 7;

                    Lives = new int[13];

                    Lives[0] = Lives[1] = 1;
                    Lives[2] = Lives[3] = Lives[4] = Lives[5] = 2;
                    for (i = 6; i < 13; i++) Lives[i] = 3;

                    return true;
                case 1:
                    GameTable = new int[OverallLevels = 40];
                    Lives = new int[40];
                    for (i = 0; i < 5; i++) { GameTable[i] = 8; Lives[i] = 3; }
                    for (i = 0; i < 7; i++) { GameTable[5 + i] = 9; Lives[5 + i] = 3; }
                    for (i = 0; i < 7; i++) { GameTable[12 + i] = 10; Lives[12 + i] = 3; }
                    for (i = 0; i < 9; i++) { GameTable[19 + i] = 11; Lives[19 + i] = 4; }
                    for (i = 0; i < 12; i++) { GameTable[28 + i] = 12; Lives[28 + i] = 4; }
                    return true;
                case 2:
                    GameTable = new int[OverallLevels = 69];
                    Lives = new int[69];
                    for (i = 0; i < 10; i++) { GameTable[i] = 13; Lives[i] = 4; }
                    for (i = 0; i < 14; i++) { GameTable[10 + i] = 14; Lives[10 + i] = 5; }
                    for (i = 0; i < 20; i++) { GameTable[24 + i] = 15; Lives[24 + i] = 5; }
                    for (i = 0; i < 25; i++) { GameTable[44 + i] = 16; Lives[44 + i] = 6; }
                    return true;
                case 3:
                    GameTable = new int[OverallLevels = 98];
                    Lives = new int[98];
                    for (i = 0; i < 3; i++) { GameTable[i] = 3; Lives[i] = 1; }
                    for (i = 0; i < 5; i++) { GameTable[3 + i] = 4; Lives[3 + i] = 2; }
                    for (i = 0; i < 7; i++) { GameTable[8 + i] = 5; Lives[8 + i] = 2; }
                    for (i = 0; i < 11; i++) { GameTable[15 + i] = 6; Lives[15 + i] = 3; }
                    for (i = 0; i < 13; i++) { GameTable[26 + i] = 7; Lives[25 + i] = 3; }
                    for (i = 0; i < 17; i++) { GameTable[39 + i] = 8; Lives[39 + i] = 4; }
                    for (i = 0; i < 19; i++) { GameTable[56 + i] = 9; Lives[56 + i] = 4; }
                    for (i = 0; i < 23; i++) { GameTable[75 + i] = 10; Lives[75 + i] = 4; }
                    break;
                case 4:
                    GameTable = new int[OverallLevels = 192];
                    Lives = new int[192];
                    for (i = 0; i < 18; i++) { GameTable[i] = 11; Lives[i] = 4; }
                    for (i = 0; i < 21; i++) { GameTable[18 + i] = 12; Lives[18 + i] = 5; }
                    for (i = 0; i < 27; i++) { GameTable[39 + i] = 13; Lives[39 + i] = 5; }
                    for (i = 0; i < 33; i++) { GameTable[66 + i] = 14; Lives[66 + i] = 5; }
                    for (i = 0; i < 42; i++) { GameTable[99 + i] = 15; Lives[99 + i] = 6; }
                    for (i = 0; i < 51; i++) { GameTable[141 + i] = 16; Lives[141 + i] = 6; }
                    return true;
                case 5:
                    GameTable = new int[OverallLevels = 124];
                    Lives = new int[124];
                    for (i = 0; i < 3; i++) { GameTable[i] = 3; Lives[i] = 1; }
                    for (i = 0; i < 5; i++) { GameTable[3 + i] = 4; Lives[3 + i] = 1; }
                    for (i = 0; i < 7; i++) { GameTable[8 + i] = 5; Lives[8 + i] = 2; }
                    for (i = 0; i < 13; i++) { GameTable[15 + i] = 6; Lives[15 + i] = 2; }
                    for (i = 0; i < 17; i++) { GameTable[28 + i] = 7; Lives[28 + i] = 3; }
                    for (i = 0; i < 21; i++) { GameTable[45 + i] = 8; Lives[45 + i] = 4; }
                    for (i = 0; i < 25; i++) { GameTable[66 + i] = 9; Lives[66 + i] = 4; }
                    for (i = 0; i < 33; i++) { GameTable[91 + i] = 10; Lives[91 + i] = 5; }
                    break;
                case 6:
                    GameTable = new int[OverallLevels = 510];
                    Lives = new int[510];
                    for (i = 0; i < 39; i++) { GameTable[i] = 11; Lives[i] = 5; }
                    for (i = 0; i < 47; i++) { GameTable[39 + i] = 12; Lives[39 + i] = 6; }
                    for (i = 0; i < 64; i++) { GameTable[86 + i] = 13; Lives[86 + i] = 7; }
                    for (i = 0; i < 81; i++) { GameTable[150 + i] = 14; Lives[150 + i] = 7; }
                    for (i = 0; i < 109; i++) { GameTable[231 + i] = 15; Lives[231 + i] = 8; }
                    for (i = 0; i < 170; i++) { GameTable[340 + i] = 16; Lives[340 + i] = 8; }
                    return true;
                default:
                    Lives = GameTable = new int[OverallLevels = 1];
                    return true;
            }

            return true;
        }

        private async void InitializeLevel(int level)
        {
            var finished = await CheckEndReached();
            if (finished) return;
            level = CurrentLevel;
            Count = GameTable[level];
            CurrentLives = Lives[level];
            Cleaned = false;
            Opened = 0;

            var crc = Circle.GeneratePoints(Count, (int)ContentPanel.RenderSize.Width, (int) ContentPanel.RenderSize.Height);

            int num = 0;
            if (Difficulty < 5)
                GenerateTags(Count, seed.Next(0, 2) == 0 ? TagType.Digit : TagType.Letter, Difficulty > 2);
            else GenerateTags(Count, TagType.Any, Difficulty == 6);

            RefillSequence();
            UpdateSeqView();
            foreach (var bc in ContentPanel.Children)
                if (bc.GetType() == typeof(Button)) { ((Button)bc).Click -= PlayTapped_Click; }

            ContentPanel.Children.Clear();
            foreach (var c in crc)
            {
                var b = new Button();
                b.BorderThickness = new Thickness(9);
                b.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, (byte)seed.Next(160, 240), (byte)seed.Next(160, 240), (byte)seed.Next(160, 240)));
                b.FontFamily = new FontFamily("Georgia");
                b.FontWeight = FontWeights.SemiBold;
                b.FontSize = 60;
                b.SetValue(Canvas.LeftProperty, c.X - 60);
                b.SetValue(Canvas.TopProperty, c.Y - 60);
                b.Width = b.Height = 2 * c.Radius;
                b.Style = Application.Current.Resources["RoundButton"] as Style;
                b.Content = b.Tag = Tags[num++];
                b.Click += PlayTapped_Click;
                ContentPanel.Children.Add(b);
            }
            WaitForIt();
        }

        private void CleanAllTags()
        {
            ReadyTip.Visibility = Visibility.Collapsed;
            var cpc = ContentPanel.Children;

            for (int i = 0; i < cpc.Count; i++)
                if (cpc[i].GetType() == typeof(Button)) ((Button)cpc[i]).Content = null;
            Cleaned = true;
            Opened = 0;
        }
        private void WaitForIt()
        {
            if (playStyle == PlayStyle.TimedRun)
                Task.Delay(Count * 700 * diffM()).ContinueWith(new Action<Task>((x) => this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () => CleanAllTags())));
        }
        private bool Initialize()
        {
            SelectPlayStyle();
            Tutorial.Show(this.Frame);
            return InitializeLevelSequence();
        }

        private async void PlayTapped_Click(object sender, RoutedEventArgs e)
        {
            var curr = sender as Button;
            if (!Cleaned)
            {
                if ((string)curr.Tag == Tags[0]) CleanAllTags();
                else return;
            }

            if (curr.Tag == null) return;

            if (Tags[Opened] == (string)curr.Tag)
            {
                curr.Content = Tags[Opened++];
                curr.Tag = null;
                TrySuccess();
                UpdateSeqView();
            }
            else
            {
                CurrentLives--;
                await TryFail();
            }
        }

        private void TrySuccess()
        {
            if (Opened == Count)
                InitializeLevel(++CurrentLevel);
        }

        Brush BrightRedBrush = new SolidColorBrush(Color.FromArgb(255, 255, 140, 140));
        Brush WhiteBrush = new SolidColorBrush(Colors.White);

        private async Task TryFail()
        {
            if (CurrentLives < 0)
            {
                ContentPanel.Visibility = Visibility.Collapsed;
                FailTip.Visibility = Visibility.Visible;
                await Task.Delay(2000);
                FailTip.Visibility = Visibility.Collapsed;
                ContentPanel.Visibility = Visibility.Visible;
                if (LastFail == true) InitializeLevel(--CurrentLevel);
                else
                {
                    LastFail = true;
                    InitializeLevel(CurrentLevel);
                }
                
            }
            else            
            {
                ContentPanel.Visibility = Visibility.Collapsed;
                LayoutRoot.Background = BrightRedBrush;
                await Task.Delay(500);
                LayoutRoot.Background = WhiteBrush;
                await Task.Delay(500);
                LayoutRoot.Background = BrightRedBrush;
                await Task.Delay(500);
                LayoutRoot.Background = WhiteBrush;
                await Task.Delay(500);
                LayoutRoot.Background = BrightRedBrush;
                await Task.Delay(500);
                LayoutRoot.Background = WhiteBrush;
                ContentPanel.Visibility = Visibility.Visible;
            }
                
        }

        private async Task<bool> CheckEndReached()
        {
            if (CurrentLevel < 0) CurrentLevel = 0;
            if (CurrentLevel == OverallLevels)
            {
                await new MessageDialog("You advanced through all levels on that difficulty. Now it's time to move on!", "Memoriam®").ShowAsync();
                CurrentLevel = 0;
                Frame.Navigate(typeof(ChangeDifficulty));
                return true;
            }
            return false;
        }

        private void GenerateTags(int count, TagType type, bool random)
        {
            Tags = new string[count];

            for (int i = 0; i < count; i++)
                if (type == TagType.Digit)
                    Tags[i] = random ? seed.Next(1, 99).ToString() : (1 + i).ToString();
                else if (type == TagType.Letter)
                    Tags[i] = random ? ((char)('A' + seed.Next(0, 26))).ToString() : ((char)('A' + i)).ToString();
                else
                    if (random)
                        Tags[i] = seed.Next(0, 2) == 0 ? ((char)('A' + seed.Next(0, 26))).ToString() + ((char)('A' + seed.Next(0, 26))).ToString() : seed.Next(1, 99).ToString();
                    else
                        Tags[i] = seed.Next(0, 2) == 0 ? ((char)('A' + seed.Next(0, 26))).ToString() : seed.Next(1, 99).ToString();
        }
        
        TextBlock[] sequence;

        private void UpdateSeqView()
        {
            if (Opened != 0) sequence[Opened - 1].Foreground = DashView.Foreground;
            if (Opened != sequence.Length) sequence[Opened].Foreground = LevelsView.Foreground;
        }
        
        void RefillSequence()
        {
            SequenceContainer.Children.Clear();

            sequence = new TextBlock[Tags.Length];
            for (int i = 0; i < Tags.Length; i++)
            {
                sequence[i] = new TextBlock();
                sequence[i].Style = LivesView.Style;
                sequence[i].Text = Tags[i];
                sequence[i].Margin = new Thickness(4, 0, 4, 0);
                SequenceContainer.Children.Add(sequence[i]);
            }
        }

        private async Task<int> CheckForSaves()
        {
            int lastlevel = 0;
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey((playStyle == PlayStyle.GeneralRun ? "g" : "t") + Difficulty.ToString()))
                lastlevel = (int)ApplicationData.Current.RoamingSettings.Values[(playStyle == PlayStyle.GeneralRun ? "g" : "t") + Difficulty.ToString()];

            if (lastlevel == 0) return 0;

            var res = await Question.Show("Last level you played on this difficulty is " + (lastlevel + 1) + ". Do you want to continue or start from scratch?", "Memoriam", "Continue", "Restart");

            CurrentLevel = res ? lastlevel : 0;

            return CurrentLevel;
        }
 
        private int diffM()
        {
            if (Difficulty == 0 || Difficulty == 3 || Difficulty == 5) return 2;
            else if (Difficulty == 1 || Difficulty == 4) return 3;
            else if (Difficulty == 2) return 4;
            else return 5;

        }


        private async void Play_Loaded(object sender, RoutedEventArgs e)
        {
            if (Initialize())
            {
                await CheckForSaves();
                InitializeLevel(CurrentLevel);
            }
        }
    }

    enum TagType { Digit, Letter, Any };
}
