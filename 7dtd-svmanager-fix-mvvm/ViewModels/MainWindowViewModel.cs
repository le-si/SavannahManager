﻿using _7dtd_svmanager_fix_mvvm.Views;
using CommonStyleLib.ViewModels;
using LanguageEx;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using _7dtd_svmanager_fix_mvvm.Backup.Models;
using _7dtd_svmanager_fix_mvvm.Backup.ViewModels;
using _7dtd_svmanager_fix_mvvm.Backup.Views;
using _7dtd_svmanager_fix_mvvm.Models;
using _7dtd_svmanager_fix_mvvm.PlayerController.Models;
using _7dtd_svmanager_fix_mvvm.PlayerController.ViewModels;
using _7dtd_svmanager_fix_mvvm.PlayerController.Views;
using _7dtd_svmanager_fix_mvvm.PlayerController.Views.Pages;
using _7dtd_svmanager_fix_mvvm.Settings.Models;
using _7dtd_svmanager_fix_mvvm.Settings.ViewModels;
using _7dtd_svmanager_fix_mvvm.Settings.Views;
using _7dtd_svmanager_fix_mvvm.Setup.ViewModels;
using CommonStyleLib.Views;
using _7dtd_svmanager_fix_mvvm.Setup.Views;
using _7dtd_svmanager_fix_mvvm.Update.Models;
using _7dtd_svmanager_fix_mvvm.Update.ViewModels;
using _7dtd_svmanager_fix_mvvm.Update.Views;
using CommonExtensionLib.Extensions;
using CommonStyleLib.ExMessageBox;
using Log;
using SvManagerLibrary.Player;
using SvManagerLibrary.Telnet;

namespace _7dtd_svmanager_fix_mvvm.ViewModels
{
    public class UserDetail
    {
        public string ID { get; set; }
        public string Level { get; set; }
        public string Name { get; set; }
        public string Health { get; set; }
        public string ZombieKills { get; set; }
        public string PlayerKills { get; set; }
        public string Death { get; set; }
        public string Score { get; set; }
        public string Coord { set; get; }
        public string SteamId { get; set; }

        public PlayerInfo ToPlayerInfo()
        {
            return new PlayerInfo
            {
                Id = ID,
                Level = Level,
                Name = Name,
                Health = Health,
                ZombieKills = ZombieKills,
                PlayerKills = PlayerKills,
                Deaths = Death,
                Score = Score,
                Coord = Coord,
                SteamId = SteamId
            };
        }

