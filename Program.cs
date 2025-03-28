using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace Computer_Graphics_Programming_Blue_Meteorite
{
    public static class Program
    {
        private static Form1 form1;
        private static Scene scene;

        [STAThread]
        public static void Main()
        {
            SceneState ss = new SceneState();
            
            // Create Scene first
            var nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(800, 600),
                Title = " ' ' |. -121",
            };
            scene = new Scene(GameWindowSettings.Default, nativeWindowSettings, ss);

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
            var ss = (SceneState)obj;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            form1 = new Form1(ss, scene);
            Application.Run(form1);
        }
    }
}
