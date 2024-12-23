using System.Drawing;
using System.Windows.Forms;

namespace Computer_Graphics_Programming___Blue_Meteorite
{
    public class HelpDialog : Form
    {
        public HelpDialog()
        {
            Text = "Справка";
            Width = 650;
            Height = 500;
            StartPosition = FormStartPosition.CenterScreen;

            var label = new Label
            {
                Text = "Название проекта: Голубой метеорит\n\n" +
                       "Задача: Разработка графического приложения на C# с использованием OpenGL\n\n" +
                       "Описание: Создание интерактивного графического приложения на языке C# с использованием библиотеки OpenGL, которое будет моделировать трехмерную сцену, основанную на сюжете \"Голубой метеорит\" из мультипликационной программы \"Веселая карусель\"\n\n" +
                       "Дисциплина: Программирование компьютерной графики\n\n" +
                       "Автор: Лебедев Дмитрий Игоревич\n\n" +
                       "Направление: Программная инженерия\n\n" +
                       "Группа: ПРИ-121\n\n" +
                       "Лицензия: MIT",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopLeft,
                Font = new Font("Arial", 14, FontStyle.Regular),
                AutoSize = false
            };

            var button = new Button
            {
                Text = "Ок",
                Dock = DockStyle.Bottom,
                DialogResult = DialogResult.OK,
                Font = new Font("Arial", 10, FontStyle.Regular)
            };

            button.Click += (sender, e) => Close();

            Controls.Add(label);
            Controls.Add(button);
        }

        public static void ShowHelp()
        {
            using (var helpDialog = new HelpDialog())
            {
                helpDialog.ShowDialog();
            }
        }
    }
}