        public override string ToString()
        {
            return $"{ID} {Level} {Name} {Health} {ZombieKills} {PlayerKills} {Death} {Score} {Coord} {SteamId}\r\n";
        }
    }

    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(MainWindowService windowService, MainWindowModel model, MainWindow view) : base(windowService, model)
        {
            model.AppendConsoleText += Model_AppendConsoleText;
            model.Telnet.Started += Telnet_Started;
            model.Telnet.Finished += Telnet_Finished;
            model.Telnet.ReadEvent += TelnetReadEvent;

            mainWindowService = windowService;
            this.model = model;

            #region Event Initialize
            //Loaded = new DelegateCommand(MainWindow_Loaded);
            Closing = new DelegateCommand(MainWindow_Closing);
            KeyDown = new DelegateCommand<KeyEventArgs>(MainWindow_KeyDown);

            MenuSettingsBtClick = new DelegateCommand(MenuSettingsBt_Click);
            MenuFirstSettingsBtClick = new DelegateCommand(MenuFirstSettingsBt_Click);
            MenuLangJapaneseBtClick = new DelegateCommand(MenuLangJapaneseBt_Click);
            MenuLangEnglishBtClick = new DelegateCommand(MenuLangEnglishBt_Click);
            MenuConfigEditorBtClick = new DelegateCommand(MenuConfigEditorBt_Click);
            MenuXmlEditorBtClick = new DelegateCommand(MenuXmlEditorBt_Click);
            MenuBackupEditorBtClick = new DelegateCommand(MenuBackupEditorBt_Click);
            MenuCheckUpdateBtClick = new DelegateCommand(MenuCheckUpdateBt_Click);
            MenuVersionInfoClick = new DelegateCommand(MenuVersionInfo_Click);

            StartBtClick = new DelegateCommand(StartBt_Click);
            StopBtClick = new DelegateCommand(StopBt_Click);
            TelnetBtClick = new DelegateCommand(TelnetBt_Click);
            CommandListBtClick = new DelegateCommand(CommandListBt_Click);

            PlayerListRefreshBtClick = new DelegateCommand(PlayerListRefreshBt_Click);

            PlayerContextMenuOpened = new DelegateCommand(PlayerContextMenu_Opened);
            AdminAddBtClick = new DelegateCommand(AdminAddBt_Click);
            AdminRemoveBtClick = new DelegateCommand(AdminRemoveBt_Click);
            WhiteListAddBtClick = new DelegateCommand(WhiteListAddBt_Click);
            WhiteListRemoveBtClick = new DelegateCommand(WhiteListRemoveBt_Click);
            KickBtClick = new DelegateCommand(KickBt_Click);
            BanBtClick = new DelegateCommand(BanBt_Click);
            KillBtClick = new DelegateCommand(KillBt_Click);
            WatchPlayerInfoBtClick = new DelegateCommand(WatchPlayerInfoBt_Click);

            ChatTextBoxEnterDown = new DelegateCommand<string>(ChatTextBoxEnter_Down);

            ConsoleTextBoxMouseEnter = new DelegateCommand(ConsoleTextBoxMouse_Enter);
            ConsoleTextBoxMouseLeave = new DelegateCommand(ConsoleTextBoxMouse_Leave);
            DeleteLogBtClick = new DelegateCommand(DeleteLogBt_Click);

            CmdTextBoxEnterDown = new DelegateCommand<string>(CmdTextBox_EnterDown);

            GetTimeBtClick = new DelegateCommand(GetTimeBt_Click);
            SetTimeBtClick = new DelegateCommand(SetTimeBt_Click);
            SaveWorldBtClick = new DelegateCommand(SaveWorldBt_Click);

            GetIpClicked = new DelegateCommand(GetIp_Clicked);
            CheckPortClicked = new DelegateCommand(CheckPort_Clicked);
            #endregion

            #region Property Initialize

            IsBeta = model.ObserveProperty(m => m.IsBeta).ToReactiveProperty();

            StartBTEnabled = model.ToReactivePropertyAsSynchronized(m => m.StartBtEnabled);
            TelnetBTIsEnabled = model.ToReactivePropertyAsSynchronized(m => m.TelnetBtIsEnabled);
            TelnetBTLabel = model.ToReactivePropertyAsSynchronized(m => m.TelnetBtLabel);

            UsersList = model.ToReactivePropertyAsSynchronized(m => m.UsersList);
            
            ChatLogText = model.ObserveProperty(m => m.ChatLogText).ToReactiveProperty();
            ChatInputText = model.ToReactivePropertyAsSynchronized(m => m.ChatInputText);
            
            ConnectionPanelIsEnabled = model.ToReactivePropertyAsSynchronized(m => m.ConnectionPanelIsEnabled);
            LocalModeChecked = model.ToReactivePropertyAsSynchronized(m => m.LocalMode);
            LocalModeEnabled = model.ToReactivePropertyAsSynchronized(m => m.LocalModeEnabled);
            TelnetAddressText = model.ToReactivePropertyAsSynchronized(m => m.Address);
            TelnetPortText = model.ToReactivePropertyAsSynchronized(m => m.PortText);
            TelnetPasswordText = model.ToReactivePropertyAsSynchronized(m => m.Password);

            TimeDayText = model.ToReactivePropertyAsSynchronized(m => m.TimeDayText);
            TimeHourText = model.ToReactivePropertyAsSynchronized(m => m.TimeHourText);
            TimeMinuteText = model.ToReactivePropertyAsSynchronized(m => m.TimeMinuteText);

            BottomNewsLabel = model.ToReactivePropertyAsSynchronized(m => m.BottomNewsLabel);
            #endregion

            model.InitializeWindow();
        }

