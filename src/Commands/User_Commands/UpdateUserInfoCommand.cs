﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Teszt__.src.Models;
using Teszt__.src.Services;
using static Teszt__.src.Models.DatabaseContext;
using Teszt__.src.Views.User_Views;

namespace Teszt__.src.Commands.User_Commands
{
    public class UpdateUserInfoCommand : CommandBase
    {
        private User user;

        private string initialUsername;

        private InputField inputField;

        private bool isAdmin;

        EditUserInfoView window;

        public UpdateUserInfoCommand(InputField inputField, EditUserInfoView window)
        {
            user = UserService.GetCurrentUser();

            initialUsername = user.Name;

            this.inputField = inputField;

            isAdmin = user.Admin;

            this.window = window;
        }

        StringBuilder error = new StringBuilder();

        public override void Execute(object parameter)
        {
            inputField.ClearColorOfInputFields();

            error.Clear();

            using (DAL.DatabaseContext database = new DAL.DatabaseContext())
            {
                string username = inputField.textBoxes["username"].Text;

                string email = inputField.textBoxes["email"].Text;

                SecureString password1 = inputField.passwordBoxes["password1"].SecurePassword;

                SecureString password2 = inputField.passwordBoxes["password2"].SecurePassword;

                List<User> users = database.Users.ToList();

                User currentUser = UserService.GetCurrentUser();

                users.Remove(currentUser);

                UserService.CheckAllInputs(username, email, password1, password2, ref error, ref inputField, users);

                if (error.ToString() != "")
                {
                    Message.Error(error.ToString());
                }
                else
                {
                    user.Name = username;

                    user.Email = email;

                    user.Password = JelszoTitkosito.Encrypt(SecureStringConvert.ToString(password1));

                    user.Admin = isAdmin;

                    DAL.DatabaseContext.UpdateUser();

                    window.Closing -= WindowService.OnWindowClosing;

                    Message.Success("Az adataid sikeresen módosítva lettek!");

                    window.Close();
                }
            }
        }
    }
}
