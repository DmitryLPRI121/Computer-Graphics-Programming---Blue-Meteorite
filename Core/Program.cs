using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public static class Program
    {
        private static Form1 form1;
        private static SceneSettings scene;

        [STAThread]
        public static void Main()
        {
            SceneObjects ss = new SceneObjects();
            
            // Create Scene first
            var nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(800, 600),
                Title = "Сцена 'Голубой метеорит' | Лебедев Дмитрий И. _ ПРИ-121",
            };
            scene = new SceneSettings(GameWindowSettings.Default, nativeWindowSettings, ss);

            // Start panel thread
            ParameterizedThreadStart threadStart = new ParameterizedThreadStart(runPanel);
            Thread thread = new Thread(threadStart);
            thread.Start(ss);

            // Run scene
            scene.Run();
            thread.Join();
        }

        private static void runPanel(Object obj)
        {
            var ss = (SceneObjects)obj;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            form1 = new Form1(ss, scene);
            Application.Run(form1);
        }
    }
}
