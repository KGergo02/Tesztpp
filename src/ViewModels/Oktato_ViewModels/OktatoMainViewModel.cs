﻿using System.Windows.Controls;
using System.Windows.Input;
using Teszt__.src.Commands;
using Teszt__.src.Commands.Oktato_Commands;
using Teszt__.src.Commands.User_Commands;
using Teszt__.src.Services;
using Teszt__.src.Views.Oktato_Views;
using static Teszt__.src.Models.DatabaseContext;

namespace Teszt__.src.ViewModels.Oktato_ViewModels
{
    public class OktatoMainViewModel : ViewModelBase
    {
        private string _titleName;
        public string TitleName
        {
            get
            {
                return _titleName;
            }
            set
            {
                _titleName = value;
                OnPropertyChanged(nameof(TitleName));
            }
        }

        public StackPanel _mainStackPanel;

        public Button _AddNewRowButton;

        public Grid grid = new Grid()
        {
            Margin = new System.Windows.Thickness(5)
        };
            
        private readonly User _user;

        public int modelType = 0;

        public OktatoMainViewModel(OktatoMainView window)
        {
            _user = UserService.GetCurrentUser();

            TitleName = $"Főoldal - {_user.Name}";

            _mainStackPanel = window.mainStackPanel;

            _AddNewRowButton = window.AddNewRowButton;

            grid = GridService.CreateCourseGrid(ref grid, ref _mainStackPanel);

            CreateCourseCommand = new CreateCourseCommand(ref grid, ref _mainStackPanel, this);

            CreateTestCommand = new CreateTestCommand(ref grid, ref _mainStackPanel, this);

            CreateQuestionCommand = new CreateQuestionCommand(ref grid, ref _mainStackPanel, this);

            LogOutCommand = new LogOutCommand();

            AddNewRowCommand = new AddNewRowCommand(ref grid, this);

            SendCommand = new SendCommand(ref grid, ref _mainStackPanel, this);

            ShowUserProfileCommand = new ShowUserProfileCommand();

            ShowResultsCommand = new ShowResultsCommand();

            NavigationService.GetNavigationWindow().Closing += WindowService.OnWindowClosingLogoutUserQuestion;
        }

        public ICommand CreateCourseCommand { get; }
        public ICommand CreateTestCommand { get; }
        public ICommand CreateQuestionCommand { get; }
        public ICommand LogOutCommand { get; }
        public ICommand AddNewRowCommand { get; }
        public ICommand SendCommand { get; }
        public ICommand ViewCourseCommand { get; }
        public ICommand ShowUserProfileCommand { get; }
        public ICommand ShowResultsCommand { get; }
    }
}