        #region Fields
        private readonly MainWindowService mainWindowService;
        private readonly Models.MainWindowModel model;
        private StringBuilder consoleLog = new StringBuilder();

        private bool consoleIsFocus = false;
        #endregion

        #region EventProperties
        public ICommand MenuSettingsBtClick { get; set; }
        public ICommand MenuFirstSettingsBtClick { get; set; }
        public ICommand MenuLangJapaneseBtClick { get; set; }
        public ICommand MenuLangEnglishBtClick { get; set; }
        public ICommand MenuConfigEditorBtClick { get; set; }
        public ICommand MenuXmlEditorBtClick { get; set; }
        public ICommand MenuBackupEditorBtClick { get; set; }
        public ICommand MenuCheckUpdateBtClick { get; set; }
        public ICommand MenuVersionInfoClick { get; set; }

        public ICommand StartBtClick { get; set; }
        public ICommand StopBtClick { get; set; }
        public ICommand TelnetBtClick { get; set; }
        public ICommand CommandListBtClick { get; set; }

        public ICommand PlayerListRefreshBtClick { get; set; }
        
        public ICommand PlayerContextMenuOpened { get; set; }
        public ICommand AdminAddBtClick { get; set; }
        public ICommand AdminRemoveBtClick { get; set; }
        public ICommand WhiteListAddBtClick { get; set; }
        public ICommand WhiteListRemoveBtClick { get; set; }
        public ICommand KickBtClick { get; set; }
        public ICommand BanBtClick { get; set; }
        public ICommand KillBtClick { get; set; }
        public ICommand WatchPlayerInfoBtClick { get; set; }

        public ICommand ChatTextBoxEnterDown { get; set; }

        public ICommand ConsoleTextBoxMouseEnter { get; set; }
        public ICommand ConsoleTextBoxMouseLeave { get; set; }
        public ICommand DeleteLogBtClick { get; set; }

        public ICommand CmdTextBoxEnterDown { get; set; }

        public ICommand GetTimeBtClick { get; set; }
        public ICommand SetTimeBtClick { get; set; }
        public ICommand SaveWorldBtClick { get; set; }

        public ICommand GetIpClicked { get; set; }
        public ICommand CheckPortClicked { get; set; }
        #endregion

        #region Properties
        public ReactiveProperty<bool> IsBeta { get; set; }

        public TextBox ConsoleTextBox { get; set; }
        
        public ReactiveProperty<bool> StartBTEnabled { get; set; }
        public ReactiveProperty<bool> TelnetBTIsEnabled { get; set; }
        public ReactiveProperty<string> TelnetBTLabel { get; set; }
        
        public ReactiveProperty<ObservableCollection<UserDetail>> UsersList { get; set; }
        private int usersListSelectedIndex = -1;
        public int UsersListSelectedIndex
        {
            get => usersListSelectedIndex;
            set => SetProperty(ref usersListSelectedIndex, value);
        }

        private bool adminContextEnabled;
        public bool AdminContextEnabled
        {
            get => adminContextEnabled;
            set => SetProperty(ref adminContextEnabled, value);
        }
        private bool whitelistContextEnabled;
        public bool WhitelistContextEnabled
        {
            get => whitelistContextEnabled;
            set => SetProperty(ref whitelistContextEnabled, value);
        }
        private bool kickContextEnabled;
        public bool KickContextEnabled
        {
            get => kickContextEnabled;
            set => SetProperty(ref kickContextEnabled, value);
        }
        private bool banContextEnabled;
        public bool BanContextEnabled
        {
            get => banContextEnabled;
            set => SetProperty(ref banContextEnabled, value);
        }
        private bool watchPlayerInfoContextEnabled;
        public bool WatchPlayerInfoContextEnabled
        {
            get => watchPlayerInfoContextEnabled;
            set => SetProperty(ref watchPlayerInfoContextEnabled, value);
        }
        
        public ReactiveProperty<string> ChatLogText { get; set; }
        public ReactiveProperty<string> ChatInputText { get; set; }

