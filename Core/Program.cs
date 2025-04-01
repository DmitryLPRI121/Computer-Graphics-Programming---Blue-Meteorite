using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public static class Program
    {
        private static Form1 form1;
        private static SceneSettings scene;
        private static bool isClosing = false;

        [STAThread]
        public static void Main()
        {
            SceneObjects ss = new SceneObjects();
            
            // Создаем сцену первой
            var nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(800, 600),
                Title = "Сцена 'Голубой метеорит' | Лебедев Дмитрий И. _ ПРИ-121",
            };
            scene = new SceneSettings(GameWindowSettings.Default, nativeWindowSettings, ss);

            // Инициализируем фильтры до создания формы
            scene.InitializeFilters();

            // Запускаем поток панели
            ParameterizedThreadStart threadStart = new ParameterizedThreadStart(runPanel);
            Thread thread = new Thread(threadStart);
            thread.Start(ss);

            // Добавляем обработчик закрытия окна
            scene.Closing += (e) =>
            {
                isClosing = true;
                if (form1 != null && !form1.IsDisposed)
                {
                    form1.Invoke((MethodInvoker)delegate
                    {
                        form1.Close();
                    });
                }
            };

            // Запускаем сцену
            scene.Run();
            thread.Join();
        }

        private static void runPanel(Object obj)
        {
            var ss = (SceneObjects)obj;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            form1 = new Form1(ss, scene, scene.grayscaleFilter, scene.sepiaFilter, scene.blurFilter, scene.pixelizedFilter, scene.nightVisionFilter, scene.sharpnessFilter);
            
            // Добавляем обработчик закрытия формы
            form1.FormClosing += (s, e) =>
            {
                if (!isClosing)
                {
                    e.Cancel = true;
                    form1.Hide();
                }
            };
            
            Application.Run(form1);
        }
    }
}
