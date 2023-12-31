﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using Teszt__.src.DAL;
using Teszt__.src.ViewModels;
using Teszt__.src.ViewModels.Hallgato_ViewModels;
using Teszt__.src.Views.Hallgato_Views;
using static Teszt__.src.Models.DatabaseContext;

namespace Teszt__.src.Services
{
    public static class TestService
    {
        private static bool testNeedsToBeSent;

        public static void TestLabelClickedEvent(object sender, MouseButtonEventArgs e)
        {
            Label label = (Label)sender;
            
            Test test = (Test)label.Tag;

            MessageBoxResult result = Message.Question($"Biztos indítani szeretnéd a {test.Name} nevű tesztet?\n\nEgy teszt indítása több időt is igénybe vehet, kérem várjon!");
            
            if(result == MessageBoxResult.Yes)
            {
                CheckTimeOfTest(test);
            }
        }

        public static async void CheckTimeOfTest(Test test)
        {
            DateTime currentTime = await UserService.GetCurrentTimeAsync();

            List<string> startTestTimes = test.StartTime.Split(':').ToList();
            
            List<string> endTestTimes = test.EndTime.Split(':').ToList();

            List<string> date = test.Date.Replace(" ", "").Split('.').ToList();

            if (endTestTimes[0].Equals("24"))
            {
                endTestTimes[0] = "23";
                endTestTimes[1] = "59";
            }

            DateTime testDate = new DateTime(
                Convert.ToInt32(date[0]),
                Convert.ToInt32(date[1]),
                Convert.ToInt32(date[2])
                );

            DateTime startDateTime = new DateTime(
                testDate.Year,
                testDate.Month, 
                testDate.Day, 
                Convert.ToInt32(startTestTimes[0]), 
                Convert.ToInt32(startTestTimes[1]), 
                0
                );

            DateTime endDateTime = new DateTime(
                testDate.Year,
                testDate.Month,
                testDate.Day,
                Convert.ToInt32(endTestTimes[0]),
                Convert.ToInt32(endTestTimes[1]),
                0
                );

            if(startDateTime <= currentTime && currentTime < endDateTime)
            {
                StartTest(test);
            }
            else if(startDateTime > currentTime)
            {
                Message.Error($"A kezdési idő előtt a teszt nem indítható!\n\nIndítható:\n{test.Date.Replace(" ", "")} ({test.StartTime} - {test.EndTime})");
            }
            else if(currentTime > endDateTime)
            {
                Message.Error($"A teszt határideje lejárt!");
            }
        }

        public static void StartTest(Test test)
        {
            testNeedsToBeSent = true;

            NavigationService.GetNavigationWindow().Closing += WindowService.OnTestClosing;

            NavigationService.NavigateToTestView(test);
        }

        public static void EndTest(Test test, TestView window)
        {
            if(!testNeedsToBeSent)
            {
                return;
            }
            
            int pontSzam = 0;

            int elerhetoPontszam = DatabaseContext.CalculateMaxTestPoint(test);

            List<Answer> userAnswers = new List<Answer>();

            foreach (StackPanel stackPanel in window.mainDockPanel.Children)
            {
                foreach (Control item in stackPanel.Children)
                {
                    if(item.Tag != null)
                    {
                        Answer answer = new Answer();

                        answer.QuestionId = Convert.ToInt32(item.Tag.ToString());

                        if (item is RadioButton radioButton)
                        {
                            answer.Value = radioButton.Content.ToString();
                            
                            answer.Correct = radioButton.IsChecked.Equals(true);
                        }
                        else if(item is CheckBox checkBox)
                        {
                            answer.Value = checkBox.Content.ToString();

                            answer.Correct = checkBox.IsChecked.Equals(true);
                        }
                        else if (item is TextBox textBox)
                        {
                            answer.Value = textBox.Text;

                            using (DatabaseContext database = new DatabaseContext())
                            {
                                Answer correctAnswer = database.Answers.Where(answ => answ.Value.ToUpper() == answer.Value.ToUpper() && answ.QuestionId == answer.QuestionId).ToList()[0];

                                if(correctAnswer != null)
                                {
                                    answer.Correct = true;
                                }
                                else
                                {
                                    answer.Correct = false;
                                }
                            }
                        }

                        userAnswers.Add(answer);
                    }
                }

                Question question = null;

                bool answeredCorrectly = true;

                foreach (Answer answer in userAnswers)
                {
                    using (DatabaseContext database = new DatabaseContext())
                    {
                        if(question == null)
                        {
                            question = (Question)database.FindById(answer.QuestionId, typeof(Question));
                        }

                        List<Answer> answers = database.GetAnswersOfQuestion(question);

                        Answer correctAnswer = null;

                        foreach (Answer item in answers)
                        {
                            if(item.QuestionId == answer.QuestionId && item.Value.ToUpper() == answer.Value.ToUpper())
                            {
                                correctAnswer = item;

                                break;
                            }
                        }

                        if(!correctAnswer.Equals(answer))
                        {
                            answeredCorrectly = false;
                        }
                    }
                }

                if (answeredCorrectly)
                {
                    pontSzam += question.Value;
                }

                answeredCorrectly = true;

                userAnswers.Clear();
            }

            if(testNeedsToBeSent)
            {
                testNeedsToBeSent = false;

                NavigationService.GetNavigationWindow().Closing -= WindowService.OnTestClosing;

                NavigationService.NavigateToHallgatoView();

                NavigationService.GetNavigationWindow().Closing += WindowService.OnWindowClosingLogoutUserQuestion;

                ResultView resultView = new ResultView();

                resultView.DataContext = new ResultViewModel(pontSzam, elerhetoPontszam, resultView);

                resultView.ShowDialog();
            }
        }
    }
}
