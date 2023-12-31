﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;
using Teszt__.src.Commands;
using Teszt__.src.DAL;
using Teszt__.src.Services;
using Teszt__.src.Views;

namespace Teszt__.src.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private NavigationWindow _navigationWindow;

        public bool NeedsLoading = true;

        public NavigationWindow GetNavigationWindow()
        {
            return _navigationWindow;
        }

        public MainWindowViewModel(NavigationWindow navWindow)
        {
            _navigationWindow = navWindow;

            HallgatoLoginCommand = new BeforeLoginCommand(false);

            OktatoLoginCommand = new BeforeLoginCommand(true);

            RegisterLabelCommand = new RegisterLabelCommand();

            navWindow.Navigating += WindowService.OnWindowNavigation;

            LoadedMainCommand loadedMainCommand = new LoadedMainCommand(NeedsLoading);

            if (loadedMainCommand.CanExecute(this))
            {
                loadedMainCommand.Execute(this);
            }
        }

        public ICommand HallgatoLoginCommand
        {
            get;
        }

        public ICommand OktatoLoginCommand
        {
            get;
        }

        public ICommand RegisterLabelCommand 
        {
            get;
        }
    }
}