        private string consoleLogText;
        public string ConsoleLogText
        {
            get => consoleLogText;
            set => SetProperty(ref consoleLogText, value);
        }
        private string cmdText;
        public string CmdText
        {
            get => cmdText;
            set => SetProperty(ref cmdText, value);
        }
        
        public ReactiveProperty<bool> ConnectionPanelIsEnabled { get; set; }
        public ReactiveProperty<bool> LocalModeChecked { get; set; }
        public ReactiveProperty<bool> LocalModeEnabled { get; set; }
        public ReactiveProperty<string> TelnetAddressText { get; set; }
        public ReactiveProperty<string> TelnetPortText { get; set; }
        public ReactiveProperty<string> TelnetPasswordText { get; set; }
        
        public ReactiveProperty<string> TimeDayText { get; set; }
        public ReactiveProperty<string> TimeHourText { get; set; }
        public ReactiveProperty<string> TimeMinuteText { get; set; }
        
        public ReactiveProperty<string> BottomNewsLabel { get; set; }
        #endregion

        #region EventMethods
        protected override void MainWindow_Loaded()
        {
            model.Initialize();

            model.RefreshLabels();

            if (model.Setting.IsFirstBoot)
                MenuFirstSettingsBt_Click();

            var task = model.CheckUpdate();
            var task2 = task.ContinueWith(continueTask =>
            {
                var dialogResult = continueTask.Result;
                if (dialogResult == ExMessageBoxBase.DialogResult.Yes)
                {
                    var updFormModel = new UpdFormModel();
                    updFormModel.Initialize();

                    WindowManageService.Dispatch(() =>
                    {
                        var vm = new UpdFormViewModel(new WindowService(), updFormModel, true);
                        WindowManageService.Show<UpdForm>(vm);
                    });
                }
            }).ContinueWith(t =>
            {
                if (t.Exception != null)
                    foreach (var exceptionInnerException in t.Exception.InnerExceptions)
                        App.ShowAndWriteException(exceptionInnerException);
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
        protected override void MainWindow_Closing()
        {
            model.SettingsSave();
            model.Dispose();
        }
        protected override void MainWindow_KeyDown(KeyEventArgs e)
        {
            model.PushShortcutKey(e.Key);
        }

        private void MenuSettingsBt_Click()
        {
            var setting = model.Setting;
            var keyManager = model.ShortcutKeyManager;

            var settingModel = new SettingModel(setting, keyManager);
            var vm = new SettingWindowViewModel(new WindowService(), settingModel);
            WindowManageService.ShowDialog<SettingWindow>(vm);
            model.IsBeta = setting.IsBetaMode;
        }
        private void MenuFirstSettingsBt_Click()
        {
            var setting = model.Setting;
            WindowManageService.ShowDialog<InitializeWindow>(window =>
            {
                var initModel = new Setup.Models.InitializeWindowModel(setting, window.MainFrame.NavigationService);
                return new InitializeWindowViewModel(new WindowService(), initModel);
            });
        }
        private void MenuLangJapaneseBt_Click()
        {
            model.ChangeCulture(ResourceService.Japanese);
            model.RefreshLabels();
        }
        private void MenuLangEnglishBt_Click()
        {
            model.ChangeCulture(ResourceService.English);
            model.RefreshLabels();
        }
        private void MenuConfigEditorBt_Click()
        {
            model.RunConfigEditor();
        }

        private void MenuXmlEditorBt_Click()
        {
            model.RunXmlEditor();
        }
        private void MenuBackupEditorBt_Click()
        {
            var setting = model.Setting;
            var backupModel = new BackupSelectorModel(setting);
            var vm = new BackupSelectorViewModel(new WindowService(), backupModel);
            WindowManageService.Show<BackupSelector>(vm);
        }
        private void MenuCheckUpdateBt_Click()
        {
            var updFormModel = new UpdFormModel();
            var vm = new UpdFormViewModel(new WindowService(), updFormModel);
            WindowManageService.Show<UpdForm>(vm);
        }
        private void MenuVersionInfo_Click()
        {
            var versionInfoModel = new VersionInfoModel();
            var vm = new VersionInfoViewModel(new WindowService(), versionInfoModel);
            WindowManageService.ShowDialog<VersionInfo>(vm);
        }

        private void StartBt_Click()
        {
            model.ServerStart();
        }
        private void StopBt_Click()
        {
            var isForceShutdown = model.ServerStop();
            if (!isForceShutdown)
                return;

            var forceShutdownerModel = new ForceShutdownerModel();
            var vm = new ForceShutdownerViewModel(new WindowService(), forceShutdownerModel);
            WindowManageService.Show<ForceShutdowner>(vm);
        }
        private void TelnetBt_Click()
        {
            model.TelnetConnectOrDisconnect();
        }
        private void CommandListBt_Click()
        {

        }

        private void PlayerListRefreshBt_Click()
        {
            model.PlayerRefresh();
        }

        private void PlayerContextMenu_Opened()
        {
            int index = UsersListSelectedIndex;
            if (index < 0)
            {
                AdminContextEnabled = false;
                WhitelistContextEnabled = false;
                KickContextEnabled = false;
                BanContextEnabled = false;
                WatchPlayerInfoContextEnabled = false;
            }
            else
            {
                AdminContextEnabled = true;
                WhitelistContextEnabled = true;
                KickContextEnabled = true;
                BanContextEnabled = true;
                WatchPlayerInfoContextEnabled = true;
            }
        }
        private void AdminAddBt_Click()
        {
            var playerInfo = model.GetUserDetail(UsersListSelectedIndex);
            var name = string.IsNullOrEmpty(playerInfo.ID) ? string.Empty : playerInfo.ID;

            var playerBaseModel = new PlayerBaseModel();
            var adminAdd = new AdminAdd(model, AddType.Type.Admin, name);
            WindowManageService.ShowDialog<PlayerBase>(window =>
            {
                window.Page = adminAdd;
                window.AssignEnded();
                window.Navigate();
                return new PlayerBaseViewModel(new WindowService(), playerBaseModel)
                {
                    WindowTitle = "Add"
                };
            });
        }
        private void AdminRemoveBt_Click()
        {
            model.RemoveAdmin(UsersListSelectedIndex);
        }
        private void WhiteListAddBt_Click()
        {
            var playerInfo = model.GetUserDetail(UsersListSelectedIndex);
            var name = string.IsNullOrEmpty(playerInfo.ID) ? string.Empty : playerInfo.ID;

            var playerBaseModel = new PlayerBaseModel();
            var whitelistAdd = new AdminAdd(model, AddType.Type.Whitelist, name);
            WindowManageService.ShowDialog<PlayerBase>(window =>
            {
                window.Page = whitelistAdd;
                window.AssignEnded();
                window.Navigate();
                return new PlayerBaseViewModel(new WindowService(), playerBaseModel)
                {
                    WindowTitle = "Whitelist"
                };
            });
        }
        private void WhiteListRemoveBt_Click()
        {
            model.RemoveWhitelist(UsersListSelectedIndex);
        }
        private void KickBt_Click()
        {
            var playerInfo = model.GetUserDetail(UsersListSelectedIndex);
            var name = string.IsNullOrEmpty(playerInfo.ID) ? string.Empty : playerInfo.ID;

            var playerBaseModel = new PlayerBaseModel();
            var kick = new Kick(model, name);
            WindowManageService.ShowDialog<PlayerBase>(window =>
            {
                window.Page = kick;
                window.AssignEnded();
                window.Navigate();
                return new PlayerBaseViewModel(new WindowService(), playerBaseModel)
                {
                    WindowTitle = "Kick"
                };
            });
        }
        private void BanBt_Click()
        {
            var playerInfo = model.GetUserDetail(UsersListSelectedIndex);
            var name = string.IsNullOrEmpty(playerInfo.ID) ? string.Empty : playerInfo.ID;

            var playerBaseModel = new PlayerBaseModel();
            var ban = new Ban(model, name);
            WindowManageService.ShowDialog<PlayerBase>(window =>
            {
                window.Page = ban;
                window.AssignEnded();
                window.Navigate();
                return new PlayerBaseViewModel(new WindowService(), playerBaseModel)
                {
                    WindowTitle = "Ban"
                };
            });
        }
        private void KillBt_Click()
        {

        }
        private void WatchPlayerInfoBt_Click()
        {
            var playerInfo = model.GetSelectedPlayerInfo(UsersListSelectedIndex);
            var playerInfoModel = new PlayerInfoModel();
            var vm = new PlayerInfoViewModel(new WindowService(), playerInfoModel);
            vm.SetPlayer(playerInfo);
            WindowManageService.ShowDialog<PlayerInfoView>(vm);
        }
        
        private void ChatTextBoxEnter_Down(string e)
        {
            model.SendChat(e, () => model.ChatInputText = "");
        }

        private void ConsoleTextBoxMouse_Enter()
        {
            consoleIsFocus = true;
        }
        private void ConsoleTextBoxMouse_Leave()
        {
            consoleIsFocus = false;
        }
        private void DeleteLogBt_Click()
        {
            consoleLog.Clear();
            ConsoleLogText = "";
        }

        private void CmdTextBox_EnterDown(string e)
        {
            model.SendCommand(e);
            CmdText = string.Empty;
        }

        private void GetTimeBt_Click()
        {
            model.SetTimeToTextBox();
        }
        private void SetTimeBt_Click()
        {
            model.SetTimeToGame();
        }
        private void SaveWorldBt_Click()
        {
            model.SendCommand("saveworld");
        }

        private void GetIp_Clicked()
        {
            var ipAddressGetterModel = new IpAddressGetterModel();
            var vm = new IpAddressGetterViewModel(new WindowService(), ipAddressGetterModel);
            WindowManageService.Show<IpAddressGetter>(vm);
        }
        private void CheckPort_Clicked()
        {
            var portCheckModel = new PortCheckModel();
            var vm = new PortCheckViewModel(new WindowService(), portCheckModel);
            WindowManageService.Show<PortCheck>(vm);
        }


        private void Model_AppendConsoleText(object sender, Models.MainWindowModel.AppendedLogTextEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.AppendedLogText))
            {
                WindowManageService.Dispatch(DispatcherPriority.Background, () =>
                {
                    AppendConsoleText(e.AppendedLogText, e.MaxLength);
                });
            }
        }
        #endregion

        #region Methods
        private void AppendConsoleText(string text, int maxLength)
        {
            if (consoleLog == null)
            {
                consoleLog = new StringBuilder(maxLength * 2);
            }
            consoleLog.Append(text);
            if (consoleLog.Length > maxLength)
            {
                consoleLog.Remove(0, consoleLog.Length - maxLength);
            }

            ConsoleLogText = consoleLog.ToString();

            if (!consoleIsFocus)
            {
                if (!consoleIsFocus)
                {
                    mainWindowService.Select(ConsoleLogText.Length, 0);
                    mainWindowService.ScrollToEnd();
                }
            }
        }
        #endregion


        private void Telnet_Started(object sender, TelnetClient.TelnetReadEventArgs e)
        {
            model.PlayerClean();
        }

        private void Telnet_Finished(object sender, TelnetClient.TelnetReadEventArgs e)
        {
            model.PlayerClean();
            model.TelnetFinish();
        }

        private void TelnetReadEvent(object sender, TelnetClient.TelnetReadEventArgs e)
        {
            var log = "{0}".FormatString(e.Log);

            //if (isStop)
            //    SocTelnetSendDirect("");

            model.LoggingStream.WriteSteam(log);

            model.AppendConsoleLog(log);

            if (log.IndexOf("Chat", StringComparison.Ordinal) > -1)
            {
                model.AddChatText(log);
            }
            if (log.IndexOf("INF Created player with id=", StringComparison.Ordinal) > -1)
            {
                WindowManageService.Dispatch(DispatcherPriority.Background, model.PlayerRefresh);
            }
            if (log.IndexOf("INF Player disconnected", StringComparison.Ordinal) > -1)
            {
                model.RemoveUser(log);
            }
        }
    }
}
